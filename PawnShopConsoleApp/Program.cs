using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawnShopLib;
using PawnShopLib.Things;

namespace PawnShopConsoleApp
{
    class Program
    {
        public static int WindowWidth { get; private set; }
        public static int WindowHeight {get; private set;}
        static void Main(string[] args)
        {
            Console.Title = "PAWN SHOP";
            WindowHeight = 30;
            WindowWidth = 120;
            SetSize();
            PrintHeader();
            PawnShop.Evaluator _evaluator = StandartEvaluators.EvaluateThing;
            Console.WriteLine("Enter the name of your pawn shop: ");
            string name = Console.ReadLine();
            bool parsed;
            decimal initialBalance;
            Console.WriteLine("Enter the balance of your pawn shop: ");
            parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out initialBalance);
            while (!parsed || initialBalance <= 0)
            {
                Console.WriteLine("Balance can`t be lower than or equal 0.");
                Console.WriteLine("Enter the balance once more: ");
                parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out initialBalance);
            };
            decimal perDayCoefficient = 0.005m;
            int maxTerm = 365;
            string entered;
            do
            {
                Console.WriteLine("Do you to change coefficient per day (now: {perDayCoefficient}) and max term (now : {maxTerm})? [Y/n]");
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
                Console.WriteLine("Enter the max term: ");
                parsed = int.TryParse(Console.ReadLine().Replace('.', ','), out maxTerm);
                while (!parsed || maxTerm <= 0)
                {
                    Console.WriteLine("Max term can`t be lower than or equal 0.");
                    Console.WriteLine("Enter the max term once more: ");
                    parsed = int.TryParse(Console.ReadLine().Replace('.', ','), out maxTerm);
                };
            }
            PawnShop p1 = new PawnShop(name, initialBalance, _evaluator, perDayCoefficient, maxTerm);
            Console.Read();//удалить!!!
        }
        public static void SetSize()
        {
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize(WindowWidth, WindowHeight);
            Console.SetWindowSize(WindowWidth, WindowHeight);
        }
        public static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            const string name = "PAWN SHOP";
            Console.WriteLine('\n' + name.PadLeft(WindowWidth / 2 + name.Length / 2) + '\n');
            Console.ResetColor();
        }
    }
}
