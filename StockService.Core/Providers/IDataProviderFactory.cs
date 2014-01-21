using System;
using System.Collections.Generic;
namespace StockService.Core.Providers
{
    public interface IDataProviderFactory
    {
        IEnumerable<StockService.Core.Company> GetCompanies();
        List<StockService.Core.Market> GetMarketsData();
        void LoadCaches();
        T GetDataProvider<T>(Market market);
    }
}
