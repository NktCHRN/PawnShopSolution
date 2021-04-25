using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class PawnShop
    {
        private CustomersBase _customersBase;
        public string Name { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Revenue { get; private set; }
        public decimal Costs { get; private set; }
        public PawnShop(string name, decimal initialBalance)
        {
            if (name != null)
                Name = name;
            else
                Name = "";
            if (initialBalance >= 0)
                Balance = initialBalance;
            else
                throw new Exception("Balance can`t be less than zero");
            _customersBase = new CustomersBase();
            Revenue = 0;
            Costs = 0;
        }
    }
}
