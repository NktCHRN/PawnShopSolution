using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public sealed class Customer : IBuyer
    {
        private static int _customersQuantity = 0;
        public string FirstName { get; private set; }
        public string SecondName { get; private set; }
        public string Patronymic { get; private set; }
        public DateTime BirthDay { get; private set; }
        public string ID { get; private set; }
        public DealsBase Deals { get; private set; }
        public decimal Balance { get; private set; }
        internal Customer(string firstName, string secondName, string patronymic, DateTime birthDay, decimal balance = 0)
        {
            if (firstName != null)
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    FirstName = firstName;
                else
                    throw new ArgumentException("First name can`t be empty or contain only spaces", nameof(firstName));
            }
            else
            {
                throw new ArgumentNullException("First name can`t be null", nameof(firstName));
            }
            if (secondName != null)
            {
                if (!string.IsNullOrWhiteSpace(secondName))
                    SecondName = secondName;
                else
                    throw new ArgumentException("Second name can`t be empty or contain only spaces", nameof(secondName));
            }
            else
            {
                throw new ArgumentNullException("Second name can`t be null", nameof(secondName));
            }
            if (patronymic != null)
                Patronymic = patronymic;
            else
                throw new ArgumentNullException("Patronymic can`t be null", nameof(patronymic));
            if (birthDay != null)
            {
                if (DateTime.Now.Year - birthDay.Year > 18 || (DateTime.Now.Year - birthDay.Year == 18 && (birthDay.Month < DateTime.Now.Month || (birthDay.Month == DateTime.Now.Month && birthDay.Day < DateTime.Now.Day))))
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
                    throw new TooYoungPersonException(18, age);
                }
            }
            else
            {
                throw new ArgumentNullException("Birthday can`t be null", nameof(birthDay));
            }
            _customersQuantity++;
            ID = String.Format("C{0:00000000}", _customersQuantity);
            Deals = new DealsBase();
            if (balance >= 0)
                Balance = balance;
            else
                throw new ArgumentException("Balance can`t be negative", nameof(balance));
        }
        public string GetFullName() => String.Format("{0} {1} {2}", SecondName, FirstName, Patronymic);
        public int GetSuccessfulDealsQuantity()
        {
            int quantity = 0;
            foreach(Deal deal in Deals)
                if (deal.IsSuccessful)
                    quantity += 1;
            return quantity;
        }
        public int GetUnsuccessfulDealsQuantity()
        {
            int quantity = 0;
            foreach (Deal deal in Deals)
                if (!deal.IsSuccessful && deal.IsClosed)
                    quantity += 1;
            return quantity;
        }
        public int GetDealsQuantity() => Deals.GetDealsQuantity();
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
                throw new ArgumentNullException("Deal can`t be null", nameof(deal));
            }
        }
        public bool IsOnDeal()
        {
            if (Deals.GetDealsQuantity() > 0)
                return !Deals[Deals.GetDealsQuantity() - 1].IsClosed;
            else
                return false;
        }
        public Deal GetLastDeal()
        {
            if (Deals.GetDealsQuantity() > 0)
                return Deals[Deals.GetDealsQuantity() - 1];
            else
                return null;
        }
        public void SpendMoney(decimal sum)
        {
            if (sum <= Balance)
                Balance -= sum;
            else
                throw new ArgumentException("Sum to spend can`t be greater than balance", nameof(sum));
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
