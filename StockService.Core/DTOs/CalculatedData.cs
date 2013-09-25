using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StockService.Core
{
    [DataContract]
    public class CalculatedData
    {
        [DataMember]
        double SmartPE { get; set; }

        [DataMember]
        double PriceSalesRatio { get; set; }
    }
}
