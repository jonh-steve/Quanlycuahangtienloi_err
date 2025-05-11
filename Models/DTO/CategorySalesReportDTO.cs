
// File: Models/DTO/CategorySalesReportDTO.cs
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class CategorySalesReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CategorySalesItemDTO> CategorySales { get; set; } = new List<CategorySalesItemDTO>();
    }

    public class CategorySalesItemDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalSales { get; set; }
        public int Quantity { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal Percentage { get; set; }
    }
}
