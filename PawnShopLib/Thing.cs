using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    [Serializable]
    public abstract class Thing
    {
        /// <summary>
        /// Weight of the thing in grams
        /// </summary>
        public double Weight { get; protected set; }
        /// <summary>
        /// Year the thing was produced in
        /// </summary>
        public int Year { get; protected set; }
        /// <summary>
        /// Costructor of the class Thing
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when weight is smaller than or equal zero
        /// or year is bigger than the year now or year is smaller than 1</exception>
        public Thing(double weight, int year)
        {
            if (weight > 0)
                Weight = weight;
            else
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight cannot be negative or equal zero");
            if (year <= DateTime.Now.Year && year > 0)
                Year = year;
            else
                throw new ArgumentOutOfRangeException(nameof(year), "Year cannot be greater than year now or smaller than 1");
        }
        public abstract override string ToString();
    }
}
