using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public static class StandardEvaluators
    {
        /// <summary>
        /// Evaluates the thing of any derived from the Thing type
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="tariff"></param>
        /// <exception cref="ArgumentNullException">Thrown when thing is null</exception>
        /// <returns>Price of the thing</returns>
        public static decimal EvaluateThing(Thing thing, Tariff tariff)
        {
            if (thing != null)
            {
                decimal price;
                if (thing is Things.AntiqueThing antiqueThing)
                    price = antiqueThing.EstimatedPrice / 1.5m;
                else if (thing is Things.Car car)
                    price = EvaluateCar(car);
                else if (thing is Things.ElectronicThing electronicThing)
                    price = EvaluateElectronicThing(electronicThing);
                else if (thing is Things.Jewel jewel)
                    price = EvaluateJewel(jewel);
                else if (thing is Things.Shares share)
                    price = EvaluateShares(share);
                else
                    price = 0;
                price *= (decimal)tariff / 100m;
                return price;
            }
            else
            {
                throw new ArgumentNullException(nameof(thing), "Thing cannot be null");
            }
        }
        private static decimal EvaluateCar(Things.Car thing)
        {
            decimal cost;
            if (thing.Mileage / 1000000m >= (DateTime.Now.Year - thing.Year) / 100m)
                cost = thing.MarketPrice * 0.8m - thing.MarketPrice * thing.Mileage / 1000000m;
            else
                cost = thing.MarketPrice * 0.8m - thing.MarketPrice * (DateTime.Now.Year - thing.Year) / 100m;
            if (cost > 0m)
                return cost;
            else
                return 0;
        }
        private static decimal EvaluateElectronicThing(Things.ElectronicThing thing)
        {
            decimal cost;
            switch (thing.Type)
            {
                case Things.ElectronicTypes.Camera:
                    cost = 7500m - (DateTime.Now.Year - thing.Year) * 300m;
                    break;
                case Things.ElectronicTypes.Computer:
                    cost = 11000m - (DateTime.Now.Year - thing.Year) * 500m;
                    break;
                case Things.ElectronicTypes.Fridge:
                    cost = 12500m - (DateTime.Now.Year - thing.Year) * 250m;
                    break;
                case Things.ElectronicTypes.Phone:
                    cost = 8000m - (DateTime.Now.Year - thing.Year) * 600m;
                    break;
                default:
                    cost = 9000m - (DateTime.Now.Year - thing.Year) * 250m;
                    break;
            }
            if (cost > 0m)
                return cost;
            else
                return 0;
        }
        private static decimal EvaluateJewel(Things.Jewel thing)
        {
            decimal cost = (decimal)thing.GoldWeight * 1200m + (decimal)thing.SilverWeight * 10m + (decimal)thing.DiamondWeight * 100000m + (decimal)thing.OtherGemsWeight * 1000m;
            return cost;
        }
        private static decimal EvaluateShares(Things.Shares thing)
        {
            decimal cost = thing.MarketPrice * 0.8m;
            return cost;
        }
    }
}
