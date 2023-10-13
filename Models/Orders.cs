namespace WebServiceShopping.Models
{
    public class Orders
    {
        public int order_id { get; set; }
        public int id_customer { get; set; }
        public DateTime order_date { get; set; }
        public int total_amount { get; set; }

        public List<OrderItem> orderItems { get; set; }
    }
}
