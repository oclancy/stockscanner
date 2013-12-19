using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockService.Core;
using StockService.Core.Providers;
using System.Linq;

namespace Data.Tests
{
    [TestClass]
    public class DataFactoryTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get;
            set;
        }


        [TestMethod]
        [DeploymentItem("App_Data")]
        public void LoadData_PopulateCache()
        {
            var container = new UnityContainer();
            
            container.RegisterInstance<IDictionary<string, Company>>(new Dictionary<string, Company>());

            LseDataReader.Read(new Market() { Name = "Aim", MarketId=2 }, Path.Combine(TestContext.DeploymentDirectory, @"lsedata.csv"));

            var dpf = new DataProviderFactory(container);

            var markets = container.Resolve<IDictionary<string, Market>>();
            var companies = container.Resolve<IDictionary<string, Company>>();

            Assert.AreEqual(3, markets.Count);
            Assert.AreEqual(11, markets.SelectMany(m => m.Value.Sectors).Count());

            Assert.AreEqual(1117, companies.Count());

        
        }
    }
}
