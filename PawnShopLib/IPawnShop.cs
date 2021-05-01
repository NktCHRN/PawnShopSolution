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
        decimal BailThing(Customer customer, Thing myThing, int term);
        void RedeemThing(Customer customer);
        bool TryProlong(Customer customer, int term);
        void BuyThing(Buyer buyer, string thingID);
    }
}
