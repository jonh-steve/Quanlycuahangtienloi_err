// File: Forms/Reports/DashboardForm.cs
using System;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using System.Drawing;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;
using System.Linq;

namespace QuanLyCuaHangTienLoi.Forms.Reports
{
    public partial class DashboardForm : Form
    {
        private readonly IReportService _reportService;
        private Timer _refreshTimer;

        public DashboardForm(IReportService reportService)
        {
            InitializeComponent();
            _reportService = reportService;

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 242, 243);
            this.Font = new Font("Segoe UI", 9F);

            // Khởi tạo timer để tự động làm mới (mỗi 5 phút)
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 5 * 60 * 1000; // 5 phút
            _refreshTimer.Tick += (sender, e) => LoadDashboard();
            _refreshTimer.Start();

            LoadDashboard();
        }

        private void LoadDashboard()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Lấy thông tin tổng quan
                var dashboardSummary = _reportService.GetDashboardSummary();

                // Hiển thị thông tin hôm nay
                lblTodaySales.Text = dashboardSummary.TodaySales.ToString("N0") + " VNĐ";
                lblTodayOrders.Text = dashboardSummary.TodayOrders.ToString();

                // Hiển thị so sánh với hôm qua
                decimal salesChange = dashboardSummary.YesterdaySales > 0
                    ? ((dashboardSummary.TodaySales - dashboardSummary.YesterdaySales) / dashboardSummary.YesterdaySales) * 100
                    : 0;

                lblSalesChange.Text = salesChange.ToString("0.0") + "%";
                lblSalesChange.ForeColor = salesChange >= 0 ? Color.Green : Color.Red;

                // Hiển thị thông tin tháng hiện tại
                lblMonthRevenue.Text = dashboardSummary.CurrentMonthRevenue.ToString("N0") + " VNĐ";
                lblMonthProfit.Text = dashboardSummary.CurrentMonthProfit.ToString("N0") + " VNĐ";

                // Hiển thị xu hướng
                lblRevenueTrend.Text = dashboardSummary.SalesTrend.ToString("0.0") + "%";
                lblRevenueTrend.ForeColor = dashboardSummary.SalesTrend >= 0 ? Color.Green : Color.Red;

                lblProfitTrend.Text = dashboardSummary.ProfitTrend.ToString("0.0") + "%";
                lblProfitTrend.ForeColor = dashboardSummary.ProfitTrend >= 0 ? Color.Green : Color.Red;

                // Hiển thị cảnh báo tồn kho
                lblLowStockCount.Text = dashboardSummary.LowStockProductCount.ToString();

                // Cập nhật biểu đồ doanh số 7 ngày qua
                UpdateSalesChart();

                // Cập nhật biểu đồ top sản phẩm
                UpdateTopProductsChart();

                // Cập nhật biểu đồ doanh số theo danh mục
                UpdateCategorySalesChart();

                // Cập nhật thời gian làm mới cuối cùng
                lblLastRefresh.Text = "Cập nhật lần cuối: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dashboard: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("DashboardForm.LoadDashboard", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UpdateSalesChart()
        {
            // Lấy báo cáo doanh số 7 ngày qua
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-6);
            var salesReport = _reportService.GetSalesReport(startDate, endDate);

            // Cấu hình biểu đồ doanh số
            chartSales.Series.Clear();
            chartSales.AxisX.Clear();
            chartSales.AxisY.Clear();

            var salesSeries = new LineSeries
            {
                Title = "Doanh số",
                Values = new ChartValues<decimal>(salesReport.DailySales.Select(x => x.TotalAmount)),
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 10,
                LineSmoothness = 0.3,
                Stroke = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(255, 105, 180)), // Màu hồng
                Fill = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(128, 255, 182, 193)) // Màu hồng nhạt
            };

            chartSales.Series.Add(salesSeries);

            chartSales.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Ngày",
                Labels = salesReport.DailySales.Select(x => x.Date.ToString("dd/MM")).ToList(),
                Separator = new LiveCharts.Wpf.Separator
                {
                    Step = 1
                }
            });

            chartSales.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Doanh số (VNĐ)",
                LabelFormatter = value => value.ToString("N0")
            });

            chartSales.LegendLocation = LiveCharts.LegendLocation.Top;
        }

        private void UpdateTopProductsChart()
        {
            // Lấy báo cáo hiệu suất sản phẩm tháng hiện tại
            var endDate = DateTime.Now;
            var startDate = new DateTime(endDate.Year, endDate.Month, 1);
            var productReport = _reportService.GetProductPerformanceReport(startDate, endDate, 5);

            // Cấu hình biểu đồ top sản phẩm
            pieChartProducts.Series.Clear();

            foreach (var product in productReport.TopSellingProducts)
            {
                pieChartProducts.Series.Add(new PieSeries
                {
                    Title = product.ProductName,
                    Values = new ChartValues<decimal> { product.TotalAmount },
                    DataLabels = true,
                    LabelPoint = point => product.ProductName + ": " + point.Y.ToString("N0") + " VNĐ"
                });
            }

            pieChartProducts.LegendLocation = LiveCharts.LegendLocation.Bottom;
        }

        private void UpdateCategorySalesChart()
        {
            // Lấy báo cáo doanh số theo danh mục tháng hiện tại
            var endDate = DateTime.Now;
            var startDate = new DateTime(endDate.Year, endDate.Month, 1);
            var categoryReport = _reportService.GetCategorySalesReport(startDate, endDate);

            // Cấu hình biểu đồ doanh số theo danh mục
            chartCategories.Series.Clear();
            chartCategories.AxisX.Clear();
            chartCategories.AxisY.Clear();

            var categories = categoryReport.CategorySales.OrderByDescending(c => c.TotalSales).Take(5).ToList();

            var categorySeries = new ColumnSeries
            {
                Title = "Doanh số",
                Values = new ChartValues<decimal>(categories.Select(x => x.TotalSales)),
                Fill = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(255, 182, 193)) // Pink
            };

            chartCategories.Series.Add(categorySeries);

            chartCategories.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Danh mục",
                Labels = categories.Select(x => x.CategoryName).ToList(),
                LabelsRotation = 45
            });

            chartCategories.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Doanh số (VNĐ)",
                LabelFormatter = value => value.ToString("N0")
            });

            chartCategories.LegendLocation = LiveCharts.LegendLocation.None;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void btnViewSalesReport_Click(object sender, EventArgs e)
        {
            try
            {
                var salesReportForm = new SalesReportForm(_reportService);
                salesReportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở báo cáo doanh số: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("DashboardForm.ViewSalesReport", ex);
            }
        }

        private void btnViewInventoryReport_Click(object sender, EventArgs e)
        {
            try
            {
                var inventoryReportForm = new InventoryReportForm(_reportService);
                inventoryReportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở báo cáo tồn kho: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("DashboardForm.ViewInventoryReport", ex);
            }
        }

        private void btnViewLowStock_Click(object sender, EventArgs e)
        {
            try
            {
                var inventoryReport = _reportService.GetInventoryReport();
                if (inventoryReport.LowStockProducts.Count == 0)
                {
                    MessageBox.Show("Không có sản phẩm nào sắp hết hàng.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var lowStockForm = new LowStockProductsForm(inventoryReport.LowStockProducts);
                lowStockForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở danh sách sản phẩm sắp hết hàng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("DashboardForm.ViewLowStock", ex);
            }
        }

        private void DashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dừng timer khi đóng form
            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer.Dispose();
            }
        }

        // Designer code...
    }
}