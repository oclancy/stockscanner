using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService.Providers
{
    class LseCompanyDataProvider : ICompanyDataProvider
    {
        public async Task<CompanyStatistics> FetchDataAsync(string symbol)
        {
            return new CompanyStatistics();
        }
    }
}
