// Tạo file trong thư mục /Models/DTO
using System;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class ProductListItemDTO
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal SellPrice { get; set; }
        public int CurrentStock { get; set; }
        public bool IsActive { get; set; }
        public bool IsLowStock { get; set; }
    }
}