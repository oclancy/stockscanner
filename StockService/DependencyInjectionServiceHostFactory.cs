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

namespace StockService
{
    /// <summary>
    /// Produces <see cref="DependencyInjectionServiceHost"/>s.
    /// </summary>
    public class DependencyInjectionServiceHostFactory : ServiceHostFactory
    {
        IUnityContainer m_container = new UnityContainer();

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

            return new ServiceHost(serviceType, baseAddresses);
        }

        /// <summary>
        /// Initialization logic that any derived type would use to set up the ServiceLocator provider.  Look to UnityServiceHostFactory to see how this is done, if implementing for 
        /// another IoC engine.
        /// </summary>
        protected void RegisterDependencies()
        {
            DataProviderFactory m_dataProviderFactory = new DataProviderFactory(m_container, HostingEnvironment.ApplicationPhysicalPath);

            IDictionary<string, CompanyStatistics> cache = new Dictionary<string, CompanyStatistics>();

            m_container.RegisterInstance("DataProviderFactory", m_dataProviderFactory);
            m_container.RegisterInstance("CompanyDataCache", cache);
        }
    }
}