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
        public DealsBase Deals { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Revenue { get; private set; }
        public decimal Costs { get; private set; }
        public PawnShop(string name, decimal initialBalance, Evaluator delToEvaluator)
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
                throw new ArgumentNullException("Evaluator can`t be null", nameof(delToEvaluator));
            Revenue = 0;
            Costs = 0;
        }
        public decimal EstimateThing(Customer customer, Thing myThing) => _evaluator.Invoke(myThing, DefineTariff(customer));
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
        public decimal BailThing(Customer customer, Thing myThing, int period)
        {
            throw new NotImplementedException();
        }
        public void RedeemThing(string thingID)
        {
            throw new NotImplementedException();
        }
        public void Prolong(Customer customer, int period)
        {
            throw new NotImplementedException();
        }
        public void BuyThing(Buyer buyer, string thingID)
        {
            throw new NotImplementedException();
        }
    }
}
