﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public delegate decimal Evaluator(Thing thing, Tariff tariff);
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
                    _perDayCoefficient = value;
                else
                    throw new ArgumentException("PerDayCoefficient can`t be negative", nameof(value));
            }
        }
        private readonly DealsBase _deals;
        private readonly List<Customer> _customers;
        public IReadOnlyList<Customer> CustomersList
        {
            get
            {
                return _customers;
            }
        }
        public DealsBase Deals { 
            get 
            { 
                UpdateDeals();
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
                throw new ArgumentException("Balance can`t be less than zero", nameof(initialBalance));
            _deals = new DealsBase();
            if (delToEvaluator != null)
                _evaluator = delToEvaluator;
            else
                _evaluator = StandartEvaluators.EvaluateThing;
            if (perDayCoefficient > 0)
                PerDayCoefficient = perDayCoefficient;
            else
                throw new ArgumentException("Coefficient can`t be less than zero", nameof(perDayCoefficient));
            if (maxTerm > 0)
                MaxTerm = maxTerm;
            else
                throw new ArgumentException("MaxTerm can`t be smaller than or equal zero", nameof(maxTerm));
            Revenue = 0;
            Costs = 0;
            _customers = new List<Customer>();
        }
        public Customer AddCustomer(string firstName, string secondName, string patronymic, DateTime birthDay, decimal balance = 0)
        {
            Customer newCustomer = new Customer(firstName, secondName, patronymic, birthDay, balance);
            _customers.Add(newCustomer);
            return newCustomer;
        }
        public Customer FindCustomer(string id)
        {
            if (id != null)
            {
                foreach(Customer customer in _customers)
                {
                    if (customer.ID == id)
                        return customer;
                }
                return null;
            }
            else
            {
                throw new ArgumentNullException("ID can`t be null", nameof(id));
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
        private Tariff DefineTariff(Customer customer)
        {
            if (customer.GetDealsQuantity() <= 6)
                return Tariff.Standart;
            else if ((double)customer.GetSuccessfulDealsQuantity() / customer.GetUnsuccessfulDealsQuantity() >= 1.5)
                return Tariff.Preferential;
            else if ((double)customer.GetSuccessfulDealsQuantity() / customer.GetUnsuccessfulDealsQuantity() <= 0.5)
                return Tariff.LowPenalty;
            else
                return Tariff.Standart;
        }
        public decimal BailThing(Customer customer, Thing myThing, int term)
        {
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
                                throw new ArgumentException($"Term is negative or too big (MaxTerm: {MaxTerm})", nameof(term));
                            }
                        }
                        else
                        {
                            throw new ArgumentException("PawnShop hasn`t got enough money for this deal");
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException("Thing can`t be null", nameof(myThing));
                    }
                }
                else
                {
                    throw new BusyObjectException("Customer is already on deal. Close the last deal first");
                }
            }
            else
            {
                throw new ArgumentNullException("Customer can`t be null", nameof(customer));
            }
        }
        public void RedeemThing(Customer customer)
        {
            UpdateDeals();
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
                    }
                    else
                    {
                        throw new ArgumentException($"Customer hasn`t got enought money: {customer.Balance:F3}; required: {price:F3}");
                    }
                }
                else
                {
                    throw new BusyObjectException("Customer is not on deal.");
                }
            }
            else
            {
                throw new ArgumentNullException("Customer can`t be null", nameof(customer));
            }
        }
        public bool TryProlong(Customer customer, int term)
        {
            UpdateDeals();
            if (customer != null)
            {
                if (customer.IsOnDeal())
                {
                    return customer.Deals[customer.GetDealsQuantity() - 1].Prolong(term, PerDayCoefficient);
                }
                throw new BusyObjectException("Customer is not on deal.");
            }
            else
            {
                throw new ArgumentNullException("Customer can`t be null", nameof(customer));
            }
        }
        public void BuyThing(Buyer buyer, string thingID)
        {
            UpdateDeals();
            if (buyer != null)
            {
                if (_deals[thingID] != null && _deals[thingID].IsOnSale) {
                    decimal price = _deals[thingID].MarketPrice;
                    if (buyer.Balance >= price)
                    {
                        buyer.SpendMoney(price);
                        Balance += price;
                        _deals[thingID].SellThing();
                        _deals[thingID].PawnShopProfit = price - _deals[thingID].Price;
                        Revenue += price;
                    }
                    else
                    {
                        throw new ArgumentException($"Buyer hasn`t got enought money: {buyer.Balance:F3}; required: {price:F3}");
                    }
                }
                else
                {
                    throw new ArgumentNullException("Thing wasn`t found or not on sale", nameof(thingID));
                }
            }
            else 
            {
                throw new ArgumentNullException("Buyer can`t be null", nameof(buyer));
            }
        }
        public void UpdateDeals()
        {
            DateTime currentTime = DateTime.Now;
            for (int i = 0; i < _deals.GetDealsQuantity(); i++)
            {
                if (!_deals[i].IsClosed)
                {
                    if (DateTimeToDays(currentTime) - DateTimeToDays(_deals[i].StartTime) > _deals[i].Term + _deals[i].PenaltyMaxTerm)
                        _deals[i].Close(true);//+ в логинг
                    else if (DateTimeToDays(currentTime) - DateTimeToDays(_deals[i].StartTime) > _deals[i].Term)
                    {
                        _deals[i].SetPenalty(PerDayCoefficient, (DateTimeToDays(DateTime.Now) - DateTimeToDays(_deals[i].StartTime) - _deals[i].Term));
                    }
                }
            }
        }
        public decimal GetNetProfit() => Revenue - Costs;
        internal static int DateTimeToDays(DateTime time)
        {
            return time.Year * 365 + time.Month * 30 + time.Day;
        }
    }
}
