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
        bool RedeemThing(Customer customer);
        bool Prolong(Customer customer, int term);
        bool BuyThing(Buyer buyer, string thingID);
    }
}
