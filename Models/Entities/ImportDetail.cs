// Tạo file trong thư mục /Models/Entities
using System;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class ImportDetail
    {
        public int ImportDetailID { get; set; }
        public int ImportID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string BatchNumber { get; set; }
        public string Note { get; set; }
        public string Unit { get; set; }
    }
}