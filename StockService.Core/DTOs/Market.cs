using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StockService.Core
{
    [DataContract]
    public class Market //: IEquatable<Market>
    {
        public Market()
        {
            Sectors = new List<Sector>();
        }

        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public string Name{get;set;}

        [DataMember]
        public int MarketId{get;set;}

        public virtual IList<Sector> Sectors
        {
            get;
            set;
        }

        //public bool Equals(Market other)
        //{
        //    if (object.ReferenceEquals(other, this)) return true;
        //    if (object.ReferenceEquals(other, null)) return false;

        //    return this.MarketId == other.MarketId;
        //}

        //public override int GetHashCode()
        //{
        //    return MarketId.GetHashCode();
        //}

        //public override bool Equals(object obj)
        //{
        //    return this.Equals(obj as Market);
        //}

        //public static bool operator ==(Market leftOperand, Market rightOperand)
        //{
        //    if (ReferenceEquals(null, leftOperand)) return ReferenceEquals(null, rightOperand);
        //    return leftOperand.Equals(rightOperand);
        //}

        //public static bool operator !=(Market leftOperand, Market rightOperand)
        //{
        //    return !(leftOperand == rightOperand);
        //}
    }
}
