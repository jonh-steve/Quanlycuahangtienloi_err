// Tạo file trong thư mục /Models/DTO
using System;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class InventoryReportDTO
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalCostValue { get; set; }
        public decimal TotalSellValue { get; set; }
        public decimal PotentialProfit { get; set; }
    }
}