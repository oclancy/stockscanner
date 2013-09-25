using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockService.Core.Providers;

namespace StockService.Core
{
    public class CalculatedStaticticsProvider : ICalculatedCompanyDataProvider
    {
        IDictionary<string, StockQuote> m_stockQuoteCache;
        IDictionary<string,CompanyStatistics> m_companyDataCache;

        public CalculatedStaticticsProvider(IDictionary<string,StockQuote> stockQuoteCache,
                                            IDictionary<string,CompanyStatistics> companyDataCache)
        {
            m_companyDataCache = companyDataCache;
            m_stockQuoteCache = stockQuoteCache;
        }

        public CalculatedData FetchData(Company company)
        {
            if (!m_stockQuoteCache.ContainsKey(company.Symbol) && !m_companyDataCache.ContainsKey(company.Symbol)) 
                return null;

            return new CalculatedData();
        }
    }
}
