namespace WebServiceShopping.Models
{
    public class OrderItem
    {
        public int orderItemId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public string imageProduct { get; set; }
        public int quantity { get; set; }
        public decimal subtotal { get; set; }
    }
}
