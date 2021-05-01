using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public sealed class PawnShop : IPawnShop
    {
        public delegate decimal Evaluator(Thing thing, Tariffs tariff);
        private Evaluator _evaluator;
        private decimal _perDayCoefficient;
        private decimal _saleCoefficient;
        public DealsBase Deals { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Revenue { get; private set; }
        public decimal Costs { get; private set; }
        public PawnShop(string name, decimal initialBalance, Evaluator delToEvaluator = null, decimal perDayCoefficient = 0.005m, decimal saleCoefficient = 1.5m)
        {
            if (name != null)
                Name = name;
            else
                Name = "";
            if (initialBalance >= 0)
                Balance = initialBalance;
            else
                throw new ArgumentException("Balance can`t be less than zero", nameof(initialBalance));
            Deals = new DealsBase();
            if (delToEvaluator != null)
                _evaluator = delToEvaluator;
            else
                _evaluator = StandartEvaluators.EvaluateThing;
            if (perDayCoefficient > 0)
                _perDayCoefficient = perDayCoefficient;
            else
                throw new ArgumentException("Coefficient can`t be less than zero", nameof(perDayCoefficient));
            if (saleCoefficient > 1)
                _saleCoefficient = saleCoefficient;
            else
                throw new ArgumentException("Coefficient can`t be less than zero", nameof(saleCoefficient));
            Revenue = 0;
            Costs = 0;
        }
        public decimal EstimateThing(Customer customer, Thing myThing) => (decimal)_evaluator?.Invoke(myThing, DefineTariff(customer));
        public decimal GetRedemptionPrice(Customer customer, Thing myThing, int term)
        {
            decimal price = EstimateThing(customer, myThing);
            price += price * _perDayCoefficient * term;
            if (price < 0)
                price = 0;
            return price;
        }
        private Tariffs DefineTariff(Customer customer)
        {
            if (customer.GetDealsQuantity() <= 6)
                return Tariffs.Standart;
            else if ((double)customer.GetSuccessfulDealsQuantity() / customer.GetUnsuccessfulDealsQuantity() >= 1.5)
                return Tariffs.Preferential;
            else if ((double)customer.GetSuccessfulDealsQuantity() / customer.GetUnsuccessfulDealsQuantity() <= 0.5)
                return Tariffs.LowPenalty;
            else
                return Tariffs.Standart;
        }
        public decimal BailThing(Customer customer, Thing myThing, int term)
        {
            if (customer != null)
            {
                if (!customer.IsOnDeal())
                {
                    if (myThing != null)
                    {
                        Tariffs tariff = DefineTariff(customer);
                        decimal price = _evaluator.Invoke(myThing, tariff);
                        if (Balance >= price)
                        {
                            Deal newDeal = new Deal(customer, myThing, term, tariff, price, _perDayCoefficient, _saleCoefficient);
                            Deals.AddDeal(newDeal);
                            Balance -= price;
                            Costs += price;
                            return price;
                        }
                        else
                        {
                            throw new ArgumentException("PawnShop hasn`t got enough money for this deal");
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException("Your thing was null", nameof(myThing));
                    }
                }
                else
                {
                    throw new BusyObjectException("Customer is already on deal. Close the last deal first");
                }
            }
            else
            {
                throw new ArgumentNullException("Customer was null", nameof(customer));
            }
        }
        public bool RedeemThing(Customer customer)
        {
            UpdateDeals();
            if (customer != null)
            {
                if (customer.IsOnDeal())
                {
                    decimal price = customer.Deals[customer.GetDealsQuantity() - 1].RedemptionPrice;
                    if (customer.Balance >= price) 
                    {
                        customer.SpendMoney(price);
                        customer.Deals[customer.GetDealsQuantity() - 1].Close(false);
                        Balance += price;
                        Revenue += price;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else
            {
                throw new ArgumentNullException("Customer was null", nameof(customer));
            }
        }
        public bool Prolong(Customer customer, int term)
        {
            UpdateDeals();
            if (customer != null)
            {
                if (customer.IsOnDeal())
                {
                    return customer.Deals[customer.GetDealsQuantity() - 1].ProlongDeal(term, _perDayCoefficient);
                }
                return false;
            }
            else
            {
                throw new ArgumentNullException("Customer was null", nameof(customer));
            }
        }
        public bool BuyThing(Buyer buyer, string thingID)
        {
            UpdateDeals();
            if (buyer != null)
            {
                if (Deals[thingID] != null && Deals[thingID].IsOnSale && buyer.Balance >= Deals[thingID].MarketPrice) {
                    decimal price = Deals[thingID].MarketPrice;
                    buyer.SpendMoney(price);
                    Balance += price;
                    Deals[thingID].SellThing();
                    Revenue += price;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else 
            {
                throw new ArgumentNullException("Buyer was null", nameof(buyer));
            }
        }
        public void UpdateDeals()
        {
            DateTime currentTime = DateTime.Now;
            for(int i = 0; i < Deals.GetDealsQuantity(); i++)
                if (!Deals[i].IsClosed && (currentTime.Year - Deals[i].StartTime.Year) * 365 + (currentTime.Month - Deals[i].StartTime.Month) * 30 + currentTime.Day - Deals[i].StartTime.Day > Deals[i].Term)
                    Deals[i].Close(true);
        }
    }
}
