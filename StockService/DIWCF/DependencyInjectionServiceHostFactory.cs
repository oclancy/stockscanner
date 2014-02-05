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
        static IUnityContainer m_container = new UnityContainer();

        static Lazy<DataProviderFactory> m_lazyDataProviderFactory = new Lazy<DataProviderFactory>(() => new DataProviderFactory(m_container));

        static Logger m_logger = LogManager.GetCurrentClassLogger();

        //static IDictionary<string, Market> m_markets = new ConcurrentDictionary<string, Market>();
        //static IDictionary<string, Company> m_companies = new ConcurrentDictionary<string, Company>();

        DataProviderFactory DataProviderFactory { get { return m_lazyDataProviderFactory.Value; } }

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

            //HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            //Register the service as a type so it can be found from the instance provider
            m_container.RegisterType(serviceType);

            RegisterDependencies();

            return new DependencyInjectionServiceHost(serviceType, baseAddresses);
        }

        /// <summary>
        /// Initialization logic that any derived type would use to set up the ServiceLocator provider.  Look to UnityServiceHostFactory to see how this is done, if implementing for 
        /// another IoC engine.
        /// </summary>
        private void RegisterDependencies()
        {

            if (m_container.IsRegistered<DataProviderFactory>()) return;

            m_container.RegisterInstance< ICalculatedCompanyDataProvider>(new CalculatedStaticticsProvider());
            m_container.RegisterInstance<IDataProviderFactory>(DataProviderFactory);
         }
    }
}