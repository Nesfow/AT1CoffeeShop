namespace AT1CoffeeShop.Models
{
    // Order class represents Order in the database
    public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}
}