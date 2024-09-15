namespace AT1CoffeeShop.Models.BusinessModels
{
    // This is a non-database class that represents business entity - an order;
    // It is used to display order on screen
    public class OrderToDisplay
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<Tuple<string, int>> OrderItems { get; set; } = [];
        public decimal TotalPrice { get; set; } = 0;
    }
}
