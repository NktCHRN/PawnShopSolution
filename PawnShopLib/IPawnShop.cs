using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public interface IPawnShop
    {
        decimal EstimateThing(Thing myThing);
        decimal BailThing(Thing myThing, DateTime period);
        void RedeemThing(string thingID);
        void Prolong(DateTime period);
    }
}
