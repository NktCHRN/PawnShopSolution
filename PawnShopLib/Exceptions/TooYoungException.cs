using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class TooYoungException : Exception
    {
        public TooYoungException() : base() { }
        public TooYoungException(string message) : base(message) { }
        public TooYoungException(int ageRequired, int age) : base(String.Format("The minimal allowed age is {0}. The age was {1}", ageRequired, age)) { }
        public TooYoungException(string message, Exception innerException) : base(message, innerException) { }
        protected TooYoungException(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext sc) : base(si, sc) { }
        public override string ToString() { return Message; }
    }
}
