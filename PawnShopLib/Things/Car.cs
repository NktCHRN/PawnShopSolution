using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib.Things
{
    [Serializable]
    public class Car : Thing
    {
        /// <summary>
        /// Average price of the car now
        /// </summary>
        public decimal MarketPrice { get; private set; }
        public int Mileage { get; private set; }
        /// <summary>
        /// Brandname of the car and model
        /// </summary>
        public string BrandName { get; private set; }
        /// <summary>
        /// Constructor of the car class
        /// </summary>
        /// <param name="year">Year the car was produced in</param>
        /// <param name="weight"></param>
        /// <param name="marketPrice">Average price of the car now</param>
        /// <param name="mileage"></param>
        /// <param name="brandName">Brandname of the car and model</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws when price or mileage is negative</exception>
        /// <exception cref="ArgumentException">Thrown when brandname if empty</exception>
        /// /// <exception cref="ArgumentNullException">Thrown when brandname if null</exception>
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
                    throw new ArgumentNullException("Car`s brand name cannot be empty or contain only spaces", nameof(brandName));
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
