// File: Models/DTO/SalesReportDTO.cs
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class SalesReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySalesDTO> DailySales { get; set; } = new List<DailySalesDTO>();
        public List<ProductSalesDTO> TopProducts { get; set; } = new List<ProductSalesDTO>();
        public List<PaymentMethodStatsDTO> PaymentMethodStats { get; set; } = new List<PaymentMethodStatsDTO>();
        public List<SalesDetailDTO> DetailedSales { get; set; } = new List<SalesDetailDTO>();
    }

    public class DailySalesDTO
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ProductSalesDTO
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
    }

    public class PaymentMethodStatsDTO
    {
        public int MethodID { get; set; }
        public string MethodName { get; set; }
        public int OrderCount { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class SalesDetailDTO
    {
        public int OrderID { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int ItemCount { get; set; }
    }
}
