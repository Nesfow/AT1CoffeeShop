namespace AT1CoffeeShop.Models.BusinessModels
{
    // This class is used to store OrderToDisplay objects to be displayed
    // on the screen when called "View Orders" option
    public class OrdersToDisplay
    {
        public List<OrderToDisplay> AllOrdersToDisplay { get; set; } = [];
    }
}
