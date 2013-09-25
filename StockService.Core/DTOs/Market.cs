using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StockService.Core
{
    [DataContract]
    public class Market : IEquatable<Market>
    {
        [DataMember]
        public string Name{get;set;}

        [DataMember]
        public int Id{get;set;}

        private List<Sector> m_sectors = new List<Sector>();
        public List<Sector> Sectors { get { return m_sectors; } set { m_sectors = value; } }

        public bool Equals(Market other)
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
            return this.Equals(obj as Market);
        }

        public static bool operator ==(Market leftOperand, Market rightOperand)
        {
            if (ReferenceEquals(null, leftOperand)) return ReferenceEquals(null, rightOperand);
            return leftOperand.Equals(rightOperand);
        }

        public static bool operator !=(Market leftOperand, Market rightOperand)
        {
            return !(leftOperand == rightOperand);
        }
    }
}
