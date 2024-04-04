
using MySql.Data.MySqlClient;
using System.Data;
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
        public Response productAll(MySqlConnection connection, int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand sql = new MySqlCommand("sp_filter_products_by_price", connection);
            sql.CommandType = CommandType.StoredProcedure;

            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@priceRange", priceRange);
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dt = new DataTable();
            
            adapter.Fill(dt);

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

            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

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
                response.StatusCode = 400;
                response.StatusMessage = "Không tìm thấy sản phẩm nào!";
                response.arrayProduct = null;
                response.Pagination = null;
            }
            connection.Close();
            return response;
        }

        public Response getProductNew(MySqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            string query = "SELECT * FROM sanpham ORDER BY ngaynhaphang DESC LIMIT 5";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            
            adapter.Fill(dt);

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
            if (products.Count > 0 )
            {
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách sản phẩm mới nhất";
                response.arrayProduct = products;
            }
            else
            {
                response.StatusCode = 400;
                response.StatusMessage = "Không tìm thấy sản phẩm nào!";
                response.arrayProduct = null;
            }
            connection.Close();
            return response;
        }
        public Response productByCategoryId(MySqlConnection connection,int categoryId, int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand sql = new MySqlCommand("getProductByCategoryId", connection);
            sql.CommandType = CommandType.StoredProcedure;

            sql.Parameters.AddWithValue("@categoryId", categoryId);
            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@priceRange", priceRange);
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

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

            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

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
            connection.Close();
            return response;
        }

        public Response product_dress(MySqlConnection connection, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand sql = new MySqlCommand("sp_sanpham_dress_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;

            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
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

            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

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
            connection.Close();
            return response;
        }

        public Response product_shirt(MySqlConnection connection, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand sql = new MySqlCommand("sp_sanpham_shirt_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;

            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

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

            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

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
            connection.Close();
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

            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

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
        public Response FilterProductsByPrice(MySqlConnection connection, int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();

            MySqlCommand sql = new MySqlCommand("sp_filter_products_by_price", connection);
            sql.CommandType = CommandType.StoredProcedure;

            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@priceRange", priceRange);
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
                    product.gianhap = Convert.ToInt32(dt.Rows[i]["gianhap"]);
                    product.giaban = Convert.ToInt32(dt.Rows[i]["giaban"]);
                    product.thongtinsp = Convert.ToString(dt.Rows[i]["thongtinsp"]);
                    product.slsanpham = Convert.ToInt32(dt.Rows[i]["slsanpham"]);
                    product.ngaynhaphang = Convert.ToDateTime(dt.Rows[i]["ngaynhaphang"]);
                    product.anhsp = Convert.ToString(dt.Rows[i]["anhsp"]);

                    products.Add(product);
                }
            }

            int totalProducts = Convert.ToInt32(sql.Parameters["@totalProducts"].Value);

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
                response.StatusMessage = "Danh sách sản phẩm theo giá";
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
            connection.Close();
            return response;
        }
    }
}
