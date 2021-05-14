﻿using System;
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
        public int MaxTerm { get; private set; }
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
        public PawnShop(string name, decimal initialBalance, Evaluator delToEvaluator = null, decimal perDayCoefficient = 0.005m, int maxTerm = 365)
        {
            if (name != null)
                Name = name;
            else
                Name = "";
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
            if (maxTerm > 4)
                MaxTerm = maxTerm;
            else
                throw new ArgumentOutOfRangeException(nameof(maxTerm), "MaxTerm cannot be smaller than five");
            Revenue = 0;
            Costs = 0;
            _customers = new List<Customer>();
        }
        public Customer AddCustomer(string firstName, string secondName, string patronymic, DateTime birthDay, string password, decimal balance = 0)
        {
            Customer newCustomer = new Customer(firstName, secondName, patronymic, birthDay, password, PerDayCoefficient, balance);
            _customers.Add(newCustomer);
            return newCustomer;
        }
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
        public decimal EstimateThing(Customer customer, Thing myThing) => (decimal)_evaluator?.Invoke(myThing, DefineTariff(customer));
        public decimal GetRedemptionPrice(Customer customer, Thing myThing, int term)
        {
            decimal price = EstimateThing(customer, myThing);
            price += price * PerDayCoefficient * term;
            if (price < 0)
                price = 0;
            return price;
        }
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
                                Deal newDeal = new Deal(customer, myThing, term, tariff, price, PerDayCoefficient, MaxTerm);
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
        public bool TryProlong(Customer customer, int term)
        {
            _deals.Update();
            if (customer != null)
            {
                if (customer.IsOnDeal())
                    return customer.Deals[customer.GetDealsQuantity() - 1].Prolong(term, PerDayCoefficient);
                throw new BusyObjectException("Customer is not on deal.");
            }
            else
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }
        }
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
        public decimal GetNetProfit() => Revenue - Costs;
        internal static int DateTimeToDays(DateTime time)
        {
            int days = 0;
            days += 366;
            for (int i = 1; i < time.Year; i++)
            {
                if (DateTime.IsLeapYear(i))
                    days += 366;
                else
                    days += 365;
            }
            for (int i = 1; i < time.Month; i++)
            {
                days += DateTime.DaysInMonth(time.Year, i);
            }
            days += time.Day;
            return days;
        }
        internal static DateTime DaysToDateTime(int days)
        {
            int year = 1;
            int month = 1;
            days -= 366;
            for (int i = 1; days > (DateTime.IsLeapYear(i) ? 366 : 365); i++)
            {
                if (DateTime.IsLeapYear(i))
                    days -= 366;
                else
                    days -= 365;
                year++;
            }
            for (int i = 1; days > DateTime.DaysInMonth(year, i); i++)
            {
                days -= DateTime.DaysInMonth(year, i);
                month++;
            }
            return new DateTime(year, month, days);
        }
    }
}
