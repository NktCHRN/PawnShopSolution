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
    [Serializable]
    public class AntiqueThing : Thing
    {
        public AntiqueTypes Type { get; private set; }
        public decimal EstimatedPrice { get; private set; }
        public AntiqueThing(int year, double weight, AntiqueTypes type, decimal estimatedPrice) : base(weight, year)
        {
            Type = type;
            const int minimalAge = 30;
            if (DateTime.Now.Year - year < minimalAge)
                throw new TooYoungException($"If thing is an antique one, it should be at least {minimalAge} years old. It was {DateTime.Now.Year - year} years old");
            if (estimatedPrice >= 0)
                EstimatedPrice = estimatedPrice;
            else
                throw new ArgumentOutOfRangeException(nameof(estimatedPrice), "Price cannot be negative");
        }
        public override string ToString()
        {
            string antiqueThingDescription = (Type != AntiqueTypes.AntiqueJewel) ? Type.ToString() : "Antique jewel";
            antiqueThingDescription += $": {Year}; {Weight:F3} g";
            return antiqueThingDescription;
        }
    }
}
