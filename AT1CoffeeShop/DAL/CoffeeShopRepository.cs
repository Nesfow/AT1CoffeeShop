using AT1CoffeeShop.Models.BusinessModels;
using AT1CoffeeShop.Models.DBModels;
using System.Data.SqlClient;


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

        }

        public void CancelOrder()
        {

        }
    }
}
