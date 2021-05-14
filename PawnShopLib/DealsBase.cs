using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public enum DealSortingTypes
    {
        ID,
        PriceAsceding,
        PriceDescending
    }
    [Serializable]
    public class DealsBase : IEnumerator, IEnumerable
    {
        private readonly List<Deal> _deals;
        private int _position = -1;
        private decimal _perDayCoefficient;
        internal decimal PerDayCoefficient
        {
            set
            {
                Update();
                if (value > 0)
                    _perDayCoefficient = value;
                else
                    throw new ArgumentException("PerDayCoefficient cannot be negative", nameof(value));
            }
        }
        internal DealsBase(decimal perDayCoefficient)
        {
            _deals = new List<Deal>();
            if (perDayCoefficient > 0)
                PerDayCoefficient = perDayCoefficient;
            else
                throw new ArgumentOutOfRangeException(nameof(perDayCoefficient), "Coefficient cannot be smaller than zero");
        }
        public int GetLength() => _deals.Count();
        public IReadOnlyList<Deal> GetFilteredOnSale<T>(DealSortingTypes sortBy = DealSortingTypes.ID) where T : Thing
        {
            Update();
            List<Deal> onSale = new List<Deal>();
            foreach(Deal deal in _deals)
                if (deal.IsOnSale && deal.Thing is T)
                    onSale.Add(deal);
            if (sortBy == DealSortingTypes.PriceAsceding)
                onSale.Sort((left, right) => Deal.CompareDealsByPrice(left, right));
            else if (sortBy == DealSortingTypes.PriceDescending)
                onSale.Sort((left, right) => -Deal.CompareDealsByPrice(left, right));
            else
                onSale.Sort((left, right) => String.Compare(left.ID, right.ID));
            return onSale;
        }
        public IReadOnlyList<Deal> GetFullList(DealSortingTypes sortBy = DealSortingTypes.ID)
        {
            Update();
            List<Deal> FullDealsList = new List<Deal>();
            foreach (Deal deal in _deals)
                FullDealsList.Add(deal);
            if (sortBy == DealSortingTypes.PriceAsceding)
                FullDealsList.Sort((left, right) => Deal.CompareDealsByPrice(left, right));
            else if (sortBy == DealSortingTypes.PriceDescending)
                FullDealsList.Sort((left, right) => -Deal.CompareDealsByPrice(left, right));
            else
                FullDealsList.Sort((left, right) => String.Compare(left.ID, right.ID));
            return FullDealsList;
        }
        public Deal FindDeal(string id)
        {
            Update();
            if (id != null)
            {
                foreach (Deal deal in _deals)
                    if (deal.ID == id)
                        return deal;
                return null;
            }
            else
            {
                throw new ArgumentNullException("Deal`s ID cannot be null", nameof(id));
            }
        }
        public Deal this[int id]
        {
            get
            {
                Update();
                if (id >= 0 && id < _deals.Count)
                    return _deals[id];
                else
                    throw new IndexOutOfRangeException($"{nameof(id)} was not in bounds of the Base (should be from 0 to {_deals.Count - 1})");
            }
        }
        internal void Add(Deal newDeal)
        {
            if (newDeal != null)
                _deals.Add(newDeal);
            else
                throw new ArgumentNullException("New deal cannot be null", nameof(newDeal));
        }
        public IEnumerator GetEnumerator()
        {
            Reset();
            return (IEnumerator)this;
        }
        //IEnumerator
        public bool MoveNext()
        {
            if (_position < _deals.Count() - 1)
            {
                _position++;
                return true;
            }
            else
                return false;
        }
        //IEnumerable
        public void Reset()
        {
            _position = -1;
        }
        //IEnumerable
        public object Current
        {
            get
            {
                if (_position == -1 || _position >= _deals.Count())
                    throw new InvalidOperationException();
                return _deals[_position];
            }
        }
        public void Update()
        {
            DateTime currentTime = DateTime.Now;
            for (int i = 0; i < _deals.Count(); i++)
            {
                if (!_deals[i].IsClosed)
                {
                    if (PawnShop.DateTimeToDays(currentTime) - PawnShop.DateTimeToDays(_deals[i].StartTime) > _deals[i].Term + _deals[i].PenaltyMaxTerm)
                        _deals[i].Close(true);//+ в логинг
                    else if (PawnShop.DateTimeToDays(currentTime) - PawnShop.DateTimeToDays(_deals[i].StartTime) > _deals[i].Term)
                        _deals[i].SetPenalty(_perDayCoefficient, (PawnShop.DateTimeToDays(DateTime.Now) - PawnShop.DateTimeToDays(_deals[i].StartTime) - _deals[i].Term));
                }
            }
        }
    }
}
