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
    [Serializable]
    public class ElectronicThing : Thing
    {
        public ElectronicTypes Type { get; private set; }
        /// <summary>
        /// The constructor of the ElectronicThing class
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weight"></param>
        /// <param name="type">Type from ElectronicTypes enum</param>
        public ElectronicThing(int year, double weight, ElectronicTypes type) : base(weight, year)
        {
            Type = type;
        }
        public override string ToString()
        {
            return $"{Type}: {Year}; {Weight:F3} g";
        }
    }
}
