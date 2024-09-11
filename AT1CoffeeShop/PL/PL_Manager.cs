using AT1CoffeeShop.BLL;

namespace AT1CoffeeShop.PL
{
    public class PL_Manager
    {
        static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CoffeeShop;Integrated Security=True;";
        static CoffeeShopManager coffeeShopManager = new CoffeeShopManager(connectionString);
        public static void Run()
        {
            Console.WriteLine("Welcome to the Coffee Shop Manager Console!");
            Console.WriteLine("----------------------------------------------");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Please, select the function you would like to perform:");
                Console.WriteLine("1. Create new order");
                Console.WriteLine("2. View all current orders");
                Console.WriteLine("3. Update order");
                Console.WriteLine("4. Cancel order");
                Console.WriteLine("5. Exit");
                Console.WriteLine("Enter your choice: ");
                Console.WriteLine();

                switch (Console.ReadLine())
                {
                    case "1":
                        coffeeShopManager.CreateOrder();
                        break;
                    case "2":
                        coffeeShopManager.ViewOrders();
                        break;
                    case "3":
                        coffeeShopManager.UpdateOrder();
                        break;
                    case "4":
                        coffeeShopManager.CancelOrder();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("I am sorry, but this option doesn't exist. Please try another value :)");
                        break;
                }
            }
        }
    }

}