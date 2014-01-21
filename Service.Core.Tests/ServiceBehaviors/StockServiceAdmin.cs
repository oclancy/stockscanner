using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockService.Core;
using StockService.Core.DTOs;
using StockService.Core.Providers;

namespace Service.Core.Tests
{
    [TestClass]
    public class StockServiceAdmin
    {
        [TestMethod]
        public void Scan()
        {
            var admin = new StockService.StockServiceAdmin();
            var container = new UnityContainer();

            admin.DataProviderFactory = new StockService.Core.Providers.DataProviderFactory(container);

            admin.Scan();

            Thread.Sleep(60 * 1 * 1000);
        }

        [TestMethod]
        [DeploymentItem("Data")]
        public void ScanCompanies()
        {
            var admin = new StockService.StockServiceAdmin();
            var container = new UnityContainer();

            admin.DataProviderFactory = new StockService.Core.Providers.DataProviderFactory(container);

            admin.ScanCompanies();
        }

        [TestMethod]
        [DeploymentItem("Data")]
        public void ScanCompany()
        {
            var admin = new StockService.StockServiceAdmin();
            var container = new UnityContainer();
            List<Company> companies;

            using (var cxt = new StockScannerContext())
            {
                companies = cxt.Companies
                    .AsNoTracking()
                    .Include("Industry.Sector.Market")
                    .Include("CompanyStatistics")
                    .Include("StockQuote")
                    .Where(c => c.CompanyId >= 1 && c.CompanyId <= 2)
                    .ToList();
                 companies.ForEach(c =>
                    {
                        if (c.CompanyStatistics != null)
                            c.CompanyStatistics.LastUpdated = DateTime.UtcNow.AddDays(-1);

                        if (c.StockQuote != null)
                            c.StockQuote.LastUpdated = DateTime.UtcNow.AddDays(-1);
                    });

               cxt.SaveChanges();
            }

            var dataFactory = new Mock<IDataProviderFactory>();
            var stockProvider = new YahooFinanceStockDataProvider();
            var companyProvider = new YahooCompanyDataProvider();

            dataFactory.Setup(d => d.GetCompanies()).Returns( companies );
            dataFactory.Setup( d=>d.GetDataProvider<ICompanyDataProvider>(It.IsAny<Market>())).Returns( companyProvider );
            dataFactory.Setup(d => d.GetDataProvider<IStockProvider>(It.IsAny<Market>())).Returns(stockProvider);

            admin.DataProviderFactory = dataFactory.Object;
            admin.Scan();

            Thread.Sleep(40000);

            using (var cxt = new StockScannerContext())
            {
                companies = cxt.Companies.Where(c => c.CompanyId >= 1 && c.CompanyId <= 2).ToList();

                Assert.IsTrue(companies.TrueForAll( c => c.CompanyStatistics.LastUpdated.Date == DateTime.UtcNow.Date));
                Assert.IsTrue(companies.TrueForAll( c => c.StockQuote.LastUpdated.Date == DateTime.UtcNow.Date));
            }
        }
    }
}
