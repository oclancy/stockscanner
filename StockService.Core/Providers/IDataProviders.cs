using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService.Core.Providers
{
    public interface ICompanyDataProvider
    {
        Task<CompanyStatistics> FetchDataAsync(string symbol);
    }

    public interface ISectorDataProvider
    {
        Task<List<Sector>> FetchDataAsync();
    }

    public interface IStockProvider
    {
        Task<StockQuote> FetchDataAsync(string symbol);
    }

    public interface ICompanyProvider
    {
        Task<List<Company>> FetchDataAsync(int industry);
    }

}
