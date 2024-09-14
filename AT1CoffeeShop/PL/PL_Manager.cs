using AT1CoffeeShop.BLL;

// PL, stands for presentation layer - responsible for Console representation of the app
namespace AT1CoffeeShop.PL
{
    public class PL_Manager
    {
        // Connection string to the database - retrieved from the SQL Server object explorer
        static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CoffeeShop;Integrated Security=True;";

        static CoffeeShopManager coffeeShopManager = new CoffeeShopManager(connectionString);
        // Main program logic
        public static void Run()
        {
            Console.WriteLine("Welcome to the Coffee Shop Manager Console!");
            Console.WriteLine("----------------------------------------------");

            // Creating while loop to make program work until user desides to stop it,
            // by choosing option 5 or closing the application
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("Please, select the function you would like to perform:");
                Console.WriteLine("1. Create new order");
                Console.WriteLine("2. View all current orders");
                Console.WriteLine("3. Update order");
                Console.WriteLine("4. Cancel order");
                Console.WriteLine("5. Exit");
                Console.WriteLine("Enter your choice: ");
                Console.WriteLine();

                // Wrapping the main application part in the try catch block to handle errors
                try
                {
                    switch (Console.ReadLine())
                    {
                        // If user input 1 - it will start the process of order creation
                        case "1":
                            coffeeShopManager.CreateOrder();
                            break;
                        // If user input 2 - it will start the process of viewing all orders
                        case "2":
                            coffeeShopManager.ViewOrders();
                            break;
                        // If user input 3 - it will start the process of updating order
                        case "3":
                            coffeeShopManager.UpdateOrder();
                            break;
                        // If user input 4 - it will start the process of cancelling order
                        case "4":
                            coffeeShopManager.CancelOrder();
                            break;
                        // If user input 5 - it will stop the application
                        case "5":
                            exit = true;
                            break;
                        default:
                            // Notification to the user of the wrong input
                            Console.WriteLine("I am sorry, but this option doesn't exist. Please try another value :)");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("I am sorry, but the error has occured. Please, record the error message and notify support team.");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}