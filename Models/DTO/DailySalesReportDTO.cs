using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    // Thêm các DTO cho báo cáo doanh số
    public class DailySalesReportDTO
    {
        public DateTime SalesDate { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalTax { get; set; }
        public decimal NetSales { get; set; }
    }
}
