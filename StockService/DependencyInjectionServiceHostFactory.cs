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

namespace StockService
{
    /// <summary>
    /// Produces <see cref="DependencyInjectionServiceHost"/>s.
    /// </summary>
    public class DependencyInjectionServiceHostFactory : ServiceHostFactory
    {
        IUnityContainer m_container = new UnityContainer();
        DataProviderFactory m_dataProviderFactory;

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

            serviceHost.Closed += serviceHost_Closed; ;

            return serviceHost;
        }

        private void InflateFromPersistence()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<StockScannerContext>());

            using (var dbcontext = new StockScannerContext() )
            {
                var stocks = m_container.Resolve<IDictionary<string, StockQuote>>();
                dbcontext.StockQuote.ForEach( s=> stocks.Add(s.Company.Symbol, s));

                var statistics = m_container.Resolve<IDictionary<string, CompanyStatistics>>();
                dbcontext.CompanyStatistics.ForEach(s => statistics.Add(s.Company.Symbol, s));
            }
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            using (var dbcontext = new StockScannerContext())
            {
                var stocks = m_container.Resolve<IDictionary<string, StockQuote>>();
                stocks.Where(s => s.Value.LastUpdate > DateTime.UtcNow.AddDays(-1) &&
                                  s.Value.Company.Id != null)
                      .ForEach(s =>
                      {
                          dbcontext.StockQuote.Attach(s.Value);
                      });
                      
                stocks.Where(s => s.Value.Company.Id == null)
                      .ForEach(s => dbcontext.StockQuote.Add(s.Value));

                var statistics = m_container.Resolve<IDictionary<string, CompanyStatistics>>();

                statistics.Where(s => s.Value.LastUpdated > DateTime.UtcNow.AddDays(-1) &&
                                      s.Value.Company.Id != null)
                            .ForEach(s =>
                            {
                                dbcontext.CompanyStatistics.Attach(s.Value);
                            });

                statistics.Where(s => s.Value.Company.Id == null)
                          .ForEach(s => dbcontext.CompanyStatistics.Add(s.Value));

                dbcontext.SaveChanges();
            }
        }

        private async void StartLongRunningTasks()
        {
            CancellationTokenSource sorc = new CancellationTokenSource();

            var market = m_dataProviderFactory.GetMarketsData()
                                              .First(m => m.Name == "Main Market");

            var sectors = await m_dataProviderFactory.GetDataProvider<ISectorDataProvider>(market).FetchDataAsync(market);
            var companiesTasks = sectors.SelectMany(sector => sector.Industries.Select(async i => await m_dataProviderFactory.GetDataProvider<ICompanyProvider>(market)
                                                                                                                            .FetchDataAsync(i)));
            await Task.WhenAll(companiesTasks);
            var companies = companiesTasks.SelectMany(ct => ct.Result)
                                          .ToList();

            companies.ForEach(company =>
            {
                Task.Run(() => m_dataProviderFactory.GetDataProvider<ICompanyDataProvider>(market)
                                                    .FetchDataAsync(company));
                Task.Run(() => m_dataProviderFactory.GetDataProvider<IStockProvider>(market)
                                                    .FetchDataAsync(company));
            });
        }

        /// <summary>
        /// Initialization logic that any derived type would use to set up the ServiceLocator provider.  Look to UnityServiceHostFactory to see how this is done, if implementing for 
        /// another IoC engine.
        /// </summary>
        protected void RegisterDependencies()
        {
            IDictionary<string, CompanyStatistics> cache = new ConcurrentDictionary<string, CompanyStatistics>();
            IDictionary<string, StockQuote> cache2 = new ConcurrentDictionary<string, StockQuote>();

            m_container.RegisterInstance(cache);
            m_container.RegisterInstance(cache2);
            m_container.RegisterInstance< ICalculatedCompanyDataProvider>(new CalculatedStaticticsProvider(cache2, cache));

            m_dataProviderFactory = new DataProviderFactory(m_container, HostingEnvironment.ApplicationPhysicalPath);
            m_container.RegisterInstance(m_dataProviderFactory);
        }
    }
}