using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawnShopLib;

namespace PawnShopConsoleApp
{
    public static class MainMenu
    {
        public static void PrintMainMenu(PawnShop pawnShop)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("What do you want to do?");
            PrintHelp();
            bool parsed;
            int choice;
            do
            {
                Console.WriteLine("\nEnter the number 1 - 8: ");
                parsed = int.TryParse(Console.ReadLine(), out choice);      //пытаемся спарсить введенное в int
                if (!parsed || choice < 1 || choice > 8)
                {
                    Console.WriteLine("Error: you entered not a number or number was smaller than or equal 0 or bigger than 8.");
                    choice = 0;
                }
                switch (choice)
                {
                    case 1:
                        break;
                    case 2:
                        PrintAllDeals(pawnShop);
                        break;
                    case 3:
                        PrintCustomers(pawnShop);
                        break;
                    case 4:
                        PrintInfo(pawnShop);
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                }
                if (choice >= 1 && choice < 8)
                {
                    Console.Clear();
                    Program.PrintHeader();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("What do you want to do?");
                    PrintHelp();
                }
            } while (choice != 8);
        }
        public static void PrintAllDeals(PawnShop pawnShop)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Deals: ");
            Console.WriteLine($"{"ID", -10}{"Full name", -30}{"Thing", -55}{"Start time", -11}{"Term(w/o fine)", -15}{"Status", -10}");
            Console.ForegroundColor = ConsoleColor.Green;
            IReadOnlyList<Deal> allDeals = pawnShop.Deals.GetFullList();
            foreach (Deal deal in allDeals)
                Console.WriteLine($"{deal.ID, -10}{deal.Customer.GetFullName(), -30}{deal.Thing, -55}{deal.StartTime.Day + "." + deal.StartTime.Month + "." + deal.StartTime.Year, -11}{deal.Term, -15}{(deal.IsClosed ? "Closed" : "Not closed"), -10}");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nPress [ENTER] to go back to main menu");
            Console.Read();
            Console.ResetColor();
        }
        public static void PrintCustomers(PawnShop pawnShop)
        {
                Console.Clear();
                Program.PrintHeader();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Customers: ");
                Console.WriteLine($"{"ID",-10}{"Full name",-30}{"Successful deals",-17}{"Unsuccessful deals",-19}{"Status",-15}");
                Console.ForegroundColor = ConsoleColor.Green;
                IReadOnlyList<Customer> allCustomers = pawnShop.CustomersList;
                foreach (Customer customer in allCustomers)
                    Console.WriteLine($"{customer.ID,-10}{customer.GetFullName(),-30}{customer.GetSuccessfulDealsQuantity(), -17}{customer.GetUnsuccessfulDealsQuantity(), -19}{(customer.IsOnDeal() ? "On deal" : "Not on deal"), -15}");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\nPress [ENTER] to go back to main menu");
                Console.Read();
                Console.ResetColor();
        }
        public static void PrintInfo(PawnShop pawnShop)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Information: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Revenue: " + Program.CutZeros(pawnShop.Revenue) + " hrn");
            Console.WriteLine("Costs: " + Program.CutZeros(pawnShop.Costs) + " hrn");
            Console.WriteLine("Net profit: " + Program.CutZeros(pawnShop.GetNetProfit()) + " hrn");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nPress [ENTER] to go back to main menu");
            Console.Read();
            Console.ResetColor();
        }
        public static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Print sale list");
            Console.WriteLine("2. Print full deals list");
            Console.WriteLine("3. Print customers list");
            Console.WriteLine("4. Print revenue, costs and net profit");
            Console.WriteLine("5. Switch to customer mode");
            Console.WriteLine("6. Switch to buyer mode");
            Console.WriteLine("7. Print help");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("8. Exit");
            Console.ResetColor();
        }
    }
}
