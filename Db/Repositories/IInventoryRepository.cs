// File: /Db/Repositories/IInventoryRepository.cs
// Mô tả: Interface định nghĩa các phương thức truy cập dữ liệu kho hàng
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public interface IInventoryRepository
    {
        // Lấy toàn bộ thông tin tồn kho
        List<Inventory> GetAllInventory();

        // Lấy thông tin tồn kho theo sản phẩm
        Inventory GetInventoryByProductID(int productID);

        // Lấy danh sách tồn kho theo mức còn lại (hàng sắp hết)
        List<Inventory> GetLowStockInventory();

        // Cập nhật số lượng tồn kho
        bool UpdateInventoryQuantity(int productID, int newQuantity, string note, int employeeID);

        // Lấy lịch sử giao dịch tồn kho của sản phẩm
        List<InventoryTransaction> GetInventoryTransactionsByProduct(int productID, DateTime? startDate, DateTime? endDate);

        // Lấy tất cả giao dịch tồn kho trong khoảng thời gian
        List<InventoryTransaction> GetInventoryTransactions(DateTime startDate, DateTime endDate, string transactionType = null);

        // Thêm giao dịch tồn kho
        int AddInventoryTransaction(InventoryTransaction transaction);

        // Kiểm tra số lượng tồn kho
        bool CheckInventoryAvailability(int productID, int requiredQuantity);
    }
}