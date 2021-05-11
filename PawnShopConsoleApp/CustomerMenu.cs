using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawnShopLib;
using PawnShopLib.Things;

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
                catch (OverflowException exc)
                {
                    Console.WriteLine("Denied due to technical problems");
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
                parsed = int.TryParse(Console.ReadLine(), out choice);
                if (!parsed || choice < minPoint || choice > maxPoint)
                {
                    Console.WriteLine($"Error: you entered not a number or number was smaller than {minPoint} or bigger than {maxPoint}.");
                    Console.WriteLine($"Help - {maxPoint - 1}");
                    choice = minPoint - 1;
                }
                switch (choice)
                {
                    case 1:
                        MainMenu.PrintCustomer(pawnShop, customer);
                        break;
                    case 2:
                        Thing thing = EnterThing();
                        EstimateThing(pawnShop, customer, thing);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("\nPress [ENTER] to go back to customer`s menu");
                        Console.ReadLine();
                        Console.ResetColor();
                        break;
                    case 3:
                        if (!customer.IsOnDeal())
                        {
                            thing = EnterThing();
                            decimal price = EstimateThing(pawnShop, customer, thing);
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            if (price != 0)
                            {
                                string entered;
                                Console.WriteLine("Do you really want to bail this thing?[Y/n]");
                                entered = Console.ReadLine().Trim() + " ";
                                while (entered.ToLower()[0] != 'n' && entered.ToLower()[0] != 'y')
                                {
                                    Console.WriteLine("Error. Please, enter Y on n once more");
                                    Console.WriteLine("Do you really want to bail this thing?[Y/n]");
                                    entered = Console.ReadLine().Trim() + " ";
                                }
                                if (entered.ToLower()[0] == 'y')
                                {
                                    price = BailThing(pawnShop, customer, thing);
                                    if (price != 0)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("\nCongratulations!");
                                        Console.WriteLine($"Dear customer, you got {Program.CutZeros(price)} hrn from this deal");
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    }
                                    Console.WriteLine("\nPress [ENTER] to go back to customer`s menu");
                                    Console.ReadLine();
                                    Console.ResetColor();
                                }
                            }
                            else
                            {
                                if (thing != null)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.WriteLine("\nDenied.");
                                    Console.WriteLine("Unfortunately, we cannot buy your thing because it is too cheap now or error occured");
                                    Console.WriteLine("\nPress [ENTER] to go back to customer`s menu");
                                    Console.ReadLine();
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.WriteLine("\nPress [ENTER] to go back to customer`s menu");
                                    Console.ReadLine();
                                    Console.ResetColor();
                                }
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Program.PrintHeader();
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("Unfortunately, you already have a hanging deal");
                            Console.WriteLine("\nPress [ENTER] to go back to customer`s menu");
                            Console.ReadLine();
                            Console.ResetColor();
                        }
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
        public static decimal EstimateThing(PawnShop pawnShop, Customer customer, Thing thing)
        {
            decimal price;
            if (thing != null)
            {
                price = pawnShop.EstimateThing(customer, thing);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nYour tariff: {pawnShop.DefineTariff(customer)}");
                if (pawnShop.DefineTariff(customer) == Tariff.Standard)
                    Console.WriteLine($"To get a {Tariff.Preferential} tariff, you should have at least 6 deals and your successful/unsuccessful coefficient should be at least 1.5 or you should have 0 unsucessful deals");
                else if (pawnShop.DefineTariff(customer) == Tariff.LowPenalty)
                    Console.WriteLine($"To get a {Tariff.Standard} tariff, your successful/unsuccessful coefficient should be bigger than 0.5");
                Console.WriteLine($"\nYou thing was estimated as {Program.CutZeros(price)} hrn");
            }
            else
            {
                price = 0;
            }
            Console.ResetColor();
            return price;
        }
        public static Thing EnterThing()
        {
            Console.Clear();
            Program.PrintHeader();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            bool parsed;
            int choice;
            int minPoint = 0;
            int maxPoint = 5;
            Thing thing = null;
            Console.WriteLine("Choose the type of thing: ");
            Console.WriteLine("[0 - go back to customer`s menu]");
            Console.WriteLine("1. Antique thing");
            Console.WriteLine("2. Car");
            Console.WriteLine("3. Electronic thing");
            Console.WriteLine("4. Jewel");
            Console.WriteLine("5. Shares");
            Console.WriteLine($"Enter {minPoint} - {maxPoint}: ");
            parsed = int.TryParse(Console.ReadLine(), out choice);
            while (!parsed || choice < minPoint || choice > maxPoint)
            {
                Console.WriteLine("You entered the wrong number");
                Console.WriteLine($"Enter {minPoint} - {maxPoint} once more [0, if you want to go back to customer`s menu]: ");
                parsed = int.TryParse(Console.ReadLine(), out choice);
            }
            if (choice != 0)
            {
                int year;
                if (choice != 1)
                {
                    Console.WriteLine("\nEnter the year you bought it: ");
                    parsed = int.TryParse(Console.ReadLine(), out year);
                    while (!parsed || year > DateTime.Now.Year || year < 0)
                    {
                        Console.WriteLine("You entered the wrong year");
                        Console.WriteLine("Enter the year you bought thing once more: ");
                        parsed = int.TryParse(Console.ReadLine(), out year);
                    }
                }
                else
                {
                    Console.WriteLine("\nEnter the year of the antique thing: ");
                    parsed = int.TryParse(Console.ReadLine(), out year);
                    while (!parsed || year > DateTime.Now.Year)
                    {
                        Console.WriteLine("You entered the wrong year");
                        Console.WriteLine("Enter the year year of the antique thing once more: ");
                        parsed = int.TryParse(Console.ReadLine(), out year);
                    }
                }
                switch (choice)
                {
                    case 1:
                        double weight;
                        Console.WriteLine("\nEnter the weight:");
                        parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out weight);
                        while (!parsed || weight <= 0)
                        {
                            Console.WriteLine("You entered the wrong weight");
                            Console.WriteLine("Enter the weight once more:");
                            parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out weight);
                        }
                        AntiqueTypes antiqueType;
                        int typeChoice;
                        minPoint = 1;
                        maxPoint = 5;
                        Console.WriteLine("\nChoose the type of antique thing: ");
                        Console.WriteLine("1. Antique jewel");
                        Console.WriteLine("2. Icon");
                        Console.WriteLine("3. Medal");
                        Console.WriteLine("4. Painting");
                        Console.WriteLine("5. Watches");
                        Console.WriteLine($"Enter {minPoint} - {maxPoint}: ");
                        parsed = int.TryParse(Console.ReadLine(), out typeChoice);
                        while (!parsed || typeChoice < minPoint || typeChoice > maxPoint)
                        {
                            Console.WriteLine("You entered the wrong number");
                            Console.WriteLine($"Enter {minPoint} - {maxPoint} once more: ");
                            parsed = int.TryParse(Console.ReadLine(), out typeChoice);
                        }
                        switch (typeChoice)
                        {
                            case 1:
                                antiqueType = AntiqueTypes.AntiqueJewel;
                                break;
                            case 2:
                                antiqueType = AntiqueTypes.Icon;
                                break;
                            case 3:
                                antiqueType = AntiqueTypes.Medal;
                                break;
                            case 4:
                                antiqueType = AntiqueTypes.Painting;
                                break;
                            default:
                                antiqueType = AntiqueTypes.Watches;
                                break;
                        }
                        decimal estimatedPrice;
                        Console.WriteLine("\nAntique things should be estimated by an expert beforehand");
                        Console.WriteLine("Enter the expert estimated price:");
                        parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out estimatedPrice);
                        while (!parsed || estimatedPrice <= 0)
                        {
                            Console.WriteLine("You entered the wrong expert estimated price");
                            Console.WriteLine("Enter the expert estimated price once more:");
                            parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out estimatedPrice);
                        }
                        try
                        {
                            thing = new AntiqueThing(year, weight, antiqueType, estimatedPrice);
                        }
                        catch (TooYoungException exc)
                        {
                            Console.WriteLine("\nFailed to estimate the thing because it is not an antique one");
                            Console.WriteLine(exc.Message);
                        }
                        break;
                    case 2:
                        Console.WriteLine("\nEnter the weight:");
                        parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out weight);
                        while (!parsed || weight <= 0)
                        {
                            Console.WriteLine("You entered the wrong weight");
                            Console.WriteLine("Enter the weight once more:");
                            parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out weight);
                        }
                        decimal marketPrice;
                        Console.WriteLine("\nEnter the price you bought the car:");
                        parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out marketPrice);
                        while (!parsed || marketPrice < 0)
                        {
                            Console.WriteLine("You entered the wrong price");
                            Console.WriteLine("Enter the price you bought the car once more:");
                            parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out marketPrice);
                        }
                        int mileage;
                        Console.WriteLine("\nEnter the mileage: ");
                        parsed = int.TryParse(Console.ReadLine(), out mileage);
                        while (!parsed || mileage < 0)
                        {
                            Console.WriteLine("You entered the wrong mileage");
                            Console.WriteLine("Enter the mileage once more: ");
                            parsed = int.TryParse(Console.ReadLine(), out mileage);
                        }
                        string brandName;
                        Console.WriteLine("Enter the brand name: ");
                        brandName = Console.ReadLine();
                        while (string.IsNullOrWhiteSpace(brandName))
                        {
                            Console.WriteLine("Brand name can`t be empty");
                            Console.WriteLine("Enter the brand name once more: ");
                            brandName = Console.ReadLine();
                        }
                        thing = new Car(year, weight, marketPrice, mileage, brandName);
                        break;
                    case 3:
                        Console.WriteLine("\nEnter the weight:");
                        parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out weight);
                        while (!parsed || weight <= 0)
                        {
                            Console.WriteLine("You entered the wrong weight");
                            Console.WriteLine("Enter the weight once more:");
                            parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out weight);
                        }
                        minPoint = 1;
                        maxPoint = 5;
                        Console.WriteLine("\nChoose the type of electronic thing: ");
                        Console.WriteLine("1. Camera");
                        Console.WriteLine("2. Computer");
                        Console.WriteLine("3. Fridge");
                        Console.WriteLine("4. Phone");
                        Console.WriteLine("5. Washer");
                        Console.WriteLine($"Enter {minPoint} - {maxPoint}: ");
                        parsed = int.TryParse(Console.ReadLine(), out typeChoice);
                        while (!parsed || typeChoice < minPoint || typeChoice > maxPoint)
                        {
                            Console.WriteLine("You entered the wrong number");
                            Console.WriteLine($"Enter {minPoint} - {maxPoint} once more: ");
                            parsed = int.TryParse(Console.ReadLine(), out typeChoice);
                        }
                        ElectronicTypes electronicType;
                        switch (typeChoice)
                        {
                            case 1:
                                electronicType = ElectronicTypes.Camera;
                                break;
                            case 2:
                                electronicType = ElectronicTypes.Computer;
                                break;
                            case 3:
                                electronicType = ElectronicTypes.Fridge;
                                break;
                            case 4:
                                electronicType = ElectronicTypes.Phone;
                                break;
                            default:
                                electronicType = ElectronicTypes.Washer;
                                break;
                        }
                        thing = new ElectronicThing(year, weight, electronicType);
                        break;
                    case 4:
                        minPoint = 1;
                        maxPoint = 5;
                        Console.WriteLine("\nChoose the type of jewel: ");
                        Console.WriteLine("1. Complicated jewel");
                        Console.WriteLine("2. Gold ingot");
                        Console.WriteLine("3. Silver ingot");
                        Console.WriteLine("4. Diamond");
                        Console.WriteLine("5. Another precious gem");
                        Console.WriteLine($"Enter {minPoint} - {maxPoint}: ");
                        parsed = int.TryParse(Console.ReadLine(), out typeChoice);
                        while (!parsed || typeChoice < minPoint || typeChoice > maxPoint)
                        {
                            Console.WriteLine("You entered the wrong number");
                            Console.WriteLine($"Enter {minPoint} - {maxPoint} once more: ");
                            parsed = int.TryParse(Console.ReadLine(), out typeChoice);
                        }
                        JewelTypes jewelType;
                        switch (typeChoice)
                        {
                            case 1:
                                jewelType = JewelTypes.ComplicatedJewel;
                                break;
                            case 2:
                                jewelType = JewelTypes.GoldIngot;
                                break;
                            case 3:
                                jewelType = JewelTypes.SilverIngot;
                                break;
                            case 4:
                                jewelType = JewelTypes.Diamond;
                                break;
                            default:
                                jewelType = JewelTypes.AnotherGem;
                                break;
                        }
                        if (jewelType == JewelTypes.ComplicatedJewel)
                        {
                            double goldWeight, silverWeight, diamondWeight, otherGemsWeight;
                            Console.WriteLine("\nEnter the gold weight:");
                            parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out goldWeight);
                            while (!parsed || goldWeight < 0)
                            {
                                Console.WriteLine("You entered the wrong weight");
                                Console.WriteLine("Enter the gold weight once more:");
                                parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out goldWeight);
                            }
                            Console.WriteLine("\nEnter the silver weight:");
                            parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out silverWeight);
                            while (!parsed || silverWeight < 0)
                            {
                                Console.WriteLine("You entered the wrong weight");
                                Console.WriteLine("Enter the silver weight once more:");
                                parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out silverWeight);
                            }
                            Console.WriteLine("\nEnter the diamond weight:");
                            parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out diamondWeight);
                            while (!parsed || diamondWeight < 0)
                            {
                                Console.WriteLine("You entered the wrong weight");
                                Console.WriteLine("Enter the diamond weight once more:");
                                parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out diamondWeight);
                            }
                            Console.WriteLine("\nEnter the other gems weight:");
                            parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out otherGemsWeight);
                            while (!parsed || otherGemsWeight < 0)
                            {
                                Console.WriteLine("You entered the wrong weight");
                                Console.WriteLine("Enter the other gems weight once more:");
                                parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out otherGemsWeight);
                            }
                            try
                            {
                                thing = new Jewel(year, goldWeight, silverWeight, diamondWeight, otherGemsWeight);
                            }
                            catch (ArgumentOutOfRangeException exc)
                            {
                                Console.WriteLine("\nFailed to estimate the thing");
                                Console.WriteLine("The whole " + exc.Message.ToLower().Remove(exc.Message.LastIndexOf('\n')));//TEST!!!
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nEnter the weight:");
                            parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out weight);
                            while (!parsed || weight <= 0)
                            {
                                Console.WriteLine("You entered the wrong weight");
                                Console.WriteLine("Enter the weight once more:");
                                parsed = double.TryParse(Console.ReadLine().Replace('.', ','), out weight);
                            }
                            thing = new Jewel(year, weight, jewelType);
                        }
                        break;
                    case 5:
                        Console.WriteLine("\nEnter the market price of the shares:");
                        parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out marketPrice);
                        while (!parsed || marketPrice < 0)
                        {
                            Console.WriteLine("You entered the wrong price");
                            Console.WriteLine("Enter the market price of the shares once more:");
                            parsed = decimal.TryParse(Console.ReadLine().Replace('.', ','), out marketPrice);
                        }
                        string companyName;
                        Console.WriteLine("Enter the company name: ");
                        companyName = Console.ReadLine();
                        while (string.IsNullOrWhiteSpace(companyName))
                        {
                            Console.WriteLine("Company name can`t be empty");
                            Console.WriteLine("Enter the company name once more: ");
                            companyName = Console.ReadLine();
                        }
                        thing = new Shares(year, marketPrice, companyName);
                        break;
                }
                if (thing != null)
                    Console.WriteLine($"\nYour thing: {thing}");
            }
            return thing;
        }
        public static decimal BailThing(PawnShop pawnShop, Customer customer, Thing thing)
        {
            const int minTerm = 5;
            int maxTerm = pawnShop.MaxTerm;
            int term;
            bool parsed;
            bool reenter;
            int choice;
            bool isImmediateSaleAble = thing is Car || thing is ElectronicThing || thing is Jewel;
            do
            {
                reenter = false;
                Console.WriteLine("\nChoose the term for your deal: ");
                Console.WriteLine($"The minimal term is {minTerm}");
                Console.WriteLine($"The maximal term is {maxTerm}");
                if (isImmediateSaleAble)
                    Console.WriteLine("For your thing, the immediate sale is able. Just enter 0 for immediate sale");
                Console.WriteLine("Enter the term:");
                parsed = int.TryParse(Console.ReadLine(), out term);
                while (!parsed || (term < minTerm && !(isImmediateSaleAble && term == 0)) || term > maxTerm)
                {
                    Console.WriteLine("You entered the wrong term");
                    Console.WriteLine("Enter the term once more: ");
                    parsed = int.TryParse(Console.ReadLine(), out term);
                }
                if (term != 0)
                {
                    Console.WriteLine($"The redemption will cost you {Program.CutZeros(pawnShop.GetRedemptionPrice(customer, thing, term))} hrn");
                    Console.WriteLine("Do you want to continue? ");
                    Console.WriteLine("0. Reenter the term");
                    Console.WriteLine("1. Continue");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("2. Quit");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    const int minPoint = 0;
                    const int maxPoint = 2;
                    Console.WriteLine($"\nEnter the number {minPoint} - {maxPoint}: ");
                    parsed = int.TryParse(Console.ReadLine(), out choice);
                    while (!parsed || choice < minPoint || choice > maxPoint)
                    {
                        Console.WriteLine($"Error: you entered not a number or number was smaller than {minPoint} or bigger than {maxPoint}.");
                        Console.WriteLine($"Help - {maxPoint - 1}");
                        parsed = int.TryParse(Console.ReadLine(), out choice);
                    }
                    if (choice == 0)
                        reenter = true;
                }
                else
                {
                    choice = 1;
                }
            } while (reenter);
            if (choice == 2)
                return 0;
            try
            {
                return pawnShop.BailThing(customer, thing, term);
            }
            catch (ArgumentException exc)
            {
                Console.WriteLine("Denied. Unfortunately, we are not able to handle this deal");
                Console.WriteLine(exc.Message);
                return 0;
            }
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
