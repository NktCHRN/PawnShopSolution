using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class BusyObjectException : ArgumentException
    {
        public BusyObjectException() : base() { }
        public BusyObjectException(string message) : base(message) { }
        public BusyObjectException(string message, string paramName) : base(message, paramName) { }
        public BusyObjectException(string message, Exception innerException) : base(message, innerException) { }
        protected BusyObjectException(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext sc) : base(si, sc) { }
        public override string ToString() { return Message; }
    }
}
