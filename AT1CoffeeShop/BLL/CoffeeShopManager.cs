using AT1CoffeeShop.DAL;

// BLL - business logic layer. It represents business actions
namespace AT1CoffeeShop.BLL
{
    public class CoffeeShopManager
    {
        // This object is used to perform CRUD operations on the database
        private readonly CoffeeShopRepository coffeeShopRepository;

        // This is a CoffeeShopManager constructor that gets 1 parameter - connectionString
        // connectionString is then passes to new CoffeeShopRepository object to establish connection with the database
        public CoffeeShopManager(string connectionString)
        {
            coffeeShopRepository = new CoffeeShopRepository(connectionString);
        }

        // Represents Create element of CRUD
        public void CreateOrder()
        {
            coffeeShopRepository.CreateOrder();
        }

        // Represents Read element of CRUD
        public void ViewOrders()
        {
            coffeeShopRepository.ViewOrders();
        }

        // Represents Update element of CRUD
        public void UpdateOrder()
        {
            coffeeShopRepository.UpdateOrder();
        }

        // Represents Delete element of CRUD
        public void CancelOrder()
        {
            coffeeShopRepository.CancelOrder();
        }
    }
}
