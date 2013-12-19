using System;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            admin.ScanCompanies();
        }
    }
}
