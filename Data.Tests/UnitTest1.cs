using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using StockService.Core.DTOs;
using System.Data.Entity;
using StockService.Core;
using StockService.Core.Providers;
using System.Reactive.Linq;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using StockService.Core.Extension;

namespace Data.Tests
{
    [TestClass]
    public class DbContext
    {
        [TestInitialize]
        public void Initialise()
        {
            using (var dbcontext = new StockScannerContext())
            {
                if (dbcontext.Database.Exists())
                { 
                    dbcontext.Database.Delete(); 
                }
                dbcontext.Database.Create();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var dbcontext = new StockScannerContext())
            {
                if (dbcontext.Database.Exists())
                    dbcontext.Database.Delete();
            }
        }

        [TestMethod]
        public void BasicTest()
        {
            var market = new Market() { Name = "Market1" };

            var sector = new Sector(){ Name="Sector1", Market=market};

            var industry = new Industry(){ Name="Industry", Sector= sector};

            var company = new Company(){ Name="Company", Industry = industry};

            industry.Companies.Add(company);
            sector.Industries.Add(industry);
            market.Sectors.Add(sector);

            using (var dbcontext = new StockScannerContext())
            {
                dbcontext.Markets.Add(market);

                //dbcontext.Sectors.Add(sector);

                //dbcontext.Industries.Add(industry);
                
                //dbcontext.Companies.Add(company);

                dbcontext.SaveChanges();
            }


            using (var dbcontext = new StockScannerContext())
            {
                Assert.AreEqual(1, dbcontext.Markets.Count());

                var m = dbcontext.Markets
                                 .FirstOrDefault();

                Assert.IsNotNull(m);

                Assert.AreEqual(1, m.Sectors.Count);

                var s = m.Sectors.First();

                Assert.AreEqual(1, s.Industries.Count);

                var i = s.Industries.First();

                Assert.AreEqual(1, i.Companies.Count);

                var c = i.Companies.First();


            }

        }

        [TestMethod]
        public void LoadSim()
        {
            var markets = MarketDataProvider.FetchData();

            using (var context = new StockScannerContext())
            {
                Assert.AreEqual(3, context.Markets.Count());

                var market = context.Markets.First();

                Observable.Range(0, 10)
                    .ForEach(i => market.Sectors.Add(new Sector() { Name = "Sector" + i, Market = market, MarketId = market.MarketId }));

                Observable.Range(0, 10)
                    .ForEach(i => market.Sectors.ForEach(s => s.Industries.Add(new Industry() { Name = "Industry" + i, Sector = s, SectorId = s.SectorId })));

                Observable.Range(0, 10)
                    .ForEach(i => market.Sectors
                                        .SelectMany(s => s.Industries)
                                        .ToList()
                                        .ForEach(ind => ind.Companies.Add(new Company() { Name = "Company" + i, Industry = ind, IndustryId = ind.IndustryId })));
                
                //market.Sectors
                //      .SelectMany( s=> s.Industries )
                //      .ToList()
                //      .SelectMany( i=>i.Companies)
                //      .ToList()
                //      .ForEach( c => dbcontext.Companies.Add(c));

                context.SaveChanges();
            }


            using (var dbcontext = new StockScannerContext())
            {
                Assert.AreEqual(3, dbcontext.Markets.Count());

                var m = dbcontext.Markets
                                 .FirstOrDefault();

                Assert.IsNotNull(m);

                Assert.AreEqual(10, m.Sectors.Count);

                var s = m.Sectors.First();

                Assert.AreEqual(10, s.Industries.Count);

                var i = s.Industries.First();

                Assert.AreEqual(10, i.Companies.Count);

                var c = i.Companies.First();
            }
        }

        [TestMethod]
        public void LoadFromFile()
        {
            var markets = new Dictionary<string,Market>();
            var companies = new Dictionary<string, Company>();
            var container = new UnityContainer();

            container.RegisterInstance<IDictionary<string,Company>>( new Dictionary<string,Company>() );

            var m_dataProviderFactory = new DataProviderFactory(container, Environment.CurrentDirectory, markets, companies);

            Assert.AreEqual(3, markets.Count);
            Assert.AreEqual(23, markets.SelectMany(m=>m.Value.Sectors).Count());

            markets.Clear();
            companies.Clear();
            m_dataProviderFactory = new DataProviderFactory(container, Environment.CurrentDirectory, markets, companies);

            Assert.AreEqual(3, markets.Count);
            Assert.AreEqual(23, markets.SelectMany(m => m.Value.Sectors).Count());
        }

        [TestMethod]
        public void SaveStatistics()
        {
            var company = new Company();
            using (var cxt = new StockScannerContext())
            {
                var market = new Market();
                var ind = new Industry();
                var sec = new Sector();
                ind.Companies.Add(company);
                sec.Industries.Add(ind);
                market.Sectors.Add(sec);
                cxt.Markets.Add(market);

                cxt.SaveChanges();
            }


            using (var cxt = new StockScannerContext())
            {
                cxt.Companies.Attach(company);
            }

        }
    }
}
