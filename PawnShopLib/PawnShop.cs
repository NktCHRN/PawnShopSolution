using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public delegate decimal Evaluator(Thing thing, Tariff tariff);
    [Serializable]
    public sealed class PawnShop : IPawnShop
    {
        private readonly Evaluator _evaluator;
        private decimal _perDayCoefficient;
        public int MinTerm { get; private set; }
        public int MaxTerm { get; private set; }
        /// <summary>
        /// Percent of sum added every day on every deal
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value is negative (in setter)</exception>
        public decimal PerDayCoefficient 
        {
            get
            {
                return _perDayCoefficient;
            }
            set
            {
                if (value > 0) 
                {
                    _perDayCoefficient = value;
                    _deals.PerDayCoefficient = value;
                    foreach (Customer customer in _customers)
                        customer.Deals.PerDayCoefficient = value;
                }
                else
                    throw new ArgumentException("PerDayCoefficient cannot be negative", nameof(value));
            }
        }
        private readonly DealsBase _deals;
        private readonly List<Customer> _customers;
        public IReadOnlyList<Customer> CustomersList
        {
            get
            {
                _deals.Update();
                List<Customer> newList = new List<Customer>();
                foreach (Customer customer in _customers)
                    newList.Add(customer);
                newList.Sort((left, right) => String.Compare(left.ID, right.ID));
                return newList;
            }
        }
        public DealsBase Deals { 
            get 
            { 
                _deals.Update();
                return _deals; 
            }
        }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Revenue { get; private set; }
        public decimal Costs { get; private set; }
        /// <summary>
        /// The constructor of the class PawnShop
        /// </summary>
        /// <param name="name"></param>
        /// <param name="initialBalance"></param>
        /// <param name="delToEvaluator">Delegate to evaluator. To use a default one,
        /// just do not write anything or write delegate to EvaluateThing() in StandardEvaluators static class</param>
        /// <param name="perDayCoefficient">Percent of sum added every day on every deal</param>
        /// <param name="minTerm">Minimal term. Should be at least 5</param>
        /// <param name="maxTerm">Max term of any deal</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when initialBalance smaller than 0, 
        /// perDayCoefficient if smaller than or equal zero, 
        /// when minTerm is lower than 5
        /// or when maxTerm is smaller than MinTerm (initially, 5)
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when name is null</exception>
        public PawnShop(string name, decimal initialBalance, decimal perDayCoefficient = 0.005m, int minTerm = 5, int maxTerm = 365, Evaluator delToEvaluator = null)
        {
            if (name != null)
                Name = name;
            else
                throw new ArgumentNullException(nameof(name), "Name cannot be null");
            if (initialBalance >= 0)
                Balance = initialBalance;
            else
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Balance cannot be smaller than zero");
            if (delToEvaluator != null)
                _evaluator = delToEvaluator;
            else
                _evaluator = StandardEvaluators.EvaluateThing;
            if (perDayCoefficient > 0)
                _perDayCoefficient = perDayCoefficient;
            else
                throw new ArgumentOutOfRangeException(nameof(perDayCoefficient), "Coefficient cannot be smaller than zero");
            _deals = new DealsBase(perDayCoefficient);
            const int minPenaltyTerm = 5;
            if (minTerm >= minPenaltyTerm)
                MinTerm = minTerm;
            else
                throw new ArgumentOutOfRangeException(nameof(minTerm), $"Min term cannot be lower than {minPenaltyTerm}");
            if (maxTerm >= MinTerm)
                MaxTerm = maxTerm;
            else
                throw new ArgumentOutOfRangeException(nameof(maxTerm), "MaxTerm cannot be smaller than five");
            Revenue = 0;
            Costs = 0;
            _customers = new List<Customer>();
        }
        /// <summary>
        /// Add a customer
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="secondName"></param>
        /// <param name="patronymic"></param>
        /// <param name="birthDay"></param>
        /// <param name="password"></param>
        /// <param name="balance"></param>
        /// <exception cref="ArgumentNullException">Thrown inside the method if one of strings is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when balance is negative</exception>
        /// <exception cref="TooYoungException">Thrown when the person is younger than 18 years old</exception>
        /// <exception cref="OverflowException">Thrown when there are too many customers (unable to create a new ID)</exception>
        /// <returns>new Customer object</returns>
        public Customer AddCustomer(string firstName, string secondName, string patronymic, DateTime birthDay, string password, decimal balance = 0)
        {
            Customer newCustomer = new Customer(firstName, secondName, patronymic, birthDay, password, PerDayCoefficient, balance);
            _customers.Add(newCustomer);
            return newCustomer;
        }
        /// <summary>
        /// Finds a customer
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException">Thrown when the parameter id is null</exception>
        /// <returns>Customer if found, null if not</returns>
        public Customer FindCustomer(string id)
        {
            _deals.Update();
            if (id != null)
            {
                foreach(Customer customer in _customers)
                    if (customer.ID == id)
                        return customer;
                return null;
            }
            else
            {
                throw new ArgumentNullException("ID cannot be null", nameof(id));
            }
        }
        /// <summary>
        /// Calculates a price of thing
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="myThing"></param>
        /// <exception cref="ArgumentNullException">Thrown when customer of thing is null</exception>
        /// <returns>The price of thing</returns>
        public decimal EstimateThing(Customer customer, Thing myThing)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            else if (myThing == null)
                throw new ArgumentNullException(nameof(myThing), "Thing cannot be null");
            return (decimal)_evaluator?.Invoke(myThing, DefineTariff(customer));
        }
        /// <summary>
        /// Calculates a redemption price
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="myThing"></param>
        /// <param name="term"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when term is negative</exception>
        /// <returns>a redemption price</returns>
        public decimal GetRedemptionPrice(Customer customer, Thing myThing, int term)
        {
            if (term < 0)
                throw new ArgumentOutOfRangeException(nameof(term), "Term cannot be negative");
            decimal price = EstimateThing(customer, myThing);
            price += price * PerDayCoefficient * term;
            if (price < 0)
                price = 0;
            return price;
        }
        /// <summary>
        /// Defines a tariff for a customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>If totally smaller than 6 closed deals, Standard 
        /// Else:
        /// Preferential if 0 unsuccessful deals or successful/unsuccessful coefficient is higher than or equal 1.5
        /// LowPenatry, if successful/unsuccessful coefficient is smaller than or equal 0.5
        /// Standard in other cases</returns>
        public Tariff DefineTariff(Customer customer)
        {
            if (customer.GetSuccessfulDealsQuantity() + customer.GetUnsuccessfulDealsQuantity() < 6)
                return Tariff.Standard;
            else if (customer.GetUnsuccessfulDealsQuantity() == 0 || (double)customer.GetSuccessfulDealsQuantity() / customer.GetUnsuccessfulDealsQuantity() >= 1.5)
                return Tariff.Preferential;
            else if ((double)customer.GetSuccessfulDealsQuantity() / customer.GetUnsuccessfulDealsQuantity() <= 0.5)
                return Tariff.LowPenalty;
            else
                return Tariff.Standard;
        }
        /// <summary>
        /// Bail thing
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="myThing"></param>
        /// <param name="term"></param>
        /// <exception cref="ArgumentNullException">Thrown when customer of thing is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when term is negative</exception>
        /// <exception cref="ArgumentException">Thrown when pawnshop has not got enough money for this deal</exception>
        /// <exception cref="BusyObjectException">Thrown when customer is already on deal (IsOnDeal propetry is true)</exception>
        /// <exception cref="OverflowException">Thrown when there are too many deals (unable to create a new ID)</exception>
        /// <returns>Sum customer got from the deal</returns>
        public decimal BailThing(Customer customer, Thing myThing, int term)
        {
            _deals.Update();
            if (customer != null)
            {
                if (!customer.IsOnDeal())
                {
                    if (myThing != null)
                    {
                        Tariff tariff = DefineTariff(customer);
                        decimal price = _evaluator.Invoke(myThing, tariff);
                        if (Balance >= price)
                        {
                            if (term >= 0 && term <= MaxTerm)
                            {
                                Deal newDeal = new Deal(customer, myThing, term, tariff, price, PerDayCoefficient, MinTerm, MaxTerm);
                                _deals.Add(newDeal);
                                Balance -= price;
                                Costs += price;
                                return price;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(term), $"Term is negative or too big (MaxTerm: {MaxTerm})");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("PawnShop has not got enough money for this deal");
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(myThing), "Thing cannot be null");
                    }
                }
                else
                {
                    throw new BusyObjectException("Customer is already on deal. Close the last deal first");
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }
        }
        /// <summary>
        /// Redeem Thing
        /// </summary>
        /// <param name="customer"></param>
        /// <exception cref="ArgumentNullException">Thrown when customer is null</exception>
        /// <exception cref="ArgumentException">Thrown when customer has not got enough money for this deal</exception>
        /// <exception cref="BusyObjectException">Thrown when customer is not on deal (IsOnDeal propetry is false)</exception>
        /// <returns>Redeemed thing</returns>
        public Thing RedeemThing(Customer customer)
        {
            _deals.Update();
            if (customer != null)
            {
                if (customer.IsOnDeal())
                {
                    decimal price = customer.Deals[customer.GetDealsQuantity() - 1].RedemptionPrice + customer.Deals[customer.GetDealsQuantity() - 1].Penalty;
                    if (customer.Balance >= price)
                    {
                        customer.SpendMoney(price);
                        customer.Deals[customer.GetDealsQuantity() - 1].Close(false);
                        customer.Deals[customer.GetDealsQuantity() - 1].PawnShopProfit = price - customer.Deals[customer.GetDealsQuantity() - 1].Price;
                        Balance += price;
                        Revenue += price;
                        return customer.Deals[customer.GetDealsQuantity() - 1].Thing;
                    }
                    else
                    {
                        throw new ArgumentException($"Customer has not got enought money: {customer.Balance:F3} hrn; required: {price:F3} hrn");
                    }
                }
                else
                {
                    throw new BusyObjectException("Customer is not on deal.");
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }
        }
        /// <summary>
        /// Prolongate a deal
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="term"></param>
        /// <exception cref="ArgumentNullException">Thrown when customer is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when term is negative or equals 0</exception>
        /// <exception cref="BusyObjectException">Thrown when customer is not on deal (IsOnDeal propetry is false)</exception>
        /// <returns>True if prolongation was successful, false if not</returns>
        public bool TryProlong(Customer customer, int term)
        {
            _deals.Update();
            if (customer != null)
            {
                if (term > 0)
                {
                    if (customer.IsOnDeal())
                        return customer.Deals[customer.GetDealsQuantity() - 1].Prolong(term, PerDayCoefficient);
                    else
                        throw new BusyObjectException("Customer is not on deal.");
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(term), "Term cannot be negative or equal 0");
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }
        }
        /// <summary>
        /// Buy a thing
        /// </summary>
        /// <param name="buyer"></param>
        /// <param name="thingID">ID of the thing buyer wants to buy</param>
        /// <exception cref="ArgumentNullException">Thrown when buyer is null or deal was not found</exception>
        /// <exception cref="ArgumentException">Thrown when thing is not on sale now</exception>
        /// <exception cref="ArgumentException">Thrown when buyer has not got enough money for this deal</exception>
        /// <returns>Bought thing</returns>
        public Thing BuyThing(IBuyer buyer, string thingID)
        {
            _deals.Update();
            if (buyer != null)
            {
                Deal found = _deals.FindDeal(thingID);
                if (found != null) {
                    if (found.IsOnSale) {
                        decimal price = found.MarketPrice;
                        if (buyer.Balance >= price)
                        {
                            buyer.SpendMoney(price);
                            Balance += price;
                            found.SellThing();
                            found.PawnShopProfit = price - found.Price;
                            Revenue += price;
                            return found.Thing;
                        }
                        else
                        {
                            throw new ArgumentException($"Buyer has not got enought money: {buyer.Balance:F3} hrn; required: {price:F3} hrn");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Thing was not on sale", nameof(thingID));
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(thingID), "Thing was not found");
                }
            }
            else 
            {
                throw new ArgumentNullException(nameof(buyer), "Buyer cannot be null");
            }
        }
        /// <summary>
        /// Calculate a net profit
        /// </summary>
        /// <returns>Revenue - Costs</returns>
        public decimal GetNetProfit() => Revenue - Costs;
    }
}
