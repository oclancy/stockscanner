using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Hosting;
using Microsoft.Practices.Unity;
using StockService.Core;
using StockService.Core.Providers;
using System.ServiceModel.Activation;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using StockService.Core.DTOs;
using StockService.Core.Extension;
using System.Data.Entity;
using NLog;

namespace StockService
{
    /// <summary>
    /// Produces <see cref="DependencyInjectionServiceHost"/>s.
    /// </summary>
    public class DependencyInjectionServiceHostFactory : ServiceHostFactory
    {
        IUnityContainer m_container = new UnityContainer();
        DataProviderFactory m_dataProviderFactory;
        Logger m_logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a <see cref="DependencyInjectionServiceHost"/> for a specified type of service with a specific base address. 
        /// </summary>
        /// <returns>
        /// A <see cref="DependencyInjectionServiceHost"/> for the type of service specified with a specific base address.
        /// </returns>
        /// <param name="serviceType">
        /// Specifies the type of service to host. 
        /// </param>
        /// <param name="baseAddresses">
        /// The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.
        /// </param>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceLocator.SetLocatorProvider( ()=> new UnityServiceLocator(m_container) );

            //Register the service as a type so it can be found from the instance provider
            m_container.RegisterType(serviceType);
            
            RegisterDependencies();
            
            InflateFromPersistence();

            StartLongRunningTasks();

            var serviceHost = new ServiceHost(serviceType, baseAddresses);

            serviceHost.Closing += SaveData; ;

            return serviceHost;
        }

        private void InflateFromPersistence()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<StockScannerContext>());

            using (var dbcontext = new StockScannerContext() )
            {
                dbcontext.Configuration.LazyLoadingEnabled = false;
                dbcontext.Companies.Include( c=> c.StockQuote).Load();
                dbcontext.Companies.Include(c => c.CompanyStatistics).Load();
                var companies = m_container.Resolve<IDictionary<string, Company>>();
                dbcontext.Companies.ForEach(s => companies.Add(s.Symbol, s));
            }
        }

        private void SaveData(Company company)
        {
            using (var dbcontext = new StockScannerContext())
            {
                if (company.Id.HasValue)
                {
                    dbcontext.Companies.Attach(company);
                }
                else
                {
                    dbcontext.Companies.Add(company);
                }
                
                Commit(dbcontext);
            }
        }

        void SaveData(object sender, EventArgs e)
        {
            using (var dbcontext = new StockScannerContext())
            {
                var companies = m_container.Resolve<IDictionary<string, Company>>();

                companies.ForEach(c =>
                {
                    if (c.Value.Id.HasValue)
                    {
                        dbcontext.Companies.Attach(c.Value);
                    }
                    else
                    {
                        dbcontext.Companies.Add(c.Value);
                    }
                });

                Commit(dbcontext);
               }
        }

        private void Commit(StockScannerContext dbcontext)
        {
            try
            {
                dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                m_logger.ErrorException("Exception whilst saving", ex);
            }
        }

        private async void StartLongRunningTasks()
        {
            CancellationTokenSource sorc = new CancellationTokenSource();
            var companiesContainer = m_container.Resolve<IDictionary<string, Company>>();

            var market = m_dataProviderFactory.GetMarketsData()
                                              .First(m => m.Name == "Main Market");

            var sectors = await m_dataProviderFactory.GetDataProvider<ISectorDataProvider>(market).FetchDataAsync(market);
            var companiesTasks = sectors.SelectMany(sector => sector.Industries.Select(async i => await m_dataProviderFactory.GetDataProvider<ICompanyProvider>(market)
                                                                                                                            .FetchDataAsync(i)));
            await Task.WhenAll(companiesTasks);
            var companies = companiesTasks.SelectMany(ct => ct.Result)
                                          .ToList();

            m_logger.Info("Starting data gathering at {0}", DateTime.Now);

                        
            companies.ForEach( async company =>
            {
                if (!companiesContainer.ContainsKey(company.Symbol))
                    companiesContainer.Add(company.Symbol, company);

                try
                {
                    company.CompanyStatistics = await m_dataProviderFactory.GetDataProvider<ICompanyDataProvider>(market)
                                                                    .FetchDataAsync(company);
                    company.StockQuote = await m_dataProviderFactory.GetDataProvider<IStockProvider>(market)
                                                                    .FetchDataAsync(company);
                    SaveData(company);
                }
                catch (Exception ex)
                {
                    m_logger.ErrorException(string.Format("Exception whilst getting company details for {0}",company.Name), ex);
                }
            });

        }

        
        /// <summary>
        /// Initialization logic that any derived type would use to set up the ServiceLocator provider.  Look to UnityServiceHostFactory to see how this is done, if implementing for 
        /// another IoC engine.
        /// </summary>
        protected void RegisterDependencies()
        {
            IDictionary<string, Company> companies = new ConcurrentDictionary<string, Company>();

            m_container.RegisterInstance(companies);
            
            m_container.RegisterInstance< ICalculatedCompanyDataProvider>(new CalculatedStaticticsProvider(companies));

            m_dataProviderFactory = new DataProviderFactory(m_container, HostingEnvironment.ApplicationPhysicalPath);
            m_container.RegisterInstance(m_dataProviderFactory);
        }
    }
}