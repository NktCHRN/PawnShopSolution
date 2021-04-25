using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class Buyer
    {
        public decimal Balance { get; private set; }
        public Buyer(decimal balance)
        {
            if (balance >= 0)
                Balance = balance;
            else
                throw new ArgumentException("Balance can`t be negative", nameof(balance));
        }
        public void SpendMoney(decimal sum)
        {
            if (sum <= Balance)
                Balance -= sum;
            else
                throw new ArgumentException("Sum to spend can`t be greater than balance", nameof(sum));//мб переделать со своим Exception
        }
        public void EarnMoney(decimal sum)
        {
            if (sum >= 0)
                Balance += sum;
            else
                throw new ArgumentException("Sum can`t be negative", nameof(sum));
        }
    }
}
