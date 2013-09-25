using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockService.Core;
using StockService.Core.Providers;


namespace Service.Core.Tests
{
    [TestClass]
    public class YahooFinanceStockData
    {
        YahooFinanceStockDataProvider m_provider = new YahooFinanceStockDataProvider();
        
        [TestMethod]
        public void GetYahooStockData()
        {
            StockQuote cs;
            var y = m_provider.FetchDataAsync(new Company() { Name = "Yahoo", Symbol = "YHOO" });

            y.Wait();

            Assert.IsNull(y.Exception);
            Assert.IsTrue(y.IsCompleted);
            cs = y.Result;

        }
    }
}
