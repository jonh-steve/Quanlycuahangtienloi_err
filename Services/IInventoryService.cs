// File: /Services/IInventoryService.cs
// Mô tả: Interface định nghĩa các phương thức nghiệp vụ kho hàng
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;

namespace QuanLyCuaHangTienLoi.Services
{
    public interface IInventoryService
    {
        // Lấy danh sách tồn kho với thông tin đầy đủ (kết hợp với thông tin sản phẩm)
        List<InventoryDTO> GetInventoryList();

        // Lấy thông tin tồn kho theo sản phẩm
        InventoryDTO GetProductInventory(int productID);

        // Điều chỉnh số lượng tồn kho
        bool AdjustInventory(int productID, int newQuantity, string reason, int employeeID);

        // Kiểm tra hàng tồn kho thấp và tạo cảnh báo
        List<InventoryDTO> GetLowStockWarnings();

        // Lấy lịch sử giao dịch tồn kho
        List<InventoryTransactionDTO> GetInventoryHistory(int productID, DateTime? startDate, DateTime? endDate);

        // Xử lý khi bán hàng (giảm tồn kho)
        bool ProcessStockForSale(int productID, int quantity, int orderID);

        // Xử lý khi nhập hàng (tăng tồn kho)
        bool ProcessStockForImport(int productID, int quantity, int importID, decimal costPrice);

        // Xử lý khi hủy đơn hàng (hoàn lại tồn kho)
        bool ProcessStockForOrderCancellation(int orderID);

        // Xuất báo cáo tồn kho
        byte[] ExportInventoryReport(DateTime asOfDate);
    }
}