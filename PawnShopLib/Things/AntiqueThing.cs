using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib.Things
{
    public enum AntiqueTypes
    {
        Watches,
        Icon,
        Painting,
        AntiqueJewel,
        Medal
    }
    public class AntiqueThing : Thing
    {
        public AntiqueTypes Type { get; private set; }
        public decimal EstimatedPrice { get; private set; }
        public AntiqueThing(double weight, int year, AntiqueTypes type, decimal estimatedPrice) : base(weight, year)
        {
            Type = type;
            if (estimatedPrice >= 0)
                EstimatedPrice = estimatedPrice;
            else
                throw new ArgumentException("Price can`t be negative", nameof(estimatedPrice));
        }
        public override string ToString()
        {
            string antiqueThingDescription = (Type != AntiqueTypes.AntiqueJewel) ? Type.ToString() : "Antique jewel";
            antiqueThingDescription += $": {Year}; {Weight:F3} g";
            return antiqueThingDescription;
        }
    }
}
