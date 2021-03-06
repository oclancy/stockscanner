﻿using System;
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
using System.IO;
using System.Reflection;
using System.Data;

namespace Data.Tests
{
    [TestClass]
    public class DbContext
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

        [TestMethod]
        public void UpdateStatistics()
        {
            const string CompanyName = "Test1";
            var company = new Company() { Name = CompanyName  };

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

            CompanyStatistics compStats = new CompanyStatistics();
            using (var cxt = new StockScannerContext())
            {
                var comp = cxt.Companies.First(c => c.Name == CompanyName);
                comp.CompanyStatistics = compStats;
                comp.CompanyStatistics.LastUpdated = new DateTime(2014, 1, 1);
                cxt.SaveChanges();
            }

            Company comp2;            
            using (var cxt = new StockScannerContext())
            {
                compStats = cxt.Companies.First(c => c.Name == CompanyName).CompanyStatistics;
                Assert.AreEqual(new DateTime(2014, 1, 1), compStats.LastUpdated);
                comp2 = compStats.Company;
                Assert.IsNotNull(comp2);
            }
            
            comp2.CompanyStatistics.LastUpdated = new DateTime(2014, 1, 2);

            using (var cxt = new StockScannerContext())
            {
                cxt.Companies.Attach(comp2);
                cxt.Entry(comp2.CompanyStatistics).State = EntityState.Modified;
                cxt.SaveChanges();
            }

            using (var cxt = new StockScannerContext())
            {
                compStats = cxt.Companies.First(c => c.Name == CompanyName).CompanyStatistics;
                Assert.AreEqual(new DateTime(2014, 1, 2), compStats.LastUpdated);
                comp2 = compStats.Company;
                Assert.IsNotNull(comp2);
            }
        }

        [TestMethod]
        public void UpdatePropertyOfDetachedObject()
        {
            const string CompanyName = "Test1";
            var company = new Company() { Name = CompanyName };

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

            CompanyStatistics compStats = new CompanyStatistics();
            Company comp;
            using (var cxt = new StockScannerContext())
            {
                cxt.Configuration.ProxyCreationEnabled = false;
                comp = cxt.Companies.First(c => c.Name == CompanyName);
            }

            comp.CompanyStatistics = compStats;
            comp.CompanyStatistics.CompanyId = comp.CompanyId;

            using (var cxt = new StockScannerContext())
            {
                //cxt.Companies.Attach(comp); // can attach but not neccesary
                cxt.Entry(comp.CompanyStatistics).State = EntityState.Added;
                cxt.SaveChanges();
            }

            using (var cxt = new StockScannerContext())
            {
                comp = cxt.Companies.First(c => c.Name == CompanyName);
                Assert.IsNotNull(comp.CompanyStatistics);
            }


        }
    }
}
