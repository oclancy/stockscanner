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
                markets = dbcontext.Markets
                                   .Include("Sectors.Industries.Companies")
                                   .ToList();

                if (markets.Any())
                    return markets;

                markets = new List<Market> { new Market(){Name="Main Market", MarketId=1, Symbol="L"}, 
                                             new Market(){Name="AIM", MarketId=2, Symbol="L"}, 
                                             new Market(){Name="NASDAQ", MarketId=3, Symbol=string.Empty}};

                markets.ForEach(m=> dbcontext.Markets.Add(m));

                dbcontext.SaveChanges();
            }
            return markets;
        }
    }
}