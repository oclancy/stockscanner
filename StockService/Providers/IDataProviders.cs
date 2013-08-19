using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService.Providers
{
    interface ICompanyDataProvider
    {
        Task<CompanyStatistics> FetchDataAsync(string symbol);
    }

    interface ISectorDataProvider
    {
        Task<List<Sector>> FetchDataAsync();
    }

    interface IStockProvider
    {
        Task<StockQuote> FetchDataAsync(string symbol);
    }

    interface ICompanyProvider
    {
        Task<List<Company>> FetchDataAsync(int industry);
    }

}
