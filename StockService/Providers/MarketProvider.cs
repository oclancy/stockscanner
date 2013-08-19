using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockService.Core;


namespace StockService.Providers
{
    public class MarketDataProvider
    {
        public static List<Market> FetchData()
        {
            return new List<Market> { new Market("Main Market",0), 
                                      new Market("AIM",1), 
                                      new Market("NASDAQ",2) };
        }
    }
}