using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    [Serializable]
    public class Customer : RegisteredUser, IBuyer
    {
        private static int _customersQuantity = 0;
        /// <summary>
        /// Total quantity of customers
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when customer quantity now is not 0 or given value is negative or equal 0 (in setter)</exception>
        public static int CustomersQuantity
        {
            get
            {
                return _customersQuantity;
            }
            set
            {
                if (value >= _customersQuantity && _customersQuantity == 0)
                {
                    _customersQuantity = value;
                }
                else
                {
                    throw new InvalidOperationException("CustomerQuantity may be setted only on creation of the project");
                }
            }
        }
        public string FirstName { get; private set; }
        public string SecondName { get; private set; }
        public string Patronymic { get; private set; }
        public DateTime BirthDay { get; private set; }
        /// <summary>
        /// Automatically generated ID (ex.C00000001)
        /// </summary>
        public string ID { get; private set; }
        private readonly DealsBase _deals;
        public DealsBase Deals 
        { 
            get 
            {
                _deals.Update();
                return _deals;
            }
        }
        public decimal Balance { get; private set; }
        internal Customer(string firstName, string secondName, string patronymic, DateTime birthDay, string password, decimal perDayCoefficient, decimal balance = 0) : base(password)
        {
            if (firstName != null)
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    FirstName = firstName;
                else
                    throw new ArgumentException("First name cannot be empty or contain only spaces", nameof(firstName));
            }
            else
            {
                throw new ArgumentNullException(nameof(firstName), "First name cannot be null");
            }
            if (secondName != null)
            {
                if (!string.IsNullOrWhiteSpace(secondName))
                    SecondName = secondName;
                else
                    throw new ArgumentException("Second name cannot be empty or contain only spaces", nameof(secondName));
            }
            else
            {
                throw new ArgumentNullException(nameof(secondName), "Second name cannot be null");
            }
            if (patronymic != null)
                Patronymic = patronymic;
            else
                throw new ArgumentNullException(nameof(patronymic), "Patronymic cannot be null");
            const int minimalAge = 18;
                if (DateTime.Now.Year - birthDay.Year > minimalAge || (DateTime.Now.Year - birthDay.Year == 18 && (birthDay.Month < DateTime.Now.Month || (birthDay.Month == DateTime.Now.Month && birthDay.Day < DateTime.Now.Day))))
                {
                    BirthDay = new DateTime(birthDay.Year, birthDay.Month, birthDay.Day);
                }
                else
                {
                    int age = DateTime.Now.Year - birthDay.Year;
                    if (birthDay.Month > DateTime.Now.Month || (birthDay.Month == DateTime.Now.Month && birthDay.Day >= DateTime.Now.Day))
                        age--;
                    if (age < 0)
                        age = 0;
                    throw new TooYoungException(minimalAge, age);
                }
            if (perDayCoefficient > 0)
                _deals = new DealsBase(perDayCoefficient);
            else
                throw new ArgumentOutOfRangeException(nameof(perDayCoefficient), "Per day coefficient cannot be negative");
            if (balance >= 0)
                Balance = balance;
            else
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative");
            _customersQuantity++;
            const int maxCustomers = 99999999;
            if (_customersQuantity > maxCustomers)
                throw new OverflowException("Too many customers. Unable to create an ID");
            ID = String.Format("C{0:00000000}", _customersQuantity);
        }
        public string GetFullName() => String.Format("{0} {1} {2}", SecondName, FirstName, Patronymic);
        public int GetSuccessfulDealsQuantity()
        {
            int quantity = 0;
            DealsBase deals = Deals;
            foreach(Deal deal in deals)
                if (deal.IsSuccessful)
                    quantity += 1;
            return quantity;
        }
        public int GetUnsuccessfulDealsQuantity()
        {
            int quantity = 0;
            DealsBase deals = Deals;
            foreach (Deal deal in deals)
                if (!deal.IsSuccessful && deal.IsClosed)
                    quantity += 1;
            return quantity;
        }
        public int GetDealsQuantity() => Deals.GetLength();
        internal void AddDeal(Deal deal)
        {
            if (deal != null)
            {
                if (!IsOnDeal())
                    Deals.Add(deal);
                else
                    throw new BusyObjectException("Customer is already on deal. Close the last deal first");
            }
            else
            {
                throw new ArgumentNullException("Deal cannot be null", nameof(deal));
            }
        }
        /// <summary>
        /// Checks if customer is on deal now or not
        /// </summary>
        /// <returns>True if the customer is on deal, false if not</returns>
        public bool IsOnDeal()
        {
            DealsBase deals = Deals;
            if (deals.GetLength() > 0)
                return !deals[deals.GetLength() - 1].IsClosed;
            else
                return false;
        }
        /// <summary>
        /// Get the last customer`s deal
        /// </summary>
        /// <returns>The last deal; null if there are no deals yet</returns>
        public Deal GetLastDeal()
        {
            DealsBase deals = Deals;
            if (deals.GetLength() > 0)
                return deals[deals.GetLength() - 1];
            else
                return null;
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
