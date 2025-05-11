// Tạo file trong thư mục /Models/DTO
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class ImportDTO
    {
        public int ImportID { get; set; }
        public string ImportCode { get; set; }
        public DateTime ImportDate { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string CreatedByName { get; set; }

        // Thuộc tính bổ sung
        public int ItemCount { get; set; }
        public List<ImportDetailDTO> Details { get; set; }
    }
}