using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockService.Core
{
    public class YahooCompanyDataProvider
    {
        const string BASE_URL = "http://finance.yahoo.com/q/ks?s={0}+Key+Statistics";

        public async static Task<CompanyStatistics> FetchDataAsync( string symbol )
        {
            var t =  Task.Run(() =>
            {
                string url = string.Format(BASE_URL, symbol);

                return Parse(XDocument.Load(url));
            });

            await t;

            return t.Result;
        }

        private static CompanyStatistics Parse(XDocument doc)
        {
            return new CompanyStatistics();
        }

    }
}
