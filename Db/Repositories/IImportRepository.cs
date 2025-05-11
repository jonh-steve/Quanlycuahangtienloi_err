// File: /Db/Repositories/IImportRepository.cs
// Mô tả: Interface định nghĩa các phương thức truy cập dữ liệu nhập hàng
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public interface IImportRepository
    {
        // Lấy tất cả phiếu nhập
        List<Import> GetAllImports();

        // Lấy phiếu nhập theo ID
        Import GetImportByID(int importID);

        // Lấy phiếu nhập theo khoảng thời gian
        List<Import> GetImportsByDateRange(DateTime startDate, DateTime endDate, int? supplierID = null, string status = null);

        // Tạo phiếu nhập mới
        int CreateImport(Import import, List<ImportDetail> importDetails);

        // Cập nhật phiếu nhập
        bool UpdateImport(Import import);

        // Hủy phiếu nhập
        bool CancelImport(int importID, string cancelReason, int employeeID);

        // Lấy chi tiết phiếu nhập
        List<ImportDetail> GetImportDetailsByImportID(int importID);

        // Thêm chi tiết phiếu nhập
        bool AddImportDetail(ImportDetail detail);

        // Xóa chi tiết phiếu nhập
        bool DeleteImportDetail(int importDetailID);

        // Cập nhật chi tiết phiếu nhập
        bool UpdateImportDetail(ImportDetail detail);
    }
}