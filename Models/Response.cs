namespace WebServiceShopping.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ProductType> arrayProductType { get; set; }
        public ProductType productType { get; set; }
        public List<Product> arrayProduct { get; set; }
        public Product product { get; set; }
        public List<Customers> arrayCustomer { get; set; }
        public Customers customer { get; set; }
        public List<Cart> arrayCart { get; set; }
        public Cart cart { get; set; }
        public List<Orders> arrayOrders { get; set; }
        public Orders orders { get; set; }
        public List<OrderItem> arrayOrderItem { get; set; }
        public OrderItem orderItem { get; set; }
        public Login login { get; set; }
        public PaginationInfo Pagination { get; set; }
        public SendOtpResponse sendOtpResponse { get; set; }
        public int id_customer { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
        public string[] Roles { get; set; }
    }
}
