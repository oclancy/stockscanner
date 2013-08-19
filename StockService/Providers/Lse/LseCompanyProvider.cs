using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService.Providers
{
    class LseCompanyProvider : ICompanyProvider
    {
        public async Task<List<Company>> FetchDataAsync(int industry)
        {
            return new List<Company>();
        }
    }
}
