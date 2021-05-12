﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class Deal
    {
        public Customer Customer { get; private set; }
        public Thing Thing { get; private set; }
        public decimal Price { get; private set; }
        public decimal RedemptionPrice { get; private set; }
        public decimal MarketPrice { get; private set; }
        public DateTime StartTime { get; private set; }
        public int Term { get; private set; }   // in days
        public int PenaltyMaxTerm { get; private set; }
        public decimal Penalty { get; private set; }
        public Tariff Tariff { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsOnSale { get; private set; }
        public bool IsSuccessful { get; private set; }
        public decimal PawnShopProfit { get; internal set; }
        public string ID { get; private set; }
        public static int DealsCount { get; private set; }
        private int _minTerm;
        private int _maxTerm;
        static Deal()
        {
            DealsCount = 0;
        }
        internal Deal(Customer customer, Thing thing, int term, Tariff tariff, decimal price, decimal perDayCoefficient, int maxTerm)
        {
            if (customer != null) {
                if (!customer.IsOnDeal())
                {
                    Customer = customer;
                    Customer.AddDeal(this);
                }
                else
                    throw new BusyObjectException("Customer is already on deal. Close the last deal first");
            }
            else
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            if (thing != null)
                Thing = thing;
            else
                throw new ArgumentNullException(nameof(thing), "Thing cannot be null");
            _minTerm = 5;
            if (maxTerm < 5)
                throw new ArgumentOutOfRangeException(nameof(maxTerm), $"Max term should be at least {_minTerm} days");
            _maxTerm = maxTerm;
            if (term >= 0)
            {
                Term = term;
                if (term >= _minTerm && term <= maxTerm)
                {
                    Term = term;
                    PenaltyMaxTerm = 5;
                    IsClosed = false;
                    IsOnSale = false;
                    IsSuccessful = false;
                }
                else if (term == 0 && (thing is Things.Car || thing is Things.ElectronicThing || thing is Things.Jewel))
                {
                    Term = 0;
                    PenaltyMaxTerm = 0;
                    IsClosed = true;
                    IsOnSale = true;
                    IsSuccessful = true;
                }
                else
                {
                    throw new ArgumentException($"This type of thing cannot be sold immediately or term was between 1 and 4 days or bigger than {maxTerm} days", nameof(thing));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(term), "Term cannot be negative");
            }
            Tariff = tariff;
            if (price > 0)
                Price = price;
            else
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative or equal 0");
            RedemptionPrice = price + price * perDayCoefficient * term;
            MarketPrice = price + price * perDayCoefficient * (maxTerm + _minTerm);
            if (tariff != Tariff.Standard)
                MarketPrice /= (decimal)tariff / 100.0m;
            if (RedemptionPrice > MarketPrice)
                MarketPrice = RedemptionPrice;
            StartTime = DateTime.Now;
            PawnShopProfit = 0;
            Penalty = 0;
            DealsCount++;
            const int maxDealsCount = 99999999;
            if (DealsCount > maxDealsCount)
                throw new OverflowException("Too many deals. Unable to create an ID");
            ID = String.Format("D{0:00000000}", DealsCount);
        }
        public DateTime GetLastNonPenaltyDate()
        {
            int tempTerm = Term + PawnShop.DateTimeToDays(StartTime);
            return PawnShop.DaysToDateTime(tempTerm);
        }
        public DateTime GetLastDate()
        {
            int tempTerm = Term + PenaltyMaxTerm + PawnShop.DateTimeToDays(StartTime);
            return PawnShop.DaysToDateTime(tempTerm);
        }
        public int GetLastNonPenaltyTerm()
        {
                DateTime currentTime = DateTime.Now;
                int daysLast = Term - (PawnShop.DateTimeToDays(currentTime) - PawnShop.DateTimeToDays(StartTime));
                return (daysLast >= 0) ? daysLast : 0;
        }
        public int GetLastTerm()
        {
                DateTime currentTime = DateTime.Now;
                int daysLast = Term + PenaltyMaxTerm - (PawnShop.DateTimeToDays(currentTime) - PawnShop.DateTimeToDays(StartTime));
                return (daysLast >= 0) ? daysLast : 0;
        }
        public static int CompareDealsByPrice(Deal left, Deal right)
        {
            if (left.MarketPrice > right.MarketPrice)
                return 1;
            else if (left.MarketPrice == right.MarketPrice)
                return 0;
            else
                return -1;
        }
        internal void Close(bool toSale)
        {
            if (!IsClosed)
            {
                IsClosed = true;
                if (toSale)
                {
                    IsOnSale = true;
                    IsSuccessful = false;
                }
                else
                {
                    IsOnSale = false;
                    IsSuccessful = Penalty == 0m;
                }
            }
        }
        internal bool Prolong(int additionalTerm, decimal perDayCoefficient)
        {
            if (additionalTerm > 0 && additionalTerm <= GetMaxProlongationTerm())
            {
                if (Penalty > 0 && additionalTerm < PawnShop.DateTimeToDays(DateTime.Now) - PawnShop.DateTimeToDays(StartTime) - Term)
                {
                    decimal oldPenalty = CalculatePenalty(perDayCoefficient, additionalTerm);
                    RedemptionPrice += Price * perDayCoefficient * additionalTerm + oldPenalty;
                    SetPenalty(perDayCoefficient, PawnShop.DateTimeToDays(DateTime.Now) - PawnShop.DateTimeToDays(StartTime) - Term - additionalTerm);
                }
                else
                {
                    RedemptionPrice += Price * perDayCoefficient * additionalTerm + Penalty;
                    Penalty = 0;
                }
                Term += additionalTerm;
                return true;
            }
            else
            {
                return false;
            }
        }
        internal void SellThing()
        {
            IsOnSale = false;
        }
        public decimal CalculatePenalty(decimal perDayCoefficient, int days)
        {
            if (days >= 0)
                return days * perDayCoefficient * 2m * Price;
            else
                return 0;
        }
        internal void SetPenalty(decimal perDayCoefficient, int days)
        {
            if (perDayCoefficient > 0)
            {
                if (days >= 0 && days <= PenaltyMaxTerm)
                    Penalty = CalculatePenalty(perDayCoefficient, days);
                else
                    throw new ArgumentOutOfRangeException(nameof(days), "Too big or small penatly term");
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(perDayCoefficient), "PerDayCoefficient cannot be negative");
            }
        }
        public int GetMaxProlongationTerm() => _maxTerm + _minTerm - Term;
    }
}
