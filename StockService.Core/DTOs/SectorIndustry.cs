using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core
{
    [DataContract]
    public class Industry
    {
        public Industry(int id, string name)
        {
            Id = id;
            Name = name;

            Companies = new List<Company>();
        }
        [DataMember]
        public int Id{get;set;}
        [DataMember]
        public string Name{get;set;}

        public List<Company> Companies { get; set; }
    }

    [DataContract]
    public class Sector
    {
        private List<Industry> m_industries = new List<Industry>();

        public Sector(string name)
        {
            Name = name;
        }
        
        [DataMember]
        public List<Industry> Industries { get { return m_industries; } set { m_industries = value; } }

        [DataMember]
        public string Name { get; set; }
    }
}
