using System.Numerics;

namespace WebServiceShopping.Models
{
    public class Product
    {
        public int productId { get; set; }
        public int categoryId { get; set; }
        public string productName { get; set; }
        public Decimal entryPrice { get; set; }
        public Decimal price { get; set; }
        public string descProduct { get; set; }
        public int quantityStock { get; set; }
        public int quantitySold { get; set; }
        public DateTime dateAdded { get; set; }
        public string imageProduct { get; set; }
        public Decimal discountPercentage { get; set; }
        public Decimal discountedPrice { get; set; }
    }
}
