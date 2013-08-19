using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockService.Core;
using StockService.Providers;

namespace StockService.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class YahooCompanyProvider : ICompanyProvider
    {
        private const string BASE_URL = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.industry%20where%20id%3D%22{0}%22&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";

        public async Task<List<Company>> FetchDataAsync(int industry)
        {
            var t = Task.Run( () =>
            {
                string url = string.Format(BASE_URL, industry);

                return Parse(XDocument.Load(url));
            });

            await t;

            return t.Result;

        }

        private static List<Company> Parse(XDocument doc)
        {
            var retVal = doc.Descendants(@"company")
                .Select(company => new Company(company.Attribute("name").Value, company.Attribute("symbol").Value));

            return retVal.ToList();
        }

   }

}
