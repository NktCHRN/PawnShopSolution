using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    [Serializable]
    public class Deal
    {
        public Customer Customer { get; private set; }
        public Thing Thing { get; private set; }
        public decimal Price { get; private set; }
        public decimal RedemptionPrice { get; private set; }
        public decimal MarketPrice { get; private set; }
        public DateTime StartTime { get; private set; }
        /// <summary>
        /// Term in days
        /// </summary>
        public int Term { get; private set; }
        /// <summary>
        /// Max penalty term
        /// </summary>
        public int PenaltyMaxTerm { get; private set; }
        public decimal Penalty { get; private set; }
        public Tariff Tariff { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsOnSale { get; private set; }
        public bool IsSuccessful { get; private set; }
        public decimal PawnShopProfit { get; internal set; }
        /// <summary>
        /// Automatically generated ID (ex.D00000001)
        /// </summary>
        public string ID { get; private set; }
        private static int _dealsCount;
        /// <summary>
        /// Total quantity of deals
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when deals quantity now is not 0 or given value is negative or equal 0 (in setter)</exception>
        public static int DealsCount 
        { 
            get 
            {
                return _dealsCount;
            }
            set
            {
                if (value >= _dealsCount && _dealsCount == 0)
                {
                    _dealsCount = value;
                }
                else
                {
                    throw new InvalidOperationException("DealsCount may be setted only on creation of the project");
                }
            }
        }
        private readonly int _minTerm;
        private readonly int _maxTerm;
        static Deal()
        {
            _dealsCount = 0;
        }
        internal Deal(Customer customer, Thing thing, int term, Tariff tariff, decimal price, decimal perDayCoefficient, int minTerm, int maxTerm)
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
            if (minTerm > 0)
                _minTerm = minTerm;
            else
                throw new ArgumentOutOfRangeException(nameof(minTerm), "Min term cannot be negative or equal zero");
            if (maxTerm < _minTerm)
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
            _dealsCount++;
            const int maxDealsCount = 99999999;
            if (_dealsCount > maxDealsCount)
                throw new OverflowException("Too many deals. Unable to create an ID");
            ID = String.Format("D{0:00000000}", _dealsCount);
        }
        public DateTime GetLastNonPenaltyDate()
        {
            int tempTerm = Term + DateTimeConverter.DateTimeToDays(StartTime);
            return DateTimeConverter.DaysToDateTime(tempTerm);
        }
        public DateTime GetLastDate()
        {
            int tempTerm = Term + PenaltyMaxTerm + DateTimeConverter.DateTimeToDays(StartTime);
            return DateTimeConverter.DaysToDateTime(tempTerm);
        }
        public int GetLastNonPenaltyTerm()
        {
                DateTime currentTime = DateTime.Now;
                int daysLast = Term - (DateTimeConverter.DateTimeToDays(currentTime) - DateTimeConverter.DateTimeToDays(StartTime));
                return (daysLast >= 0) ? daysLast : 0;
        }
        public int GetLastTerm()
        {
                DateTime currentTime = DateTime.Now;
                int daysLast = Term + PenaltyMaxTerm - (DateTimeConverter.DateTimeToDays(currentTime) - DateTimeConverter.DateTimeToDays(StartTime));
                return (daysLast >= 0) ? daysLast : 0;
        }
        /// <summary>
        /// Deals comparer (compares by price)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <exception cref="ArgumentNullException">Thrown if at least one of deals is null</exception>
        /// <returns>1 if left has bigger price, 0 if the prices are equal, -1 if right has bigger price</returns>
        public static int CompareDealsByPrice(Deal left, Deal right)
        {
            if (left != null && right != null)
            {
                if (left.MarketPrice > right.MarketPrice)
                    return 1;
                else if (left.MarketPrice == right.MarketPrice)
                    return 0;
                else
                    return -1;
            }
            else
            {
                throw new ArgumentNullException("At least, one of the params was null");
            }
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
                if (Penalty > 0 && additionalTerm < DateTimeConverter.DateTimeToDays(DateTime.Now) - DateTimeConverter.DateTimeToDays(StartTime) - Term)
                {
                    decimal oldPenalty = CalculatePenalty(perDayCoefficient, additionalTerm);
                    RedemptionPrice += Price * perDayCoefficient * additionalTerm + oldPenalty;
                    SetPenalty(perDayCoefficient, DateTimeConverter.DateTimeToDays(DateTime.Now) - DateTimeConverter.DateTimeToDays(StartTime) - Term - additionalTerm);
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
        /// <summary>
        /// Calculates potential penalty for this deal
        /// </summary>
        /// <param name="perDayCoefficient">Coefficient per day (should be in pawn shop)</param>
        /// <param name="days">Quantity of days for penalty</param>
        /// <returns></returns>
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
