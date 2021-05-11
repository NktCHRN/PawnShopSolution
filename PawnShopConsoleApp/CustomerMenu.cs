using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawnShopLib;

namespace PawnShopConsoleApp
{
    public static class CustomerMenu
    {
        public static void LoginCustomer(PawnShop pawnShop)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            string id;
            Customer customer;
            Console.WriteLine("Are you already a cutomer? ");
            Console.WriteLine("If yes, enter your ID (ex. C00000001)");
            Console.WriteLine("Otherwise, enter 0");
            id = Console.ReadLine();
            customer = pawnShop.FindCustomer(id);
            while (customer == null && id.Trim() != "0")
            {
                Console.WriteLine("Not found.");
                Console.WriteLine("Enter the id of the customer once more (ex. C00000001):");
                Console.WriteLine("Enter 0 to quit");
                id = Console.ReadLine();
                customer = pawnShop.FindCustomer(id);
            };
            if (id.Trim() == "0")
                customer = RegisterCustomer(pawnShop);
            if (customer != null)
                PrintCustomerMenu(pawnShop, customer);
            Console.ResetColor();
        }
        public static Customer RegisterCustomer(PawnShop pawnShop)
        {
            Customer customer = null;
            string firstName, secondName, patronymic;
            Console.WriteLine("Enter the first name: ");
            firstName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(firstName))
            {
                Console.WriteLine("First name can`t be empty");
                Console.WriteLine("Enter the first name once more: ");
                firstName = Console.ReadLine();
            }
            Console.WriteLine("Enter the second name: ");
            secondName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(secondName))
            {
                Console.WriteLine("First name can`t be empty");
                Console.WriteLine("Enter the second name once more: ");
                secondName = Console.ReadLine();
            }
            Console.WriteLine("Enter the patronymic: ");
            patronymic = Console.ReadLine();
            bool parsed;
            short day, month, year;
            DateTime birthday;
            do
            {
                try
                {
                    Console.WriteLine("Enter the day of birth: ");
                    parsed = short.TryParse(Console.ReadLine(), out day);
                    while (!parsed || day < 0)
                    {
                        Console.WriteLine("Error: day can`t be negative");
                        Console.WriteLine("Enter the day of birth once more: ");
                        parsed = short.TryParse(Console.ReadLine(), out day);
                    }
                    Console.WriteLine("Enter the month of birth: ");
                    parsed = short.TryParse(Console.ReadLine(), out month);
                    while (!parsed || month < 0)
                    {
                        Console.WriteLine("Error: month can`t be negative");
                        Console.WriteLine("Enter the month of birth once more: ");
                        parsed = short.TryParse(Console.ReadLine(), out month);
                    }
                    Console.WriteLine("Enter the year of birth: ");
                    parsed = short.TryParse(Console.ReadLine(), out year);
                    while (!parsed || year < 0)
                    {
                        Console.WriteLine("Error: year can`t be negative");
                        Console.WriteLine("Enter the year of birth once more: ");
                        parsed = short.TryParse(Console.ReadLine(), out year);
                    }
                    birthday = new DateTime(year, month, day);
                    customer = pawnShop.AddCustomer(firstName, secondName, patronymic, birthday);
                    parsed = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Error: wrong date");
                    Console.WriteLine("Please, enter the date once more");
                    parsed = false;
                }
                catch (TooYoungException exc)
                {
                    Console.WriteLine("Registration denied: you are not mature enough");
                    Console.WriteLine(exc.Message);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nPress [ENTER] to go back to main menu");
                    Console.ReadLine();
                    parsed = true;
                }
            } while (!parsed);
            return customer;
        }
        public static void PrintCustomerMenu(PawnShop pawnShop, Customer customer)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Hello, {customer.GetFullName()}");
            Console.WriteLine("What do you want to do?");
            PrintHelp();
            bool parsed;
            int choice;
            const int minPoint = 1;
            const int maxPoint = 8;
            do
            {
                Console.WriteLine($"\nEnter the number {minPoint} - {maxPoint}: ");
                parsed = int.TryParse(Console.ReadLine(), out choice);      //пытаемся спарсить введенное в int
                if (!parsed || choice < minPoint || choice > maxPoint)
                {
                    Console.WriteLine($"Error: you entered not a number or number was smaller than {minPoint} or bigger than {maxPoint}.");
                    Console.WriteLine($"Help - {maxPoint - 1}");
                    choice = minPoint - 1;
                }
                switch (choice)
                {
                    case 1:
                        MainMenu.PrintCustomer(customer);
                        break;
                    case 2:

                        break;
                    case 3:

                        break;
                    case 4:
                        if (customer.IsOnDeal())
                            MainMenu.PrintDeal(customer.GetLastDeal());
                        else
                            PrintNoHangingDealsError();
                        break;
                    case 5:

                        break;
                    case 6:

                        break;
                }
                if (choice >= minPoint && choice < maxPoint)
                {
                    Console.Clear();
                    Program.PrintHeader();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Hello, {customer.GetFullName()}");
                    Console.WriteLine("What do you want to do?");
                    PrintHelp();
                }
            } while (choice != maxPoint);
        }
        public static void PrintNoHangingDealsError()
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Congratulations!");
            Console.WriteLine("Dear customer, you do not have any hanging deals");
            Console.WriteLine("\nPress [ENTER] to go back to customer`s menu");
            Console.ReadLine();
            Console.ResetColor();
        }
        public static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Print detailed info about me");
            Console.WriteLine("2. Estimate thing");
            Console.WriteLine("3. Bail thing");
            Console.WriteLine("4. Print detailed info about my hanging deal");
            Console.WriteLine("5. Redeem thing");
            Console.WriteLine("6. Prolong hanging deal");
            Console.WriteLine("7. Print help");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("8. Quit");
            Console.ResetColor();
        }
    }
}
