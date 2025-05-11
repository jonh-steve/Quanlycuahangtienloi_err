
// File: Models/DTO/DashboardSummaryDTO.cs
using System;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class DashboardSummaryDTO
    {
        public decimal TodaySales { get; set; }
        public decimal YesterdaySales { get; set; }
        public decimal LastWeekSales { get; set; }
        public decimal LastMonthSales { get; set; }
        public int TodayOrders { get; set; }
        public int LowStockProductCount { get; set; }
        public decimal CurrentMonthRevenue { get; set; }
        public decimal CurrentMonthProfit { get; set; }
        public decimal SalesTrend { get; set; } // % tăng/giảm so với tháng trước
        public decimal ProfitTrend { get; set; } // % tăng/giảm so với tháng trước
    }
}