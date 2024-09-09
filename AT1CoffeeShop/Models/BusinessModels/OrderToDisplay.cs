﻿namespace AT1CoffeeShop.Models.BusinessModels
{
    public class OrderToDisplay
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<Tuple<string, int>> CoffeeNames { get; set; } = [];
        public decimal TotalPrice { get; set; } = 0;
    }
}
