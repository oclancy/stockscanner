using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core
{
    [DataContract]
    public class Company 
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public int CompanyId { get; set; }
        
        [DataMember]
        public virtual Industry Industry{ get; set; }
        public int IndustryId { get; set; }

        public virtual CompanyStatistics CompanyStatistics { get; set; }
        
        public virtual StockQuote StockQuote { get; set; }
        
    }
}
