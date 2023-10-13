using System.Numerics;

namespace WebServiceShopping.Models
{
    public class Product
    {
        public int idsp { get; set; }
        public int idloaisp { get; set; }
        public string tensp { get; set; }
        public Decimal gianhap { get; set; }
        public Decimal giaban { get; set; }
        public string thongtinsp { get; set; }
        public int slsanpham { get; set; }
        public DateTime ngaynhaphang { get; set; }
        public string anhsp { get; set; }
    }
}
