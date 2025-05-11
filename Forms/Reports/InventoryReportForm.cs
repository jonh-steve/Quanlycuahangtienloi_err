// File: Forms/Reports/InventoryReportForm.cs
using System;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using System.Drawing;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyCuaHangTienLoi.Forms.Reports
{
    public partial class InventoryReportForm : Form
    {
        private readonly IReportService _reportService;
        private readonly ReportExporter _reportExporter;
        private InventoryReportDTO _inventoryReport;

        public InventoryReportForm(IReportService reportService)
        {
            InitializeComponent();
            _reportService = reportService;
            _reportExporter = new ReportExporter();

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 242, 243);
            this.Font = new Font("Segoe UI", 9F);

            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += TabControl1_DrawItem;

            LoadInventoryReport();
        }

        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Tùy chỉnh màu sắc tab
            var tabPage = this.tabControl1.TabPages[e.Index];
            var tabBounds = this.tabControl1.GetTabRect(e.Index);

            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            // Màu nền tab được chọn
            if (this.tabControl1.SelectedIndex == e.Index)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 192, 203)), e.Bounds); // Pink
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 228, 225)), e.Bounds); // MistyRose
            }

            // Màu văn bản trong tab
            e.Graphics.DrawString(tabPage.Text, e.Font, Brushes.Black, tabBounds, sf);
        }

        private void LoadInventoryReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Lấy báo cáo tồn kho
                _inventoryReport = _reportService.GetInventoryReport();

                // Hiển thị tổng quan
                lblTotalProducts.Text = _inventoryReport.TotalProducts.ToString();
                lblTotalValue.Text = _inventoryReport.TotalInventoryValue.ToString("N0") + " VNĐ";
                lblLowStockCount.Text = _inventoryReport.LowStockProducts.Count.ToString();

                // Hiển thị danh sách tồn kho
                dgvInventory.DataSource = _inventoryReport.InventoryDetails;

                // Hiển thị danh sách sản phẩm sắp hết hàng
                dgvLowStock.DataSource = _inventoryReport.LowStockProducts;

                // Cập nhật biểu đồ tồn kho theo danh mục
                UpdateCategoryChart();

                // Cập nhật biểu đồ giá trị tồn kho
                UpdateInventoryValueChart();

                // Highlight các sản phẩm sắp hết hàng
                HighlightLowStockProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo tồn kho: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("InventoryReportForm.LoadInventoryReport", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UpdateCategoryChart()
        {
            // Cấu hình biểu đồ tồn kho theo danh mục
            pieChart1.Series.Clear();

            foreach (var category in _inventoryReport.InventoryByCategory.OrderByDescending(c => c.TotalValue))
            {
                pieChart1.Series.Add(new PieSeries
                {
                    Title = category.CategoryName,
                    Values = new ChartValues<decimal> { category.TotalValue },
                    DataLabels = true,
                    LabelPoint = point => category.CategoryName + ": " + point.Y.ToString("N0") + " VNĐ"
                });
            }

            pieChart1.LegendLocation = LiveCharts.LegendLocation.Bottom;
        }

        private void UpdateInventoryValueChart()
        {
            // Cấu hình biểu đồ phân tích giá trị tồn kho
            cartesianChart1.Series.Clear();
            cartesianChart1.AxisX.Clear();
            cartesianChart1.AxisY.Clear();

            // Lấy top 10 sản phẩm có giá trị tồn kho cao nhất
            var topInventoryItems = _inventoryReport.InventoryDetails
                .OrderByDescending(i => i.TotalValue)
                .Take(10)
                .ToList();

            var series = new ColumnSeries
            {
                Title = "Giá trị tồn kho",
                Values = new ChartValues<decimal>(topInventoryItems.Select(i => i.TotalValue)),
                Fill = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(255, 182, 193)) // Pink
            };

            cartesianChart1.Series.Add(series);

            cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Sản phẩm",
                Labels = topInventoryItems.Select(i => i.ProductName).ToList(),
                LabelsRotation = 45
            });

            cartesianChart1.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Giá trị (VNĐ)",
                LabelFormatter = value => value.ToString("N0")
            });

            cartesianChart1.LegendLocation = LiveCharts.LegendLocation.Top;
        }

        private void HighlightLowStockProducts()
        {
            // Làm nổi bật các sản phẩm sắp hết hàng trong lưới tồn kho
            foreach (DataGridViewRow row in dgvInventory.Rows)
            {
                int productId = Convert.ToInt32(row.Cells["ProductID"].Value);

                if (_inventoryReport.LowStockProducts.Any(p => p.ProductID == productId))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200); // Màu hồng nhạt cảnh báo
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                string filePath = _reportExporter.ExportInventoryReportToExcel(_inventoryReport);

                if (MessageBox.Show("Xuất báo cáo thành công! Bạn có muốn mở file không?", "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất báo cáo: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("InventoryReportForm.ExportExcel", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                string filePath = _reportExporter.ExportInventoryReportToPdf(_inventoryReport);

                if (MessageBox.Show("Xuất báo cáo thành công! Bạn có muốn mở file không?", "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất báo cáo: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("InventoryReportForm.ExportPdf", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInventoryReport();
        }

        private void btnPrintLowStock_Click(object sender, EventArgs e)
        {
            try
            {
                if (_inventoryReport.LowStockProducts.Count == 0)
                {
                    MessageBox.Show("Không có sản phẩm nào sắp hết hàng.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string filePath = _reportExporter.ExportLowStockReportToPdf(_inventoryReport.LowStockProducts);

                if (MessageBox.Show("Xuất báo cáo sản phẩm sắp hết hàng thành công! Bạn có muốn mở file không?",
                    "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất báo cáo: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("InventoryReportForm.PrintLowStock", ex);
            }
        }

        private void btnAnalyzeInventoryTurnover_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Hiển thị form phân tích tốc độ luân chuyển hàng hóa
                var inventoryTurnoverForm = new InventoryTurnoverAnalysisForm(_reportService);
                inventoryTurnoverForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở form phân tích: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("InventoryReportForm.AnalyzeTurnover", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Designer code...
    }
}