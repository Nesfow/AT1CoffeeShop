using AT1CoffeeShop.BLL;
using AT1CoffeeShop.Models.BusinessModels;
using AT1CoffeeShop.Models.DBModels;
using System.Data.SqlClient;
using System.Xml.Linq;


namespace AT1CoffeeShop.DAL
{
    public class CoffeeShopRepository
    {
        private readonly string connectionString;

        public CoffeeShopRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void CreateOrder()
        {

        }

        public void ViewOrders()
        {
            int currentRowOrderId;
            int rowOrderIdToCompare = 0;
            OrdersToDisplay allOrdersToDisplay = new();
            OrderToDisplay orderToDisplay;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT Orders.OrderId, CustomerName, CoffeeName, CoffeePrice, ItemQty FROM Orders " +
                                     "JOIN OrderItems ON Orders.OrderId = OrderItems.OrderId " +
                                     "JOIN Items ON Items.ItemId = OrderItems.ItemId " +
                                     "ORDER BY Orders.OrderId";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        orderToDisplay = new() { OrderId = 0 };

                        while (reader.Read())
                        {
                            currentRowOrderId = reader.GetInt32(0);

                            if (currentRowOrderId == rowOrderIdToCompare)
                            {
                                orderToDisplay.OrderItems.Add(Tuple.Create(reader.GetString(2), reader.GetInt32(4)));
                                orderToDisplay.TotalPrice += reader.GetDecimal(3) * reader.GetInt32(4);
                            }
                            else
                            {
                                allOrdersToDisplay.AllOrdersToDisplay.Add(orderToDisplay);

                                rowOrderIdToCompare = reader.GetInt32(0);
                                orderToDisplay = new()
                                {
                                    OrderId = reader.GetInt32(0),
                                    CustomerName = reader.GetString(1)
                                };
                                orderToDisplay.OrderItems.Add(Tuple.Create(reader.GetString(2), reader.GetInt32(4)));
                                orderToDisplay.TotalPrice += reader.GetDecimal(3) * reader.GetInt32(4);
                            }
                        }
                        allOrdersToDisplay.AllOrdersToDisplay.Add(orderToDisplay);
                    }
                }
            }

            Console.WriteLine("Orders: ");
            Console.WriteLine("====================");
            foreach (var order in allOrdersToDisplay.AllOrdersToDisplay.Skip(1))
            {
                Console.WriteLine($"Order #{order.OrderId} - {order.CustomerName}");
                Console.WriteLine($"Items: ");
                foreach (var item in order.OrderItems)
                {
                    Console.WriteLine($"\t{item.Item1}: {item.Item2} cup(s)");
                }
                Console.WriteLine($"Total price: {order.TotalPrice}");
                Console.WriteLine("......................");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void UpdateOrder()
        {
            Console.WriteLine("Enter the Id of the order to update: \n");
            int orderIdToUpdate = Convert.ToInt32(Console.ReadLine());

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Change customer name");
                Console.WriteLine("2. Add item to order");
                Console.WriteLine("3. Remove order item");
                Console.WriteLine("4. Go back");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("New customer name: ");
                        var newCustomerName = Console.ReadLine();
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string updateQuery = "UPDATE Orders SET CustomerName = @NewCustomerName WHERE OrderId = @OrderId";
                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@OrderId", orderIdToUpdate);
                                command.Parameters.AddWithValue("@NewCustomerName", newCustomerName);
                                command.ExecuteNonQuery();
                                Console.WriteLine("Customer name was updated.");
                            }
                        }

                        Console.WriteLine("Anything else?");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");
                        if (Console.ReadLine() == "2")
                        {
                            exit = true;
                        }
                        break;
                    case "2":
                        Console.WriteLine("What item would you like to add to the order: ");

                        // Dynamically checking items that possible to add.
                        string selectQuery = "SELECT ItemId, CoffeeName FROM Items";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            using (SqlCommand command = new SqlCommand(selectQuery, connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Console.WriteLine($"Id: {reader.GetInt32(0)}, Coffee name: {reader.GetString(1)}");
                                    }
                                }
                            }
                        }

                        int itemToAdd = Convert.ToInt16(Console.ReadLine());
                        Console.WriteLine("How many items?");
                        int itemQty = Convert.ToInt16(Console.ReadLine());
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            string insertQuery = "INSERT INTO OrderItems VALUES (@OrderId, @ItemId, @ItemQty)";
                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@OrderId", orderIdToUpdate);
                                command.Parameters.AddWithValue("@ItemId", itemToAdd);
                                command.Parameters.AddWithValue("@ItemQty", itemQty);
                                command.ExecuteNonQuery();
                            }

                            Console.WriteLine("Item added.");
                        }

                        Console.WriteLine("Anything else?");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");
                        if (Console.ReadLine() == "2")
                        {
                            exit = true;
                        }

                        break;
                    case "3":
                        //coffeeShopManager.UpdateOrder();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("I am sorry, but this option doesn't exist. Please try another value :)");
                        break;
                }

            }
        }

        public void CancelOrder()
        {

        }
    }
}
