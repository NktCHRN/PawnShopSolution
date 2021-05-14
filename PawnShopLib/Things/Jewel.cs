using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib.Things
{
    public enum JewelTypes
    {
        GoldIngot,
        SilverIngot,
        Diamond,
        AnotherGem,
        ComplicatedJewel
    }
    [Serializable]
    public class Jewel : Thing
    {
        /// <summary>
        /// Weight of the gold in grams
        /// </summary>
        public double GoldWeight { get; private set; }
        /// <summary>
        /// Weight of the silver in grams
        /// </summary>
        public double SilverWeight { get; private set; }
        /// <summary>
        /// Weight of the diamond in grams
        /// </summary>
        public double DiamondWeight { get; private set; }
        /// <summary>
        /// Weight of other gems in grams
        /// </summary>
        public double OtherGemsWeight { get; private set; }
        public JewelTypes Type { get; private set; }
        private Jewel(double weight, int year) : base(weight, year)
        {
            Type = 0;
            GoldWeight = 0;
            SilverWeight = 0;
            DiamondWeight = 0;
            OtherGemsWeight = 0;
        }
        /// <summary>
        /// Constructor of the class Jewel only for solid jewel types (GoldIngot, SilverIngot, Diamond and Gem)
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weight">Weight in grams</param>
        /// <param name="solidJewelType">Only GoldIngot, SilverIngot, Diamond or Gem</param>
        /// <exception cref="ArgumentException">Thrown when type is not a solid one</exception>
        public Jewel(int year, double weight, JewelTypes solidJewelType) : this(weight, year)
        {
            Type = solidJewelType;
            switch (solidJewelType) {
                case JewelTypes.GoldIngot:
                    GoldWeight = weight;
                    break;
                case JewelTypes.SilverIngot:
                    SilverWeight = weight;
                    break;
                case JewelTypes.Diamond:
                    DiamondWeight = weight;
                    break;
                case JewelTypes.AnotherGem:
                    OtherGemsWeight = weight;
                    break;
                default:
                    throw new ArgumentException("Type should be GoldIngot, SilverIngot, Diamond or AnotherGem in this constructor", nameof(solidJewelType));
            }
        }
        /// <summary>
        /// Constructor of the class Jewel only for complicated jewel
        /// </summary>
        /// <param name="year"></param>
        /// <param name="goldWeight">Weight of the gold in grams</param>
        /// <param name="silverWeight">Weight of the silver in grams</param>
        /// <param name="diamondWeight">Weight of the diamond in grams</param>
        /// <param name="otherGemsWeight">Weight of other gems in grams</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one of weights is negative</exception>
        public Jewel(int year, double goldWeight, double silverWeight, double diamondWeight, double otherGemsWeight) : base(goldWeight + silverWeight + diamondWeight + otherGemsWeight, year)
        {
            Type = JewelTypes.ComplicatedJewel;
            if (goldWeight >= 0)
                GoldWeight = goldWeight;
            else throw new ArgumentOutOfRangeException(nameof(goldWeight), "Weight cannot be lower than zero");
            if (silverWeight >= 0)
                SilverWeight = silverWeight;
            else throw new ArgumentOutOfRangeException(nameof(silverWeight), "Weight cannot be lower than zero");
            if (diamondWeight >= 0)
                DiamondWeight = diamondWeight;
            else throw new ArgumentOutOfRangeException(nameof(diamondWeight), "Weight cannot be lower than zero");
            if (otherGemsWeight >= 0)
                OtherGemsWeight = otherGemsWeight;
            else throw new ArgumentOutOfRangeException(nameof(otherGemsWeight), "Weight cannot be lower than zero");
        }
        public override string ToString()
        {
            string jewelDescription = "";
            switch (Type)
            {
                case JewelTypes.GoldIngot:
                    jewelDescription += $"Gold ingot: {GoldWeight:F3} g";
                    break;
                case JewelTypes.SilverIngot:
                    jewelDescription += $"Silver ingot: {SilverWeight:F3} g";
                    break;
                case JewelTypes.Diamond:
                    jewelDescription += $"{Type}: {DiamondWeight:F3} g";
                    break;
                case JewelTypes.AnotherGem:
                    jewelDescription += $"Precious gem: {OtherGemsWeight:F3} g";
                    break;
                default:
                    jewelDescription += "Jewel: ";
                    if (GoldWeight > 0)
                        jewelDescription += $"gold {GoldWeight:F3}g; ";
                    if (SilverWeight > 0)
                        jewelDescription += $"silv {SilverWeight:F3}g; ";
                    if (DiamondWeight > 0)
                        jewelDescription += $"diam {DiamondWeight:F3}g; ";
                    if (OtherGemsWeight > 0)
                        jewelDescription += $"gems {OtherGemsWeight:F3}g; ";
                    jewelDescription = jewelDescription.Remove(jewelDescription.Length - 2, 2);
                    break;
            }
            return jewelDescription;
        }
    }
}
