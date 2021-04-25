﻿using System;
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
                throw new ArgumentException("Weight can`t be negative or equal zero", nameof(weight));
            if (year <= DateTime.Now.Year)
                Year = year;
            else
                throw new ArgumentException("Year can`t be greater than year now", nameof(year));
        }
        public abstract Thing GetDeepCopy();
    }
}
