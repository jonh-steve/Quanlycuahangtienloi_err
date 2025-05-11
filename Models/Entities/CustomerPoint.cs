// Steve-Thuong_hai
using System;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class CustomerPoints
    {
        public int PointID { get; set; }

        public int CustomerID { get; set; }

        public int Points { get; set; }

        public string PointType { get; set; } // Earn, Redeem, Adjust

        public string Description { get; set; }

        public int? ReferenceID { get; set; } // OrderID nếu liên quan đến đơn hàng

        public string ReferenceType { get; set; } // Order, Manual, Promotion

        public DateTime TransactionDate { get; set; }

        public int CreatedBy { get; set; } // EmployeeID của người thực hiện

        // Constructor
        public CustomerPoints()
        {
            TransactionDate = DateTime.Now;
        }
    }
}