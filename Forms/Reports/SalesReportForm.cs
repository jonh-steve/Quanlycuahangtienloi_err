// Steve-Thuong_hai
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.WinForms;

namespace QuanLyCuaHangTienLoi.Forms.Reports
{
    public partial class SalesReportForm : Form
    {
        private readonly ReportService _reportService;
        private DateTime _startDate;
        private DateTime _endDate;

        // Colors for pink theme
        private readonly Color _primaryColor = Color.FromArgb(255, 192, 203); // Pink
        private readonly Color _secondaryColor = Color.FromArgb(255, 182, 193); // LightPink
        private readonly Color _accentColor = Color.FromArgb(219, 112, 147); // PaleVioletRed
        private readonly Color _textColor = Color.FromArgb(60, 60, 60); // Dark gray for text

        public SalesReportForm(ReportService reportService)
        {
            InitializeComponent();
            _reportService = reportService;

            // Mặc định là báo cáo tháng hiện tại
            _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _endDate = _startDate.AddMonths(1).AddDays(-1);

            // Áp dụng theme màu hồng
            ApplyPinkTheme();

            // Thiết lập DateTimePicker
            dtpStartDate.Value = _startDate;
            dtpEndDate.Value = _endDate;

            // Load dữ liệu ban đầu
            LoadReport();
        }

        private void ApplyPinkTheme()
        {
            // Set form properties
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.Text = "Báo cáo doanh số bán hàng";

            // Set panel properties
            panelTop.BackColor = _primaryColor;

            // Set label properties
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);

            lblStartDate.ForeColor = _textColor;
            lblStartDate.Font = new Font("Segoe UI", 9.5F);

            lblEndDate.ForeColor = _textColor;
            lblEndDate.Font = new Font("Segoe UI", 9.5F);

            // Set DateTimePicker
            dtpStartDate.Font = new Font("Segoe UI", 9.5F);
            dtpEndDate.Font = new Font("Segoe UI", 9.5F);

            // Set buttons
            btnGenerateReport.BackColor = _accentColor;
            btnGenerateReport.ForeColor = Color.White;
            btnGenerateReport.FlatStyle = FlatStyle.Flat;
            btnGenerateReport.FlatAppearance.BorderSize = 0;
            btnGenerateReport.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnGenerateReport.Cursor = Cursors.Hand;

            btnExport.BackColor = _accentColor;
            btnExport.ForeColor = Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnExport.Cursor = Cursors.Hand;

            btnClose.BackColor = Color.Gray;
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;

            // Set tab control
            tabControl.Font = new Font("Segoe UI", 9.5F);

            // Set DataGridViews
            dgvDailySales.BackgroundColor = Color.White;
            dgvDailySales.BorderStyle = BorderStyle.None;
            dgvDailySales.RowHeadersVisible = false;
            dgvDailySales.EnableHeadersVisualStyles = false;
            dgvDailySales.ColumnHeadersDefaultCellStyle.BackColor = _secondaryColor;
            dgvDailySales.ColumnHeadersDefaultCellStyle.ForeColor = _textColor;
            dgvDailySales.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvDailySales.ColumnHeadersHeight = 40;
            dgvDailySales.RowsDefaultCellStyle.BackColor = Color.White;
            dgvDailySales.RowsDefaultCellStyle.ForeColor = _textColor;
            dgvDailySales.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvDailySales.RowTemplate.Height = 35;

            dgvProductSales.BackgroundColor = Color.White;
            dgvProductSales.BorderStyle = BorderStyle.None;
            dgvProductSales.RowHeadersVisible = false;
            dgvProductSales.EnableHeadersVisualStyles = false;
            dgvProductSales.ColumnHeadersDefaultCellStyle.BackColor = _secondaryColor;
            dgvProductSales.ColumnHeadersDefaultCellStyle.ForeColor = _textColor;
            dgvProductSales.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvProductSales.ColumnHeadersHeight = 40;
            dgvProductSales.RowsDefaultCellStyle.BackColor = Color.White;
            dgvProductSales.RowsDefaultCellStyle.ForeColor = _textColor;
            dgvProductSales.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvProductSales.RowTemplate.Height = 35;

