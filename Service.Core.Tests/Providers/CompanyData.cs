using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockService.Core;
using StockService.Core.Providers;


namespace Service.Core.Tests
{
    [TestClass]
    public class YahooFinanceCompanyData
    {
        YahooCompanyDataProvider m_companydataprovider = new YahooCompanyDataProvider();
        
        [TestMethod]
        public void GetYahooFinanceCompanyData()
        {
            CompanyStatistics cs;
            var y = m_companydataprovider.FetchDataAsync(new Company() { Name = "Yahoo", Symbol = "YHOO" });

            y.Wait();

            Assert.IsNull(y.Exception);
            Assert.IsTrue(y.IsCompleted);
            cs = y.Result;


        }

        [TestMethod]
        public void GetBATSCompanyData()
        {
            CompanyStatistics cs;
            var y = m_companydataprovider.FetchDataAsync(new Company()
            {
                Name = "British American Tobacco",
                Symbol = "BATS",
                Industry = new Industry() { Sector = new Sector() { Market = new Market() { Symbol = "L" } } }
            });

            y.Wait();

            Assert.IsNull(y.Exception);
            Assert.IsTrue(y.IsCompleted);
            cs = y.Result;

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
