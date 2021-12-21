using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{

    [DataContract]
    public enum Unit
    {
        [EnumMember] Pa, [EnumMember] Db, [EnumMember] C
    }

    [DataContract]
    public class Measurement
    {
        Unit unit;
        double value;
        DateTime time;

        public Measurement()
        {
        }
        
        public Measurement(Unit unit, double value, DateTime time)
        {
            this.Unit = unit;
            this.Value = value;
            this.Time = time;
        }

        [DataMember]
        public Unit Unit
        {
            get => unit;
            set => unit = value;
        }
        [DataMember]
        public double Value
        {
            get => value;
            set => this.value = value;
        }
        [DataMember]
        public DateTime Time
        {
            get => time;
            set => time = value;
        }

        public override string ToString()
        {
            return String.Format("Time : {0}, Value : {1} {2}", Time, Value, Unit);
        }
    }
}
