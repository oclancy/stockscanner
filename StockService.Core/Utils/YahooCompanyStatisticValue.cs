using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Utils
{
    internal class YahooCompanyStatisticValueAttribute : Attribute
    {
        internal YahooCompanyStatisticValueAttribute( string yahooSting )
        {
            IdString = yahooSting;
        }

        public string IdString { get; set; }
    }
}
