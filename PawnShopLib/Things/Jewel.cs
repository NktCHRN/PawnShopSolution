﻿using System;
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
    public class Jewel : Thing
    {
        public double GoldWeight { get; private set; }
        public double SilverWeight { get; private set; }
        public double DiamondWeight { get; private set; }
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
        public Jewel(int year, double goldWeight, double silverWeight, double diamondWeight, double otherGemsWeight) : base(goldWeight + silverWeight + diamondWeight + otherGemsWeight, year)
        {
            Type = JewelTypes.ComplicatedJewel;
            if (goldWeight >= 0)
                GoldWeight = goldWeight;
            else throw new ArgumentException("Weight can`t be lower than zero", nameof(goldWeight));
            if (silverWeight >= 0)
                SilverWeight = silverWeight;
            else throw new ArgumentException("Weight can`t be lower than zero", nameof(silverWeight));
            if (diamondWeight >= 0)
                DiamondWeight = diamondWeight;
            else throw new ArgumentException("Weight can`t be lower than zero", nameof(diamondWeight));
            if (otherGemsWeight >= 0)
                OtherGemsWeight = otherGemsWeight;
            else throw new ArgumentException("Weight can`t be lower than zero", nameof(otherGemsWeight));
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
                    jewelDescription += $"{Type.ToString()}: {DiamondWeight:F3} g";
                    break;
                case JewelTypes.AnotherGem:
                    jewelDescription += $"Precious gem: {OtherGemsWeight:F3} g";
                    break;
                default:
                    jewelDescription += "Jewel: ";
                    if (GoldWeight > 0)
                        jewelDescription += $"gold {GoldWeight:F3} g; ";
                    if (SilverWeight > 0)
                        jewelDescription += $"silver {SilverWeight:F3} g; ";
                    if (DiamondWeight > 0)
                        jewelDescription += $"diamond {DiamondWeight:F3} g; ";
                    if (OtherGemsWeight > 0)
                        jewelDescription += $"other gems {OtherGemsWeight:F3} g; ";
                    jewelDescription = jewelDescription.Remove(jewelDescription.Length - 2, 2);
                    break;
            }
            return jewelDescription;
        }
    }
}
