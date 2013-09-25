using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core
{
    [DataContract]
    public class Industry : IEquatable<Industry>
    {
        private List<Company> m_companies = new List<Company>();

        [DataMember]
        public int Id{get;set;}

        [DataMember]
        public string Name{get;set;}

        [DataMember]
        public Sector Sector { get; set; }

        public virtual List<Company> Companies { 
            get{return m_companies;}
            set{m_companies=value;}
        }

        public bool Equals(Industry other)
        {
            if (object.ReferenceEquals(other, this)) return true;
            if (object.ReferenceEquals(other, null)) return false;

            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Industry);
        }

        public static bool operator ==(Industry leftOperand, Industry rightOperand)
        {
            if (ReferenceEquals(null, leftOperand)) return ReferenceEquals(null, rightOperand);
            return leftOperand.Equals(rightOperand);
        }

        public static bool operator !=(Industry leftOperand, Industry rightOperand)
        {
            return !(leftOperand == rightOperand);
        }
    }

    [DataContract(IsReference=true)]
    public class Sector : IEquatable<Sector>
    {
        private List<Industry> m_industries = new List<Industry>();

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public List<Industry> Industries { get { return m_industries; } set { m_industries = value; } }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Market Market {get;set;}

        public bool Equals(Sector other)
        {
            if (object.ReferenceEquals(other, this)) return true;
            if (object.ReferenceEquals(other, null)) return false;

            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Sector);
        }

        public static bool operator ==(Sector leftOperand, Sector rightOperand)
        {
            if (ReferenceEquals(null, leftOperand)) return ReferenceEquals(null, rightOperand);
            return leftOperand.Equals(rightOperand);
        }

        public static bool operator !=(Sector leftOperand, Sector rightOperand)
        {
            return !(leftOperand == rightOperand);
        }
    }
}
