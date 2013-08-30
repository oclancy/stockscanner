using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockService.Core.Providers
{
    public class YahooSectorIndustryDataProvider : ISectorDataProvider
    {
        const string BASE_URL = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.sectors&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";

        public async Task<List<Sector>> FetchDataAsync( )
        {
            var t =  Task.Run(() =>
            {
                return Parse(XDocument.Load(BASE_URL));
            });

            await t;

            return t.Result;
        }

        private static List<Sector> Parse(XDocument doc)
        {
            var retVal = doc.Descendants(@"sector")
                            .Select( sector =>  new Sector( sector.Attribute("name").Value )
                                    {
                                        Industries = new List<Industry>( sector.Elements()
                                                                               .Select(ind => new Industry( int.Parse(ind.Attribute("id").Value),
                                                                                                            ind.Attribute("name").Value)))
                                    });
            
            return retVal.ToList();
        }

    }
}
