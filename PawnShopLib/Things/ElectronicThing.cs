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
        public ElectronicThing(double weight, int year, ElectronicTypes type) : base(weight, year)
        {
            Type = type;
        }
        public override Thing GetDeepCopy() => new ElectronicThing(Weight, Year, Type);
    }
}
