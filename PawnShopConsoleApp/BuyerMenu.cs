using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawnShopLib;
using PawnShopLib.Things;

namespace PawnShopConsoleApp
{
    public static class BuyerMenu
    {
        public static void LoginBuyer(PawnShop pawnShop)
        {
            Buyer buyer = RegisterBuyer();
            PrintBuyerMenu(pawnShop, buyer);
        }
        public static Buyer RegisterBuyer()
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello, dear buyer!");
            bool parsed;
            Console.WriteLine("\nEnter how much money do you have:");
            parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out decimal balance);
            while (!parsed || balance < 0)
            {
                Console.WriteLine("You entered the wrong sum");
                Console.WriteLine("Enter how much money do you have once more:");
                parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out balance);
            }
            Buyer newBuyer = new Buyer(balance);
            Console.ResetColor();
            return newBuyer;
        }
        public static void PrintBuyerMenu(PawnShop pawnShop, Buyer buyer)
        {
            int filter = 0;
            int sortBy = 1;
            bool error = false;
            string entered;
            List<Thing> boughtThings = new List<Thing>();
            do
            {
                Console.Clear();
                Program.PrintHeader();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Balance: {Program.CutZeros(buyer.Balance)} hrn");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Things on sale: ");
                Console.WriteLine($"Filter: f{filter}; Sort by s{sortBy}");
                Console.WriteLine("[Filters: f0 - All; f1 - Antique things; f2 - Cars; f3 - Electronic things; f4 - Jewel; f5 - Shares]");
                Console.WriteLine("[Sorting types: s1 - Price ascending; s2 - Price descending]");
                Console.WriteLine("[To buy any item just enter it`s number (ex., 1)]");
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
                Console.WriteLine();
                if (error)
                    Console.WriteLine("Input error. Please, enter the information once more");
                Console.WriteLine("Enter any filter, sorting type or number of thing you want to buy. 0 - quit");
                entered = Console.ReadLine().Trim();
                error = false;
                if (entered.Length > 1 && char.IsDigit(entered[1]) && char.IsLetter(entered[0]))
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
                else if (entered.Length >= 1 && char.IsDigit(entered[0]))
                {
                    int choice = int.Parse(entered);
                    if (choice > 0 && choice <= onSale.Count()) {
                        choice--;
                        Console.WriteLine($"Are you sure you want to buy {onSale[choice].Thing}?[Y/n]");
                        entered = Console.ReadLine().Trim() + " ";
                        while (entered.ToLower()[0] != 'n' && entered.ToLower()[0] != 'y')
                        {
                            Console.WriteLine("Error. Please, enter Y on n once more");
                            Console.WriteLine($"Are you sure you want to buy {onSale[choice].Thing} for {onSale[choice].MarketPrice} hrn?[Y/n]");
                            entered = Console.ReadLine().Trim() + " ";
                        }
                        if (entered.ToLower()[0] == 'y')
                        {
                            Console.WriteLine();
                            try
                            {
                                Thing thing = pawnShop.BuyThing(buyer, onSale[choice].ID);
                                boughtThings.Add(thing);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Congratulations!");
                                Console.WriteLine($"You bought {onSale[choice].Thing} successfully!");
                                Console.WriteLine($"Money left: {Program.CutZeros(buyer.Balance)} hrn");
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                            }
                            catch (ArgumentException exc)
                            {
                                Console.WriteLine("Denied");
                                if (exc.Message.Contains("\n"))
                                    Console.WriteLine(exc.Message.Remove(exc.Message.LastIndexOf('\n')));
                                else
                                    Console.WriteLine(exc.Message.Replace("Buyer", "you"));
                            }
                            Console.WriteLine("\nPress [ENTER] to continue");
                            Console.ReadLine();
                            Console.ResetColor();
                        }
                    }
                }
                else
                {
                    error = true;
                }
            } while (entered != "0");
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("In this session you bought: ");
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < boughtThings.Count(); i++)
                Console.WriteLine($"{i + 1}. {boughtThings[i]}");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"\nMoney returned to you: {Program.CutZeros(buyer.Balance)} hrn");
            Console.WriteLine("\nPress [ENTER] to go back to main menu");
            Console.ReadLine();
            Console.ResetColor();
        }
    }
}
