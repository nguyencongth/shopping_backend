namespace WebServiceShopping.Models
{
    public class Cart
    {
        public int cartID { get; set; }
        public int id_customer { get; set; }
        public int idsp { get; set; }
        public int idloaisp { get; set; }
        public string anhsp { get; set; }
        public string tensp { get; set; }
        public Decimal giaban { get; set; }
        public int quantity { get; set; }
        public DateTime dateAdded { get; set; }
        //public Decimal TotalPrice { get; set; }


    }
}
