using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //add
        }

        public void ViewOrders()
        {

        }

        public void UpdateOrder()
        {

        }

        public void CancelOrder()
        {

        }
    }
}
