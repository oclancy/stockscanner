using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockService.Core;
using StockService.Core.DTOs;
using System.Data.Entity;

namespace StockService.Core.Providers
{
    public class MarketDataProvider
    {
        public static List<Market> FetchData()
        {
            List<Market> markets;
            using (var dbcontext = new StockScannerContext())
            {
                //dbcontext.Configuration.ProxyCreationEnabled = false;
                //dbcontext.Markets.Include(c => c.Sectors).Load();
                //dbcontext.Sectors.Include(c => c.Industries).Load();
                //dbcontext.Industries.Include(c => c.Companies).Load();
                //dbcontext.Companies.Include(c => c.StockQuote).Load();
                //dbcontext.Companies.Include(c => c.CompanyStatistics).Load();

                markets = dbcontext.Markets.ToList();

                if (markets.Any())
                {
                    var @throw = markets.SelectMany(m=>m.Sectors.SelectMany(s=> s.Industries.SelectMany(i=>i.Companies))).ToList();
                    return markets;
                }

                markets = new List<Market> { new Market(){Name="Main Market", MarketId=1}, 
                                             new Market(){Name="AIM", MarketId=2}, 
                                             new Market(){Name="NASDAQ", MarketId=3}};

                markets.ForEach(m=> dbcontext.Markets.Add(m));

                dbcontext.SaveChanges();
            }
            return markets;
        }
    }
}