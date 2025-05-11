// Steve-Thuong_hai
using System;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class CustomerPointHistoryDTO
    {
        public int PointID { get; set; }

        public int CustomerID { get; set; }

        public string CustomerName { get; set; }

        public int Points { get; set; }

        public string PointType { get; set; }

        public string Description { get; set; }

        public int? ReferenceID { get; set; }

        public string ReferenceType { get; set; }

        public string ReferenceDescription { get; set; }

        public DateTime TransactionDate { get; set; }

        public string CreatedByName { get; set; }
    }
}