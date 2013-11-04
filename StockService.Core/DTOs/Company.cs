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
    public class Company //: IEquatable<Company>
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
        public int? CompanyStatisticsId { get; set; }

        public virtual StockQuote StockQuote { get; set; }
        public int? StockQuoteId { get; set; }

        //public bool Equals(Company other)
        //{
        //    if (object.ReferenceEquals(other, this)) return true;
        //    if (object.ReferenceEquals(other, null)) return false;

        //    return this.CompanyId == other.CompanyId;
        //}

        //public override int GetHashCode()
        //{
        //    return CompanyId;
        //}

        //public override bool Equals(object obj)
        //{
        //    return this.Equals(obj as Company);
        //}

        //public static bool operator ==(Company leftOperand, Company rightOperand)
        //{
        //    if (ReferenceEquals(null, leftOperand)) return ReferenceEquals(null, rightOperand);
        //    return leftOperand.Equals(rightOperand);
        //}

        //public static bool operator !=(Company leftOperand, Company rightOperand)
        //{
        //    return !(leftOperand == rightOperand);
        //}

    }
}
