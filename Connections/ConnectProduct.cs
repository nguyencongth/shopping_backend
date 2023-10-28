
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing.Printing;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectProduct
    {
        public Response getProductId(MySqlConnection connection, int idsp)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("sp_productDetail", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("IN_idsp", idsp);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
            connection.Close();

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.idsp = Convert.ToInt32(dt.Rows[i]["idsp"]);
                    product.idloaisp = Convert.ToInt32(dt.Rows[i]["idloaisp"]);
                    product.tensp = Convert.ToString(dt.Rows[i]["tensp"]);
                    product.gianhap = Convert.ToDecimal(dt.Rows[i]["gianhap"]);
                    product.giaban = Convert.ToDecimal(dt.Rows[i]["giaban"]);
                    product.thongtinsp = Convert.ToString(dt.Rows[i]["thongtinsp"]);
                    product.slsanpham = Convert.ToInt32(dt.Rows[i]["slsanpham"]);
                    product.ngaynhaphang = Convert.ToDateTime(dt.Rows[i]["ngaynhaphang"]);
                    product.anhsp = Convert.ToString(dt.Rows[i]["anhsp"]);

                    products.Add(product);
                }
            }

            if (products.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Thông tin chi tiết sản phẩm";
                response.arrayProduct = products;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm thấy sản phẩm nào!";
                response.arrayProduct = null;
            }
            return response;
        }
        public Response productAll(MySqlConnection connection, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            //string query = "Select * from sanpham";
            //MySqlCommand sql = new MySqlCommand("sp_sanpham_all", connection);
            MySqlCommand sql = new MySqlCommand("sp_sanpham_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;

            // Thêm tham số phân trang
            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            // Thêm tham số OUT để trả về tổng số sản phẩm
            sql.Parameters.Add("@totalProducts", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dt = new DataTable();
            
            adapter.Fill(dt);
            connection.Close();

            List<Product> products = new List<Product>();
            if(dt.Rows.Count > 0 )
            {
                for(int i = 0; i < dt.Rows.Count; i++ ) {
                    Product product = new Product();
                    product.idsp = Convert.ToInt32(dt.Rows[i]["idsp"]);
                    product.idloaisp = Convert.ToInt32(dt.Rows[i]["idloaisp"]);
                    product.tensp = Convert.ToString(dt.Rows[i]["tensp"]);
                    product.gianhap = Convert.ToDecimal(dt.Rows[i]["gianhap"]);
                    product.giaban = Convert.ToDecimal(dt.Rows[i]["giaban"]);
                    product.thongtinsp = Convert.ToString(dt.Rows[i]["thongtinsp"]);
                    product.slsanpham = Convert.ToInt32(dt.Rows[i]["slsanpham"]);
                    product.ngaynhaphang = Convert.ToDateTime(dt.Rows[i]["ngaynhaphang"]);
                    product.anhsp = Convert.ToString(dt.Rows[i]["anhsp"]);

                    products.Add(product);
                }
            }

            // Lấy giá trị tổng số sản phẩm từ tham số OUT
            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

            // Tạo thông tin phân trang
            PaginationInfo paginationInfo = new PaginationInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalProducts,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize)
            };

            if (products.Count > 0 )
            {
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách tất cả sản phẩm";
                response.arrayProduct = products;
                response.Pagination = paginationInfo;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm thấy sản phẩm nào!";
                response.arrayProduct = null;
                response.Pagination = null;
            }
            return response;
        }

        public Response product_dress(MySqlConnection connection, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            //string query = "Select * from sanpham";
            //MySqlCommand sql = new MySqlCommand("sp_sanpham_all", connection);
            MySqlCommand sql = new MySqlCommand("sp_sanpham_dress_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;

            // Thêm tham số phân trang
            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            // Thêm tham số OUT để trả về tổng số sản phẩm
            sql.Parameters.Add("@totalProducts", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
            connection.Close();

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.idsp = Convert.ToInt32(dt.Rows[i]["idsp"]);
                    product.idloaisp = Convert.ToInt32(dt.Rows[i]["idloaisp"]);
                    product.tensp = Convert.ToString(dt.Rows[i]["tensp"]);
                    product.gianhap = Convert.ToDecimal(dt.Rows[i]["gianhap"]);
                    product.giaban = Convert.ToDecimal(dt.Rows[i]["giaban"]);
                    product.thongtinsp = Convert.ToString(dt.Rows[i]["thongtinsp"]);
                    product.slsanpham = Convert.ToInt32(dt.Rows[i]["slsanpham"]);
                    product.ngaynhaphang = Convert.ToDateTime(dt.Rows[i]["ngaynhaphang"]);
                    product.anhsp = Convert.ToString(dt.Rows[i]["anhsp"]);

                    products.Add(product);
                }
            }

            // Lấy giá trị tổng số sản phẩm từ tham số OUT
            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

            // Tạo thông tin phân trang
            PaginationInfo paginationInfo = new PaginationInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalProducts,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize)
            };

            if (products.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách tất cả sản phẩm";
                response.arrayProduct = products;
                response.Pagination = paginationInfo;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm thấy sản phẩm nào!";
                response.arrayProduct = null;
                response.Pagination = null;
            }
            return response;
        }

        public Response product_shirt(MySqlConnection connection, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand sql = new MySqlCommand("sp_sanpham_shirt_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;

            // Thêm tham số phân trang
            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            // Thêm tham số OUT để trả về tổng số sản phẩm
            sql.Parameters.Add("@totalProducts", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
            connection.Close();

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.idsp = Convert.ToInt32(dt.Rows[i]["idsp"]);
                    product.idloaisp = Convert.ToInt32(dt.Rows[i]["idloaisp"]);
                    product.tensp = Convert.ToString(dt.Rows[i]["tensp"]);
                    product.gianhap = Convert.ToDecimal(dt.Rows[i]["gianhap"]);
                    product.giaban = Convert.ToDecimal(dt.Rows[i]["giaban"]);
                    product.thongtinsp = Convert.ToString(dt.Rows[i]["thongtinsp"]);
                    product.slsanpham = Convert.ToInt32(dt.Rows[i]["slsanpham"]);
                    product.ngaynhaphang = Convert.ToDateTime(dt.Rows[i]["ngaynhaphang"]);
                    product.anhsp = Convert.ToString(dt.Rows[i]["anhsp"]);

                    products.Add(product);
                }
            }

            // Lấy giá trị tổng số sản phẩm từ tham số OUT
            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

            // Tạo thông tin phân trang
            PaginationInfo paginationInfo = new PaginationInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalProducts,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize)
            };

            if (products.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách tất cả sản phẩm";
                response.arrayProduct = products;
                response.Pagination = paginationInfo;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm thấy sản phẩm nào!";
                response.arrayProduct = null;
                response.Pagination = null;
            }
            return response;
        }
        public Response SearchProduct(MySqlConnection connection, string keyword, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand sql = new MySqlCommand("sp_search_product_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;
            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@keyword", keyword);
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
            connection.Close();

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.idsp = Convert.ToInt32(dt.Rows[i]["idsp"]);
                    product.idloaisp = Convert.ToInt32(dt.Rows[i]["idloaisp"]);
                    product.tensp = Convert.ToString(dt.Rows[i]["tensp"]);
                    product.gianhap = Convert.ToDecimal(dt.Rows[i]["gianhap"]);
                    product.giaban = Convert.ToDecimal(dt.Rows[i]["giaban"]);
                    product.thongtinsp = Convert.ToString(dt.Rows[i]["thongtinsp"]);
                    product.slsanpham = Convert.ToInt32(dt.Rows[i]["slsanpham"]);
                    product.ngaynhaphang = Convert.ToDateTime(dt.Rows[i]["ngaynhaphang"]);
                    product.anhsp = Convert.ToString(dt.Rows[i]["anhsp"]);

                    products.Add(product);
                }
            }

            // Lấy giá trị tổng số sản phẩm từ tham số OUT
            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

            // Tạo thông tin phân trang
            PaginationInfo paginationInfo = new PaginationInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalProducts,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize)
            };

            if (products.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Kết quả tìm kiếm sản phẩm";
                response.arrayProduct = products;
                response.Pagination = paginationInfo;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm thấy sản phẩm nào!";
                response.arrayProduct = null;
                response.Pagination = null;
            }
            return response;
        }
    }
}
