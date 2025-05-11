using System;
using System.Collections.Generic;
using System.Data;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public interface IReportRepository
    {
        // Báo cáo doanh số
        DataSet GetSalesReport(DateTime startDate, DateTime endDate, string groupBy = "Day");

        // Báo cáo sản phẩm
        DataTable GetTopSellingProducts(DateTime startDate, DateTime endDate, int topCount = 10);

        // Báo cáo danh mục
        DataTable GetCategorySalesReport(DateTime startDate, DateTime endDate);

        // Báo cáo phương thức thanh toán
        DataTable GetPaymentMethodReport(DateTime startDate, DateTime endDate);

        // Báo cáo tồn kho
        DataTable GetInventoryValueReport();
        DataTable GetLowStockReport();

        // Báo cáo nhân viên
        DataTable GetEmployeeSalesPerformance(DateTime startDate, DateTime endDate);

        // Báo cáo doanh thu và lợi nhuận
        decimal GetTotalRevenue(DateTime startDate, DateTime endDate);
        decimal GetTotalProfit(DateTime startDate, DateTime endDate);
        decimal GetTotalCost(DateTime startDate, DateTime endDate);
    }
}