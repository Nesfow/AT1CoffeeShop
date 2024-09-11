using System;
using System.Data.SqlClient;

namespace AT1CoffeeShop.DAL
{
    public class OrderRepository
    {
        private readonly string connectionString;

        public OrderRepository(string connectionString)
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

                    bool addMoreItems = true;
                    while (addMoreItems)
                    {
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

                        Console.WriteLine("Do you want to add more items to the order? (yes/no): ");
                        addMoreItems = Console.ReadLine()?.ToLower() == "yes";
                    }
                }
            }
        }

        public void ViewOrders()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectOrdersQuery = "SELECT OrderId, CustomerName FROM Orders";

                using (SqlCommand command = new SqlCommand(selectOrdersQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Orders:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Order ID: {reader.GetInt32(0)}, Customer Name: {reader.GetString(1)}");
                        }
                    }
                }
            }

        }

        public void UpdateOrder()
        {

        }

        public void CancelOrder()
        {
            Console.Write("Enter the Order ID to cancel: ");
            int orderId = Convert.ToInt32(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteOrderQuery = "DELETE FROM Orders WHERE OrderId = @OrderId";

                using (SqlCommand command = new SqlCommand(deleteOrderQuery, connection))
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
