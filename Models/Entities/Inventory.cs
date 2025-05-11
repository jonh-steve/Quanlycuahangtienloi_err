// Tạo file trong thư mục /Models/Entities
using System;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class Inventory
    {
        public int InventoryID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }

        // Thuộc tính bổ sung
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal TotalCostValue { get { return Quantity * CostPrice; } }
        public decimal TotalSellValue { get { return Quantity * SellPrice; } }
    }
}