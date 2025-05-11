using System;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class SalesReportDTO
    {
        public DateTime ReportDate { get; set; }
        public int OrderCount { get; set; }
        public decimal GrossSales { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal NetSales { get; set; }
        public int CustomerCount { get; set; }

        // Các thuộc tính định dạng
        public string FormattedDate
        {
            get
            {
                return ReportDate.ToString("dd/MM/yyyy");
            }
        }

        public string FormattedWeek
        {
            get
            {
                return $"Tuần {System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(ReportDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday)}, {ReportDate.Year}";
            }
        }

        public string FormattedMonth
        {
            get
            {
                return ReportDate.ToString("MM/yyyy");
            }
        }

        public string FormattedGrossSales
        {
            get
            {
                return GrossSales.ToString("N0") + " VNĐ";
            }
        }

        public string FormattedNetSales
        {
            get
            {
                return NetSales.ToString("N0") + " VNĐ";
            }
        }

        public decimal ProfitRate
        {
            get
            {
                return NetSales == 0 ? 0 : (NetSales - GrossSales) / GrossSales * 100;
            }
        }

        public string FormattedProfitRate
        {
            get
            {
                return ProfitRate.ToString("0.00") + "%";
            }
        }
    }
}