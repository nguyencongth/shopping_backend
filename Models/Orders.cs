namespace WebServiceShopping.Models
{
    public class Orders
    {
        public int orderId { get; set; }
        public int customerId { get; set; }
        public DateTime orderDate { get; set; }
        public Decimal totalAmount { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string shippingAddress { get; set; }
        public string paymentMethod { get; set; }
        public int orderStatus { get; set; }
        public List<OrderItem> orderItems { get; set; }
    }
}
