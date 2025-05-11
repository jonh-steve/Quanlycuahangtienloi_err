// Tạo file trong thư mục /Models/Entities
using System;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class InventoryTransaction
    {
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string TransactionType { get; set; }
        public int Quantity { get; set; }
        public int PreviousQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? ReferenceID { get; set; }
        public string ReferenceType { get; set; }
        public string Note { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}