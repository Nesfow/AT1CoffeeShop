using AT1CoffeeShop.Models.BusinessModels;
using System.Data.SqlClient;

// DAL - data access layer. Here are performed actions with the database,
// like connecting to DB and performing CRUD operation.
namespace AT1CoffeeShop.DAL
{
    public class CoffeeShopRepository
    {
        private readonly string connectionString;
        // This is a class constructor that accepts connection string during initialization
        public CoffeeShopRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // This method will be called when new order should be created
        public void CreateOrder()
        {
            // Requesting user to input customer name for new order
            Console.WriteLine("Enter customer name: ");
            string customerName = Console.ReadLine();

            // Establishing DB connection with SqlConnection object
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Opening connection
                connection.Open();
                // Creating SQL query in raw text format
                // SELECT SCOPE_IDENTITY(); is required to be able to automatically insert new ID into new row.
                string insertOrderQuery = "INSERT INTO Orders (CustomerName) VALUES (@CustomerName); SELECT SCOPE_IDENTITY();";

                // Creating SQL command to perform SQL query
                using (SqlCommand command = new SqlCommand(insertOrderQuery, connection))
                {
                    // Substituting placeholder @CustomerName with real value, based on the user input
                    command.Parameters.AddWithValue("@CustomerName", customerName);
                    // Executing SQL command and converting returned value into integer.
                    int orderId = Convert.ToInt32(command.ExecuteScalar());
                    // Displaying success message
                    Console.WriteLine("Order created successfully. Order ID: " + orderId);
                    Console.WriteLine();

                    // Setting default value for loop to start
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

                        // Asking user to input coffee ID to add to the order
                        Console.WriteLine("Enter item ID: ");
                        int itemId = Convert.ToInt32(Console.ReadLine());

                        // Asking user to input quantity of coffee cups to add to the order
                        Console.WriteLine("Enter quantity: ");
                        int quantity = Convert.ToInt32(Console.ReadLine());

                        // Creating new SQL raw query
                        string insertOrderItemQuery = "INSERT INTO OrderItems (OrderId, ItemId, ItemQty) VALUES (@OrderId, @ItemId, @ItemQty)";
                        using (SqlCommand itemCommand = new SqlCommand(insertOrderItemQuery, connection))
                        {
                            // Substituting placeholders with real values based on the user input
                            itemCommand.Parameters.AddWithValue("@OrderId", orderId);
                            itemCommand.Parameters.AddWithValue("@ItemId", itemId);
                            itemCommand.Parameters.AddWithValue("@ItemQty", quantity);
                            itemCommand.ExecuteNonQuery();
                        }

                        // Ask user if they would like to add something else to the order
                        Console.WriteLine("Anything else?");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");
                        // If user chose no - break the loop and exit order creation
                        if (Console.ReadLine() == "2")
                        {
                            addMoreItems = true;
                        }
                    }
                }
            }
        }

        // This method will be called when list of orders should be displayed
        public void ViewOrders()
        {
            // These two variables are used to compare rows,
            // because there could be several rows of the the same order with different items per each row
            int currentRowOrderId;
            int rowOrderIdToCompare = 0;

            // This object is used to store all orders to be displayed
            OrdersToDisplay allOrdersToDisplay = new();
            // This object is used to temporarily store data of each order
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
                        // Creating blank order to be compared to. It will be later skipped during orders displaying
                        orderToDisplay = new() { OrderId = 0 };
                        // Reading returned data row by row
                        while (reader.Read())
                        {
                            // Getting ID of the order in the current row
                            currentRowOrderId = reader.GetInt32(0);
                            // Checking if the ID of the current row is the same with the previous row
                            // If yes - it means that this row represents the same order, but contatins different items
                            if (currentRowOrderId == rowOrderIdToCompare)
                            {
                                // Because this row represents the same order - add order items to it and add cost to the order
                                orderToDisplay.OrderItems.Add(Tuple.Create(reader.GetString(2), reader.GetInt32(4)));
                                orderToDisplay.TotalPrice += reader.GetDecimal(3) * reader.GetInt32(4);
                            }
                            // If ID of the current row is different with the previous row
                            // it means that this is the new order. Thus, overwrite and store values in temporary object orderToDisplay
                            else
                            {
                                // Add previous order to the orders list
                                allOrdersToDisplay.AllOrdersToDisplay.Add(orderToDisplay);
                                // Assign new value of the ID to the rowOrderIdToCompare
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
                        // Adding last order to the all allOrdersToDisplay list
                        allOrdersToDisplay.AllOrdersToDisplay.Add(orderToDisplay);
                    }
                }
            }

            // Displaying all orders in the allOrdersToDisplay object
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

        // This method will be called when some order should be updated
        public void UpdateOrder()
        {
            // Requesting user to input the ID of the order to be updated
            Console.WriteLine("Enter the Id of the order to update:");
            int orderIdToUpdate = Convert.ToInt32(Console.ReadLine());

            bool exit = false;
            while (!exit)
            {
                // Displaying options of this method
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1. Change customer name");
                Console.WriteLine("2. Add item to order");
                Console.WriteLine("3. Remove order item");
                Console.WriteLine("4. Go back");
                switch (Console.ReadLine())
                {
                    // If user input 1 - it will change customer's name
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

                        // Asking user if they would like to change something else in the order
                        // If no - stop order changing and break the loop
                        Console.WriteLine("Anything else?");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");
                        if (Console.ReadLine() == "2")
                        {
                            exit = true;
                        }
                        break;
                    // If user input 2 - it will add more items to the order
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

                        // Here used should input id of the available items
                        int itemToAdd = Convert.ToInt16(Console.ReadLine());
                        Console.WriteLine("How many items?");
                        // Here used should input number of selected item
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
                        // Asking user if they would like to change something else in the order
                        // If no - stop order changing and break the loop
                        Console.WriteLine("Anything else?");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");
                        if (Console.ReadLine() == "2")
                        {
                            exit = true;
                        }
                        break;
                    // If user input 3 - it will remove item from the order
                    case "3":
                        // Displaying items in the order
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

                        // Here user input ID of the item to remove
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
                        // Asking user if they would like to change something else in the order
                        // If no - stop order changing and break the loop
                        Console.WriteLine("Anything else?");
                        Console.WriteLine("1. Yes");
                        Console.WriteLine("2. No");
                        if (Console.ReadLine() == "2")
                        {
                            exit = true;
                        }

                        break;
                    // If user input 4 - it will stop order updates
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
