using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace StockService.Core.Providers
{
    public class YahooCompanyDataProvider : ICompanyDataProvider
    {
        const string BASE_URL = "http://finance.yahoo.com/q/ks?s={0}+Key+Statistics";

        public async Task<CompanyStatistics> FetchDataAsync( string symbol )
        {
            var webReq=WebRequest.CreateHttp(string.Format(BASE_URL, symbol));

            var t = await webReq.GetResponseAsync();

            var doc = new HtmlDocument();
            doc.Load(t.GetResponseStream());
            var cs = Parse(doc);
            cs.Symbol = symbol;
            return cs;
        }

        private static CompanyStatistics Parse(HtmlDocument doc)
        {
            var root = doc.DocumentNode;
            var tables = root.Descendants("table");
            var values = tables.Where( table => table.Attributes.FirstOrDefault( a=> a.Name=="class" && a.Value=="yfnc_datamodoutline1") != null )
                  .SelectMany( table => table.Descendants("table")
                                           .SelectMany( node => node.Descendants("tr") )
                  .Select( row =>
                  {
                    var nameCell = row.Elements("td")
                                      .Where( cell => cell.Attributes.FirstOrDefault( a=> a.Name=="class" && a.Value=="yfnc_tablehead1" ) != null )
                                      .FirstOrDefault();

                    var valueCell = row.Elements("td")
                                      .Where( cell => cell.Attributes.FirstOrDefault( a=> a.Name=="class" && a.Value=="yfnc_tabledata1" ) != null )
                                      .FirstOrDefault();

                    if (valueCell == null || nameCell == null)
                        return null;

                    if(valueCell.HasChildNodes)
                        valueCell = valueCell.FirstChild;

                    return new Tuple<string,string>(nameCell.InnerText, valueCell.InnerText);
                  }))
                  .Where( t => t!=null)
                  .Distinct()
                  .ToDictionary( kvp => kvp.Item1, kvp => kvp.Item2);

            return new CompanyStatistics(values);
        }

    }
}
