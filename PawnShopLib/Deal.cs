using System;
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
        private decimal _penalty;
        public decimal Penalty { 
            get { return _penalty; } 
        }
        public Tariff Tariff { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsOnSale { get; private set; }
        public bool IsSuccessful { get; private set; }
        public decimal PawnShopProfit { get; internal set; }
        public string ID { get; private set; }
        public static int DealsCount { get; private set; }
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
                throw new ArgumentNullException("Customer can`t be null", nameof(customer));
            if (thing != null)
                Thing = thing;
            else
                throw new ArgumentNullException("Thing can`t be null", nameof(thing));
            if (term >= 0)
            {
                Term = term;
                if (term > 4)
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
                    throw new ArgumentException("This type of thing can`t be sold immediately or term was between 1 and 4 days", nameof(thing));
                }
            }
            else
            {
                throw new ArgumentException("Term can`t be negative", nameof(term));
            }
            Tariff = tariff;
            if (price > 0)
                Price = price;
            else
                throw new ArgumentException("Price can`t be negative or equal 0", nameof(price));
            RedemptionPrice = price + price * perDayCoefficient * term;
            MarketPrice = price + price * perDayCoefficient * maxTerm;
            if (RedemptionPrice > MarketPrice)
                MarketPrice = RedemptionPrice;
            StartTime = DateTime.Now;
            PawnShopProfit = 0;
            _penalty = 0;
            DealsCount++;
            ID = String.Format("D{0:00000000}", DealsCount);
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
            if (additionalTerm > 0 && RedemptionPrice + Price * perDayCoefficient * additionalTerm <= MarketPrice)
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
                    _penalty = 0;
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
        private decimal CalculatePenalty(decimal perDayCoefficient, int days)
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
                    _penalty = CalculatePenalty(perDayCoefficient, days);
                else
                    throw new ArgumentException("Too big or small penatly term", nameof(days));
            }
            else
            {
                throw new ArgumentException("PerDayCoefficient can`t be negative", nameof(perDayCoefficient));
            }
        }
    }
}
