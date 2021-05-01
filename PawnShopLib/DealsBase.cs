using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public enum SortingTypes
    {
        ID,
        PriceAsceding,
        PriceDescending
    }
    public class DealsBase
    {
        private delegate bool Comparer(Deal left, Deal right);
        private readonly List<Deal> _deals;
        public DealsBase()
        {
            _deals = new List<Deal>();
        }
        public int GetDealsQuantity() => _deals.Count();
        public List<Deal> GetFilteredOnSale<T>(SortingTypes sortBy = SortingTypes.ID) where T : Thing
        {
            List<Deal> onSale = new List<Deal>();
            foreach(Deal deal in _deals)
            {
                if (deal.IsOnSale && deal.Thing is T)
                    onSale.Add(deal);
            }
            if (sortBy == SortingTypes.PriceAsceding)
            {
                onSale.Sort((left, right) => Deal.CompareDealsByPrice(left, right));
            }
            else if (sortBy == SortingTypes.PriceDescending)
            {
                onSale.Sort((left, right) => -Deal.CompareDealsByPrice(left, right));
            }
            else
            {
                onSale.Sort((left, right) => String.Compare(left.ID, right.ID));
            }
            return onSale;
        }
        public List<Deal> GetFullList(SortingTypes sortBy = SortingTypes.ID)
        {
            List<Deal> FullDealsList = new List<Deal>();
            foreach (Deal deal in _deals)
                FullDealsList.Add(deal);
            if (sortBy == SortingTypes.PriceAsceding)
            {
                FullDealsList.Sort((left, right) => Deal.CompareDealsByPrice(left, right));
            }
            else if (sortBy == SortingTypes.PriceDescending)
            {
                FullDealsList.Sort((left, right) => -Deal.CompareDealsByPrice(left, right));
            }
            else
            {
                FullDealsList.Sort((left, right) => String.Compare(left.ID, right.ID));
            }
            return FullDealsList;
        }
        public Deal this[string id]
        {
            get
            {
                if (id != null)
                {
                    foreach (Deal deal in _deals)
                        if (deal.ID == id)
                            return deal;
                    return null;
                }
                else
                {
                    throw new ArgumentNullException("Deal`s ID can`t be null", nameof(id));
                }
            }
        }
        public Deal this[int id]
        {
            get
            {
                if (id >= 0 && id < _deals.Count)
                {
                    return _deals[id];
                }
                else
                {
                    throw new IndexOutOfRangeException($"{nameof(id)} was not in bounds of the Base (should be from 0 to {_deals.Count - 1})");
                }
            }
        }
        internal void AddDeal(Deal newDeal)
        {
            if (newDeal != null)
                _deals.Add(newDeal);
            else
                throw new ArgumentNullException("New deal can`t be null", nameof(newDeal));
        }
    }
}
