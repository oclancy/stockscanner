using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Practices.Unity;
using StockService.Core;

namespace StockService.Core.Providers
{
    public class DataProviderFactory
    {
        static IList<Market> m_markets;
        static IUnityContainer m_container;

        public  DataProviderFactory( IUnityContainer container, string localPath )
        {
            m_container = container;

            m_markets = MarketDataProvider.FetchData();

            var aim = m_markets.First(m => m.Name == "AIM");
            var mm = m_markets.First(m => m.Name == "Main Market");

            LseDataReader.Read( m_markets.First(m=> m.Name =="Main Market"),
                                Path.Combine(localPath, "App_Data", "lsedata.csv"));
            
            LseDataReader.Read(m_markets.First(m => m.Name == "AIM"),
                                Path.Combine(localPath, "App_Data", "lsedata.csv"));


            var nasdaqStockDataProvider = container.Resolve<YahooNasdaqStockDataProvider>();
            var yahooFinanceStockDataProvider = container.Resolve <YahooFinanceStockDataProvider>();
            var companyDataProvider = container.Resolve<YahooCompanyDataProvider>();

            m_container.RegisterInstance<ICompanyDataProvider>("NASDAQ", companyDataProvider);
            m_container.RegisterInstance<ICompanyProvider>("NASDAQ", new YahooCompanyProvider());
            m_container.RegisterInstance<ISectorDataProvider>("NASDAQ", new YahooSectorIndustryDataProvider());
            m_container.RegisterInstance<IStockProvider>("NASDAQ", nasdaqStockDataProvider);

            m_container.RegisterInstance<ICompanyDataProvider>("AIM", companyDataProvider);
            m_container.RegisterInstance<ICompanyProvider>("AIM", new LseCompanyProvider(aim));
            m_container.RegisterInstance<ISectorDataProvider>("AIM", new LseSectorIndustryDataProvider(aim));
            m_container.RegisterInstance<IStockProvider>("AIM", yahooFinanceStockDataProvider);

            m_container.RegisterInstance<ICompanyDataProvider>("Main Market", companyDataProvider);
            m_container.RegisterInstance<ICompanyProvider>("Main Market", new LseCompanyProvider(mm));
            m_container.RegisterInstance<ISectorDataProvider>("Main Market", new LseSectorIndustryDataProvider(mm));
            m_container.RegisterInstance<IStockProvider>("Main Market", yahooFinanceStockDataProvider);
        }


        public T GetDataProvider<T>(Market market)
        {
            switch(market.Id)
            {
                case 0: return m_container.Resolve<T>("Main Market");
                case 1: return m_container.Resolve<T>("AIM");
                case 2: return m_container.Resolve<T>("NASDAQ");
                default: throw new FaultException("Unknown market");
            }
        }

        public List<Market> GetMarketsData()
        {
            return MarketDataProvider.FetchData();
        }
    }
}