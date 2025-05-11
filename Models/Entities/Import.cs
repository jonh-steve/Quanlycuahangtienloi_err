// Tạo file trong thư mục /Models/Entities
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class Import
    {
        public int ImportID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ImportCode { get; set; }
        public DateTime ImportDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Thuộc tính bổ sung
        public List<ImportDetail> Details { get; set; }
    }
}