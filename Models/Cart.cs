namespace WebServiceShopping.Models
{
    public class Cart
    {
        public int cartId { get; set; }
        public int customerId { get; set; }
        public int productId { get; set; }
        public int categoryId { get; set; }
        public string imageProduct { get; set; }
        public string productName { get; set; }
        public Decimal price { get; set; }
        public int quantity { get; set; }
        public DateTime dateAdded { get; set; }


    }
}
