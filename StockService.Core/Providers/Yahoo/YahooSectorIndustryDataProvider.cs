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

        public async Task<IList<Sector>>FetchDataAsync(Market market)
        {
            return  await Task.Run(() =>
                Parse(XDocument.Load(BASE_URL),market)
            );
        }

        private static IList<Sector> Parse(XDocument doc, Market market)
        {
            var retVal = doc.Descendants(@"sector")
                            .Select(sector => 
                                {
                                    var s = new Sector()
                                    {
                                        Name=sector.Attribute("name").Value,
                                        Market = market
                                    };
                                    s.Industries = new List<Industry>(sector.Elements()
                                                                               .Select(ind => new Industry(){
                                                                                                            IndustryId = int.Parse(ind.Attribute("id").Value),
                                                                                                            Name=ind.Attribute("name").Value, 
                                                                                                            Sector = s }));
                                    return s;
                                });
            
            return retVal.ToList();
        }

    }
}
