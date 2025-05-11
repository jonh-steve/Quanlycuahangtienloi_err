// File: /Models/Entities/OrderDetail.cs
namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }

        // Thông tin bổ sung (không lưu trong CSDL)
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Unit { get; set; }
    }
}