using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawnShopLib;
using PawnShopLib.Things;

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
            const int minPoint = 1;
            const int maxPoint = 10;
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
                        PrintOnSale(pawnShop);
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
                        PrintDeal(pawnShop);
                        break;
                    case 6:
                        PrintCustomer(pawnShop);
                        break;
                    case 7:
                        CustomerMenu.LoginCustomer(pawnShop);
                        break;
                    case 8:
                        break;
                }
                if (choice >= minPoint && choice < maxPoint)
                {
                    Console.Clear();
                    Program.PrintHeader();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("What do you want to do?");
                    PrintHelp();
                }
            } while (choice != maxPoint);
        }
        public static void PrintOnSale(PawnShop pawnShop)
        {
            int filter = 0;
            int sortBy = 1;
            string entered;
            do
            {
                Console.Clear();
                Program.PrintHeader();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Things on sale: ");
                Console.WriteLine($"Filter: f{filter}; Sort by s{sortBy}");
                Console.WriteLine("[Filters: f0 - All; f1 - Antique things; f2 - Cars; f3 - Electronic things; f4 - Jewel; f5 - Shares]");
                Console.WriteLine("[Sorting types: s1 - Price ascending; s2 - Price descending]");
                Console.WriteLine("[0 - quit]");
                Console.WriteLine($"{"Number",-8}{"Price",-15}{"Thing"}");
                Console.ForegroundColor = ConsoleColor.Green;
                IReadOnlyList<Deal> onSale;
                DealSortingTypes type;
                if (sortBy == 2)
                    type = DealSortingTypes.PriceDescending;
                else
                    type = DealSortingTypes.PriceAsceding;
                switch (filter)
                {
                    case 1:
                        onSale = pawnShop.Deals.GetFilteredOnSale<AntiqueThing>(type);
                        break;
                    case 2:
                        onSale = pawnShop.Deals.GetFilteredOnSale<Car>(type);
                        break;
                    case 3:
                        onSale = pawnShop.Deals.GetFilteredOnSale<ElectronicThing>(type);
                        break;
                    case 4:
                        onSale = pawnShop.Deals.GetFilteredOnSale<Jewel>(type);
                        break;
                    case 5:
                        onSale = pawnShop.Deals.GetFilteredOnSale<Shares>(type);
                        break;
                    default:
                        onSale = pawnShop.Deals.GetFilteredOnSale<Thing>(type);
                        break;
                }
                for (int i = 0; i < onSale.Count(); i++)
                {
                    Console.WriteLine($"{i + 1,-8}{Program.CutZeros(onSale[i].MarketPrice),-15}{onSale[i].Thing}");
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\nEnter any filter or sorting type. 0 - quit");
                entered = Console.ReadLine().Trim();
                if (entered.Length > 1 && char.IsDigit(entered[1]))
                {
                    if (entered[0] == 's' && entered[1] >= '1' && entered[1] <= '2')
                    {
                        sortBy = entered[1] - '0';
                    }
                    else if (entered[0] == 'f' && entered[1] >= '0' && entered[1] <= '5')
                    {
                        filter = entered[1] - '0';
                    }
                }
            } while (entered != "0");
            Console.ResetColor();
        }
        public static void PrintAllDeals(PawnShop pawnShop)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Deals: ");
            Console.WriteLine($"{"ID", -10}{"Full name", -30}{"Thing", -60}{"Start time", -11}{"Term(w/o fine)", -15}{"Status", -10}");
            Console.ForegroundColor = ConsoleColor.Green;
            IReadOnlyList<Deal> allDeals = pawnShop.Deals.GetFullList();
            foreach (Deal deal in allDeals)
                Console.WriteLine($"{deal.ID, -10}{deal.Customer.GetFullName(), -30}{deal.Thing, -60}{deal.StartTime.Day + "." + deal.StartTime.Month + "." + deal.StartTime.Year, -11}{deal.Term, -15}{(deal.IsClosed ? "Closed" : "Not closed"), -10}");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nPress [ENTER] to go back to main menu");
            Console.ReadLine();
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
                Console.ReadLine();
                Console.ResetColor();
        }
        public static void PrintInfo(PawnShop pawnShop)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Information about pawnshop \"{pawnShop.Name}\": ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Revenue: " + Program.CutZeros(pawnShop.Revenue) + " hrn");
            Console.WriteLine("Costs: " + Program.CutZeros(pawnShop.Costs) + " hrn");
            Console.WriteLine("Net profit: " + Program.CutZeros(pawnShop.GetNetProfit()) + " hrn");
            Console.WriteLine("Balance: " + Program.CutZeros(pawnShop.Balance) + " hrn");
            Console.WriteLine();
            Console.WriteLine($"Max term: {pawnShop.MaxTerm} days");
            Console.WriteLine($"Coefficient per day: {Program.CutZeros(pawnShop.PerDayCoefficient * 100m)}%");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nPress [ENTER] to go back to main menu");
            Console.ReadLine();
            Console.ResetColor();
        }
        public static void PrintDeal(Deal deal)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Information about the deal: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"ID: {deal.ID}");
            Console.WriteLine($"Name: {deal.Customer.GetFullName()}");
            Console.WriteLine($"Thing: {deal.Thing}");
            Console.WriteLine($"Start time: {deal.StartTime.Day + "." + deal.StartTime.Month + "." + deal.StartTime.Year}");
            Console.WriteLine($"Term: {deal.Term}");
            Console.WriteLine($"Last non-penalty: {deal.GetLastNonPenaltyDate().Day + "." + deal.GetLastNonPenaltyDate().Month + "." + deal.GetLastNonPenaltyDate().Year}");
            Console.WriteLine($"Penalty term (max): {deal.PenaltyMaxTerm}");
            Console.WriteLine($"Last with penalty: {deal.GetLastDate().Day + "." + deal.GetLastDate().Month + "." + deal.GetLastDate().Year}");
            Console.WriteLine($"Status: {(deal.IsClosed ? "Closed" : "Not closed")}");
            if (deal.IsClosed)
            {
                Console.WriteLine($"Was successful? {(deal.IsSuccessful ? "Yes" : "No")}");
                Console.WriteLine($"Is on sale? {(deal.IsOnSale ? "Yes" : "No")}");
            }
            else
            {
                Console.WriteLine($"Days left (w/o penalty):  {deal.GetLastNonPenaltyTerm()}");
                Console.WriteLine($"Days left (with penalty): {deal.GetLastTerm()}");
            }
            Console.WriteLine($"Tariff: {deal.Tariff}");
            Console.WriteLine($"Initial price: {Program.CutZeros(deal.Price)}");
            Console.WriteLine($"Redemption price: {Program.CutZeros(deal.RedemptionPrice)}");
            if (deal.Penalty > 0)
                Console.WriteLine($"Penalty: {Program.CutZeros(deal.Penalty)}");
            if (deal.IsOnSale)
                Console.WriteLine($"Market price: {Program.CutZeros(deal.MarketPrice)}");
            if (deal.IsClosed)
                Console.WriteLine($"Pawn shop profit: {Program.CutZeros(deal.PawnShopProfit)}");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nPress [ENTER] to go back to menu");
            Console.ReadLine();
            Console.ResetColor();
        }
        public static void PrintDeal(PawnShop pawnShop)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            string id;
            Deal deal;
            Console.WriteLine("Enter the id of the deal (ex. D00000001):");
            id = Console.ReadLine();
            deal = pawnShop.Deals[id];
            while (deal == null && id.Trim() != "0")
            {
                Console.WriteLine("Not found.");
                Console.WriteLine("Enter the id of the deal once more (ex. D00000001):");
                Console.WriteLine("Enter 0 to quit");
                id = Console.ReadLine();
                deal = pawnShop.Deals[id];
            };
            if (id.Trim() != "0")
            {
                PrintDeal(deal);
            }
            else
            {
                Console.ResetColor();
            }
        }
        public static void PrintCustomer(Customer customer)
        {
                Console.Clear();
                Program.PrintHeader();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Information about the customer: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"ID: {customer.ID}");
                Console.WriteLine($"Name: {customer.GetFullName()}");
                Console.WriteLine($"Birthday: {customer.BirthDay.Day + "." + customer.BirthDay.Month + "." + customer.BirthDay.Year}");
                Console.WriteLine($"Successful deals: {customer.GetSuccessfulDealsQuantity()}");
                Console.WriteLine($"Unsuccessful deals: {customer.GetUnsuccessfulDealsQuantity()}");
                Console.WriteLine($"Status: {(customer.IsOnDeal() ? "On deal" : "Not on deal")}");
                Console.WriteLine("Balance: " + Program.CutZeros(customer.Balance) + " hrn");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\nCustomer`s deals: ");
                Console.WriteLine($"{"ID",-10}{"Thing",-55}{"Start time",-11}{"Term(w/o fine)",-15}{"Status",-10}");
                Console.ForegroundColor = ConsoleColor.Green;
                IReadOnlyList<Deal> allDeals = customer.Deals.GetFullList();
                foreach (Deal deal in allDeals)
                    Console.WriteLine($"{deal.ID,-10}{deal.Thing,-55}{deal.StartTime.Day + "." + deal.StartTime.Month + "." + deal.StartTime.Year,-11}{deal.Term,-15}{(deal.IsClosed ? "Closed" : "Not closed"),-10}");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\nPress [ENTER] to go back to menu");
                Console.ReadLine();
                Console.ResetColor();
        }
        public static void PrintCustomer(PawnShop pawnShop)
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            string id;
            Customer customer;
            Console.WriteLine("Enter the id of the customer (ex. C00000001):");
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
            if (id.Trim() != "0")
            {
                PrintCustomer(customer);
            }
            else
            {
                Console.ResetColor();
            }
        }
        public static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Print sale list");
            Console.WriteLine("2. Print full deals list");
            Console.WriteLine("3. Print customers list");
            Console.WriteLine("4. Print revenue, costs and net profit");
            Console.WriteLine("5. Print detailed info about a deal");
            Console.WriteLine("6. Print detailed info about a customer");
            Console.WriteLine("7. Switch to customer mode");
            Console.WriteLine("8. Switch to buyer mode");
            Console.WriteLine("9. Print help");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("10. Quit");
            Console.ResetColor();
        }
    }
}
