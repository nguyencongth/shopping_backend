namespace WebServiceShopping.Models
{
    public class OrderItem
    {
        public int order_item_id { get; set; }
        public int idsp { get; set; }
        public string tensp { get; set; }
        public string anhsp { get; set; }
        public int quantity { get; set; }
        public decimal subtotal { get; set; }
    }
}
