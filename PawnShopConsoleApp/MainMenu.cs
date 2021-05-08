using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnShopConsoleApp
{
    public static class MainMenu
    {
        public static void PrintMainMenu()
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("What do you want to do?");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. ");
        }
    }
}
