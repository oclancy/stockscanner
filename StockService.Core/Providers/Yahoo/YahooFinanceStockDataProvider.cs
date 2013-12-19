using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockService.Core;

namespace StockService.Core.Providers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using HtmlAgilityPack;
    using Microsoft.Practices.Unity;
using NLog;
    using StockService.Core.DTOs;

    public class YahooFinanceStockDataProvider : IStockProvider
    {
        [Dependency]
        public IDictionary<string, Company> Cache { get; set; }

        private const string BASE_URL = "http://finance.yahoo.com/q?s={0}&ql=1";
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<StockQuote> FetchDataAsync(Company company)
        {
            using (var cxt = new StockScannerContext())
            {
                company = cxt.Companies.FirstOrDefault(c => c.CompanyId == company.CompanyId);

                if (company.StockQuote != null &&
                    company.StockQuote.LastUpdated > DateTime.UtcNow.AddDays(-1)) return company.StockQuote;
            }

            HttpWebRequest webReq = null;
            var doc = new HtmlDocument();
            var cs = company.StockQuote ?? new StockQuote() { Company = company };
            try
            {
                webReq = WebRequest.CreateHttp(string.Format(BASE_URL, company.Symbol));
                var t = await webReq.GetResponseAsync();
                using (var stream = t.GetResponseStream())
                {
                    doc.Load(stream);
                }
                Parse(doc.DocumentNode.Descendants("body").First(), cs);
            }
            catch (WebException ex)
            {
                Logger.ErrorException(string.Format("Exception whilst retrieving url data: {0}", webReq.RequestUri), ex);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(string.Format("Unknown Exception whilst getting data: {0}", webReq.RequestUri), ex);
            }

            cs.LastUpdated = DateTime.UtcNow;
            return cs;
        }

        private static void Parse(HtmlNode body, StockQuote quote)
        {
            var infoDiv = body.SelectSingleNode("div[@id='yfi_doc']/div[@id='yfi_bd']/div[@id='yfi_investing_content']");

            var priceDiv = infoDiv.SelectSingleNode("div[@class='rtq_div']/div[@class='yui-g']/div[@id='yfi_rt_quote_summary']/div[@class='yfi_rt_quote_summary_rt_top']");

            var statistics = infoDiv.SelectSingleNode("div[@class='yui-u first yfi-start-content']").Descendants("table").ToArray();

            var analytics = infoDiv.SelectSingleNode("div[4]/div[@id='yfi_analysts']/div[2]/table");

            if (!statistics.Any()) return;

            HtmlNode node = statistics[0].SelectSingleNode("tr[4]/td");
            if(node!=null)
                quote.Ask = GetDecimal(node.InnerText);
                
            node = statistics[0].SelectSingleNode("tr[3]/td");
            if (node != null)
                quote.Bid = GetDecimal(node.InnerText);

            node = statistics[0].SelectSingleNode("tr[2]/td");
            if (node != null) 
                quote.Open = GetDecimal(node.InnerText);

            node = statistics[0].SelectSingleNode("tr[1]/td");
            if (node != null) 
                quote.PreviousClose = GetDecimal(node.InnerText);
                
            node = statistics[0].SelectSingleNode("p/span[2]/span");
            if (node != null) 
                quote.Change = GetDecimal(node.InnerText);

            node = statistics[1].SelectSingleNode("tr[6]/td");
            if (node != null)
                quote.PeRatio = GetDecimal(node.InnerText);
                
            node = statistics[1].SelectSingleNode("tr[7]/td");
            if (node != null)
                quote.EarningsShare = GetDecimal(node.InnerText);
                
            node = statistics[1].SelectSingleNode("tr[5]/td");
            if (node != null)
                quote.MarketCapitalization = GetDecimal(node.InnerText);
                
            node = statistics[1].SelectSingleNode("tr[4]/td");
            if (node != null)
                quote.AverageDailyVolume = GetDecimal(node.InnerText);
                
            node = statistics[1].SelectSingleNode("tr[3]/td");
            if (node != null) 
                quote.Volume = GetDecimal(node.InnerText);

            node = statistics[1].SelectSingleNode("tr[1]/td/span[1]");
            if (node != null)
                quote.DailyLow = GetDecimal(node.InnerText);

            node = statistics[1].SelectSingleNode("tr[1]/td/span[2]");
            if (node != null)
                quote.DailyHigh = GetDecimal(node.InnerText);

            node = statistics[1].SelectSingleNode("tr[2]/td/span[1]");
            if (node != null)
                quote.YearlyLow = GetDecimal(node.InnerText);

            node = statistics[1].SelectSingleNode("tr[2]/td/span[2]");
            if (node != null)
                quote.YearlyHigh = GetDecimal(node.InnerText);

            var divAndYield = statistics[1].SelectSingleNode("tr[8]/td");
            if (!string.IsNullOrEmpty(divAndYield.InnerText) && divAndYield.InnerText.Contains('('))
            {
                var divAndYieldArray = divAndYield.InnerText.Split('(');
                if (node != null)
                    quote.Yield = GetDecimal(divAndYieldArray[0]);

                node = statistics[1].SelectSingleNode("tr[8]/td/span[1]");
                if (node != null)
                    quote.Dividend = GetDecimal(divAndYieldArray[1].Substring(0, divAndYieldArray[1].Length-1));
            }  

            node = analytics.SelectSingleNode("tr[4]/td");
            if (node != null)
                quote.PegRatio = GetDecimal(node.InnerText);
                
            //quote.PriceEpsEstimateCurrentYear = GetDecimal(q.Element("PriceEPSEstimateCurrentYear").Value);
            //quote.PriceEpsEstimateNextYear = GetDecimal(q.Element("PriceEPSEstimateNextYear").Value);

                        //quote.DividendShare = GetDecimal(q.Element("DividendShare").Value);
            //quote.LastTradeDate = GetDateTime(q.Element("LastTradeDate").Value + " " + q.Element("LastTradeTime").Value);
                
            //quote.EpsEstimateCurrentYear = GetDecimal(q.Element("EPSEstimateCurrentYear").Value);
            //quote.EpsEstimateNextYear = GetDecimal(q.Element("EPSEstimateNextYear").Value);
            //quote.EpsEstimateNextQuarter = GetDecimal(q.Element("EPSEstimateNextQuarter").Value);

            //quote.BookValue = GetDecimal(statistics[1].SelectSingleNode("tbody/tr[5]/td").InnerText);
            //quote.Ebitda = GetDecimal(q.Element("EBITDA").Value);
            //quote.ChangeFromYearLow = GetDecimal(q.Element("ChangeFromYearLow").Value);
            //quote.PercentChangeFromYearLow = GetDecimal(q.Element("PercentChangeFromYearLow").Value);
            //quote.ChangeFromYearHigh = GetDecimal(q.Element("ChangeFromYearHigh").Value);
            //quote.LastTradePrice = GetDecimal(q.Element("LastTradePriceOnly").Value);
            //quote.PercentChangeFromYearHigh = GetDecimal(q.Element("PercebtChangeFromYearHigh").Value); //missspelling in yahoo for field name
            //quote.FiftyDayMovingAverage = GetDecimal(q.Element("FiftydayMovingAverage").Value);
            //quote.TwoHunderedDayMovingAverage = GetDecimal(q.Element("TwoHundreddayMovingAverage").Value);
            //quote.ChangeFromTwoHundredDayMovingAverage = GetDecimal(q.Element("ChangeFromTwoHundreddayMovingAverage").Value);
            //quote.PercentChangeFromTwoHundredDayMovingAverage = GetDecimal(q.Element("PercentChangeFromTwoHundreddayMovingAverage").Value);
            //quote.PercentChangeFromFiftyDayMovingAverage = GetDecimal(q.Element("PercentChangeFromFiftydayMovingAverage").Value);
            //quote.Name = q.Element("Name").Value;
                
            //quote.ChangeInPercent = GetDecimal(q.Element("ChangeinPercent").Value);
            //quote.PriceSales = GetDecimal(q.Element("PriceSales").Value);
            //quote.PriceBook = GetDecimal(q.Element("PriceBook").Value);
            //quote.ExDividendDate = GetDateTime(q.Element("ExDividendDate").Value);
                
            //quote.DividendPayDate = GetDateTime(q.Element("DividendPayDate").Value);
            //quote.ShortRatio = GetDecimal(q.Element("ShortRatio").Value);
            //quote.OneYearPriceTarget = GetDecimal(q.Element("OneyrTargetPrice").Value);
            //quote.StockExchange = q.Element("StockExchange").Value;
            //quote.LastUpdate = DateTime.Now;
        }

        private static decimal? GetDecimal(string input)
        {
            if (input == null) return null;

            input = input.Replace("%", "");

            decimal value;

            if (Decimal.TryParse(input, out value)) return value;
            return null;
        }

        private static DateTime? GetDateTime(string input)
        {
            if (input == null) return null;

            DateTime value;

            if (DateTime.TryParse(input, out value)) return value;
            return null;
        }
    }

}
