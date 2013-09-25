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
        Task<CompanyStatistics> FetchDataAsync(Company company);
    }
    
    public interface ICalculatedCompanyDataProvider
    {
        CalculatedData FetchData(Company company);
    }

    public interface ISectorDataProvider
    {
        Task<List<Sector>> FetchDataAsync(Market market);
    }

    public interface IStockProvider
    {
        Task<StockQuote> FetchDataAsync(Company company);
    }

    public interface ICompanyProvider
    {
        Task<List<Company>> FetchDataAsync(Industry company);
    }

}
