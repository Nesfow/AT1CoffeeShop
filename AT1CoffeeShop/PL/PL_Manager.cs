using AT1CoffeeShop.BLL;
using System;

namespace AT1CoffeeShop.PL
{
    public class PL_CoffeeShop
    {
        public static void Run()
        {
            OrderManager orderManager = new(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CoffeeshopDB;Integrated Security=True;");
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Create Order");
                Console.WriteLine("2. View Orders");
                Console.WriteLine("3. Update Order");
                Console.WriteLine("4. Cancel Order");
                Console.WriteLine("5. Exit");
                Console.WriteLine("Enter your choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        orderManager.CreateOrder();
                        break;
                    case "2":
                        orderManager.ViewOrders();
                        break;
                    case "3":
                        orderManager.UpdateOrder();
                        break;
                    case "4":
                        orderManager.CancelOrder();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}
