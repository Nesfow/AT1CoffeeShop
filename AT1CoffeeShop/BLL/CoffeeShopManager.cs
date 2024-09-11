using AT1CoffeeShop.DAL;
using System;

namespace AT1CoffeeShop.BLL
{
    public class OrderManager
    {
        private readonly OrderRepository orderRepository;

        public OrderManager(string connectionString)
        {
            orderRepository = new OrderRepository(connectionString);
        }

        public void CreateOrder()
        {
            orderRepository.CreateOrder();
        }

        public void ViewOrders()
        {
            orderRepository.ViewOrders();
        }

        public void UpdateOrder()
        {
            orderRepository.UpdateOrder();
        }

        public void CancelOrder()
        {
            orderRepository.CancelOrder();
        }
    }
}
