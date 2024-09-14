namespace AT1CoffeeShop.Models
{
    // OrderItem class represents OrderItem in the database
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int ItemQty { get; set; }
    }
}