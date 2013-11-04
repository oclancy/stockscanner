using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Practices.Unity;
using NLog;
using StockService.Core;
using StockService.Core.DTOs;
using StockService.Core.Extension;

namespace StockService.Core.Providers
{
    public class DataProviderFactory
    {
        static IUnityContainer m_container;
        Logger m_logger = LogManager.GetCurrentClassLogger();

        IDictionary<string, Market> m_markets;

        public DataProviderFactory( IUnityContainer container, 
                                    string localPath,
                                    IDictionary<string, Market> markets,
                                    IDictionary<string, Company> companies)
        {
            m_container = container;

            MarketDataProvider.FetchData()
                              .ForEach(m => markets.Add(m.Name, m));

            m_markets = markets;

            var aim = markets["AIM"];
            var mm = markets["Main Market"];

            LseDataReader.Read( mm, Path.Combine(localPath, "App_Data", "lsedata.csv"));
            LseDataReader.Read( aim, Path.Combine(localPath, "App_Data", "lsedata.csv"));

            markets.ForEach(m => m.Value.Sectors.SelectMany(s => s.Industries.SelectMany(i => i.Companies))
                                    .ForEach(c =>
                                    {
                                        if (!companies.ContainsKey(c.Symbol))
                                            companies.Add(c.Symbol, c);
                                    }));

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
            switch(market.MarketId)
            {
                case 1: return m_container.Resolve<T>("Main Market");
                case 2: return m_container.Resolve<T>("AIM");
                case 3: return m_container.Resolve<T>("NASDAQ");
                default: throw new FaultException("Unknown market");
            }
        }

        public List<Market> GetMarketsData()
        {
            return m_markets.Values.ToList();
        }
    }
}