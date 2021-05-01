using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib.Things
{
    public class Shares : Thing
    {
        public int MarketPrice { get; private set; }
        public string CompanyName { get; private set; }
        public Shares(int year, int marketPrice, string companyName) : base(1, year)
        {
            if (marketPrice >= 0)
                MarketPrice = marketPrice;
            else
                throw new ArgumentException("Price can`t be negative", nameof(marketPrice));
            if (companyName != null)
                CompanyName = companyName;
            else
                throw new ArgumentNullException("CompanyName can`t be null", nameof(companyName));
        }
        public override string ToString()
        {
            return $"Shares: {CompanyName}; {MarketPrice} hrn";
        }
    }
}
