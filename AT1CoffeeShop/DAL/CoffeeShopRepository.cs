using AT1CoffeeShop.BLL;
using AT1CoffeeShop.Models.BusinessModels;
using AT1CoffeeShop.Models.DBModels;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
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
            Console.WriteLine("Enter customer name: ");
            string customerName = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertOrderQuery = "INSERT INTO Orders (CustomerName) VALUES (@CustomerName); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(insertOrderQuery, connection))
                {
                    command.Parameters.AddWithValue("@CustomerName", customerName);
                    int orderId = Convert.ToInt32(command.ExecuteScalar());

                    Console.WriteLine("Order created successfully. Order ID: " + orderId);
                    Console.WriteLine();

                    bool addMoreItems = false;
                    while (!addMoreItems)
                    {
                        Console.WriteLine("What item would you like to add to the order: ");

                        // Dynamically checking items that possible to add.
                        string selectQuery = "SELECT ItemId, CoffeeName FROM Items";
                        using (SqlCommand command2 = new SqlCommand(selectQuery, connection))
                        {
                            using (SqlDataReader reader = command2.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Id: {reader.GetInt32(0)}, Coffee name: {reader.GetString(1)}");
                                }
                            }
                        }
                    


                        Console.WriteLine("Enter item ID: ");
                        int itemId = Convert.ToInt32(Console.ReadLine());


                        Console.WriteLine("Enter quantity: ");
                        int quantity = Convert.ToInt32(Console.ReadLine());

                        string insertOrderItemQuery = "INSERT INTO OrderItems (OrderId, ItemId, ItemQty) VALUES (@OrderId, @ItemId, @ItemQty)";
                        using (SqlCommand itemCommand = new SqlCommand(insertOrderItemQuery, connection))
                        {
                            itemCommand.Parameters.AddWithValue("@OrderId", orderId);
                            itemCommand.Parameters.AddWithValue("@ItemId", itemId);
                            itemCommand.Parameters.AddWithValue("@ItemQty", quantity);
                            itemCommand.ExecuteNonQuery();
                        }

                        Console.WriteLine("Anything else?");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");
                        if (Console.ReadLine() == "2")
                        {
                            addMoreItems = true;
                        }
                    }
                }
            }
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
            }
            Console.WriteLine();
        }

        public void UpdateOrder()
        {
            Console.WriteLine("Enter the Id of the order to update:");
            int orderIdToUpdate = Convert.ToInt32(Console.ReadLine());

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nWhat would you like to do?");
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
                                Console.WriteLine("Customer name was updated. \n");
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

                            Console.WriteLine("Item added. \n");
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
                        Console.WriteLine($"What item(s) would you like to remove from order? \n" +
                            $" PLEASE NOTE: if you have separate items with the same name - they all will be removed.");

                        string selectQuery2 = "SELECT OrderItems.ItemId, CoffeeName FROM OrderItems JOIN Items ON Items.ItemId = OrderItems.ItemId WHERE OrderId = @OrderId";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            using (SqlCommand command = new SqlCommand(selectQuery2, connection))
                            {
                                command.Parameters.AddWithValue("@OrderId", orderIdToUpdate);

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Console.WriteLine($"Id: {reader.GetInt32(0)}, Coffee name: {reader.GetString(1)}");
                                    }
                                }
                            }
                        }

                        string itemIdToRemove = Console.ReadLine();
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            string deleteQuery = "DELETE FROM OrderItems WHERE OrderId = @OrderId AND ItemId = @ItemId";
                            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                            {
                                command.Parameters.AddWithValue("@OrderId", orderIdToUpdate);
                                command.Parameters.AddWithValue("@ItemId", itemIdToRemove);
                                int rowsAffected = command.ExecuteNonQuery();
                                Console.WriteLine("Item was removed successfully.");
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
            Console.Write("Enter the Order ID to cancel: ");
            int orderId = Convert.ToInt32(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string deleteOrderQuery1 = "DELETE FROM OrderItems WHERE OrderId = @OrderId";

                using (SqlCommand command = new SqlCommand(deleteOrderQuery1, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.ExecuteNonQuery();
                }


                string deleteOrderQuery2 = "DELETE FROM Orders WHERE OrderId = @OrderId";
                using (SqlCommand command = new SqlCommand(deleteOrderQuery2, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine("Order cancelled successfully.");
                    else
                        Console.WriteLine("Order not found.");
                }
            }
        }
    }
}
