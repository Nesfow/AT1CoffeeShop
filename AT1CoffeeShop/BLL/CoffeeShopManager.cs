using AT1CoffeeShop.DAL;

namespace AT1CoffeeShop.BLL
{
    public class CoffeeShopManager
    {
        private readonly CoffeeShopRepository coffeeShopRepository;

        public CoffeeShopManager(string connectionString)
        {
            coffeeShopRepository = new CoffeeShopRepository(connectionString);
        }

        public void CreateOrder()
        {
            coffeeShopRepository.CreateOrder();
        }
        public void ViewOrders()
        {
            coffeeShopRepository.ViewOrders();
        }

        public void UpdateOrder()
        {
            coffeeShopRepository.UpdateOrder();
        }
        public void CancelOrder()
        {
            coffeeShopRepository.CancelOrder();
        }
    }
}
