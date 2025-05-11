// File: /Services/IImportService.cs
// Mô tả: Interface định nghĩa các phương thức nghiệp vụ nhập hàng
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;

namespace QuanLyCuaHangTienLoi.Services
{
    public interface IImportService
    {
        // Lấy danh sách phiếu nhập
        List<ImportDTO> GetImportList(DateTime? startDate = null, DateTime? endDate = null, int? supplierID = null);

        // Lấy chi tiết phiếu nhập
        ImportDTO GetImportDetails(int importID);

        // Tạo phiếu nhập mới
        int CreateNewImport(ImportDTO importDTO, List<ImportDetailDTO> details, int employeeID);

        // Cập nhật phiếu nhập
        bool UpdateImport(ImportDTO importDTO, List<ImportDetailDTO> details);

        // Hoàn thành phiếu nhập (cập nhật tồn kho)
        bool CompleteImport(int importID, int employeeID);

        // Hủy phiếu nhập
        bool CancelImport(int importID, string reason, int employeeID);

        // Kiểm tra khả năng cập nhật phiếu nhập
        bool CanEditImport(int importID);

        // Lấy sản phẩm từ nhà cung cấp
        List<ProductDTO> GetProductsBySupplier(int supplierID);

        // Xuất phiếu nhập thành PDF
        byte[] ExportImportToPdf(int importID);

        // Tạo mã phiếu nhập mới
        string GenerateImportCode();
    }
}