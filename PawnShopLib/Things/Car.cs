using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib.Things
{
    public class Car : Thing
    {
        public decimal MarketPrice { get; private set; }
        public int Mileage { get; private set; }
        public string BrandName { get; private set; }
        public Car(double weight, int year, decimal marketPrice, int mileage, string brandName) : base(weight, year)
        {
            if (marketPrice >= 0)
                MarketPrice = marketPrice;
            else
                throw new ArgumentException("Price can`t be negative", nameof(marketPrice));
            if (mileage >= 0)
                Mileage = mileage;
            else
                throw new ArgumentException("Mileage can`t be negative", nameof(mileage));
            if (brandName != null)
                BrandName = brandName;
            else
                throw new ArgumentException("Car`s brandname can`t be null", nameof(brandName));
        }
        public override string ToString()
        {
            return $"Car: {BrandName}; {Year}; {Mileage} km; {Weight / 1000000.0:F3} ton";
        }
    }
}
