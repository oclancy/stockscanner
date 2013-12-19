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
    public class Industry 
    {
        public Industry()
        {
            Companies= new List<Company>();
        }

        [DataMember]
        public int IndustryId { get; set; }

        [DataMember]
        public string Name{get;set;}

        [DataMember]
        public virtual Sector Sector { get; set; }
        public int SectorId { get; set; }

        public virtual IList<Company> Companies
        {
            get;
            set;
        }

    }

    [DataContract(IsReference=true)]
    public class Sector
    {
        public Sector()
        {
            Industries = new List<Industry>();
        }

        [DataMember]
        public int SectorId { get; set; }

        [DataMember]
        public virtual IList<Industry> Industries
        {
            get;
            set;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public virtual Market Market {get;set;}
        public int MarketId { get; set; }

    }
}
