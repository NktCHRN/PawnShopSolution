using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class Deal
    {
        public delegate decimal Evaluator(Thing thing, Tariffs tariff);
        public Customer Customer { get; private set; }
        public Thing Thing { get; private set; }
        public decimal Price { get; private set; }
        public decimal MarketPrice { get; private set; }
        public DateTime StartTime { get; private set; }
        public int Term { get; private set; }   // in days
        public Tariffs Tariff { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsOnSale { get; private set; }
        public bool IsSuccessful { get; private set; }
        public Deal(Customer customer, Thing thing, Evaluator delToEvaluator, int term, bool onSale = false)
        {
            if (customer != null) {
                if (!customer.IsOnDeal)
                {
                    Customer = customer;
                    Customer.IsOnDeal = true;
                }
                else
                    throw new ArgumentException("You can`t start a new deal while customer is already on deal");//переделать со своим exception!!!
            }
            else
                throw new ArgumentNullException("Customer can`t be null", nameof(customer));
            if (thing != null)
                Thing = thing;
            else
                throw new ArgumentNullException("Thing can`t be null", nameof(thing));
            if (Customer.GetDealsQuantity() <= 6)
                Tariff = Tariffs.Standart;
            else if ((double)Customer.GetSuccessfulDealsQuantity() / Customer.GetUnsuccessfulDealsQuantity() >= 1.5)
                Tariff = Tariffs.Preferential;
            else if ((double)Customer.GetSuccessfulDealsQuantity() / Customer.GetUnsuccessfulDealsQuantity() <= 0.5)
                Tariff = Tariffs.LowPenalty;
            else
                Tariff = Tariffs.Standart;
            if (delToEvaluator != null)
            {
                Price = delToEvaluator.Invoke(Thing, Tariff);
                MarketPrice = Price * 1.5m;
            }
            else
            {
                throw new ArgumentNullException("Evaluator can`t be null", nameof(delToEvaluator));
            }
            if (term > 0)
                Term = term;
            else
                throw new ArgumentException("Term can`t be shorter than one day", nameof(term));
            if (onSale && (thing is Things.Car || thing is Things.ElectronicThing || thing is Things.Jewel))
            {
                IsClosed = true;
                IsOnSale = true;
                IsSuccessful = true;
                Customer.IsOnDeal = false;
            }
            else
            {
                IsClosed = false;
                IsOnSale = false;
                IsSuccessful = false;
            }
            StartTime = DateTime.Now;
        }
        public void CloseDeal()
        {
            if (!IsClosed)
            {
                Customer.IsOnDeal = false;
                IsClosed = true;
                if ((DateTime.Now.Year - StartTime.Year) * 365 + (DateTime.Now.Month - StartTime.Month) * 30 + DateTime.Now.Day - StartTime.Day <= Term)
                    IsSuccessful = true;
                else
                    IsOnSale = true;
            }
        }
        public void ProlongDeal(int additionalTerm)
        {
            //Написать!!!
        }
    }
}
