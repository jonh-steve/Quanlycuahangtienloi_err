// Tạo file trong thư mục /Models/DTO
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public string Unit { get; set; }
        public int MinimumStock { get; set; }
        public bool IsActive { get; set; }

        // Thuộc tính bổ sung
        public int CurrentStock { get; set; }
        public bool IsLowStock { get; set; }
        public string ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; }
        public decimal Profit { get { return SellPrice - CostPrice; } }
        public decimal ProfitMargin { get { return CostPrice > 0 ? (Profit / CostPrice) * 100 : 0; } }
    }
}