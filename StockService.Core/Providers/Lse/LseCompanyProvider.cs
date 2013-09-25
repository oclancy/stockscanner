using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService.Core.Providers
{
    class LseCompanyProvider : ICompanyProvider
    {
        private Market m_market;

        public LseCompanyProvider(Market aim)
        {
            // TODO: Complete member initialization
            this.m_market = aim;
        }
        public async Task<List<Company>> FetchDataAsync(Industry industry)
        {
            return await Task.Run( () =>m_market.Sectors
                           .First( s => s.Industries.FirstOrDefault( i => i.Name == industry.Name ) != null )
                           .Industries
                           .First( i => i.Name == industry.Name )
                           .Companies);
        }
    }
}
