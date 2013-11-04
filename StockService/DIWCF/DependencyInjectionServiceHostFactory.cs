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
        IDictionary<string,Market> m_markets = new ConcurrentDictionary<string,Market>();
        IDictionary<string, Company> m_companies = new ConcurrentDictionary<string, Company>();
            
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

            m_dataProviderFactory = new DataProviderFactory(m_container, 
                                                            HostingEnvironment.ApplicationPhysicalPath, 
                                                            m_markets, 
                                                            m_companies);

            m_container.RegisterInstance(m_dataProviderFactory);

            GetCompanyData();

            var serviceHost = new DependencyInjectionServiceHost(serviceType, baseAddresses);

            return serviceHost;
        }

        private void SaveCompany(Company company)
        {
            using (var dbcontext = new StockScannerContext())
            {
                if (company.CompanyId != 0)
                {
                    dbcontext.Companies.Attach(company);
                }
                else
                {
                    dbcontext.Companies.Add(company);
                }

                dbcontext.SaveChanges();
            }
        }

        private void GetCompanyData()
        {
            CancellationTokenSource sorc = new CancellationTokenSource();
                        
            m_logger.Info("Starting data gathering at {0}", DateTime.Now);

            Task.Run(() =>
            {
                m_companies.Values.ForEach( company =>
                {
                    try
                    {
                        var cs = m_dataProviderFactory.GetDataProvider<ICompanyDataProvider>(company.Industry.Sector.Market)
                                                                            .FetchDataAsync(company).Result;
                        var sq = m_dataProviderFactory.GetDataProvider<IStockProvider>(company.Industry.Sector.Market)
                                                                            .FetchDataAsync(company).Result;

                        using (var cxt = new StockScannerContext())
                        {
                            try
                            {
                                cxt.Companies.Attach(company);
                                company.StockQuote = sq;
                                company.CompanyStatistics = cs;
                                cxt.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                m_logger.ErrorException(string.Format("Exception whilst getting company details for {0}", company.Name), ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        m_logger.ErrorException(string.Format("Exception whilst getting company details for {0}", company.Name), ex);
                    }
                });
            });

        }

        /// <summary>
        /// Initialization logic that any derived type would use to set up the ServiceLocator provider.  Look to UnityServiceHostFactory to see how this is done, if implementing for 
        /// another IoC engine.
        /// </summary>
        protected void RegisterDependencies()
        {
            m_container.RegisterInstance<IDictionary<string, Company>>(m_companies);
            m_container.RegisterInstance<IDictionary<string, Market>>(m_markets);
            m_container.RegisterInstance< ICalculatedCompanyDataProvider>(new CalculatedStaticticsProvider(m_companies));
         }
    }
}