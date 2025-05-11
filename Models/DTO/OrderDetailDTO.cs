// Tạo file trong thư mục /Models/DTO
using System;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class OrderDetailDTO
    {
        public int OrderDetailID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Unit { get; set; }
    }
}