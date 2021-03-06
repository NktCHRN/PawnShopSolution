using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PawnShopLib;

namespace PawnShopConsoleApp
{
    static class Program
    {
        public static short WindowWidth { get; private set; }
        public static short WindowHeight { get; private set; }
        static void Main(string[] args)
        {
            Console.Title = "PAWN SHOP";
            const string fileName = "pawnShopCurrentState.dat";
            BinaryFormatter formatter = new BinaryFormatter();
            PawnShop pawnShop = null;
            WindowHeight = 35;
            WindowWidth = 140;
            SetSize();
            PrintHeader();
            Console.WriteLine("Program by Nikita Chernikov");
            Console.WriteLine("Group IS-02, FICT, KPI");
            bool stop = false;
            string entered;
            if (File.Exists(fileName))
            {
                Console.WriteLine("Do you want to recover the last session?[Y/n]");
                entered = Console.ReadLine().Trim() + " ";
                while (entered.ToLower()[0] != 'n' && entered.ToLower()[0] != 'y')
                {
                    Console.WriteLine("Error. Please, enter Y on n once more");
                    Console.WriteLine("Do you want to recover the last session?[Y/n]");
                    entered = Console.ReadLine().Trim() + " ";
                }
                if (entered.ToLower()[0] == 'y')
                {
                    try
                    {
                        using (FileStream fs = new FileStream(fileName, FileMode.Open))
                        {
                            pawnShop = (PawnShop)formatter.Deserialize(fs);
                        }
                        string maxCustomerID = "C00000000";
                        string maxDealID = "D00000000";
                        foreach (Customer customer in pawnShop.CustomersList)
                        {
                            if (String.Compare(customer.ID, maxCustomerID) == 1)
                                maxCustomerID = customer.ID;
                        }
                        foreach (Deal deal in pawnShop.Deals)
                        {
                            if (String.Compare(deal.ID, maxDealID) == 1)
                                maxDealID = deal.ID;
                        }
                        Customer.CustomersQuantity = int.Parse(maxCustomerID.Remove(0, 1));
                        Deal.DealsCount = int.Parse(maxDealID.Remove(0, 1));
                    }
                    catch (System.Runtime.Serialization.SerializationException)
                    {
                        Console.WriteLine("Damaged file. Unable to parse");
                        Console.WriteLine("Do you want to start a new pawnshop[Y/n]");
                        entered = Console.ReadLine().Trim() + " ";
                        while (entered.ToLower()[0] != 'n' && entered.ToLower()[0] != 'y')
                        {
                            Console.WriteLine("Error. Please, enter Y on n once more");
                            Console.WriteLine("Do you want to start a new pawnshop[Y/n]");
                            entered = Console.ReadLine().Trim() + " ";
                        }
                        if (entered.ToLower()[0] == 'n')
                            stop = true;
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        Console.WriteLine("Damaged file. Unable to parse");
                        Console.WriteLine("Do you want to start a new pawnshop[Y/n]");
                        entered = Console.ReadLine().Trim() + " ";
                        while (entered.ToLower()[0] != 'n' && entered.ToLower()[0] != 'y')
                        {
                            Console.WriteLine("Error. Please, enter Y on n once more");
                            Console.WriteLine("Do you want to start a new pawnshop[Y/n]");
                            entered = Console.ReadLine().Trim() + " ";
                        }
                        if (entered.ToLower()[0] == 'n')
                            stop = true;
                    }
                }
            }
            if (!stop)
            {
                bool parsed;
                if (pawnShop == null)
                {
                    Evaluator _evaluator = StandardEvaluators.EvaluateThing;
                    Console.WriteLine("\nEnter the name of your pawn shop: ");
                    string name = Console.ReadLine();
                    Console.WriteLine("Enter the balance of your pawn shop: ");
                    parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out decimal initialBalance);
                    while (!parsed || initialBalance <= 0)
                    {
                        Console.WriteLine("Balance can`t be lower than or equal 0.");
                        Console.WriteLine("Enter the balance once more: ");
                        parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out initialBalance);
                    };
                    Administrator admin = null;
                    bool reenter;
                    string password;
                    Console.WriteLine("Enter your password");
                    do
                    {
                        password = Console.ReadLine();
                        reenter = false;
                        try
                        {
                            admin = new Administrator(password);
                        }
                        catch (ArgumentException exc)
                        {
                            Console.WriteLine(exc.Message);
                            Console.WriteLine("Enter your password once more");
                            reenter = true;
                        }
                    } while (reenter);
                    decimal perDayCoefficient = 0.005m;
                    int minTerm = 5;
                    int maxTerm = 365;
                    do
                    {
                        Console.WriteLine("Do you want to change coefficient per day (now: " + CutZeros(perDayCoefficient * 100m) + $"%), min term (now: {minTerm}) and max term (now: {maxTerm} days)? [Y/n]");
                        entered = Console.ReadLine().Trim() + " ";
                    } while (entered.ToLower()[0] != 'n' && entered.ToLower()[0] != 'y');
                    if (entered.ToLower()[0] == 'y')
                    {
                        Console.WriteLine("Enter the coefficient per day (in %): ");
                        parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out perDayCoefficient);
                        while (!parsed || perDayCoefficient <= 0)
                        {
                            Console.WriteLine("Coefficient per day can`t be lower than or equal 0.");
                            Console.WriteLine("Enter the coefficient per day (in %) once more: ");
                            parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out perDayCoefficient);
                        };
                        perDayCoefficient /= 100;
                        Console.WriteLine("Enter the min term (days): ");
                        parsed = int.TryParse(Console.ReadLine().Replace('.', ','), out minTerm);
                        while (!parsed || minTerm < 5)
                        {
                            Console.WriteLine("Min term can`t be lower than 5.");
                            Console.WriteLine("Enter the min term once more: ");
                            parsed = int.TryParse(Console.ReadLine().Replace('.', ','), out minTerm);
                        };
                        Console.WriteLine("Enter the max term (days): ");
                        parsed = int.TryParse(Console.ReadLine().Replace('.', ','), out maxTerm);
                        while (!parsed || maxTerm < minTerm)
                        {
                            Console.WriteLine("Max term can`t be lower than 5.");
                            Console.WriteLine("Enter the max term once more: ");
                            parsed = int.TryParse(Console.ReadLine().Replace('.', ','), out maxTerm);
                        };
                    }
                    pawnShop = new PawnShop(name, initialBalance, admin, perDayCoefficient, minTerm, maxTerm, _evaluator);
                }
                Console.Clear();
                Program.PrintHeader();
                PrintHelp();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                short choice;
                const short minPoint = 1;
                const short maxPoint = 4;
                do
                {
                    Console.WriteLine($"\nEnter the number {minPoint} - {maxPoint}: ");
                    parsed = short.TryParse(Console.ReadLine(), out choice);
                    if (!parsed || choice < minPoint || choice > maxPoint)
                    {
                        Console.WriteLine($"Error: you entered not a number or number was smaller than {minPoint} or bigger than {maxPoint}.");
                        Console.WriteLine($"Quit - {maxPoint}");
                        choice = minPoint - 1;
                    }
                    switch (choice)
                    {
                        case 1:
                            AdminMenu.LoginAdmin(pawnShop);
                            break;
                        case 2:
                            CustomerMenu.LoginCustomer(pawnShop);
                            break;
                        case 3:
                            BuyerMenu.LoginBuyer(pawnShop);
                            break;
                    }
                    if (choice >= minPoint && choice < maxPoint)
                    {
                        Console.Clear();
                        PrintHeader();
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        PrintHelp();
                    }
                } while (choice != maxPoint);
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, pawnShop);
                }
            }
            Console.Clear();
        }
        public static void SetSize()
        {
            const short maxBufferHeight = short.MaxValue - 1;
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize(WindowWidth, (Console.BufferHeight > WindowHeight) ? Console.BufferHeight : maxBufferHeight);
            Console.SetWindowSize(WindowWidth, WindowHeight);
        }
        public static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            const string name = "PAWN SHOP";
            Console.WriteLine('\n' + name.PadLeft(WindowWidth / 2 + name.Length / 2) + '\n');
            Console.ResetColor();
        }
        public static string CutZeros(decimal number)
        {
            string cut = $"{number:F3}";
            if (cut.Contains(","))
            {
                while (cut[cut.Length - 1] != ',' && cut[cut.Length - 1] == '0')
                    cut = cut.Remove(cut.Length - 1);
                if (cut[cut.Length - 1] == ',')
                    cut = cut.Remove(cut.Length - 1);
            }
            return cut;
        }
        public static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Login as");
            Console.WriteLine("1. Administrator");
            Console.WriteLine("2. Customer");
            Console.WriteLine("3. Buyer");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("4. Quit");
            Console.ResetColor();
        }
    }
}
