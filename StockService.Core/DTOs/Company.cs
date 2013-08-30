using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core
{
    [DataContract]
    public class Company
    {
        public Company(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Symbol { get; set; }
    }
}
