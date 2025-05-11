
// File: Models/DTO/InventoryReportDTO.cs
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class InventoryReportDTO
    {
        public int TotalProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public List<LowStockProductDTO> LowStockProducts { get; set; } = new List<LowStockProductDTO>();
        public List<InventoryCategoryDTO> InventoryByCategory { get; set; } = new List<InventoryCategoryDTO>();
        public List<InventoryDetailDTO> InventoryDetails { get; set; } = new List<InventoryDetailDTO>();
    }

    public class LowStockProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int ReorderQuantity { get; set; }
    }

    public class InventoryCategoryDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class InventoryDetailDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int CurrentStock { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}