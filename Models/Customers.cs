namespace WebServiceShopping.Models
{
    public class Customers
    {
        public int id_customer { get; set; }
        public string fullname { get; set;}
        public string email { get; set;}
        public string phonenumber { get; set;}
        public string password { get; set;}
        public string address { get; set;}
        public int otp { get; set; }
        public DateTime otpExpiry { get; set; }
    }
}
