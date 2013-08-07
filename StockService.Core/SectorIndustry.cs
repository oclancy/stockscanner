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
        [DataMember]
        public int Id{get;set;}
        [DataMember]
        public string Name{get;set;}
    }

    [DataContract]
    public class Sector
    {
        private List<Industry> m_industries = new List<Industry>();
        
        [DataMember]
        List<Industry> Industries { get { return m_industries; } set { m_industries = value; } }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
