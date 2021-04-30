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
        static void Main(string[] args)
        {
            Console.Title = "PAWN SHOP";
            Customer c1 = new Customer("", "", "", new DateTime(2003, 4, 25));
            int i = 1;
            Console.WriteLine(nameof(i));
            Thing t1 = new Jewel(777, JewelTypes.ComplicatedJewel, 777, 0, 23, 30);
            Console.WriteLine(StandartEvaluators.EvaluateThing(t1));
            Console.Read();
        }
    }
}
