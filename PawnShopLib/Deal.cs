using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class Deal
    {
        //public delegate decimal Evaluator(Thing thing, Tariffs tariff);ПЕРЕНЕСТИ В PawnShop!!!
        public Customer Customer { get; private set; }
        public Thing Thing { get; private set; }
        public decimal Price { get; private set; }
        public decimal RedemptionPrice { get; private set; }
        public decimal MarketPrice { get; private set; }
        public DateTime StartTime { get; private set; }
        public int Term { get; private set; }   // in days
        public Tariffs Tariff { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsOnSale { get; private set; }
        public bool IsSuccessful { get; private set; }
        public string ID { get; private set; }
        public static int DealsCount { get; private set; }
        static Deal()
        {
            DealsCount = 0;
        }
        public Deal(Customer customer, Thing thing, int term, Tariffs tariff, decimal price, decimal coefficient = 1.5m)//сделать internal!!!
        {
            if (customer != null) {
                if (!customer.IsOnDeal())
                {
                    Customer = customer;
                    Customer.AddDeal(this);
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
            if (term >= 0)
            {
                Term = term;
                IsClosed = IsOnSale = IsSuccessful = term == 0 && (thing is Things.Car || thing is Things.ElectronicThing || thing is Things.Jewel);
                //if (term == 0 && (thing is Things.Car || thing is Things.ElectronicThing || thing is Things.Jewel))
                //{
                //    IsClosed = true;
                //    IsOnSale = true;
                //    IsSuccessful = true;
                //}
                //else
                //{
                //    IsClosed = false;
                //    IsOnSale = false;
                //    IsSuccessful = false;
                //}
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
            if (coefficient < 1)
                coefficient = 1.5m;
            RedemptionPrice = price + price * 0.1m * term;
            MarketPrice = price * coefficient;
            //ВСЕ ПРОВЕРКИ СДЕЛАТЬ В PawnShop!!!
            //if (Customer.GetDealsQuantity() <= 6)
            //    Tariff = Tariffs.Standart;
            //else if ((double)Customer.GetSuccessfulDealsQuantity() / Customer.GetUnsuccessfulDealsQuantity() >= 1.5)
            //    Tariff = Tariffs.Preferential;
            //else if ((double)Customer.GetSuccessfulDealsQuantity() / Customer.GetUnsuccessfulDealsQuantity() <= 0.5)
            //    Tariff = Tariffs.LowPenalty;
            //else
            //    Tariff = Tariffs.Standart;
            //if (term >= 0 && term <= 365)
            //{
            //    if (term > 7) 
            //    {
            //        Term = term;
            //        IsClosed = false;
            //        IsOnSale = false;
            //        IsSuccessful = false;
            //    }
            //    else if (term == 0 && (thing is Things.Car || thing is Things.ElectronicThing || thing is Things.Jewel))
            //    {
            //        Term = 0;
            //        IsClosed = true;
            //        IsOnSale = true;
            //        IsSuccessful = true;
            //    }
            //    else
            //    {
            //        throw new ArgumentException("Term should be sevel or more days or (go on sale immediately (0 days) and have such a type as Car, ElectronicThing or Jewel)", nameof(term));
            //    }
            //}
            //else
            //{
            //    throw new ArgumentException("Term can`t be negative or greater than a year", nameof(term));
            //}
            //if (delToEvaluator != null)
            //{
            //    Price = delToEvaluator.Invoke(Thing, Tariff);
            //    MarketPrice = Price * 1.5m;
            //}
            //else
            //{
            //    Price = StandartEvaluators.EvaluateThing(Thing, Tariff);
            //    MarketPrice = Price * 1.5m;
            //}
            StartTime = DateTime.Now;
            DealsCount++;
            ID = String.Format("D{0:00000000}", DealsCount);
        }
        //public static bool HasGreaterPrice(Deal left, Deal right) => left.MarketPrice > right.MarketPrice;
        //public static bool HasSmallerPrice(Deal left, Deal right) => left.MarketPrice < right.MarketPrice;
        public static int CompareDealsByPrice(Deal left, Deal right)
        {
            if (left.MarketPrice > right.MarketPrice)
                return 1;
            else if (left.MarketPrice == right.MarketPrice)
                return 0;
            else
                return -1;
        }
        public void CloseDeal(bool toSale)//make internal!!!
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
                    IsSuccessful = true;
                }
            }
        }
        public bool ProlongDeal(int additionalTerm)//make internal!!!
        {
            if (additionalTerm > 0 && RedemptionPrice + Price * 0.1m * additionalTerm <= MarketPrice)
            {
                Term += additionalTerm;
                RedemptionPrice += Price * 0.1m * additionalTerm;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
