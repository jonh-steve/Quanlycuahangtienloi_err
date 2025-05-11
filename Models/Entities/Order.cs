// File: /Models/Entities/Order.cs
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class Order
    {
        public int OrderID { get; set; }
        public string OrderCode { get; set; }
        public int? CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal FinalAmount { get; set; }
        public int PaymentMethodID { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Thông tin bổ sung (không lưu trong CSDL)
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string EmployeeName { get; set; }
        public int ItemCount { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new List<OrderDetail>();
            OrderDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            Discount = 0;
            PaymentStatus = "Pending";
        }
    }
}