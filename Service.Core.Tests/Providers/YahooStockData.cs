using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockService.Core;
using StockService.Core.Providers;


namespace Service.Core.Tests
{
    [TestClass]
    public class YahooFinanceStockData
    {
        YahooFinanceStockDataProvider m_stockdataprovider = new YahooFinanceStockDataProvider();
        YahooCompanyDataProvider m_companydataprovider = new YahooCompanyDataProvider();

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void GetYahooStockData()
        {
            StockQuote cs;
            var y = m_stockdataprovider.FetchDataAsync(new Company() { Name = "Yahoo", Symbol = "YHOO.L" });

            y.Wait();

            Assert.IsNull(y.Exception);
            Assert.IsTrue(y.IsCompleted);
            cs = y.Result;

        }

        [TestMethod]
        public void GetCLPStockData()
        {
            StockQuote cs;
            var y = m_stockdataprovider.FetchDataAsync(new Company() { Name = "CLEAR LEISURE", Symbol = "CLP.L" });

            y.Wait();

            Assert.IsNull(y.Exception);
            Assert.IsTrue(y.IsCompleted);
            cs = y.Result;

        }

        [TestMethod]
        public void GetASHMStockData()
        {
            StockQuote cs;
            var y = m_stockdataprovider.FetchDataAsync(new Company() { Name = "Ashmore Group PLC", Symbol = "ASHM.L" });

            y.Wait();

            Assert.IsNull(y.Exception);
            Assert.IsTrue(y.IsCompleted);
            cs = y.Result;

            Assert.IsTrue(cs.Open.HasValue);
            Assert.IsTrue(cs.Bid.HasValue);
            Assert.IsTrue(cs.Ask.HasValue);
            Assert.IsTrue(cs.DailyHigh.HasValue);
            Assert.IsTrue(cs.DailyLow.HasValue);
            Assert.IsTrue(cs.YearlyHigh.HasValue);
            Assert.IsTrue(cs.YearlyLow.HasValue);
            Assert.IsTrue(cs.PreviousClose.HasValue);
            Assert.IsTrue(cs.Volume.HasValue);
            Assert.IsTrue(cs.AverageDailyVolume.HasValue);
            Assert.IsTrue(cs.MarketCapitalization.HasValue);
            Assert.IsTrue(cs.PeRatio.HasValue);
            Assert.IsTrue(cs.EarningsShare.HasValue);
            Assert.IsTrue(cs.DividendShare.HasValue);
            Assert.IsTrue(cs.PegRatio.HasValue);

        }

        [TestMethod]
        public void GetCLPCompanyData()
        {
            CompanyStatistics cs;
            var y = m_companydataprovider.FetchDataAsync(new Company() { Name = "CLEAR LEISURE", Symbol = "CLP.L" });

            y.Wait();

            Assert.IsNull(y.Exception);
            Assert.IsTrue(y.IsCompleted);
            cs = y.Result;

        }
    }
}
