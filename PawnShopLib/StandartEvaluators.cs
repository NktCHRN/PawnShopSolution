using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public static class StandartEvaluators
    {
        public static decimal EvaluateThing(Thing thing)
        {
            if (thing is Things.AntiqueThing antiqueThing)
                return antiqueThing.EstimatedPrice;
            else if (thing is Things.Car car)
                return EvaluateCar(car);
            else if (thing is Things.ElectronicThing electronicThing)
                return EvaluateElectronicThing(electronicThing);
            else if (thing is Things.Jewel jewel)
                return EvaluateJewel(jewel);
            else
                return EvaluateShares(thing as Things.Shares);
        }
        private static decimal EvaluateCar(Things.Car thing)
        {
            decimal cost = thing.MarketPrice * 0.8m - thing.MarketPrice * (decimal)thing.Mileage / 1000000m;
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
                    cost = 7500m - (decimal)(DateTime.Now.Year - thing.Year) * 300m;
                    break;
                case Things.ElectronicTypes.Computer:
                    cost = 11000m - (decimal)(DateTime.Now.Year - thing.Year) * 500m;
                    break;
                case Things.ElectronicTypes.Fridge:
                    cost = 12500m - (decimal)(DateTime.Now.Year - thing.Year) * 250m;
                    break;
                case Things.ElectronicTypes.Phone:
                    cost = 8000m - (decimal)(DateTime.Now.Year - thing.Year) * 600m;
                    break;
                default:
                    cost = 9000m - (decimal)(DateTime.Now.Year - thing.Year) * 250m;
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
