using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StockService.Core
{
    [DataContract]
    public class StockQuote
    {
        public DateTime LastUpdated { get; set; }

        [DataMember]
        public decimal? Ask { get; set; }

        [DataMember]
        public decimal? Bid { get; set; }

        [DataMember]
        public decimal? AverageDailyVolume { get; set; }

        [DataMember]
        public decimal? BookValue { get; set; }

        [DataMember]
        public decimal? Change { get; set; }

        [DataMember]
        public decimal? DividendShare { get; set; }

        [DataMember]
        public DateTime? LastTradeDate { get; set; }

        [DataMember]
        public decimal? EarningsShare { get; set; }

        [DataMember]
        public decimal? EpsEstimateCurrentYear { get; set; }

        [DataMember]
        public decimal? EpsEstimateNextYear { get; set; }

        [DataMember]
        public decimal? EpsEstimateNextQuarter { get; set; }

        [DataMember]
        public decimal? DailyLow { get; set; }

        [DataMember]
        public decimal? DailyHigh { get; set; }

        [DataMember]
        public decimal? YearlyLow { get; set; }

        [DataMember]
        public decimal? YearlyHigh { get; set; }

        [DataMember]
        public decimal? MarketCapitalization { get; set; }

        [DataMember]
        public decimal? Ebitda { get; set; }

        [DataMember]
        public decimal? ChangeFromYearLow { get; set; }

        [DataMember]
        public decimal? PercentChangeFromYearLow { get; set; }

        [DataMember]
        public decimal? ChangeFromYearHigh { get; set; }

        [DataMember]
        public decimal? LastTradePrice { get; set; }

        [DataMember]
        public decimal? PercentChangeFromYearHigh { get; set; }

        [DataMember]
        public decimal? FiftyDayMovingAverage { get; set; }

        [DataMember]
        public decimal? TwoHunderedDayMovingAverage { get; set; }

        [DataMember]
        public decimal? ChangeFromTwoHundredDayMovingAverage { get; set; }

        [DataMember]
        public decimal? PercentChangeFromTwoHundredDayMovingAverage { get; set; }

        [DataMember]
        public decimal? PercentChangeFromFiftyDayMovingAverage { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public decimal? Open { get; set; }

        [DataMember]
        public decimal? PreviousClose { get; set; }

        [DataMember]
        public decimal? ChangeInPercent { get; set; }

        [DataMember]
        public decimal? PriceSales { get; set; }

        [DataMember]
        public decimal? PriceBook { get; set; }

        [DataMember]
        public DateTime? ExDividendDate { get; set; }

        [DataMember]
        public decimal? PeRatio { get; set; }

        [DataMember]
        public DateTime? DividendPayDate { get; set; }

        [DataMember]
        public decimal? PegRatio { get; set; }

        [DataMember]
        public decimal? PriceEpsEstimateCurrentYear { get; set; }

        [DataMember]
        public decimal? PriceEpsEstimateNextYear { get; set; }

        [DataMember]
        public decimal? ShortRatio { get; set; }
        
        [DataMember]
        public decimal? OneYearPriceTarget { get; set; }

        [DataMember]
        public decimal? Volume { get; set; }

        [DataMember]
        public string StockExchange { get; set; }

        [DataMember]
        public DateTime LastUpdate { get; set; }

        //[ForeignKey("Company")]
        //public int CompanyId { get; set; }

        //[DataMember]
        //public virtual Company Company { get; set; }

        [Key]
        public int Id { get; set; }
    }
}
