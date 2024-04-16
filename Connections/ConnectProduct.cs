using System.Data;
using Microsoft.Data.SqlClient;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectProduct
    {
        public Response getProductId(SqlConnection connection, int productID)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand cmd = new SqlCommand("sp_productDetail", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IN_productId", productID);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
            connection.Close();

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.productId = Convert.ToInt32(dt.Rows[i]["productId"]);
                    product.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    product.productName = Convert.ToString(dt.Rows[i]["productName"]);
                    product.entryPrice = Convert.ToDecimal(dt.Rows[i]["entryPrice"]);
                    product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                    product.descProduct = Convert.ToString(dt.Rows[i]["descProduct"]);
                    product.quantityStock = Convert.ToInt32(dt.Rows[i]["quantityStock"]);
                    product.dateAdded = Convert.ToDateTime(dt.Rows[i]["dateAdded"]);
                    product.imageProduct = Convert.ToString(dt.Rows[i]["imageProduct"]);

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

        public Response productAll(SqlConnection connection, int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand sql = new SqlCommand("sp_filter_products_by_price", connection);
            sql.CommandType = CommandType.StoredProcedure;

            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@priceRange", priceRange);
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

            SqlDataAdapter adapter = new SqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.productId = Convert.ToInt32(dt.Rows[i]["productId"]);
                    product.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    product.productName = Convert.ToString(dt.Rows[i]["productName"]);
                    product.entryPrice = Convert.ToDecimal(dt.Rows[i]["entryPrice"]);
                    product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                    product.descProduct = Convert.ToString(dt.Rows[i]["descProduct"]);
                    product.quantityStock = Convert.ToInt32(dt.Rows[i]["quantityStock"]);
                    product.dateAdded = Convert.ToDateTime(dt.Rows[i]["dateAdded"]);
                    product.imageProduct = Convert.ToString(dt.Rows[i]["imageProduct"]);

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
                response.StatusCode = 400;
                response.StatusMessage = "Không tìm thấy sản phẩm nào!";
                response.arrayProduct = null;
                response.Pagination = null;
            }

            connection.Close();
            return response;
        }

        public Response getProductNew(SqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            string query = "SELECT TOP 5 * FROM products ORDER BY dateAdded DESC;";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.productId = Convert.ToInt32(dt.Rows[i]["productId"]);
                    product.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    product.productName = Convert.ToString(dt.Rows[i]["productName"]);
                    product.entryPrice = Convert.ToDecimal(dt.Rows[i]["entryPrice"]);
                    product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                    product.descProduct = Convert.ToString(dt.Rows[i]["descProduct"]);
                    product.quantityStock = Convert.ToInt32(dt.Rows[i]["quantityStock"]);
                    product.dateAdded = Convert.ToDateTime(dt.Rows[i]["dateAdded"]);
                    product.imageProduct = Convert.ToString(dt.Rows[i]["imageProduct"]);

                    products.Add(product);
                }
            }

            if (products.Count > 0)
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

        public Response productByCategoryId(SqlConnection connection, int categoryId, int priceRange, int page,
            int pageSize)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand sql = new SqlCommand("getProductByCategoryId", connection);
            sql.CommandType = CommandType.StoredProcedure;

            sql.Parameters.AddWithValue("@categoryId", categoryId);
            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@priceRange", priceRange);
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

            SqlDataAdapter adapter = new SqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.productId = Convert.ToInt32(dt.Rows[i]["productId"]);
                    product.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    product.productName = Convert.ToString(dt.Rows[i]["productName"]);
                    product.entryPrice = Convert.ToDecimal(dt.Rows[i]["entryPrice"]);
                    product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                    product.descProduct = Convert.ToString(dt.Rows[i]["descProduct"]);
                    product.quantityStock = Convert.ToInt32(dt.Rows[i]["quantityStock"]);
                    product.dateAdded = Convert.ToDateTime(dt.Rows[i]["dateAdded"]);
                    product.imageProduct = Convert.ToString(dt.Rows[i]["imageProduct"]);

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

        public Response product_dress(SqlConnection connection, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand sql = new SqlCommand("sp_sanpham_dress_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;

            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

            SqlDataAdapter adapter = new SqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.productId = Convert.ToInt32(dt.Rows[i]["productId"]);
                    product.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    product.productName = Convert.ToString(dt.Rows[i]["productName"]);
                    product.entryPrice = Convert.ToDecimal(dt.Rows[i]["entryPrice"]);
                    product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                    product.descProduct = Convert.ToString(dt.Rows[i]["descProduct"]);
                    product.quantityStock = Convert.ToInt32(dt.Rows[i]["quantityStock"]);
                    product.dateAdded = Convert.ToDateTime(dt.Rows[i]["dateAdded"]);
                    product.imageProduct = Convert.ToString(dt.Rows[i]["imageProduct"]);

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

        public Response product_shirt(SqlConnection connection, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand sql = new SqlCommand("sp_sanpham_shirt_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;

            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

            SqlDataAdapter adapter = new SqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.productId = Convert.ToInt32(dt.Rows[i]["productId"]);
                    product.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    product.productName = Convert.ToString(dt.Rows[i]["productName"]);
                    product.entryPrice = Convert.ToDecimal(dt.Rows[i]["entryPrice"]);
                    product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                    product.descProduct = Convert.ToString(dt.Rows[i]["descProduct"]);
                    product.quantityStock = Convert.ToInt32(dt.Rows[i]["quantityStock"]);
                    product.dateAdded = Convert.ToDateTime(dt.Rows[i]["dateAdded"]);
                    product.imageProduct = Convert.ToString(dt.Rows[i]["imageProduct"]);

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

        public Response SearchProduct(SqlConnection connection, string keyword, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand sql = new SqlCommand("sp_search_product_paginated", connection);
            sql.CommandType = CommandType.StoredProcedure;
            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@keyword", keyword);
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);

            sql.Parameters.Add("@totalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

            SqlDataAdapter adapter = new SqlDataAdapter(sql);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
            connection.Close();

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.productId = Convert.ToInt32(dt.Rows[i]["productId"]);
                    product.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    product.productName = Convert.ToString(dt.Rows[i]["productName"]);
                    product.entryPrice = Convert.ToDecimal(dt.Rows[i]["entryPrice"]);
                    product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                    product.descProduct = Convert.ToString(dt.Rows[i]["descProduct"]);
                    product.quantityStock = Convert.ToInt32(dt.Rows[i]["quantityStock"]);
                    product.dateAdded = Convert.ToDateTime(dt.Rows[i]["dateAdded"]);
                    product.imageProduct = Convert.ToString(dt.Rows[i]["imageProduct"]);

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

        public Response FilterProductsByPrice(SqlConnection connection, int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            connection.Open();

            SqlCommand sql = new SqlCommand("sp_filter_products_by_price", connection);
            sql.CommandType = CommandType.StoredProcedure;

            int startIndex = (page - 1) * pageSize;
            sql.Parameters.AddWithValue("@priceRange", priceRange);
            sql.Parameters.AddWithValue("@startIndex", startIndex);
            sql.Parameters.AddWithValue("@pageSize", pageSize);
            sql.Parameters.Add("@totalProducts", SqlDbType.Int).Direction = ParameterDirection.Output;

            SqlDataAdapter adapter = new SqlDataAdapter(sql);

            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();

            List<Product> products = new List<Product>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Product product = new Product();
                    product.productId = Convert.ToInt32(dt.Rows[i]["productId"]);
                    product.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    product.productName = Convert.ToString(dt.Rows[i]["productName"]);
                    product.entryPrice = Convert.ToDecimal(dt.Rows[i]["entryPrice"]);
                    product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                    product.descProduct = Convert.ToString(dt.Rows[i]["descProduct"]);
                    product.quantityStock = Convert.ToInt32(dt.Rows[i]["quantityStock"]);
                    product.dateAdded = Convert.ToDateTime(dt.Rows[i]["dateAdded"]);
                    product.imageProduct = Convert.ToString(dt.Rows[i]["imageProduct"]);

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