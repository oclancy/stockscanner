using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockService.Core.Extension
{
    public static class StringExtension
    {
        static Regex m_removeSubscript = new Regex(@"[^\(]*(\(.*\))");

        public static string FormatStatistic(this string source)
        {
            var match = m_removeSubscript.Match( source );
            if(match.Success)
                return match.Value;
            else 
                return source;
        }
    }
}
