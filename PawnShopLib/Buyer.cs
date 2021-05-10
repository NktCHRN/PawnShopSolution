using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class Buyer : IBuyer
    {
        public decimal Balance { get; private set; }
        public Buyer(decimal balance)
        {
            if (balance >= 0)
                Balance = balance;
            else
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative");
        }
        public void SpendMoney(decimal sum)
        {
            if (sum <= Balance)
                Balance -= sum;
            else
                throw new ArgumentOutOfRangeException(nameof(sum), "Sum to spend cannot be greater than balance");
        }
        public void EarnMoney(decimal sum)
        {
            if (sum >= 0)
                Balance += sum;
            else
                throw new ArgumentOutOfRangeException(nameof(sum), "Sum cannot be negative");
        }
    }
}
