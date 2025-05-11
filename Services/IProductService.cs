// File: Services/IProductService.cs (Interface)
using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Services
{
    public interface IProductService
    {
        // Lấy tất cả sản phẩm
        List<ProductDTO> GetAllProducts(bool includeInactive = false);

        // Tìm kiếm sản phẩm với phân trang
        ProductSearchResultDTO SearchProducts(string searchTerm, int? categoryID = null, bool? isActive = null, int page = 1, int pageSize = 20);

        // Lấy chi tiết sản phẩm theo ID
        ProductDTO GetProductByID(int productID);

        // Lấy chi tiết sản phẩm theo mã
        ProductDTO GetProductByCode(string productCode);

        // Lấy chi tiết sản phẩm theo barcode
        ProductDTO GetProductByBarcode(string barcode);

        // Tạo sản phẩm mới
        int CreateProduct(ProductDTO product, int createdBy);

        // Cập nhật thông tin sản phẩm
        bool UpdateProduct(ProductDTO product, int modifiedBy);

        // Vô hiệu hóa sản phẩm
        bool DeactivateProduct(int productID);

        // Thêm hình ảnh sản phẩm
        int AddProductImage(int productID, string imagePath, int displayOrder = 0, bool isDefault = false);

        // Xóa hình ảnh sản phẩm
        bool DeleteProductImage(int imageID);

        // Đặt hình ảnh mặc định
        bool SetDefaultImage(int productID, int imageID);

        // Thêm lịch sử giá sản phẩm
        int AddProductPrice(int productID, decimal costPrice, decimal sellPrice, DateTime effectiveDate, int createdBy);

        // Cập nhật giá sản phẩm
        bool UpdateProductPrice(int productID, decimal costPrice, decimal sellPrice, int modifiedBy);

        // Lấy sản phẩm có tồn kho thấp
        List<ProductDTO> GetLowStockProducts();

        // Kiểm tra sản phẩm tồn tại
        bool IsProductCodeExists(string productCode, int? excludeProductID = null);

        // Kiểm tra barcode tồn tại
        bool IsBarcodeExists(string barcode, int? excludeProductID = null);
    }
}