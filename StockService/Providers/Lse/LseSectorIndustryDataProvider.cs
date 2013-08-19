using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService.Providers
{
    class LseSectorIndustryDataProvider : ISectorDataProvider
    {
        public async Task<List<Sector>> FetchDataAsync()
        {
            return new List<Sector>();
        }
    }
}
