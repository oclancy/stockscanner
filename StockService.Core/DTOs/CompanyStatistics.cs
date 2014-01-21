using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using StockService.Core.Utils;

namespace StockService.Core
{
    [DataContract]
    public class CompanyStatistics
    {
        private Dictionary<string, string> retVal;

        public DateTime LastUpdated { get; set; }

        [DataMember]
        public Dictionary<string, string> Statistics { get; set; }

        [DataMember]
        public virtual Company Company { get; set; }
        [Key,ForeignKey("Company")]
        public int CompanyId { get; set; }

        //public int CompanyStatisticsId { get; set; }

        [DataMember]
        [YahooCompanyStatisticValue("Enterprise Value")]
        public decimal? EnterpriseValue { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Trailing P/E")]
        public decimal? TrailingPE { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Market Cap")]
        public decimal? MarketCap { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Forward P/E")]
        public decimal? ForwardPE { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("PEG Ratio")]
        public decimal? PEGRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Price/Sales")]
        public decimal? PriceSalesRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Price/Book")]
        public decimal? PriceBookRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Enterprise Value/Revenue")]
        public decimal? EnterpriseValueToRevenueRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Enterprise Value/EBITDA")]
        public decimal? EnterpriseValueToEBITDARatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Profit Margin")]
        public decimal? ProfitMargin { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Operating Margin")]
        public decimal? OperatingMargin { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Return on Assets")]
        public decimal? ReturnOnAssets { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Return on Equity")]
        public decimal? ReturnOnEquity { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Revenue")]
        public decimal? Revenue { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Revenue Per Share")]
        public decimal? RevenuePerShare { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Qtrly Revenue Growth")]
        public decimal? QtrlyRevenueGrowth { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Gross Profit")]
        public decimal? GrossProfit { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("EBITDA")]
        public decimal? EBITDA { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Net Income Avl to Common")]
        public decimal? NetIncomeAvltoCommon { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Diluted EPS")]
        public decimal? DilutedEPS { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Qtrly Earnings Growth")]
        public decimal? QtrlyEarningsGrowth { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Total Cash")]
        public decimal? TotalCash { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Total Cash Per Share")]
        public decimal? TotalCashPerShare { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Total Debt")]
        public decimal? TotalDebt { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Total Debt/Equity")]
        public decimal? TotalDebtToEquityRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Current Ratio")]
        public decimal? CurrentRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Book Value Per Share")]
        public decimal? BookValuePerShare { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Operating Cash Flow")]
        public decimal? OperatingCashFlow { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Levered Free Cash Flow")]
        public decimal? LeveredFreeCashFlow { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Trailing Annual Dividend Yield")]
        public decimal? TrailingAnnualDividendYield { get; set; }

        
        internal static void FromYahooValues(Dictionary<string, string> values, ref CompanyStatistics cs)
        {
            foreach (var pi in cs.GetType().GetProperties())
            {
                var attr = pi.GetCustomAttributes(typeof(YahooCompanyStatisticValueAttribute),false).FirstOrDefault();
                if (attr != null)
                {
                    var yahooFieldId = attr as YahooCompanyStatisticValueAttribute;
                    string yahooField = values.Keys.FirstOrDefault( k => k.StartsWith( yahooFieldId.IdString) );
                    if (!string.IsNullOrEmpty(yahooField))
                    {
                        decimal tempDble;
                        decimal? yahooFieldValue = (decimal?)null;
                        string valueAsString = values[yahooField];
                        if (decimal.TryParse(valueAsString, out tempDble))
                        {
                            yahooFieldValue = tempDble;
                        }
                        else if (char.IsLetter(valueAsString.Last()) || valueAsString.Last() =='%')
                        {
                            int factor; 
                            switch(char.ToLower(valueAsString.Last()))
                            {
                                case 'k': factor = 1000;
                                    break;
                                case 'b': factor = 1000000000;
                                    break;
                                case 'm': factor = 1000000;
                                    break;
                                default: factor = 1;
                                    break;
                            }

                            if(decimal.TryParse(valueAsString.Substring(0, valueAsString.Length-1), out tempDble))
                                yahooFieldValue = tempDble * factor;

                        }
 
                        pi.SetMethod.Invoke(cs, new object[] { yahooFieldValue });
                    }
                }
            }
        }
    }
}
