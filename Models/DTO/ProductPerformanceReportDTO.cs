
// File: Models/DTO/ProductPerformanceReportDTO.cs
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class ProductPerformanceReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ProductSalesDTO> TopSellingProducts { get; set; } = new List<ProductSalesDTO>();
        public List<ProductSalesDTO> WorstSellingProducts { get; set; } = new List<ProductSalesDTO>();
        public List<ProductProfitabilityDTO> MostProfitableProducts { get; set; } = new List<ProductProfitabilityDTO>();
    }

    public class ProductProfitabilityDTO
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
        public string CategoryName { get; set; }
    }
}
