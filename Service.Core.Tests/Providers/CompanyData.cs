using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockService.Core;
using StockService.Core.Providers;


namespace Service.Core.Tests
{
    [TestClass]
    public class CompanyData
    {
        YahooCompanyDataProvider m_provider = new YahooCompanyDataProvider();
        
        [TestMethod]
        public void GetYahoo()
        {
            CompanyStatistics cs;
            var y = m_provider.FetchDataAsync("YHOO");

            y.Wait();

            Assert.IsNull(y.Exception);
            Assert.IsTrue(y.IsCompleted);
            cs = y.Result;


        }
    }
}
