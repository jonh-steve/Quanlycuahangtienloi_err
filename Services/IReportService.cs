using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.DTO;

namespace QuanLyCuaHangTienLoi.Services
{
    public interface IReportService
    {
        // Báo cáo doanh số
        List<SalesReportDTO> GetDailySalesReport(DateTime startDate, DateTime endDate);
        List<SalesReportDTO> GetWeeklySalesReport(DateTime startDate, DateTime endDate);
        List<SalesReportDTO> GetMonthlySalesReport(DateTime startDate, DateTime endDate);

        // Báo cáo sản phẩm bán chạy
        List<ProductSalesReportDTO> GetTopSellingProducts(DateTime startDate, DateTime endDate, int topCount = 10);

        // Báo cáo doanh thu theo danh mục
        List<CategorySalesReportDTO> GetCategorySalesReport(DateTime startDate, DateTime endDate);

        // Báo cáo theo phương thức thanh toán
        List<PaymentMethodReportDTO> GetPaymentMethodReport(DateTime startDate, DateTime endDate);

        // Báo cáo tồn kho
        List<InventoryReportDTO> GetInventoryValueReport();
        List<InventoryReportDTO> GetLowStockReport();

        // Báo cáo lợi nhuận
        decimal GetTotalRevenue(DateTime startDate, DateTime endDate);
        decimal GetTotalProfit(DateTime startDate, DateTime endDate);
        decimal GetTotalCost(DateTime startDate, DateTime endDate);

        // Lấy dữ liệu cho biểu đồ
        object GetChartData(string chartType, DateTime startDate, DateTime endDate);

        // Xuất báo cáo
        bool ExportToExcel(string reportType, DateTime startDate, DateTime endDate, string filePath);
        bool ExportToPdf(string reportType, DateTime startDate, DateTime endDate, string filePath);
        /// <summary>
        /// Lấy báo cáo doanh số theo khoảng thời gian
        /// </summary>
        SalesReportDTO GetSalesReport(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy báo cáo lợi nhuận theo khoảng thời gian
        /// </summary>
        ProfitReportDTO GetProfitReport(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy báo cáo hiệu suất sản phẩm theo khoảng thời gian
        /// </summary>
        ProductPerformanceReportDTO GetProductPerformanceReport(DateTime startDate, DateTime endDate, int topCount = 20);

        /// <summary>
        /// Lấy báo cáo tồn kho hiện tại
        /// </summary>
        InventoryReportDTO GetInventoryReport();

        /// <summary>
        /// Lấy thông tin tổng quan cho dashboard
        /// </summary>
        DashboardSummaryDTO GetDashboardSummary();


        /// <summary>
        /// Xóa cache báo cáo
        /// </summary>
        void ClearReportCache();
    }
}