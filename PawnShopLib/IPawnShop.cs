using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public interface IPawnShop
    {
        decimal EstimateThing(Customer customer, Thing thing);
        decimal BailThing(Customer customer, Thing thing, int term);
        Thing RedeemThing(Customer customer);
        bool TryProlong(Customer customer, int term);
        Thing BuyThing(IBuyer buyer, string thingID);
    }
}
