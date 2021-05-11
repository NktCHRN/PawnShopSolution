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
        public Car(int year, double weight, decimal marketPrice, int mileage, string brandName) : base(weight, year)
        {
            if (marketPrice >= 0)
                MarketPrice = marketPrice;
            else
                throw new ArgumentOutOfRangeException(nameof(marketPrice), "Price cannot be negative");
            if (mileage >= 0)
                Mileage = mileage;
            else
                throw new ArgumentOutOfRangeException(nameof(mileage), "Mileage cannot be negative");
            if (brandName != null)
            {
                if (!string.IsNullOrWhiteSpace(brandName))
                    BrandName = brandName;
                else
                    throw new ArgumentException("Car`s brand name cannot be empty or contain only spaces", nameof(brandName));
            }
            else
                throw new ArgumentNullException(nameof(brandName), "Car`s brandname cannot be null");
        }
        public override string ToString()
        {
            return $"Car: {BrandName}; {Year}; {Mileage} km; {Weight / 1000000.0:F3} ton";
        }
    }
}
