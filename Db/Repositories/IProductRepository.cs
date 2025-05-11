// File: Db/Repositories/IProductRepository.cs (Interface)
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public interface IProductRepository
    {
        // Lấy tất cả sản phẩm
        List<ProductDTO> GetAllProducts(bool includeInactive = false);

        // Tìm kiếm sản phẩm
        List<ProductDTO> SearchProducts(string searchTerm, int? categoryID = null, bool? isActive = null, int page = 1, int pageSize = 20);

        // Lấy tổng số sản phẩm theo điều kiện tìm kiếm
        int GetProductCount(string searchTerm, int? categoryID = null, bool? isActive = null);

        // Lấy chi tiết sản phẩm theo ID
        ProductDTO GetProductByID(int productID);

        // Lấy chi tiết sản phẩm theo mã
        ProductDTO GetProductByCode(string productCode);

        // Lấy chi tiết sản phẩm theo barcode
        ProductDTO GetProductByBarcode(string barcode);

        // Thêm sản phẩm mới
        int CreateProduct(Product product);

        // Cập nhật thông tin sản phẩm
        bool UpdateProduct(Product product);

        // Xóa sản phẩm
        bool DeleteProduct(int productID);

        // Thêm hình ảnh sản phẩm
        int AddProductImage(ProductImage image);

        // Xóa hình ảnh sản phẩm
        bool DeleteProductImage(int imageID);

        // Lấy danh sách hình ảnh của sản phẩm
        List<ProductImage> GetProductImages(int productID);

        // Thêm lịch sử giá sản phẩm
        int AddProductPrice(ProductPrice price);

        // Lấy lịch sử giá của sản phẩm
        List<ProductPrice> GetProductPriceHistory(int productID);

        // Lấy sản phẩm có tồn kho thấp
        List<ProductDTO> GetLowStockProducts();
    }
}