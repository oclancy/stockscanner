using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Microsoft.Practices.Unity;
using StockService.Core;
using StockService.DataReader;

namespace StockService.Providers
{
    public class DataProviderFactory
    {
        static IList<Market> m_markets;
        static IUnityContainer m_container;

        public  DataProviderFactory(  )
        {
            m_markets = MarketDataProvider.FetchData();

            var t1 = LseDataReader.Read( m_markets.First(m=> m.Name =="Main Market"),
                                Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "lsedata.csv") );
            
            var t2 = LseDataReader.Read(m_markets.First(m => m.Name == "AIM"),
                                Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "lsedata.csv"));

            Task.WhenAll(t1, t2);

            m_container = new UnityContainer();

            m_container.RegisterInstance<ICompanyDataProvider>("NASDAQ", new YahooCompanyDataProvider());
            m_container.RegisterInstance<ICompanyProvider>("NASDAQ", new YahooCompanyProvider());
            m_container.RegisterInstance<ISectorDataProvider>("NASDAQ", new YahooSectorIndustryDataProvider());
            m_container.RegisterInstance<IStockProvider>("NASDAQ", new YahooStockDataProvider());

            m_container.RegisterInstance<ICompanyDataProvider>("AIM", new LseCompanyDataProvider());
            m_container.RegisterInstance<ICompanyProvider>("AIM", new LseCompanyProvider());
            m_container.RegisterInstance<ISectorDataProvider>("AIM", new LseSectorIndustryDataProvider());
            m_container.RegisterInstance<IStockProvider>("AIM", new LseStockDataProvider());

            m_container.RegisterInstance<ICompanyDataProvider>("Main Market", new LseCompanyDataProvider());
            m_container.RegisterInstance<ICompanyProvider>("Main Market", new LseCompanyProvider());
            m_container.RegisterInstance<ISectorDataProvider>("Main Market", new LseSectorIndustryDataProvider());
            m_container.RegisterInstance<IStockProvider>("Main Market", new LseStockDataProvider());
        }


        internal T GetDataProvider<T>(int market)
        {
            switch(market)
            {
                case 0: return m_container.Resolve<T>("Main Market");
                case 1: return m_container.Resolve<T>("AIM");
                case 2: return m_container.Resolve<T>("NASDAQ");
                default: throw new FaultException("Unknown market");
            }
        }

        internal List<Market> GetMarketsData()
        {
            return MarketDataProvider.FetchData();
        }
    }
}