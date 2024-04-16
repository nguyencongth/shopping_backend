namespace WebServiceShopping.Models
{
    public class Customers
    {
        public int customerId { get; set; }
        public string fullName { get; set;}
        public string email { get; set;}
        public string phonenumber { get; set;}
        public string password { get; set;}
        public string address { get; set;}
        public int otp { get; set; }
        public DateTime otpExpiry { get; set; }
    }
}
