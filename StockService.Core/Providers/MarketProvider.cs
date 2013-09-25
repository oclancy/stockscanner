using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockService.Core;


namespace StockService.Core.Providers
{
    public class MarketDataProvider
    {
        public static List<Market> FetchData()
        {
            return new List<Market> { new Market(){Name="Main Market", Id=0}, 
                                      new Market(){Name="AIM",Id=1}, 
                                      new Market(){Name="NASDAQ",Id=2}};
        }
    }
}