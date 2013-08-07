using System;
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
        public string Symbol { get; set; }
    }
}
