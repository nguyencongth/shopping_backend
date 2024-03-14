namespace WebServiceShopping.Models
{
    public class ResetPassword
    {
        public string email { get; set; }
        public string newPassword { get; set; }
        public int otp { get; set; }
    }
}
