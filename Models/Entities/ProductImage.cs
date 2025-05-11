// Tạo file trong thư mục /Models/Entities
using System;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class ProductImage
    {
        public int ImageID { get; set; }
        public int ProductID { get; set; }
        public string ImagePath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}