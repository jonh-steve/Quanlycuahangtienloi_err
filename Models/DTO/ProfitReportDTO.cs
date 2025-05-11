
// File: Models/DTO/ProfitReportDTO.cs
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class ProfitReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitMargin { get; set; }
        public List<DailyProfitDTO> DailyProfits { get; set; } = new List<DailyProfitDTO>();
        public List<CategoryProfitDTO> CategoryProfits { get; set; } = new List<CategoryProfitDTO>();
    }

    public class DailyProfitDTO
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
    }

    public class CategoryProfitDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
    }
}