            dgvCategorySales.BackgroundColor = Color.White;
            dgvCategorySales.BorderStyle = BorderStyle.None;
            dgvCategorySales.RowHeadersVisible = false;
            dgvCategorySales.EnableHeadersVisualStyles = false;
            dgvCategorySales.ColumnHeadersDefaultCellStyle.BackColor = _secondaryColor;
            dgvCategorySales.ColumnHeadersDefaultCellStyle.ForeColor = _textColor;
            dgvCategorySales.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvCategorySales.ColumnHeadersHeight = 40;
            dgvCategorySales.RowsDefaultCellStyle.BackColor = Color.White;
            dgvCategorySales.RowsDefaultCellStyle.ForeColor = _textColor;
            dgvCategorySales.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvCategorySales.RowTemplate.Height = 35;

            // Set label headers in summary tab
            foreach (Control control in tabPageSummary.Controls)
            {
                if (control is Label label)
                {
                    if (label.Name.StartsWith("lblHeader"))
                    {
                        label.ForeColor = _accentColor;
                        label.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                    }
                    else if (label.Name.StartsWith("lblInfo"))
                    {
                        label.ForeColor = _textColor;
                        label.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                    }
                    else if (label.Name.StartsWith("lblValue"))
                    {
                        label.ForeColor = _textColor;
                        label.Font = new Font("Segoe UI", 10F);
                    }
                }
            }
        }

        private void LoadReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Lấy dữ liệu báo cáo từ service
                var salesReport = _reportService.GetSalesReport(_startDate, _endDate);

                if (salesReport == null || !salesReport.Any())
                {
                    MessageBox.Show("Không có dữ liệu doanh số trong khoảng thời gian này", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Hiển thị tổng quan
                DisplaySummary(salesReport);

                // Hiển thị báo cáo doanh số theo ngày
                DisplayDailySalesReport(salesReport);

                // Hiển thị báo cáo doanh số theo sản phẩm
                var productSalesReport = _reportService.GetProductSalesReport(_startDate, _endDate);
                DisplayProductSalesReport(productSalesReport);

                // Hiển thị báo cáo doanh số theo danh mục
                var categorySalesReport = _reportService.GetCategorySalesReport(_startDate, _endDate);
                DisplayCategorySalesReport(categorySalesReport);

                // Tạo biểu đồ
                CreateCharts(salesReport, productSalesReport, categorySalesReport);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void DisplaySummary(List<DailySalesReportDTO> salesReport)
        {
            // Tính tổng số đơn hàng
            int totalOrders = salesReport.Sum(s => s.OrderCount);
            lblValueTotalOrders.Text = totalOrders.ToString("N0");

            // Tổng doanh thu
            decimal totalSales = salesReport.Sum(s => s.TotalSales);
            lblValueTotalSales.Text = totalSales.ToString("N0") + " VNĐ";

            // Tổng thuế
            decimal totalTax = salesReport.Sum(s => s.TotalTax);
            lblValueTotalTax.Text = totalTax.ToString("N0") + " VNĐ";

            // Doanh thu thuần
            decimal netSales = salesReport.Sum(s => s.NetSales);
            lblValueNetSales.Text = netSales.ToString("N0") + " VNĐ";

            // Doanh thu trung bình mỗi ngày
            decimal avgDailySales = salesReport.Count > 0 ? totalSales / salesReport.Count : 0;
            lblValueAvgDailySales.Text = avgDailySales.ToString("N0") + " VNĐ";

            // Ngày có doanh thu cao nhất
            var maxSalesDay = salesReport.OrderByDescending(s => s.TotalSales).FirstOrDefault();
            if (maxSalesDay != null)
            {
                lblValueMaxSalesDay.Text = maxSalesDay.SalesDate.ToString("dd/MM/yyyy")
                    + " (" + maxSalesDay.TotalSales.ToString("N0") + " VNĐ)";
            }

            // Ngày có doanh thu thấp nhất
            var minSalesDay = salesReport.OrderBy(s => s.TotalSales).FirstOrDefault();
            if (minSalesDay != null)
            {
                lblValueMinSalesDay.Text = minSalesDay.SalesDate.ToString("dd/MM/yyyy")
                    + " (" + minSalesDay.TotalSales.ToString("N0") + " VNĐ)";
            }

            // Giá trị đơn hàng trung bình
            decimal avgOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;
            lblValueAvgOrderValue.Text = avgOrderValue.ToString("N0") + " VNĐ";
        }

        private void DisplayDailySalesReport(List<DailySalesReportDTO> salesReport)
        {
            // Set up DataGridView
            dgvDailySales.AutoGenerateColumns = false;
            dgvDailySales.Columns.Clear();

            dgvDailySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SalesDate",
                HeaderText = "Ngày",
                DataPropertyName = "SalesDate",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy"
                }
            });

            dgvDailySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OrderCount",
                HeaderText = "Số đơn hàng",
                DataPropertyName = "OrderCount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvDailySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalSales",
                HeaderText = "Doanh thu",
                DataPropertyName = "TotalSales",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvDailySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalTax",
                HeaderText = "Thuế",
                DataPropertyName = "TotalTax",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvDailySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NetSales",
                HeaderText = "Doanh thu thuần",
                DataPropertyName = "NetSales",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            // Set data source
            dgvDailySales.DataSource = salesReport;
        }

        private void DisplayProductSalesReport(List<ProductSalesReportDTO> productSalesReport)
        {
            // Set up DataGridView
            dgvProductSales.AutoGenerateColumns = false;
            dgvProductSales.Columns.Clear();

            dgvProductSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Rank",
                HeaderText = "Top",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvProductSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductCode",
                HeaderText = "Mã SP",
                DataPropertyName = "ProductCode",
                Width = 80
            });

            dgvProductSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "ProductName",
                Width = 200
            });

            dgvProductSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CategoryName",
                HeaderText = "Danh mục",
                DataPropertyName = "CategoryName",
                Width = 120
            });

            dgvProductSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuantitySold",
                HeaderText = "Số lượng",
                DataPropertyName = "QuantitySold",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvProductSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalSales",
                HeaderText = "Doanh thu",
                DataPropertyName = "TotalSales",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvProductSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Profit",
                HeaderText = "Lợi nhuận",
                DataPropertyName = "Profit",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            // Set data source
            dgvProductSales.DataSource = productSalesReport;

            // Set rank column
            dgvProductSales.CellFormatting += (sender, e) =>
            {
                if (e.ColumnIndex == dgvProductSales.Columns["Rank"].Index && e.RowIndex >= 0)
                {
                    e.Value = (e.RowIndex + 1).ToString();
                }
            };
        }

        private void DisplayCategorySalesReport(List<CategorySalesReportDTO> categorySalesReport)
        {
            // Set up DataGridView
            dgvCategorySales.AutoGenerateColumns = false;
            dgvCategorySales.Columns.Clear();

            dgvCategorySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Rank",
                HeaderText = "Top",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvCategorySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CategoryName",
                HeaderText = "Danh mục",
                DataPropertyName = "CategoryName",
                Width = 200
            });

            dgvCategorySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductCount",
                HeaderText = "SL sản phẩm",
                DataPropertyName = "ProductCount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCategorySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuantitySold",
                HeaderText = "SL bán",
                DataPropertyName = "QuantitySold",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCategorySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalSales",
                HeaderText = "Doanh thu",
                DataPropertyName = "TotalSales",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCategorySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Profit",
                HeaderText = "Lợi nhuận",
                DataPropertyName = "Profit",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCategorySales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Percentage",
                HeaderText = "Tỷ lệ DT",
                DataPropertyName = "Percentage",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "P1",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            // Set data source
            dgvCategorySales.DataSource = categorySalesReport;

            // Set rank column
            dgvCategorySales.CellFormatting += (sender, e) =>
            {
                if (e.ColumnIndex == dgvCategorySales.Columns["Rank"].Index && e.RowIndex >= 0)
                {
                    e.Value = (e.RowIndex + 1).ToString();
                }
            };
        }

        private void CreateCharts(List<DailySalesReportDTO> salesReport,
            List<ProductSalesReportDTO> productSalesReport,
            List<CategorySalesReportDTO> categorySalesReport)
        {
            // Biểu đồ doanh số theo ngày
            CreateDailySalesChart(salesReport);

            // Biểu đồ top sản phẩm bán chạy
            CreateTopProductsChart(productSalesReport);

            // Biểu đồ tỷ lệ doanh thu theo danh mục
            CreateCategorySalesChart(categorySalesReport);
        }

        private void CreateDailySalesChart(List<DailySalesReportDTO> salesReport)
        {
            // Tạo series cho biểu đồ
            chartDailySales.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<decimal>(salesReport.Select(s => s.TotalSales)),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8,
                    Stroke = System.Windows.Media.Brushes.DeepPink
                },
                new LineSeries
                {
                    Title = "Số đơn hàng",
                    Values = new ChartValues<int>(salesReport.Select(s => s.OrderCount)),
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 8,
                    Stroke = System.Windows.Media.Brushes.Blue
                }
            };

            // Set labels cho trục X
            chartDailySales.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Ngày",
                Labels = salesReport.Select(s => s.SalesDate.ToString("dd/MM")).ToList()
            });

            // Set title cho trục Y
            chartDailySales.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Doanh thu",
                LabelFormatter = value => value.ToString("N0")
            });

            // Thêm trục Y phụ cho số đơn hàng
            chartDailySales.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Số đơn hàng",
                Position = LiveCharts.Wpf.AxisPosition.RightTop,
                LabelFormatter = value => value.ToString("N0")
            });

            // Show legend
            chartDailySales.LegendLocation = LegendLocation.Top;
        }

        private void CreateTopProductsChart(List<ProductSalesReportDTO> productSalesReport)
        {
            // Lấy top 10 sản phẩm
            var top10Products = productSalesReport.Take(10).ToList();

            // Tạo series cho biểu đồ
            chartTopProducts.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<decimal>(top10Products.Select(p => p.TotalSales)),
                    Fill = System.Windows.Media.Brushes.LightPink
                }
            };

            // Set labels cho trục X
            chartTopProducts.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Sản phẩm",
                Labels = top10Products.Select(p => p.ProductName.Length > 15
                    ? p.ProductName.Substring(0, 15) + "..."
                    : p.ProductName).ToList()
            });

            // Set title cho trục Y
            chartTopProducts.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Doanh thu",
                LabelFormatter = value => value.ToString("N0")
            });

            // Show legend
            chartTopProducts.LegendLocation = LegendLocation.Top;
        }

        private void CreateCategorySalesChart(List<CategorySalesReportDTO> categorySalesReport)
        {
            // Tạo series cho biểu đồ
            pieChartCategories.Series = new SeriesCollection();

            // Thêm data cho từng danh mục
            foreach (var category in categorySalesReport)
            {
                pieChartCategories.Series.Add(new PieSeries
                {
                    Title = category.CategoryName,
                    Values = new ChartValues<decimal> { category.TotalSales },
                    DataLabels = true,
                    LabelPoint = point => $"{category.CategoryName}: {category.TotalSales:N0} VNĐ ({category.Percentage:P1})"
                });
            }

            // Show legend
            pieChartCategories.LegendLocation = LegendLocation.Bottom;
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin ngày từ DateTimePicker
                _startDate = dtpStartDate.Value.Date;
                _endDate = dtpEndDate.Value.Date;

                // Kiểm tra thông tin ngày
                if (_startDate > _endDate)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tải lại báo cáo
                LoadReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|PDF Files (*.pdf)|*.pdf";
                saveFileDialog.Title = "Xuất báo cáo doanh số";
                saveFileDialog.FileName = $"BaoCaoDoanhSo_{_startDate:yyyyMMdd}_{_endDate:yyyyMMdd}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                    if (fileExtension == ".xlsx")
                    {
                        _reportService.ExportSalesReportToExcel(filePath, _startDate, _endDate);
                        MessageBox.Show("Xuất báo cáo Excel thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (fileExtension == ".pdf")
                    {
                        _reportService.ExportSalesReportToPdf(filePath, _startDate, _endDate);
                        MessageBox.Show("Xuất báo cáo PDF thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Designer components init
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageSummary = new System.Windows.Forms.TabPage();
            this.tabPageDailySales = new System.Windows.Forms.TabPage();
            this.tabPageProductSales = new System.Windows.Forms.TabPage();
            this.tabPageCategorySales = new System.Windows.Forms.TabPage();
            this.tabPageCharts = new System.Windows.Forms.TabPage();

            // Summary tab components
            this.lblHeaderSummary = new System.Windows.Forms.Label();
            this.lblInfoTotalOrders = new System.Windows.Forms.Label();
            this.lblValueTotalOrders = new System.Windows.Forms.Label();
            this.lblInfoTotalSales = new System.Windows.Forms.Label();
            this.lblValueTotalSales = new System.Windows.Forms.Label();
            this.lblInfoTotalTax = new System.Windows.Forms.Label();
            this.lblValueTotalTax = new System.Windows.Forms.Label();
            this.lblInfoNetSales = new System.Windows.Forms.Label();
            this.lblValueNetSales = new System.Windows.Forms.Label();
            this.lblInfoAvgDailySales = new System.Windows.Forms.Label();
            this.lblValueAvgDailySales = new System.Windows.Forms.Label();
            this.lblInfoMaxSalesDay = new System.Windows.Forms.Label();
            this.lblValueMaxSalesDay = new System.Windows.Forms.Label();
            this.lblInfoMinSalesDay = new System.Windows.Forms.Label();
            this.lblValueMinSalesDay = new System.Windows.Forms.Label();
            this.lblInfoAvgOrderValue = new System.Windows.Forms.Label();
            this.lblValueAvgOrderValue = new System.Windows.Forms.Label();

            // Daily sales tab components
            this.dgvDailySales = new System.Windows.Forms.DataGridView();

            // Product sales tab components
            this.dgvProductSales = new System.Windows.Forms.DataGridView();

            // Category sales tab components
            this.dgvCategorySales = new System.Windows.Forms.DataGridView();

            // Charts tab components
            this.chartDailySales = new LiveCharts.WinForms.CartesianChart();
            this.chartTopProducts = new LiveCharts.WinForms.CartesianChart();
            this.pieChartCategories = new LiveCharts.WinForms.PieChart();
            this.lblChartDailySales = new System.Windows.Forms.Label();
            this.lblChartTopProducts = new System.Windows.Forms.Label();
            this.lblChartCategories = new System.Windows.Forms.Label();

            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Size = new System.Drawing.Size(1000, 60);
            this.panelTop.Controls.Add(this.lblTitle);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Size = new System.Drawing.Size(300, 30);
            this.lblTitle.Text = "BÁO CÁO DOANH SỐ BÁN HÀNG";

            // panelFilter
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 60);
            this.panelFilter.Size = new System.Drawing.Size(1000, 60);
            this.panelFilter.Controls.Add(this.lblStartDate);
            this.panelFilter.Controls.Add(this.dtpStartDate);
            this.panelFilter.Controls.Add(this.lblEndDate);
            this.panelFilter.Controls.Add(this.dtpEndDate);
            this.panelFilter.Controls.Add(this.btnGenerateReport);

            // lblStartDate
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(20, 20);
            this.lblStartDate.Size = new System.Drawing.Size(80, 20);
            this.lblStartDate.Text = "Từ ngày:";

            // dtpStartDate
            this.dtpStartDate.Location = new System.Drawing.Point(110, 20);
            this.dtpStartDate.Size = new System.Drawing.Size(150, 25);
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            // lblEndDate
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(280, 20);
            this.lblEndDate.Size = new System.Drawing.Size(80, 20);
            this.lblEndDate.Text = "Đến ngày:";

            // dtpEndDate
            this.dtpEndDate.Location = new System.Drawing.Point(370, 20);
            this.dtpEndDate.Size = new System.Drawing.Size(150, 25);
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            // btnGenerateReport
            this.btnGenerateReport.Location = new System.Drawing.Point(540, 20);
            this.btnGenerateReport.Size = new System.Drawing.Size(150, 25);
            this.btnGenerateReport.Text = "Tạo báo cáo";
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);

            // tabControl
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 120);
            this.tabControl.Size = new System.Drawing.Size(1000, 480);
            this.tabControl.Controls.Add(this.tabPageSummary);
            this.tabControl.Controls.Add(this.tabPageDailySales);
            this.tabControl.Controls.Add(this.tabPageProductSales);
            this.tabControl.Controls.Add(this.tabPageCategorySales);
            this.tabControl.Controls.Add(this.tabPageCharts);

            // tabPageSummary
            this.tabPageSummary.Text = "Tổng quan";
            this.tabPageSummary.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageSummary.Controls.Add(this.lblHeaderSummary);
            this.tabPageSummary.Controls.Add(this.lblInfoTotalOrders);
            this.tabPageSummary.Controls.Add(this.lblValueTotalOrders);
            this.tabPageSummary.Controls.Add(this.lblInfoTotalSales);
            this.tabPageSummary.Controls.Add(this.lblValueTotalSales);
            this.tabPageSummary.Controls.Add(this.lblInfoTotalTax);
            this.tabPageSummary.Controls.Add(this.lblValueTotalTax);
            this.tabPageSummary.Controls.Add(this.lblInfoNetSales);
            this.tabPageSummary.Controls.Add(this.lblValueNetSales);
            this.tabPageSummary.Controls.Add(this.lblInfoAvgDailySales);
            this.tabPageSummary.Controls.Add(this.lblValueAvgDailySales);
            this.tabPageSummary.Controls.Add(this.lblInfoMaxSalesDay);
            this.tabPageSummary.Controls.Add(this.lblValueMaxSalesDay);
            this.tabPageSummary.Controls.Add(this.lblInfoMinSalesDay);
            this.tabPageSummary.Controls.Add(this.lblValueMinSalesDay);
            this.tabPageSummary.Controls.Add(this.lblInfoAvgOrderValue);
            this.tabPageSummary.Controls.Add(this.lblValueAvgOrderValue);

            // lblHeaderSummary
            this.lblHeaderSummary.AutoSize = true;
            this.lblHeaderSummary.Location = new System.Drawing.Point(20, 20);
            this.lblHeaderSummary.Size = new System.Drawing.Size(200, 25);
            this.lblHeaderSummary.Text = "Thông tin tổng quan doanh số";

            // lblInfoTotalOrders
            this.lblInfoTotalOrders.AutoSize = true;
            this.lblInfoTotalOrders.Location = new System.Drawing.Point(40, 60);
            this.lblInfoTotalOrders.Size = new System.Drawing.Size(150, 20);
            this.lblInfoTotalOrders.Text = "Tổng số đơn hàng:";

            // lblValueTotalOrders
            this.lblValueTotalOrders.AutoSize = true;
            this.lblValueTotalOrders.Location = new System.Drawing.Point(200, 60);
            this.lblValueTotalOrders.Size = new System.Drawing.Size(80, 20);
            this.lblValueTotalOrders.Text = "0";

            // lblInfoTotalSales
            this.lblInfoTotalSales.AutoSize = true;
            this.lblInfoTotalSales.Location = new System.Drawing.Point(40, 90);
            this.lblInfoTotalSales.Size = new System.Drawing.Size(150, 20);
            this.lblInfoTotalSales.Text = "Tổng doanh thu:";

            // lblValueTotalSales
            this.lblValueTotalSales.AutoSize = true;
            this.lblValueTotalSales.Location = new System.Drawing.Point(200, 90);
            this.lblValueTotalSales.Size = new System.Drawing.Size(80, 20);
            this.lblValueTotalSales.Text = "0 VNĐ";

            // lblInfoTotalTax
            this.lblInfoTotalTax.AutoSize = true;
            this.lblInfoTotalTax.Location = new System.Drawing.Point(40, 120);
            this.lblInfoTotalTax.Size = new System.Drawing.Size(150, 20);
            this.lblInfoTotalTax.Text = "Tổng thuế:";

            // lblValueTotalTax
            this.lblValueTotalTax.AutoSize = true;
            this.lblValueTotalTax.Location = new System.Drawing.Point(200, 120);
            this.lblValueTotalTax.Size = new System.Drawing.Size(80, 20);
            this.lblValueTotalTax.Text = "0 VNĐ";

            // lblInfoNetSales
            this.lblInfoNetSales.AutoSize = true;
            this.lblInfoNetSales.Location = new System.Drawing.Point(40, 150);
            this.lblInfoNetSales.Size = new System.Drawing.Size(150, 20);
            this.lblInfoNetSales.Text = "Doanh thu thuần:";

            // lblValueNetSales
            this.lblValueNetSales.AutoSize = true;
            this.lblValueNetSales.Location = new System.Drawing.Point(200, 150);
            this.lblValueNetSales.Size = new System.Drawing.Size(80, 20);
            this.lblValueNetSales.Text = "0 VNĐ";

            // lblInfoAvgDailySales
            this.lblInfoAvgDailySales.AutoSize = true;
            this.lblInfoAvgDailySales.Location = new System.Drawing.Point(40, 180);
            this.lblInfoAvgDailySales.Size = new System.Drawing.Size(150, 20);
            this.lblInfoAvgDailySales.Text = "Doanh thu TB/ngày:";

            // lblValueAvgDailySales
            this.lblValueAvgDailySales.AutoSize = true;
            this.lblValueAvgDailySales.Location = new System.Drawing.Point(200, 180);
            this.lblValueAvgDailySales.Size = new System.Drawing.Size(80, 20);
            this.lblValueAvgDailySales.Text = "0 VNĐ";

            // Cột 2

            // lblInfoMaxSalesDay
            this.lblInfoMaxSalesDay.AutoSize = true;
            this.lblInfoMaxSalesDay.Location = new System.Drawing.Point(400, 60);
            this.lblInfoMaxSalesDay.Size = new System.Drawing.Size(180, 20);
            this.lblInfoMaxSalesDay.Text = "Ngày doanh thu cao nhất:";

            // lblValueMaxSalesDay
            this.lblValueMaxSalesDay.AutoSize = true;
            this.lblValueMaxSalesDay.Location = new System.Drawing.Point(590, 60);
            this.lblValueMaxSalesDay.Size = new System.Drawing.Size(200, 20);
            this.lblValueMaxSalesDay.Text = "N/A";

            // lblInfoMinSalesDay
            this.lblInfoMinSalesDay.AutoSize = true;
            this.lblInfoMinSalesDay.Location = new System.Drawing.Point(400, 90);
            this.lblInfoMinSalesDay.Size = new System.Drawing.Size(180, 20);
            this.lblInfoMinSalesDay.Text = "Ngày doanh thu thấp nhất:";

            // lblValueMinSalesDay
            this.lblValueMinSalesDay.AutoSize = true;
            this.lblValueMinSalesDay.Location = new System.Drawing.Point(590, 90);
            this.lblValueMinSalesDay.Size = new System.Drawing.Size(200, 20);
            this.lblValueMinSalesDay.Text = "N/A";

            // lblInfoAvgOrderValue
            this.lblInfoAvgOrderValue.AutoSize = true;
            this.lblInfoAvgOrderValue.Location = new System.Drawing.Point(400, 120);
            this.lblInfoAvgOrderValue.Size = new System.Drawing.Size(180, 20);
            this.lblInfoAvgOrderValue.Text = "Giá trị đơn hàng trung bình:";

            // lblValueAvgOrderValue
            this.lblValueAvgOrderValue.AutoSize = true;
            this.lblValueAvgOrderValue.Location = new System.Drawing.Point(590, 120);
            this.lblValueAvgOrderValue.Size = new System.Drawing.Size(80, 20);
            this.lblValueAvgOrderValue.Text = "0 VNĐ";

            // tabPageDailySales
            this.tabPageDailySales.Text = "Doanh số theo ngày";
            this.tabPageDailySales.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageDailySales.Controls.Add(this.dgvDailySales);

            // dgvDailySales
            this.dgvDailySales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDailySales.Location = new System.Drawing.Point(10, 10);
            this.dgvDailySales.Size = new System.Drawing.Size(980, 460);
            this.dgvDailySales.ReadOnly = true;
            this.dgvDailySales.MultiSelect = false;
            this.dgvDailySales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDailySales.AllowUserToAddRows = false;
            this.dgvDailySales.AllowUserToDeleteRows = false;
            this.dgvDailySales.AllowUserToResizeRows = false;

            // tabPageProductSales
            this.tabPageProductSales.Text = "Doanh số theo sản phẩm";
            this.tabPageProductSales.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageProductSales.Controls.Add(this.dgvProductSales);

            // dgvProductSales
            this.dgvProductSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductSales.Location = new System.Drawing.Point(10, 10);
            this.dgvProductSales.Size = new System.Drawing.Size(980, 460);
            this.dgvProductSales.ReadOnly = true;
            this.dgvProductSales.MultiSelect = false;
            this.dgvProductSales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductSales.AllowUserToAddRows = false;
            this.dgvProductSales.AllowUserToDeleteRows = false;
            this.dgvProductSales.AllowUserToResizeRows = false;

            // tabPageCategorySales
            this.tabPageCategorySales.Text = "Doanh số theo danh mục";
            this.tabPageCategorySales.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageCategorySales.Controls.Add(this.dgvCategorySales);

            // dgvCategorySales
            this.dgvCategorySales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCategorySales.Location = new System.Drawing.Point(10, 10);
            this.dgvCategorySales.Size = new System.Drawing.Size(980, 460);
            this.dgvCategorySales.ReadOnly = true;
            this.dgvCategorySales.MultiSelect = false;
            this.dgvCategorySales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCategorySales.AllowUserToAddRows = false;
            this.dgvCategorySales.AllowUserToDeleteRows = false;
            this.dgvCategorySales.AllowUserToResizeRows = false;

            // tabPageCharts
            this.tabPageCharts.Text = "Biểu đồ";
            this.tabPageCharts.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageCharts.Controls.Add(this.lblChartDailySales);
            this.tabPageCharts.Controls.Add(this.chartDailySales);
            this.tabPageCharts.Controls.Add(this.lblChartTopProducts);
            this.tabPageCharts.Controls.Add(this.chartTopProducts);
            this.tabPageCharts.Controls.Add(this.lblChartCategories);
            this.tabPageCharts.Controls.Add(this.pieChartCategories);

            // lblChartDailySales
            this.lblChartDailySales.AutoSize = true;
            this.lblChartDailySales.Location = new System.Drawing.Point(10, 10);
            this.lblChartDailySales.Size = new System.Drawing.Size(250, 20);
            this.lblChartDailySales.Text = "Biểu đồ doanh số theo ngày";
            this.lblChartDailySales.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblChartDailySales.ForeColor = _accentColor;

            // chartDailySales
            this.chartDailySales.Location = new System.Drawing.Point(10, 40);
            this.chartDailySales.Size = new System.Drawing.Size(960, 160);
            this.chartDailySales.BackColor = Color.White;

            // lblChartTopProducts
            this.lblChartTopProducts.AutoSize = true;
            this.lblChartTopProducts.Location = new System.Drawing.Point(10, 210);
            this.lblChartTopProducts.Size = new System.Drawing.Size(250, 20);
            this.lblChartTopProducts.Text = "Top 10 sản phẩm bán chạy";
            this.lblChartTopProducts.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblChartTopProducts.ForeColor = _accentColor;

            // chartTopProducts
            this.chartTopProducts.Location = new System.Drawing.Point(10, 240);
            this.chartTopProducts.Size = new System.Drawing.Size(470, 200);
            this.chartTopProducts.BackColor = Color.White;

            // lblChartCategories
            this.lblChartCategories.AutoSize = true;
            this.lblChartCategories.Location = new System.Drawing.Point(500, 210);
            this.lblChartCategories.Size = new System.Drawing.Size(250, 20);
            this.lblChartCategories.Text = "Tỷ lệ doanh thu theo danh mục";
            this.lblChartCategories.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblChartCategories.ForeColor = _accentColor;

            // pieChartCategories
            this.pieChartCategories.Location = new System.Drawing.Point(500, 240);
            this.pieChartCategories.Size = new System.Drawing.Size(470, 200);
            this.pieChartCategories.BackColor = Color.White;

            // panelBottom
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 600);
            this.panelBottom.Size = new System.Drawing.Size(1000, 60);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnExport);

            // btnExport
            this.btnExport.Location = new System.Drawing.Point(780, 15);
            this.btnExport.Size = new System.Drawing.Size(100, 30);
            this.btnExport.Text = "Xuất báo cáo";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);

            // btnClose
            this.btnClose.Location = new System.Drawing.Point(890, 15);
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // SalesReportForm
            this.ClientSize = new System.Drawing.Size(1000, 660);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Báo cáo doanh số bán hàng";
        }

        private Panel panelTop;
        private Label lblTitle;
        private Panel panelFilter;
        private Label lblStartDate;
        private DateTimePicker dtpStartDate;
        private Label lblEndDate;
        private DateTimePicker dtpEndDate;
        private Button btnGenerateReport;
        private TabControl tabControl;
        private TabPage tabPageSummary;
        private TabPage tabPageDailySales;
        private TabPage tabPageProductSales;
        private TabPage tabPageCategorySales;
        private TabPage tabPageCharts;
        private Label lblHeaderSummary;
        private Label lblInfoTotalOrders;
        private Label lblValueTotalOrders;
        private Label lblInfoTotalSales;
        private Label lblValueTotalSales;
        private Label lblInfoTotalTax;
        private Label lblValueTotalTax;
        private Label lblInfoNetSales;
        private Label lblValueNetSales;
        private Label lblInfoAvgDailySales;
        private Label lblValueAvgDailySales;
        private Label lblInfoMaxSalesDay;
        private Label lblValueMaxSalesDay;
        private Label lblInfoMinSalesDay;
        private Label lblValueMinSalesDay;
        private Label lblInfoAvgOrderValue;
        private Label lblValueAvgOrderValue;
        private DataGridView dgvDailySales;
        private DataGridView dgvProductSales;
        private DataGridView dgvCategorySales;
        private CartesianChart chartDailySales;
        private CartesianChart chartTopProducts;
        private PieChart pieChartCategories;
        private Label lblChartDailySales;
        private Label lblChartTopProducts;
        private Label lblChartCategories;
        private Panel panelBottom;
        private Button btnClose;
        private Button btnExport;
    }
}