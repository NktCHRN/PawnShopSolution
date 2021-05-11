using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib.Things
{
    public enum ElectronicTypes
    {
        Computer,
        Phone,
        Washer,
        Fridge,
        Camera
    }
    public class ElectronicThing : Thing
    {
        public ElectronicTypes Type { get; private set; }
        public ElectronicThing(int year, double weight, ElectronicTypes type) : base(weight, year)
        {
            Type = type;
        }
        public override string ToString()
        {
            return $"{Type.ToString()}: {Year}; {Weight:F3} g";
        }
    }
}
