using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StockService.Core
{
    [DataContract]
    public class Market
    {
        [DataMember]
        public string Name{get;set;}

        [DataMember]
        public int Id{get;set;}

        public Market(string name, int id)
        {
            Name = name;
            Id = id;

            Sectors = new List<Sector>();
        }

        public List<Sector> Sectors { get; set; }
    }
}
