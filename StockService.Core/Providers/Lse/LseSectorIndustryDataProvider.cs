using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService.Core.Providers
{
    class LseSectorIndustryDataProvider : ISectorDataProvider
    {
        private Market m_market;

        public LseSectorIndustryDataProvider(Market aim)
        {
            // TODO: Complete member initialization
            this.m_market = aim;
        }

        public async Task<List<Sector>> FetchDataAsync(Market market)
        {
            return await Task.Run(() => m_market.Sectors);
        }
    }
}
