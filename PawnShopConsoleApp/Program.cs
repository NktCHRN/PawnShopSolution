using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawnShopLib;

namespace PawnShopConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PAWN SHOP";
            Customer c1 = new Customer("", "", "", new DateTime(2003, 4, 25));
            Console.Read();
        }
    }
}
