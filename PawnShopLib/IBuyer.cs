using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public interface IBuyer
    {
        decimal Balance { get; }
        void SpendMoney(decimal sum);
        void EarnMoney(decimal sum);
    }
}
