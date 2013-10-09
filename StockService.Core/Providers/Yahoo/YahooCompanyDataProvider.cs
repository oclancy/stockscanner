using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.Practices.Unity;
using StockService.Core.Extension;

namespace StockService.Core.Providers
{
    public class YahooCompanyDataProvider : ICompanyDataProvider
    {
        //[Dependency]
        //public IDictionary<string, CompanyStatistics> Cache { get; set; }

        [Dependency]
        public IDictionary<string, Company> Cache { get; set; }

        const string BASE_URL = "http://finance.yahoo.com/q/ks?s={0}+Key+Statistics";

        public async Task<CompanyStatistics> FetchDataAsync( Company company )
        {
            if (Cache.ContainsKey(company.Symbol) && 
                Cache[company.Symbol].CompanyStatistics != null &&
                Cache[company.Symbol].CompanyStatistics.LastUpdated > DateTime.UtcNow.AddDays(-1) )
                return Cache[company.Symbol].CompanyStatistics;

            var webReq = WebRequest.CreateHttp(string.Format(BASE_URL, company.Symbol));

            var t = await webReq.GetResponseAsync();

            var doc = new HtmlDocument();
            doc.Load(t.GetResponseStream());
            var cs = Parse(doc);
            //cs.Company = company;
            //company.CompanyStatistics = cs;
            cs.LastUpdated = DateTime.UtcNow;

            return cs;
        }

        private static CompanyStatistics Parse(HtmlDocument doc)
        {
            var retVal = new Dictionary<string, string>();
            var root = doc.DocumentNode;
            var tables = root.Descendants("table");
            var values = tables.Where(table => table.Attributes.FirstOrDefault(a => a.Name == "class" && a.Value == "yfnc_datamodoutline1") != null)
                  .SelectMany(table => table.Descendants("table")
                                           .SelectMany(node => node.Descendants("tr"))
                  .Select(row =>
                  {
                      var nameCell = row.Elements("td")
                                        .Where(cell => cell.Attributes.FirstOrDefault(a => a.Name == "class" && a.Value == "yfnc_tablehead1") != null)
                                        .FirstOrDefault();

                      var valueCell = row.Elements("td")
                                        .Where(cell => cell.Attributes.FirstOrDefault(a => a.Name == "class" && a.Value == "yfnc_tabledata1") != null)
                                        .FirstOrDefault();

                      if (valueCell == null || nameCell == null)
                          return null;

                      if (valueCell.HasChildNodes)
                          valueCell = valueCell.FirstChild;

                      return new Tuple<string, string>(nameCell.InnerText.FormatStatistic(), valueCell.InnerText);
                  }))
                  .Where(t => t != null);

            values.ForEach( v =>
            {
                if(!retVal.ContainsKey(v.Item1)) 
                    retVal.Add(v.Item1, v.Item2);
            });

            return CompanyStatistics.FromYahooValues(retVal);
        }

    }
}
