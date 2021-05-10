using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public abstract class Thing
    {
        public double Weight { get; protected set; }
        public int Year { get; protected set; }
        public Thing(double weight, int year)
        {
            if (weight > 0)
                Weight = weight;
            else
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight cannot be negative or equal zero");
            if (year <= DateTime.Now.Year)
                Year = year;
            else
                throw new ArgumentOutOfRangeException(nameof(year), "Year cannot be greater than year now");
        }
        public abstract override string ToString();
    }
}
