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
    public class Jewel : Thing
    {
        private double _goldWeight;
        private double _goldContent;
        private double GoldContent
        {
            get
            {
                return _goldContent;
            }
            set
            {
                if (value > 0 && value <= 1000)
                    _goldContent = value;
                else
                    throw new ArgumentException("Content should be between 0 and 1000", nameof(value));
            }
        }
        private double _silverWeight;
        private double _silverContent;
        private double SilverContent
        {
            get
            {
                return _silverContent;
            }
            set
            {
                if (value > 0 && value <= 1000)
                    _silverContent = value;
                else
                    throw new ArgumentException("Content should be between 0 and 1000", nameof(value));
            }
        }
        private double _diamondWeight;
        private double _otherGemsWeight;
        public JewelTypes Type { get; private set; }
        private Jewel(double weight, int year) : base(weight, year)
        {
            Type = 0;
            _goldWeight = 0;
            _goldContent = 0;
            _silverWeight = 0;
            _silverContent = 0;
            _diamondWeight = 0;
            _otherGemsWeight = 0;
        }
        public Jewel(int year, double weight, JewelTypes gemType) : this(weight, year)
        {
            Type = gemType;
            if (gemType == JewelTypes.Diamond)
                _diamondWeight = weight;
            else if (gemType == JewelTypes.AnotherGem)
            {
                _otherGemsWeight = weight;
            }
            else
            {
                throw new ArgumentException("gemType should be Diamond or AnotherGem in this constructor", nameof(gemType));
            }
        }
        public Jewel(int year, double weight, JewelTypes ingotType, double content) : this(weight, year)
        {
            Type = ingotType;
            if (ingotType == JewelTypes.GoldIngot)
            {
                _goldWeight = weight;
                GoldContent = content;
            }
            else if (ingotType == JewelTypes.SilverIngot)
            {
                _silverWeight = weight;
                SilverContent = content;
            }
            else
            {
                throw new ArgumentException("ingotType should be GoldIngot or SilverIngot in this constructor", nameof(ingotType));
            }
        }
        public Jewel(int year, JewelTypes complicatedJewelType, double goldWeight, double goldContent, double silverWeight, double silverContent, double diamondWeight, double otherGemsWeight) : base(goldWeight + silverWeight + diamondWeight + otherGemsWeight, year)
        {
            if (complicatedJewelType != JewelTypes.ComplicatedJewel)
                throw new ArgumentException("complicatedJewelType should be ComplicatedJewel in this constructor", nameof(complicatedJewelType));
            Type = complicatedJewelType;
            _goldWeight = goldWeight;
            GoldContent = goldContent;
            _silverWeight = silverWeight;
            SilverContent = silverContent;
            _diamondWeight = diamondWeight;
            _otherGemsWeight = otherGemsWeight;
        }
    }
}
