using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.Practices.Unity;
using NLog;
using StockService.Core.DTOs;
using StockService.Core.Extension;

namespace StockService.Core.Providers
{
    public class YahooCompanyDataProvider : ICompanyDataProvider
    {
        //[Dependency]
        //public IDictionary<string, CompanyStatistics> Cache { get; set; }

        const string BASE_URL = "http://finance.yahoo.com/q/ks?s={0}+Key+Statistics";
        private Logger m_logger = LogManager.GetCurrentClassLogger();

        public async Task<CompanyStatistics> FetchDataAsync( Company company )
        {
            if (company.CompanyStatistics != null &&
                company.CompanyStatistics.LastUpdated > DateTime.UtcNow.AddDays(-1)) return company.CompanyStatistics;

            HttpWebRequest webReq = null;
            var doc = new HtmlDocument();
            if (company.CompanyStatistics == null) company.CompanyStatistics = new CompanyStatistics() { CompanyId = company.CompanyId }; 

            try
            {
                webReq = WebRequest.CreateHttp(string.Format(BASE_URL, company.Symbol + "." + company.Industry.Sector.Market.Symbol));
                var t = await webReq.GetResponseAsync();
                using (var stream = t.GetResponseStream())
                {
                    doc.Load(stream);
                }

                Parse(doc, company.CompanyStatistics);
            }
            catch (WebException ex)
            {
                m_logger.ErrorException(string.Format("Exception whilst retrieving url data: {0}", webReq.RequestUri), ex);
            }
            catch (Exception ex)
            {
                m_logger.ErrorException(string.Format("Unknown Exception whilst getting data: {0}", webReq.RequestUri), ex);
            }

            return company.CompanyStatistics;
        }

        private static void Parse(HtmlDocument doc, CompanyStatistics cs)
        {
            var retVal = new Dictionary<string, List<string>>();
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
                    retVal.Add(v.Item1, new List<string>(){v.Item2});
                else
                    retVal[v.Item1].Add(v.Item2);
            });

            CompanyStatistics.FromYahooValues(retVal, ref cs);
            cs.LastUpdated = DateTime.UtcNow;
        }

    }
}
