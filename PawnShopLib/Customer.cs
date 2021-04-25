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
        private bool _isOnDeal;
        public bool IsOnDeal { 
            get 
            {
                return _isOnDeal;
            }
            internal set 
            {
                if (_isOnDeal == true)
                    throw new ArgumentException("You can`t start a new deal while your customer is already on deal. Close deal using one of methods.");
                else
                    _isOnDeal = value;
            }
        }
        public int SuccessfulDeals { get; private set; }
        public int UnsuccessfulDeals { get; private set; }
        public string FirstName { get; private set; }
        public string SecondName { get; private set; }
        public string Patronymic { get; private set; }
        public DateTime BirthDay { get; private set; }
        public string ID { get; private set; }
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
                throw new ArgumentNullException(nameof(birthDay));
            }
            _customersQuantity++;
            ID = String.Format("C{0:00000000}", _customersQuantity);
            SuccessfulDeals = 0;
            UnsuccessfulDeals = 0;
            _isOnDeal = false;
        }
        public string GetFullName() => String.Format("{0} {1} {2}", FirstName, SecondName, Patronymic);
        internal void AddSuccessFulDeal()
        {
            SuccessfulDeals++;
            _isOnDeal = false;
        }
        internal void AddUnsuccessfulDeal()
        {
            UnsuccessfulDeals++;
            _isOnDeal = false;
        }
        //добавить поле и метод для CreditHistory!!!
    }
}
