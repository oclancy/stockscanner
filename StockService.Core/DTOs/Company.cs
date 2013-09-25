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
    public class Company : IEquatable<Company>
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        [Key]
        public int? Id { get; set; }
        
        [DataMember]
        [NotMapped]
        public Industry Industry{ get; set; }

        //public virtual StockQuote StockQuote { get; set; }
        //public virtual CompanyStatistics CompanyStatistic { get; set; }

        public bool Equals(Company other)
        {
            if (object.ReferenceEquals(other, this)) return true;
            if (object.ReferenceEquals(other, null)) return false;

            return this.Name == other.Name &&
                    this.Symbol == other.Symbol;
        }

        public override int GetHashCode()
        {
            return (Name+Symbol).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Company);
        }

        public static bool operator ==(Company leftOperand, Company rightOperand)
        {
            if (ReferenceEquals(null, leftOperand)) return ReferenceEquals(null, rightOperand);
            return leftOperand.Equals(rightOperand);
        }

        public static bool operator !=(Company leftOperand, Company rightOperand)
        {
            return !(leftOperand == rightOperand);
        }

    }
}
