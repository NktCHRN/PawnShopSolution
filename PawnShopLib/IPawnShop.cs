using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public interface IPawnShop
    {
        decimal EstimateThing(Customer customer, Thing myThing);
        decimal BailThing(Customer customer, Thing myThing, int period);
        void RedeemThing(string thingID);
        void Prolong(Customer customer, int period);
        void BuyThing(Buyer buyer, string thingID);
    }
}
