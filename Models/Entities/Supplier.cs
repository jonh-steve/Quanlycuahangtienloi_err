// Tạo file trong thư mục /Models/Entities
using System;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Thuộc tính bổ sung không có trong CSDL
        public int ImportCount { get; set; }
        public DateTime? LastImportDate { get; set; }
    }
}