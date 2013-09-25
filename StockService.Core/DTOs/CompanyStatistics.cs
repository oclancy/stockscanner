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

        public int CompanyId { get; set; }

        [DataMember]
        public virtual Company Company { get; set; }

        [Key]
        public int Id { get; set; }

        [DataMember]
        [YahooCompanyStatisticValue("Enterprise Value")]
        public double? EnterpriseValue { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Trailing P/E")]
        public double? TrailingPE { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Market Cap")]
        public double? MarketCap { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Forward P/E")]
        public double? ForwardPE { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("PEG Ratio")]
        public double? PEGRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Price/Sales")]
        public double? PriceSalesRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Price/Book")]
        public double? PriceBookRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Enterprise Value/Revenue")]
        public double? EnterpriseValueToRevenueRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Enterprise Value/EBITDA")]
        public double? EnterpriseValueToEBITDARatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Profit Margin")]
        public double? ProfitMargin { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Operating Margin")]
        public double? OperatingMargin { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Return on Assets")]
        public double? ReturnOnAssets { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Return on Equity")]
        public double? ReturnOnEquity { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Revenue")]
        public double? Revenue { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Revenue Per Share")]
        public double? RevenuePerShare { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Qtrly Revenue Growth")]
        public double? QtrlyRevenueGrowth { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Gross Profit")]
        public double? GrossProfit { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("EBITDA")]
        public double? EBITDA { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Net Income Avl to Common")]
        public double? NetIncomeAvltoCommon { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Diluted EPS")]
        public double? DilutedEPS { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Qtrly Earnings Growth")]
        public double? QtrlyEarningsGrowth { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Total Cash")]
        public double? TotalCash { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Total Cash Per Share")]
        public double? TotalCashPerShare { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Total Debt")]
        public double? TotalDebt { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Total Debt/Equity")]
        public double? TotalDebtToEquityRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Current Ratio")]
        public double? CurrentRatio { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Book Value Per Share")]
        public double? BookValuePerShare { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Operating Cash Flow")]
        public double? OperatingCashFlow { get; set; }
        [DataMember]
        [YahooCompanyStatisticValue("Levered Free Cash Flow")]
        public double? LeveredFreeCashFlow { get; set; }
        
        internal static CompanyStatistics FromYahooValues(Dictionary<string, string> values)
        {
            var cs = new CompanyStatistics();

            foreach (var pi in cs.GetType().GetProperties())
            {
                var attr = pi.GetCustomAttributes(typeof(YahooCompanyStatisticValueAttribute),false).FirstOrDefault();
                if (attr != null)
                {
                    var yahooFieldId = attr as YahooCompanyStatisticValueAttribute;
                    string yahooField = values.Keys.FirstOrDefault( k => k.StartsWith( yahooFieldId.IdString) );
                    if (!string.IsNullOrEmpty(yahooField))
                    {
                        double tempDble;
                        double? yahooFieldValue = (double?)null;
                        string valueAsString = values[yahooField];
                        if (double.TryParse(valueAsString, out tempDble))
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

                            if(double.TryParse(valueAsString.Substring(0, valueAsString.Length-1), out tempDble))
                                yahooFieldValue = tempDble * factor;

                        }
 
                        pi.SetMethod.Invoke(cs, new object[] { yahooFieldValue });
                    }
                }
            }
            return cs;
        }
    }
}
