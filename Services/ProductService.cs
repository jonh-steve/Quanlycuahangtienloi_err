// File: Services/ProductService.cs (Class)
using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public List<ProductDTO> GetAllProducts(bool includeInactive = false)
        {
            return _productRepository.GetAllProducts(includeInactive);
        }

        public ProductSearchResultDTO SearchProducts(string searchTerm, int? categoryID = null, bool? isActive = null, int page = 1, int pageSize = 20)
        {
            // Kiểm tra tham số
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            // Tìm kiếm sản phẩm
            var products = _productRepository.SearchProducts(searchTerm, categoryID, isActive, page, pageSize);

            // Lấy tổng số sản phẩm
            var totalProducts = _productRepository.GetProductCount(searchTerm, categoryID, isActive);

            // Tính toán thông tin phân trang
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            return new ProductSearchResultDTO
            {
                Products = products,
                TotalProducts = totalProducts,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public ProductDTO GetProductByID(int productID)
        {
            return _productRepository.GetProductByID(productID);
        }

        public ProductDTO GetProductByCode(string productCode)
        {
            return _productRepository.GetProductByCode(productCode);
        }

        public ProductDTO GetProductByBarcode(string barcode)
        {
            return _productRepository.GetProductByBarcode(barcode);
        }

        public int CreateProduct(ProductDTO productDTO, int createdBy)
        {
            // Xác thực dữ liệu đầu vào
            ValidateProduct(productDTO);

            // Kiểm tra mã sản phẩm và barcode có trùng không
            if (IsProductCodeExists(productDTO.ProductCode))
            {
                throw new Exception("Mã sản phẩm đã tồn tại!");
            }

            if (!string.IsNullOrEmpty(productDTO.Barcode) && IsBarcodeExists(productDTO.Barcode))
            {
                throw new Exception("Mã vạch đã tồn tại!");
            }

            // Chuyển đổi từ DTO sang Entity
            var product = new Product
            {
                ProductCode = productDTO.ProductCode,
                Barcode = productDTO.Barcode,
                ProductName = productDTO.ProductName,
                CategoryID = productDTO.CategoryID,
                Description = productDTO.Description,
                CostPrice = productDTO.CostPrice,
                SellPrice = productDTO.SellPrice,
                Unit = productDTO.Unit,
                ImagePath = productDTO.ImagePath,
                MinimumStock = productDTO.MinimumStock,
                InitialStock = productDTO.CurrentStock,
                IsActive = productDTO.IsActive,
                CreatedBy = createdBy
            };

            // Thêm sản phẩm mới
            return _productRepository.CreateProduct(product);
        }

        public bool UpdateProduct(ProductDTO productDTO, int modifiedBy)
        {
            // Xác thực dữ liệu đầu vào
            ValidateProduct(productDTO);

            // Kiểm tra mã sản phẩm và barcode có trùng không
            if (IsProductCodeExists(productDTO.ProductCode, productDTO.ProductID))
            {
                throw new Exception("Mã sản phẩm đã tồn tại!");
            }

            if (!string.IsNullOrEmpty(productDTO.Barcode) && IsBarcodeExists(productDTO.Barcode, productDTO.ProductID))
            {
                throw new Exception("Mã vạch đã tồn tại!");
            }

            // Chuyển đổi từ DTO sang Entity
            var product = new Product
            {
                ProductID = productDTO.ProductID,
                ProductCode = productDTO.ProductCode,
                Barcode = productDTO.Barcode,
                ProductName = productDTO.ProductName,
                CategoryID = productDTO.CategoryID,
                Description = productDTO.Description,
                CostPrice = productDTO.CostPrice,
                SellPrice = productDTO.SellPrice,
                Unit = productDTO.Unit,
                ImagePath = productDTO.ImagePath,
                MinimumStock = productDTO.MinimumStock,
                IsActive = productDTO.IsActive,
                ModifiedBy = modifiedBy
            };

            // Cập nhật sản phẩm
            return _productRepository.UpdateProduct(product);
        }

        public bool DeactivateProduct(int productID)
        {
            // Lấy thông tin sản phẩm hiện tại
            var product = _productRepository.GetProductByID(productID);
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại!");
            }

            // Vô hiệu hóa sản phẩm
            product.IsActive = false;

            // Chuyển đổi từ DTO sang Entity và cập nhật
            var productEntity = new Product
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                CategoryID = product.CategoryID,
                Description = product.Description,
                CostPrice = product.CostPrice,
                SellPrice = product.SellPrice,
                Unit = product.Unit,
                ImagePath = product.ImagePath,
                MinimumStock = product.MinimumStock,
                IsActive = false,
                ModifiedBy = 1 // Temporary: should be the current user's ID
            };

            return _productRepository.UpdateProduct(productEntity);
        }

        public int AddProductImage(int productID, string imagePath, int displayOrder = 0, bool isDefault = false)
        {
            // Kiểm tra sản phẩm tồn tại
            var product = _productRepository.GetProductByID(productID);
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại!");
            }

            // Tạo đối tượng hình ảnh
            var image = new ProductImage
            {
                ProductID = productID,
                ImagePath = imagePath,
                DisplayOrder = displayOrder,
                IsDefault = isDefault
            };

            // Thêm hình ảnh và lấy ID
            int imageID = _productRepository.AddProductImage(image);

            // Nếu đây là hình ảnh mặc định, cập nhật các hình ảnh khác
            if (isDefault)
            {
                UpdateDefaultImage(productID, imageID);
            }

            return imageID;
        }

        public bool DeleteProductImage(int imageID)
        {
            return _productRepository.DeleteProductImage(imageID);
        }

        public bool SetDefaultImage(int productID, int imageID)
        {
            // Kiểm tra sản phẩm tồn tại
            var product = _productRepository.GetProductByID(productID);
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại!");
            }

            // Kiểm tra hình ảnh thuộc sản phẩm
            var images = _productRepository.GetProductImages(productID);
            var image = images.FirstOrDefault(i => i.ImageID == imageID);
            if (image == null)
            {
                throw new Exception("Hình ảnh không tồn tại hoặc không thuộc sản phẩm này!");
            }

            // Cập nhật hình ảnh mặc định
            return UpdateDefaultImage(productID, imageID);
        }

        private bool UpdateDefaultImage(int productID, int defaultImageID)
        {
            try
            {
                // Đây là một phương thức giả định (vì không có trong IProductRepository)
                // Trong thực tế, bạn sẽ cần thêm phương thức này vào repository

                // 1. Lấy tất cả hình ảnh của sản phẩm
                var images = _productRepository.GetProductImages(productID);

                // 2. Đặt IsDefault = false cho tất cả hình ảnh
                foreach (var image in images)
                {
                    // Bỏ qua hình ảnh mặc định mới
                    if (image.ImageID == defaultImageID)
                        continue;

                    // Nếu hình ảnh đang là mặc định, cập nhật thành false
                    if (image.IsDefault)
                    {
                        image.IsDefault = false;
                        // Cập nhật trong CSDL (giả định có phương thức này)
                        // _productRepository.UpdateProductImage(image);
                    }
                }

                // 3. Đặt IsDefault = true cho hình ảnh mặc định mới
                var defaultImage = images.FirstOrDefault(i => i.ImageID == defaultImageID);
                if (defaultImage != null && !defaultImage.IsDefault)
                {
                    defaultImage.IsDefault = true;
                    // Cập nhật trong CSDL (giả định có phương thức này)
                    // _productRepository.UpdateProductImage(defaultImage);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddProductPrice(int productID, decimal costPrice, decimal sellPrice, DateTime effectiveDate, int createdBy)
        {
            // Kiểm tra sản phẩm tồn tại
            var product = _productRepository.GetProductByID(productID);
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại!");
            }

            // Kiểm tra giá hợp lệ
            if (costPrice < 0 || sellPrice < 0)
            {
                throw new Exception("Giá sản phẩm không được âm!");
            }

            // Tạo đối tượng giá
            var price = new ProductPrice
            {
                ProductID = productID,
                CostPrice = costPrice,
                SellPrice = sellPrice,
                EffectiveDate = effectiveDate,
                CreatedBy = createdBy
            };

            // Thêm giá mới
            return _productRepository.AddProductPrice(price);
        }

        public bool UpdateProductPrice(int productID, decimal costPrice, decimal sellPrice, int modifiedBy)
        {
            // Thêm giá mới với ngày hiệu lực là hiện tại
            AddProductPrice(productID, costPrice, sellPrice, DateTime.Now, modifiedBy);

            // Cập nhật giá trong bảng Product
            var product = _productRepository.GetProductByID(productID);
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại!");
            }

            product.CostPrice = costPrice;
            product.SellPrice = sellPrice;

            // Chuyển đổi từ DTO sang Entity và cập nhật
            var productEntity = new Product
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                CategoryID = product.CategoryID,
                Description = product.Description,
                CostPrice = costPrice,
                SellPrice = sellPrice,
                Unit = product.Unit,
                ImagePath = product.ImagePath,
                MinimumStock = product.MinimumStock,
                IsActive = product.IsActive,
                ModifiedBy = modifiedBy
            };

            return _productRepository.UpdateProduct(productEntity);
        }

        public List<ProductDTO> GetLowStockProducts()
        {
            return _productRepository.GetLowStockProducts();
        }

        public bool IsProductCodeExists(string productCode, int? excludeProductID = null)
        {
            // Lấy sản phẩm theo mã
            var product = _productRepository.GetProductByCode(productCode);

            // Nếu không tìm thấy, trả về false
            if (product == null)
                return false;

            // Nếu tìm thấy và không cần loại trừ sản phẩm nào, trả về true
            if (!excludeProductID.HasValue)
                return true;

            // Nếu tìm thấy và cần loại trừ, kiểm tra ID
            return product.ProductID != excludeProductID.Value;
        }

        public bool IsBarcodeExists(string barcode, int? excludeProductID = null)
        {
            // Nếu barcode rỗng, trả về false
            if (string.IsNullOrEmpty(barcode))
                return false;

            // Lấy sản phẩm theo barcode
            var product = _productRepository.GetProductByBarcode(barcode);

            // Nếu không tìm thấy, trả về false
            if (product == null)
                return false;

            // Nếu tìm thấy và không cần loại trừ sản phẩm nào, trả về true
            if (!excludeProductID.HasValue)
                return true;

            // Nếu tìm thấy và cần loại trừ, kiểm tra ID
            return product.ProductID != excludeProductID.Value;
        }

        #region Helper Methods

        private void ValidateProduct(ProductDTO product)
        {
            // Kiểm tra tên sản phẩm
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new Exception("Tên sản phẩm không được để trống!");
            }

            // Kiểm tra mã sản phẩm
            if (string.IsNullOrWhiteSpace(product.ProductCode))
            {
                throw new Exception("Mã sản phẩm không được để trống!");
            }

            if (!Validators.IsValidProductCode(product.ProductCode))
            {
                throw new Exception("Mã sản phẩm không hợp lệ! Chỉ được chứa chữ cái, số và dấu gạch dưới, tối thiểu 3 ký tự.");
            }

            // Kiểm tra barcode
            if (!string.IsNullOrEmpty(product.Barcode) && !Validators.IsValidBarcode(product.Barcode))
            {
                throw new Exception("Mã vạch không hợp lệ!");
            }

            // Kiểm tra danh mục
            var category = _categoryRepository.GetCategoryByID(product.CategoryID);
            if (category == null)
            {
                throw new Exception("Danh mục không tồn tại!");
            }

            // Kiểm tra giá
            if (product.CostPrice < 0 || product.SellPrice < 0)
            {
                throw new Exception("Giá sản phẩm không được âm!");
            }

            // Kiểm tra tồn kho tối thiểu
            if (product.MinimumStock < 0)
            {
                throw new Exception("Tồn kho tối thiểu không được âm!");
            }
        }

        #endregion
    }
}