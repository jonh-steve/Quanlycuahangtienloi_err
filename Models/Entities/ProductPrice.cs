// Tạo file trong thư mục /Models/Entities
using System;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class ProductPrice
    {
        public int PriceID { get; set; }
        public int ProductID { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}