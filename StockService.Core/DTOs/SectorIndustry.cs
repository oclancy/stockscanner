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
    public class Industry //: IEquatable<Industry>
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

        //public bool Equals(Industry other)
        //{
        //    if (object.ReferenceEquals(other, this)) return true;
        //    if (object.ReferenceEquals(other, null)) return false;

        //    return this.Id == other.Id;
        //}

        //public override int GetHashCode()
        //{
        //    return Id;
        //}

        //public override bool Equals(object obj)
        //{
        //    return this.Equals(obj as Industry);
        //}

        //public static bool operator ==(Industry leftOperand, Industry rightOperand)
        //{
        //    if (ReferenceEquals(null, leftOperand)) return ReferenceEquals(null, rightOperand);
        //    return leftOperand.Equals(rightOperand);
        //}

        //public static bool operator !=(Industry leftOperand, Industry rightOperand)
        //{
        //    return !(leftOperand == rightOperand);
        //}
    }

    [DataContract(IsReference=true)]
    public class Sector //: IEquatable<Sector>
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

        //public bool Equals(Sector other)
        //{
        //    if (object.ReferenceEquals(other, this)) return true;
        //    if (object.ReferenceEquals(other, null)) return false;

        //    return this.Id == other.Id;
        //}

        //public override int GetHashCode()
        //{
        //    return Id;
        //}

        //public override bool Equals(object obj)
        //{
        //    return this.Equals(obj as Sector);
        //}

        //public static bool operator ==(Sector leftOperand, Sector rightOperand)
        //{
        //    if (ReferenceEquals(null, leftOperand)) return ReferenceEquals(null, rightOperand);
        //    return leftOperand.Equals(rightOperand);
        //}

        //public static bool operator !=(Sector leftOperand, Sector rightOperand)
        //{
        //    return !(leftOperand == rightOperand);
        //}
    }
}
