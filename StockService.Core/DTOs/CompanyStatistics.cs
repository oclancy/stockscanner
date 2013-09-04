﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StockService.Core
{
    [DataContract]
    public class CompanyStatistics
    {
        [DataMember]
        public Dictionary<string, string> Statistics;

        public CompanyStatistics(Dictionary<string, string> values)
        {
            // TODO: Complete member initialization
            this.Statistics = values;
        }

        [DataMember]
        public string Symbol { get; set; }
    }
}
