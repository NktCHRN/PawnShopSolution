using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    [Serializable]
    public class Buyer : IBuyer
    {
        public decimal Balance { get; private set; }
        /// <summary>
        /// The constructor of Buyer class
        /// </summary>
        /// <param name="balance"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when balance is negative</exception>
        public Buyer(decimal balance)
        {
            if (balance >= 0)
                Balance = balance;
            else
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative");
        }
        /// <summary>
        /// Method for spending money (balance is subtracted on sum)
        /// </summary>
        /// <param name="sum"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when sum is bigger than balance</exception>
        public void SpendMoney(decimal sum)
        {
            if (sum <= Balance)
                Balance -= sum;
            else
                throw new ArgumentOutOfRangeException(nameof(sum), "Sum to spend cannot be greater than balance");
        }
        /// <summary>
        /// Method for earning money (sum is added to the balance)
        /// </summary>
        /// <param name="sum"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when sum is negative</exception>
        public void EarnMoney(decimal sum)
        {
            if (sum >= 0)
                Balance += sum;
            else
                throw new ArgumentOutOfRangeException(nameof(sum), "Sum cannot be negative");
        }
    }
}
