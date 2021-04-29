using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib.Things
{
    public class Car : Thing
    {
        public int MarketPrice { get; private set; }
        public int Mileage { get; private set; }
        public Car(double weight, int year, int marketPrice, int mileage) : base(weight, year)
        {
            if (marketPrice >= 0)
                MarketPrice = marketPrice;
            else
                throw new ArgumentException("Price can`t be negative", nameof(marketPrice));
            if (mileage >= 0)
                Mileage = mileage;
            else
                throw new ArgumentException("Mileage can`t be negative", nameof(mileage));
        }
    }
}
