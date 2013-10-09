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

    public class YahooFinanceStockDataProvider : IStockProvider
    {
        //[Dependency]
        //public IDictionary<string, StockQuote> Cache{get;set;}
        [Dependency]
        public IDictionary<string, Company> Cache { get; set; }

        private const string BASE_URL = "http://finance.yahoo.com/q?s={0}&ql=1";

        public async Task<StockQuote> FetchDataAsync(Company company)
        {
            if (Cache.ContainsKey(company.Symbol) &&
                Cache[company.Symbol].StockQuote != null &&
                Cache[company.Symbol].StockQuote.LastUpdated > DateTime.UtcNow.AddDays(-1) )
                return Cache[company.Symbol].StockQuote;

            var webReq = WebRequest.CreateHttp(string.Format(BASE_URL, company.Symbol));

            var t = await webReq.GetResponseAsync();

            var doc = new HtmlDocument();
            doc.Load(t.GetResponseStream());
            var cs = Parse(doc.DocumentNode.Descendants("body").First());
            //company.StockQuote = cs;
            //cs.Company = company;
            cs.LastUpdate = DateTime.UtcNow;

            return cs;
        }

        private static StockQuote Parse(HtmlNode body)
        {

            var infoDiv = body.SelectSingleNode("div[@id='yfi_doc']/div[@id='yfi_bd']/div[@id='yfi_investing_content']");

            var priceDiv = infoDiv.SelectSingleNode("div[@class='rtq_div']/div[@class='yui-g']/div[@id='yfi_rt_quote_summary']/div[@class='yfi_rt_quote_summary_rt_top']");

            var statistics = infoDiv.SelectSingleNode("div[@class='yui-u first yfi-start-content']").Descendants("table").ToArray();

            StockQuote quote = new StockQuote();
            {
                quote.Ask = GetDecimal(statistics[0].SelectSingleNode("tr['3']/td").InnerText);
                quote.Bid = GetDecimal(statistics[0].SelectSingleNode("tr['2']/td").InnerText);
                quote.Open = GetDecimal(statistics[0].SelectSingleNode("tr['1']/td").InnerText);
                quote.PreviousClose = GetDecimal(statistics[0].SelectSingleNode("tr['0']/td").InnerText);
                //quote.Change = GetDecimal(q.Element("Change").Value);

                quote.PeRatio = GetDecimal(statistics[1].SelectSingleNode("tr['5']/td").InnerText);
                quote.EarningsShare = GetDecimal(statistics[1].SelectSingleNode("tr['6']/td").InnerText);
                quote.MarketCapitalization = GetDecimal(statistics[1].SelectSingleNode("tr['4']/td").InnerText);
                quote.AverageDailyVolume = GetDecimal(statistics[1].SelectSingleNode("tr['3']/td").InnerText);
                quote.Volume = GetDecimal(statistics[1].SelectSingleNode("tr['2']/td").InnerText);

                //quote.PegRatio = GetDecimal(q.Element("PEGRatio").Value);
                
                //quote.PriceEpsEstimateCurrentYear = GetDecimal(q.Element("PriceEPSEstimateCurrentYear").Value);
                //quote.PriceEpsEstimateNextYear = GetDecimal(q.Element("PriceEPSEstimateNextYear").Value);

                //quote.DailyLow = GetDecimal(q.Element("DaysLow").Value);
                //quote.DailyHigh = GetDecimal(q.Element("DaysHigh").Value);
                //quote.YearlyLow = GetDecimal(q.Element("YearLow").Value);
                //quote.YearlyHigh = GetDecimal(q.Element("YearHigh").Value);
                
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
            return quote;
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
