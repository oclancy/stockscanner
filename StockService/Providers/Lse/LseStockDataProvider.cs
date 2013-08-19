using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService.Providers
{
    class LseStockDataProvider : IStockProvider
    {
        public async Task<StockQuote> FetchDataAsync(string symbol)
        {
            return new StockQuote();
        }
    }
}
