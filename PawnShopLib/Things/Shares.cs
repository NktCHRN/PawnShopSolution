using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib.Things
{
    [Serializable]
    public class Shares : Thing
    {
        /// <summary>
        /// Everage price on stock exchange
        /// </summary>
        public decimal MarketPrice { get; private set; }
        public string CompanyName { get; private set; }
        /// <summary>
        /// The constructor of Shares class
        /// </summary>
        /// <param name="year">Year the shares were bought</param>
        /// <param name="marketPrice">Everage price on stock exchange</param>
        /// <param name="companyName"></param>
        public Shares(int year, decimal marketPrice, string companyName) : base(1, year)
        {
            if (marketPrice >= 0)
                MarketPrice = marketPrice;
            else
                throw new ArgumentOutOfRangeException(nameof(marketPrice), "Price cannot be negative");
            if (companyName != null)
            {
                if (!string.IsNullOrWhiteSpace(companyName))
                    CompanyName = companyName;
                else
                    throw new ArgumentException("Company name cannot be empty or contain only spaces", nameof(companyName));
            }
            else
            {
                throw new ArgumentNullException(nameof(companyName), "CompanyName cannot be null");
            }
        }
        public override string ToString()
        {
            return $"Shares: {CompanyName}; {MarketPrice:F2} hrn";
        }
    }
}
