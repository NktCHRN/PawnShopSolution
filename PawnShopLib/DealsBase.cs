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
        public void AddDeal(Deal newDeal)//make internal!!!
        {
            if (newDeal != null)
                _deals.Add(newDeal);
            else
                throw new ArgumentNullException("New deal can`t be null", nameof(newDeal));
        }
        //private static void QuickSort(List<Deal> list, int start, int end, Comparer delToComparer) 
        //{
        //    if (start < end) {
        //        int middle = Partition(list, start, end, delToComparer);
        //        QuickSort(list, start, middle - 1, delToComparer);
        //        QuickSort(list, middle + 1, end, delToComparer);
        //    }
        //}
        //private static int Partition(List<Deal> list, int start, int end, Comparer delToComparer)
        //{
        //    Deal pivot = list[end];
        //    int i = start - 1;
        //    for (int j = start; j < end; j++)
        //        if ((bool)delToComparer?.Invoke(list[j], pivot)) {
        //            i++;
        //            Swap(list, i, j);
        //        }
        //    Swap(list, i + 1, end);
        //    return i + 1;
        //}
        //private static void Swap(List<Deal> list, int left, int right)
        //{
        //    Deal temp;
        //    temp = list[left];
        //    list[left] = list[right];
        //    list[right] = temp;
        //}
    }
}
