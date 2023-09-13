namespace WebServiceShopping.Models
{
    public class PaginationInfo
    {
        public int CurrentPage { get; set; }        // Trang hiện tại
        public int ItemsPerPage { get; set; }       // Số mục trên mỗi trang
        public int TotalItems { get; set; }         // Tổng số mục
        public int TotalPages { get; set; }         // Tổng số trang

        public PaginationInfo()
        {
            CurrentPage = 1;    
            ItemsPerPage = 10; // Số mục trên mỗi trang mặc định (có thể thay đổi)
            TotalItems = 0;
            TotalPages = 0;
        }
    }
}
