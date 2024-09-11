namespace AT1CoffeeShop.Models
{
    public class Items
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int ItemQty { get; set; }
        public double ItemPrice { get; set; }

    }
}