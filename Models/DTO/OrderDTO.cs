// Tạo file trong thư mục /Models/DTO
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string Note { get; set; }

        // Thông tin khách hàng
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        // Thông tin nhân viên
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }

        // Thuộc tính bổ sung
        public int ItemCount { get; set; }
        public List<OrderDetailDTO> Details { get; set; }
    }
}