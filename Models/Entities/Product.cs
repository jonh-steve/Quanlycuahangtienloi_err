// Tạo file trong thư mục /Models/Entities
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class Product
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
        public string ImagePath { get; set; }
        public int MinimumStock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Thuộc tính bổ sung không có trong CSDL
        public int CurrentStock { get; set; }
        public bool IsLowStock { get; set; }
        public List<ProductImage> Images { get; set; }
        public List<ProductPrice> PriceHistory { get; set; }
    }
}