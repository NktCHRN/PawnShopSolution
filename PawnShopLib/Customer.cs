using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    [Serializable]
    public sealed class Customer : IBuyer
    {
        private static int _customersQuantity = 0;
        public static int CustomersQuantity
        {
            get
            {
                return _customersQuantity;
            }
            set
            {
                if (value > _customersQuantity && _customersQuantity == 0)
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
        public string ID { get; private set; }
        private string _password;
        public string Password
        {
            set
            {
                if (value != null)
                {
                    const int minSize = 4;
                    if (value.Length >= minSize)
                    {
                        foreach (char symbol in value)
                        {
                            if (char.IsWhiteSpace(symbol))
                                throw new ArgumentException("Password should not contain whitespaces");
                        }
                        _password = value;
                    }
                    else
                    {
                        throw new ArgumentException($"Password should have at least {minSize} characters");
                    }
                }
                else
                {
                    throw new ArgumentNullException("Password can`t be null", nameof(value));
                }
            }
        }
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
        internal Customer(string firstName, string secondName, string patronymic, DateTime birthDay, string password, decimal perDayCoefficient, decimal balance = 0)
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
            Password = password;
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
        public bool IsOnDeal()
        {
            DealsBase deals = Deals;
            if (deals.GetLength() > 0)
                return !deals[deals.GetLength() - 1].IsClosed;
            else
                return false;
        }
        public Deal GetLastDeal()
        {
            DealsBase deals = Deals;
            if (deals.GetLength() > 0)
                return deals[deals.GetLength() - 1];
            else
                return null;
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
        public bool CheckPassword(string toCheck)
        {
            if (toCheck != null)
                return _password == toCheck;
            else
                throw new ArgumentNullException(nameof(toCheck), "Possible password can`t be null");
        }
    }
}
