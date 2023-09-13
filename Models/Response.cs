namespace WebServiceShopping.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        // Loại sản phẩm
        public List<ProductType> arrayProductType { get; set; }
        public ProductType productType { get; set; }
        // Sản phẩm
        public List<Product> arrayProduct { get; set; }
        public Product product { get; set; }

        public List<Customers> arrayCustomer { get; set; }
        public Customers customer { get; set; }

        public PaginationInfo Pagination { get; set; }

    }
}
