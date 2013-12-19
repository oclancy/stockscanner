using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using StockService.Core;

namespace StockService.Core.Providers
{
    class LseCompanyProvider : ICompanyProvider
    {
        private string m_marketId;

        [Dependency]
        public IDictionary<string, Market> Cache{get;set;}

        public LseCompanyProvider(string marketId)
        {
            this.m_marketId = marketId;
        }

        public async Task<IList<Company>> FetchDataAsync(Industry industry)
        {
            return await Task.Run( () => 
                           Cache[m_marketId]
                           .Sectors
                           .First( s => s.Industries.FirstOrDefault( i => i.Name == industry.Name ) != null )
                           .Industries
                           .First( i => i.Name == industry.Name )
                           .Companies);
        }
    }
}
