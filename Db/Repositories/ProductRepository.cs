// File: Db/Repositories/ProductRepository.cs (Class)
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public List<ProductDTO> GetAllProducts(bool includeInactive = false)
        {
            try
            {
                var cmd = CreateCommand("app.sp_GetAllProducts");

                // Thêm tham số nếu cần
                if (!includeInactive)
                {
                    cmd.Parameters.AddWithValue("@IsActive", true);
                }

                return ExecuteReader(cmd, MapToProductDTO);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetAllProducts: {ex.Message}", ex);
                throw;
            }
        }

        public List<ProductDTO> SearchProducts(string searchTerm, int? categoryID = null, bool? isActive = null, int page = 1, int pageSize = 20)
        {
            try
            {
                var cmd = CreateCommand("app.sp_SearchProducts");

                cmd.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);

                if (categoryID.HasValue)
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID.Value);
                }

                if (isActive.HasValue)
                {
                    cmd.Parameters.AddWithValue("@IsActive", isActive.Value);
                }

                cmd.Parameters.AddWithValue("@PageNumber", page);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                return ExecuteReader(cmd, MapToProductDTO);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in SearchProducts: {ex.Message}", ex);
                throw;
            }
        }

        public int GetProductCount(string searchTerm, int? categoryID = null, bool? isActive = null)
        {
            try
            {
                var cmd = CreateCommand("app.sp_GetProductCount");

                cmd.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);

                if (categoryID.HasValue)
                {
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID.Value);
                }

                if (isActive.HasValue)
                {
                    cmd.Parameters.AddWithValue("@IsActive", isActive.Value);
                }

                return Convert.ToInt32(ExecuteScalar(cmd));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetProductCount: {ex.Message}", ex);
                throw;
            }
        }

        public ProductDTO GetProductByID(int productID)
        {
            try
            {
                var cmd = CreateCommand("app.sp_GetProductByID");
                cmd.Parameters.AddWithValue("@ProductID", productID);

                var result = ExecuteReader(cmd, MapToProductDTO);

                if (result != null && result.Count > 0)
                {
                    var product = result[0];

                    // Lấy danh sách hình ảnh
                    product.Images = GetProductImages(productID);

                    // Lấy lịch sử giá
                    product.PriceHistory = GetProductPriceHistory(productID);

                    return product;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetProductByID: {ex.Message}", ex);
                throw;
            }
        }

        public ProductDTO GetProductByCode(string productCode)
        {
            try
            {
                var cmd = CreateCommand("app.sp_GetProductByCode");
                cmd.Parameters.AddWithValue("@ProductCode", productCode);

                var result = ExecuteReader(cmd, MapToProductDTO);

                if (result != null && result.Count > 0)
                {
                    var product = result[0];

                    // Lấy danh sách hình ảnh
                    product.Images = GetProductImages(product.ProductID);

                    // Lấy lịch sử giá
                    product.PriceHistory = GetProductPriceHistory(product.ProductID);

                    return product;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetProductByCode: {ex.Message}", ex);
                throw;
            }
        }

        public ProductDTO GetProductByBarcode(string barcode)
        {
            try
            {
                var cmd = CreateCommand("app.sp_GetProductByBarcode");
                cmd.Parameters.AddWithValue("@Barcode", barcode);

                var result = ExecuteReader(cmd, MapToProductDTO);

                if (result != null && result.Count > 0)
                {
                    var product = result[0];

                    // Lấy danh sách hình ảnh
                    product.Images = GetProductImages(product.ProductID);

                    // Lấy lịch sử giá
                    product.PriceHistory = GetProductPriceHistory(product.ProductID);

                    return product;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetProductByBarcode: {ex.Message}", ex);
                throw;
            }
        }

        public int CreateProduct(Product product)
        {
            try
            {
                var cmd = CreateCommand("app.sp_CreateProduct");

                cmd.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                cmd.Parameters.AddWithValue("@Barcode", (object)product.Barcode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                cmd.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CostPrice", product.CostPrice);
                cmd.Parameters.AddWithValue("@SellPrice", product.SellPrice);
                cmd.Parameters.AddWithValue("@Unit", (object)product.Unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImagePath", (object)product.ImagePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MinimumStock", product.MinimumStock);
                cmd.Parameters.AddWithValue("@InitialStock", product.InitialStock ?? 0);
                cmd.Parameters.AddWithValue("@CreatedBy", product.CreatedBy);

                var result = ExecuteScalar(cmd);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in CreateProduct: {ex.Message}", ex);
                throw;
            }
        }

        public bool UpdateProduct(Product product)
        {
            try
            {
                var cmd = CreateCommand("app.sp_UpdateProduct");

                cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                cmd.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CostPrice", product.CostPrice);
                cmd.Parameters.AddWithValue("@SellPrice", product.SellPrice);
                cmd.Parameters.AddWithValue("@Unit", (object)product.Unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImagePath", (object)product.ImagePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MinimumStock", product.MinimumStock);
                cmd.Parameters.AddWithValue("@IsActive", product.IsActive);
                cmd.Parameters.AddWithValue("@ModifiedBy", product.ModifiedBy);

                ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in UpdateProduct: {ex.Message}", ex);
                throw;
            }
        }

        public bool DeleteProduct(int productID)
        {
            try
            {
                // Trong thực tế, chúng ta thường không xóa sản phẩm mà chỉ vô hiệu hóa
                var cmd = CreateCommand("app.sp_DeactivateProduct");
                cmd.Parameters.AddWithValue("@ProductID", productID);

                ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in DeleteProduct: {ex.Message}", ex);
                throw;
            }
        }

        public int AddProductImage(ProductImage image)
        {
            try
            {
                var cmd = CreateCommand("app.sp_AddProductImage");

                cmd.Parameters.AddWithValue("@ProductID", image.ProductID);
                cmd.Parameters.AddWithValue("@ImagePath", image.ImagePath);
                cmd.Parameters.AddWithValue("@DisplayOrder", image.DisplayOrder);
                cmd.Parameters.AddWithValue("@IsDefault", image.IsDefault);

                var result = ExecuteScalar(cmd);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in AddProductImage: {ex.Message}", ex);
                throw;
            }
        }

        public bool DeleteProductImage(int imageID)
        {
            try
            {
                var cmd = CreateCommand("app.sp_DeleteProductImage");
                cmd.Parameters.AddWithValue("@ImageID", imageID);

                ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in DeleteProductImage: {ex.Message}", ex);
                throw;
            }
        }

        public List<ProductImage> GetProductImages(int productID)
        {
            try
            {
                var cmd = CreateCommand("app.sp_GetProductImages");
                cmd.Parameters.AddWithValue("@ProductID", productID);

                return ExecuteReader(cmd, MapToProductImage);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetProductImages: {ex.Message}", ex);
                throw;
            }
        }

        public int AddProductPrice(ProductPrice price)
        {
            try
            {
                var cmd = CreateCommand("app.sp_AddProductPrice");

                cmd.Parameters.AddWithValue("@ProductID", price.ProductID);
                cmd.Parameters.AddWithValue("@CostPrice", price.CostPrice);
                cmd.Parameters.AddWithValue("@SellPrice", price.SellPrice);
                cmd.Parameters.AddWithValue("@EffectiveDate", price.EffectiveDate);
                cmd.Parameters.AddWithValue("@CreatedBy", price.CreatedBy);

                var result = ExecuteScalar(cmd);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in AddProductPrice: {ex.Message}", ex);
                throw;
            }
        }

        public List<ProductPrice> GetProductPriceHistory(int productID)
        {
            try
            {
                var cmd = CreateCommand("app.sp_GetProductPriceHistory");
                cmd.Parameters.AddWithValue("@ProductID", productID);

                return ExecuteReader(cmd, MapToProductPrice);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetProductPriceHistory: {ex.Message}", ex);
                throw;
            }
        }

        public List<ProductDTO> GetLowStockProducts()
        {
            try
            {
                var cmd = CreateCommand("app.sp_GetLowStockProducts");

                return ExecuteReader(cmd, MapToProductDTO);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in GetLowStockProducts: {ex.Message}", ex);
                throw;
            }
        }

        #region Mapping Methods

        private ProductDTO MapToProductDTO(SqlDataReader reader)
        {
            return new ProductDTO
            {
                ProductID = Convert.ToInt32(reader["ProductID"]),
                ProductCode = reader["ProductCode"].ToString(),
                Barcode = reader["Barcode"] != DBNull.Value ? reader["Barcode"].ToString() : null,
                ProductName = reader["ProductName"].ToString(),
                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                CategoryName = reader["CategoryName"].ToString(),
                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                CostPrice = Convert.ToDecimal(reader["CostPrice"]),
                SellPrice = Convert.ToDecimal(reader["SellPrice"]),
                Unit = reader["Unit"] != DBNull.Value ? reader["Unit"].ToString() : null,
                ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : null,
                MinimumStock = Convert.ToInt32(reader["MinimumStock"]),
                CurrentStock = reader["CurrentStock"] != DBNull.Value ? Convert.ToInt32(reader["CurrentStock"]) : 0,
                IsLowStock = reader["IsLowStock"] != DBNull.Value && Convert.ToBoolean(reader["IsLowStock"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

        private ProductImage MapToProductImage(SqlDataReader reader)
        {
            return new ProductImage
            {
                ImageID = Convert.ToInt32(reader["ImageID"]),
                ProductID = Convert.ToInt32(reader["ProductID"]),
                ImagePath = reader["ImagePath"].ToString(),
                DisplayOrder = Convert.ToInt32(reader["DisplayOrder"]),
                IsDefault = Convert.ToBoolean(reader["IsDefault"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };
        }

        private ProductPrice MapToProductPrice(SqlDataReader reader)
        {
            return new ProductPrice
            {
                PriceID = Convert.ToInt32(reader["PriceID"]),
                ProductID = Convert.ToInt32(reader["ProductID"]),
                CostPrice = Convert.ToDecimal(reader["CostPrice"]),
                SellPrice = Convert.ToDecimal(reader["SellPrice"]),
                EffectiveDate = Convert.ToDateTime(reader["EffectiveDate"]),
                EndDate = reader["EndDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["EndDate"]) : null,
                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : 0
            };
        }

        #endregion
    }
}