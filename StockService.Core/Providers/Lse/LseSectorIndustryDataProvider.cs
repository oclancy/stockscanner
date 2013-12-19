using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using StockService.Core;

namespace StockService.Core.Providers
{
    class LseSectorIndustryDataProvider : ISectorDataProvider
    {
        private string m_marketId;

        [Dependency]
        public IDictionary<string, Market> Cache{get;set;}

        public LseSectorIndustryDataProvider(string marketId)
        {
            // TODO: Complete member initialization
            this.m_marketId = marketId;
        }

        public async Task<IList<Sector>> FetchDataAsync(Market market)
        {
            return await Task.Run(() => Cache[m_marketId].Sectors);
        }
    }
}
