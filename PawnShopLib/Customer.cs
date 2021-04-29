using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopLib
{
    public class Customer : IPerson
    {
        private static int _customersQuantity = 0;
        public bool IsOnDeal { get; internal set; }
        public string FirstName { get; private set; }
        public string SecondName { get; private set; }
        public string Patronymic { get; private set; }
        public DateTime BirthDay { get; private set; }
        public string ID { get; private set; }
        public List<Deal> Deals { get; private set; }
        public Customer(string firstName, string secondName, string patronymic, DateTime birthDay)
        {
            if (firstName != null)
                FirstName = firstName;
            else
                throw new ArgumentNullException(nameof(firstName));
            if (secondName != null)
                SecondName = secondName;
            else
                throw new ArgumentNullException(nameof(secondName));
            if (patronymic != null)
                Patronymic = patronymic;
            else
                throw new ArgumentNullException(nameof(patronymic));
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
                    throw new TooYoungPersonException(18, age);
                }
            }
            else
            {
                throw new ArgumentNullException("Birthday can`t be null", nameof(birthDay));
            }
            _customersQuantity++;
            ID = String.Format("C{0:00000000}", _customersQuantity);
            IsOnDeal = false;
            Deals = new List<Deal>();
        }
        public string GetFullName() => String.Format("{0} {1} {2}", FirstName, SecondName, Patronymic);
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
                if (!deal.IsSuccessful)
                    quantity += 1;
            return quantity;
        }
        public int GetDealsQuantity() => Deals.Count;
    }
}
