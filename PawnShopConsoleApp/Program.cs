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
        delegate decimal Evaluator(Thing thing);
        static void Main(string[] args)
        {
            Console.Title = "PAWN SHOP";
            PawnShop p1 = new PawnShop("PS", 10000000m, StandartEvaluators.EvaluateThing, 0.005m, 1.5m);
            Customer c1 = new Customer("Sydor", "Sydorenko", "Sydorovych", new DateTime(2003, 4, 25));
            Customer c2 = new Customer("Ferod", "Fedorov", "Sydorovych", new DateTime(1994, 4, 25));
            Customer c3 = new Customer("Nikolay", "Nikolenko", "Sydorovych", new DateTime(1999, 4, 25));
            Customer c4 = new Customer("Sergey", "Nikolenko", "Sydorovych", new DateTime(1979, 4, 25));
            Buyer b1 = new Buyer(10000000m);
            Thing t1 = new Car(2000000, 2007, 700000, 200000, "Honda");
            Thing t2 = new AntiqueThing(700, 1960, AntiqueTypes.Watches, 200000);
            Thing t3 = new Jewel(2007, 15, 0, 1, 0);
            Thing t4 = new ElectronicThing(300, 2014, ElectronicTypes.Phone);
            Thing t5 = new Car(1700000, 2010, 5000000, 300000, "Porshe");
            Thing t6 = new Shares(2007, 30000, "Amazon");
            Thing t7 = new Jewel(2007, 10, JewelTypes.GoldIngot);
            decimal money = p1.BailThing(c1, t1, 30);
            //money = p1.BailThing(c2, t2, 0);
            //Console.WriteLine(t2);
            //Console.WriteLine(t1);
            //Console.WriteLine(t4);
            //Console.WriteLine(t3);
            //Console.WriteLine(t7);
            //Console.WriteLine(t6);
            //Console.WriteLine(StandartEvaluators.EvaluateThing(t1, Tariffs.Preferential));
            //Deal d1 = new Deal(c1, t1, 30, Tariffs.Standart, StandartEvaluators.EvaluateThing(t1, Tariffs.Standart), 1.5m);
            ////Deal d5 = new Deal(c2, t2, 90, Tariffs.Preferential, StandartEvaluators.EvaluateThing(t1, Tariffs.Preferential));
            //Deal d2 = new Deal(c2, t2, 90, Tariffs.Preferential, StandartEvaluators.EvaluateThing(t1, Tariffs.Preferential));
            //d2.Close(true);
            //Deal d4 = new Deal(c2, t4, 0, Tariffs.Standart, StandartEvaluators.EvaluateThing(t4, Tariffs.Standart));
            //Deal d3 = new Deal(c3, t3, 60, Tariffs.LowPenalty, StandartEvaluators.EvaluateThing(t1, Tariffs.LowPenalty), 1.5m);
            //d3.Close(false);
            //Deal d5 = new Deal(c3, t5, 0, Tariffs.Standart, StandartEvaluators.EvaluateThing(t5, Tariffs.Standart), 1.5m);
            //Deal d6 = new Deal(c4, t5, 0, Tariffs.Standart, StandartEvaluators.EvaluateThing(t5, Tariffs.Standart), 1.5m);
            //DealsBase db1 = new DealsBase();
            //db1.AddDeal(d1);
            //db1.AddDeal(d3);
            //db1.AddDeal(d2);
            //db1.AddDeal(d4);
            //db1.AddDeal(d5);
            //db1.AddDeal(d6);
            //db1.GetFullList(SortingTypes.PriceDescending);
            //db1.GetFullList(SortingTypes.PriceAsceding);
            //db1.GetFullList();
            //db1.GetFilteredOnSale<Car>(SortingTypes.PriceAsceding);
            Console.WriteLine(DateTime.Now);
            Console.ReadKey();
            c1.EarnMoney(1000000);
            p1.RedeemThing(c1);
            //p1.Prolong(c1, 30);
            Console.WriteLine(DateTime.Now);
            p1.BuyThing(b1, "D00000001");
            //p1.Deals.GetFullList();
            Console.WriteLine(p1.GetNetProfit());
            Console.Read();
        }
    }
}
