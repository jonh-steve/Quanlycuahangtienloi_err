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
    public partial class CustomerReportForm : Form
    {
        private readonly CustomerService _customerService;
        private readonly ReportService _reportService;
        private List<CustomerDTO> _customers;

        // Colors for pink theme
        private readonly Color _primaryColor = Color.FromArgb(255, 192, 203); // Pink
        private readonly Color _secondaryColor = Color.FromArgb(255, 182, 193); // LightPink
        private readonly Color _accentColor = Color.FromArgb(219, 112, 147); // PaleVioletRed
        private readonly Color _textColor = Color.FromArgb(60, 60, 60); // Dark gray for text

        public CustomerReportForm(CustomerService customerService, ReportService reportService)
        {
            InitializeComponent();
            _customerService = customerService;
            _reportService = reportService;

            // Áp dụng theme màu hồng
            ApplyPinkTheme();

            // Load dữ liệu
            LoadCustomers();

            // Tạo biểu đồ
            CreateCharts();
        }

        private void ApplyPinkTheme()
        {
            // Set form properties
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.Text = "Báo cáo khách hàng";

            // Set panel properties
            panelTop.BackColor = _primaryColor;

            // Set label properties
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);

            // Set tab control
            tabControl.Font = new Font("Segoe UI", 9.5F);

            // Set các thành phần trên tab
            foreach (Control control in tabPageOverview.Controls)
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
                        label.Font = new Font("Segoe UI", 9.5F);
                    }
                }
            }

            // Set buttons
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

            // Set DataGridView
            dgvTopCustomers.BackgroundColor = Color.White;
            dgvTopCustomers.BorderStyle = BorderStyle.None;
            dgvTopCustomers.RowHeadersVisible = false;
            dgvTopCustomers.EnableHeadersVisualStyles = false;
            dgvTopCustomers.ColumnHeadersDefaultCellStyle.BackColor = _secondaryColor;
            dgvTopCustomers.ColumnHeadersDefaultCellStyle.ForeColor = _textColor;
            dgvTopCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvTopCustomers.ColumnHeadersHeight = 40;
            dgvTopCustomers.RowsDefaultCellStyle.BackColor = Color.White;
            dgvTopCustomers.RowsDefaultCellStyle.ForeColor = _textColor;
            dgvTopCustomers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvTopCustomers.RowTemplate.Height = 35;
        }

        private void LoadCustomers()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Lấy danh sách khách hàng
                _customers = _customerService.GetAllCustomers();

                if (_customers.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu khách hàng", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Hiển thị thông tin tổng quan
                DisplayOverviewInfo();

                // Hiển thị top khách hàng
                DisplayTopCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void DisplayOverviewInfo()
        {
            // Tổng số khách hàng
            lblValueTotalCustomers.Text = _customers.Count.ToString("N0");

            // Tổng điểm tích lũy
            int totalPoints = _customers.Sum(c => c.Points);
            lblValueTotalPoints.Text = totalPoints.ToString("N0");

            // Tổng chi tiêu
            decimal totalSpent = _customers.Sum(c => c.TotalSpent);
            lblValueTotalSpent.Text = totalSpent.ToString("N0") + " VNĐ";

            // Khách hàng mới trong tháng
            int newCustomers = _customers
                .Count(c => c.CreatedDate.Month == DateTime.Now.Month && c.CreatedDate.Year == DateTime.Now.Year);
            lblValueNewCustomers.Text = newCustomers.ToString("N0");

            // Khách hàng active (có đơn hàng trong 3 tháng gần đây)
            DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);
            int activeCustomers = _customers
                .Count(c => c.LastOrderDate.HasValue && c.LastOrderDate.Value >= threeMonthsAgo);
            lblValueActiveCustomers.Text = activeCustomers.ToString("N0");

            // Khách hàng không active
            int inactiveCustomers = _customers.Count - activeCustomers;
            lblValueInactiveCustomers.Text = inactiveCustomers.ToString("N0");

            // Chi tiêu trung bình
            decimal avgSpent = _customers.Count > 0 ? totalSpent / _customers.Count : 0;
            lblValueAvgSpent.Text = avgSpent.ToString("N0") + " VNĐ";

            // Số khách hàng theo cấp độ
            int regularCustomers = _customers.Count(c => c.MembershipLevel == "Regular");
            int silverCustomers = _customers.Count(c => c.MembershipLevel == "Silver");
            int goldCustomers = _customers.Count(c => c.MembershipLevel == "Gold");
            int platinumCustomers = _customers.Count(c => c.MembershipLevel == "Platinum");

            lblValueRegularCustomers.Text = regularCustomers.ToString("N0");
            lblValueSilverCustomers.Text = silverCustomers.ToString("N0");
            lblValueGoldCustomers.Text = goldCustomers.ToString("N0");
            lblValuePlatinumCustomers.Text = platinumCustomers.ToString("N0");
        }

        private void DisplayTopCustomers()
        {
            // Set up DataGridView
            dgvTopCustomers.AutoGenerateColumns = false;
            dgvTopCustomers.Columns.Clear();

            dgvTopCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Rank",
                HeaderText = "Thứ hạng",
                Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvTopCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Tên khách hàng",
                DataPropertyName = "CustomerName",
                Width = 200
            });

            dgvTopCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PhoneNumber",
                HeaderText = "Số điện thoại",
                DataPropertyName = "PhoneNumber",
                Width = 120
            });

            dgvTopCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MembershipLevel",
                HeaderText = "Hạng thành viên",
                DataPropertyName = "MembershipLevel",
                Width = 120
            });

            dgvTopCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalSpent",
                HeaderText = "Tổng chi tiêu",
                DataPropertyName = "TotalSpent",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvTopCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OrderCount",
                HeaderText = "Số đơn hàng",
                DataPropertyName = "OrderCount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            // Lấy top 10 khách hàng có chi tiêu cao nhất
            var topCustomers = _customers
                .OrderByDescending(c => c.TotalSpent)
                .Take(10)
                .ToList();

            // Set data source
            dgvTopCustomers.DataSource = topCustomers;

            // Set rank column
            dgvTopCustomers.CellFormatting += (sender, e) =>
            {
                if (e.ColumnIndex == dgvTopCustomers.Columns["Rank"].Index && e.RowIndex >= 0)
                {
                    e.Value = (e.RowIndex + 1).ToString();
                }
            };
        }

        private void CreateCharts()
        {
            // Biểu đồ phân bố khách hàng theo cấp độ
            CreateMembershipChart();

            // Biểu đồ khách hàng theo hoạt động
            CreateActivityChart();
        }

        private void CreateMembershipChart()
        {
            // Đếm số lượng khách hàng theo cấp độ
            int regularCustomers = _customers.Count(c => c.MembershipLevel == "Regular");
            int silverCustomers = _customers.Count(c => c.MembershipLevel == "Silver");
            int goldCustomers = _customers.Count(c => c.MembershipLevel == "Gold");
            int platinumCustomers = _customers.Count(c => c.MembershipLevel == "Platinum");

            // Tạo series cho biểu đồ
            pieChartMembership.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Regular",
                    Values = new ChartValues<int> { regularCustomers },
                    DataLabels = true,
                    LabelPoint = point => $"Regular: {point.Y} ({point.Participation:P1})",
                    Fill = System.Windows.Media.Brushes.LightGray
                },
                new PieSeries
                {
                    Title = "Silver",
                    Values = new ChartValues<int> { silverCustomers },
                    DataLabels = true,
                    LabelPoint = point => $"Silver: {point.Y} ({point.Participation:P1})",
                    Fill = System.Windows.Media.Brushes.Silver
                },
                new PieSeries
                {
                    Title = "Gold",
                    Values = new ChartValues<int> { goldCustomers },
                    DataLabels = true,
                    LabelPoint = point => $"Gold: {point.Y} ({point.Participation:P1})",
                    Fill = System.Windows.Media.Brushes.Gold
                },
                new PieSeries
                {
                    Title = "Platinum",
                    Values = new ChartValues<int> { platinumCustomers },
                    DataLabels = true,
                    LabelPoint = point => $"Platinum: {point.Y} ({point.Participation:P1})",
                    Fill = System.Windows.Media.Brushes.DarkSlateBlue
                }
            };

            // Thiết lập legend
            pieChartMembership.LegendLocation = LegendLocation.Bottom;
        }

        private void CreateActivityChart()
        {
            // Định nghĩa thời điểm
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);

            // Phân loại khách hàng theo hoạt động
            int veryActiveCustomers = _customers
                .Count(c => c.LastOrderDate.HasValue && c.LastOrderDate.Value >= oneMonthAgo);

            int activeCustomers = _customers
                .Count(c => c.LastOrderDate.HasValue && c.LastOrderDate.Value >= threeMonthsAgo
                           && c.LastOrderDate.Value < oneMonthAgo);

            int inactiveCustomers = _customers
                .Count(c => c.LastOrderDate.HasValue && c.LastOrderDate.Value < threeMonthsAgo);

            int newCustomers = _customers
                .Count(c => !c.LastOrderDate.HasValue);

            // Tạo series cho biểu đồ
            pieChartActivity.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Rất tích cực (1 tháng)",
                    Values = new ChartValues<int> { veryActiveCustomers },
                    DataLabels = true,
                    LabelPoint = point => $"Rất tích cực: {point.Y} ({point.Participation:P1})",
                    Fill = System.Windows.Media.Brushes.Green
                },
                new PieSeries
                {
                    Title = "Tích cực (3 tháng)",
                    Values = new ChartValues<int> { activeCustomers },
                    DataLabels = true,
                    LabelPoint = point => $"Tích cực: {point.Y} ({point.Participation:P1})",
                    Fill = System.Windows.Media.Brushes.YellowGreen
                },
                new PieSeries
                {
                    Title = "Không tích cực",
                    Values = new ChartValues<int> { inactiveCustomers },
                    DataLabels = true,
                    LabelPoint = point => $"Không tích cực: {point.Y} ({point.Participation:P1})",
                    Fill = System.Windows.Media.Brushes.Orange
                },
                new PieSeries
                {
                    Title = "Mới (chưa mua hàng)",
                    Values = new ChartValues<int> { newCustomers },
                    DataLabels = true,
                    LabelPoint = point => $"Mới: {point.Y} ({point.Participation:P1})",
                    Fill = System.Windows.Media.Brushes.Blue
                }
            };

            // Thiết lập legend
            pieChartActivity.LegendLocation = LegendLocation.Bottom;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|PDF Files (*.pdf)|*.pdf";
                saveFileDialog.Title = "Xuất báo cáo khách hàng";
                saveFileDialog.FileName = "BaoCaoKhachHang_" + DateTime.Now.ToString("yyyyMMdd");

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                    if (fileExtension == ".xlsx")
                    {
                        ExportToExcel(filePath);
                    }
                    else if (fileExtension == ".pdf")
                    {
                        ExportToPdf(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel(string filePath)
        {
            // Tạm mô phỏng xuất Excel, trong thực tế sẽ sử dụng EPPlus
            MessageBox.Show("Đã xuất báo cáo ra file Excel: " + filePath, "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportToPdf(string filePath)
        {
            // Tạm mô phỏng xuất PDF, trong thực tế sẽ sử dụng iTextSharp
            MessageBox.Show("Đã xuất báo cáo ra file PDF: " + filePath, "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Khởi tạo Designer components
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageOverview = new System.Windows.Forms.TabPage();
            this.tabPageTopCustomers = new System.Windows.Forms.TabPage();
            this.tabPageCharts = new System.Windows.Forms.TabPage();

            // Overview tab components
            this.lblHeaderOverview = new System.Windows.Forms.Label();
            this.lblInfoTotalCustomers = new System.Windows.Forms.Label();
            this.lblValueTotalCustomers = new System.Windows.Forms.Label();
            this.lblInfoTotalPoints = new System.Windows.Forms.Label();
            this.lblValueTotalPoints = new System.Windows.Forms.Label();
            this.lblInfoTotalSpent = new System.Windows.Forms.Label();
            this.lblValueTotalSpent = new System.Windows.Forms.Label();
            this.lblInfoNewCustomers = new System.Windows.Forms.Label();
            this.lblValueNewCustomers = new System.Windows.Forms.Label();
            this.lblInfoActiveCustomers = new System.Windows.Forms.Label();
            this.lblValueActiveCustomers = new System.Windows.Forms.Label();
            this.lblInfoInactiveCustomers = new System.Windows.Forms.Label();
            this.lblValueInactiveCustomers = new System.Windows.Forms.Label();
            this.lblInfoAvgSpent = new System.Windows.Forms.Label();
            this.lblValueAvgSpent = new System.Windows.Forms.Label();

            this.lblHeaderMembership = new System.Windows.Forms.Label();
            this.lblInfoRegularCustomers = new System.Windows.Forms.Label();
            this.lblValueRegularCustomers = new System.Windows.Forms.Label();
            this.lblInfoSilverCustomers = new System.Windows.Forms.Label();
            this.lblValueSilverCustomers = new System.Windows.Forms.Label();
            this.lblInfoGoldCustomers = new System.Windows.Forms.Label();
            this.lblValueGoldCustomers = new System.Windows.Forms.Label();
            this.lblInfoPlatinumCustomers = new System.Windows.Forms.Label();
            this.lblValuePlatinumCustomers = new System.Windows.Forms.Label();

            // Top customers tab components
            this.dgvTopCustomers = new System.Windows.Forms.DataGridView();

            // Charts tab components
            this.pieChartMembership = new LiveCharts.WinForms.PieChart();
            this.pieChartActivity = new LiveCharts.WinForms.PieChart();
            this.lblChartMembership = new System.Windows.Forms.Label();
            this.lblChartActivity = new System.Windows.Forms.Label();

            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Size = new System.Drawing.Size(900, 60);
            this.panelTop.Controls.Add(this.lblTitle);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Size = new System.Drawing.Size(300, 30);
            this.lblTitle.Text = "BÁO CÁO KHÁCH HÀNG";

            // tabControl
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 60);
            this.tabControl.Size = new System.Drawing.Size(900, 440);
            this.tabControl.Controls.Add(this.tabPageOverview);
            this.tabControl.Controls.Add(this.tabPageTopCustomers);
            this.tabControl.Controls.Add(this.tabPageCharts);

            // tabPageOverview
            this.tabPageOverview.Text = "Tổng quan";
            this.tabPageOverview.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageOverview.Controls.Add(this.lblHeaderOverview);
            this.tabPageOverview.Controls.Add(this.lblInfoTotalCustomers);
            this.tabPageOverview.Controls.Add(this.lblValueTotalCustomers);
            this.tabPageOverview.Controls.Add(this.lblInfoTotalPoints);
            this.tabPageOverview.Controls.Add(this.lblValueTotalPoints);
            this.tabPageOverview.Controls.Add(this.lblInfoTotalSpent);
            this.tabPageOverview.Controls.Add(this.lblValueTotalSpent);
            this.tabPageOverview.Controls.Add(this.lblInfoNewCustomers);
            this.tabPageOverview.Controls.Add(this.lblValueNewCustomers);
            this.tabPageOverview.Controls.Add(this.lblInfoActiveCustomers);
            this.tabPageOverview.Controls.Add(this.lblValueActiveCustomers);
            this.tabPageOverview.Controls.Add(this.lblInfoInactiveCustomers);
            this.tabPageOverview.Controls.Add(this.lblValueInactiveCustomers);
            this.tabPageOverview.Controls.Add(this.lblInfoAvgSpent);
            this.tabPageOverview.Controls.Add(this.lblValueAvgSpent);
            this.tabPageOverview.Controls.Add(this.lblHeaderMembership);
            this.tabPageOverview.Controls.Add(this.lblInfoRegularCustomers);
            this.tabPageOverview.Controls.Add(this.lblValueRegularCustomers);
            this.tabPageOverview.Controls.Add(this.lblInfoSilverCustomers);
            this.tabPageOverview.Controls.Add(this.lblValueSilverCustomers);
            this.tabPageOverview.Controls.Add(this.lblInfoGoldCustomers);
            this.tabPageOverview.Controls.Add(this.lblValueGoldCustomers);
            this.tabPageOverview.Controls.Add(this.lblInfoPlatinumCustomers);
            this.tabPageOverview.Controls.Add(this.lblValuePlatinumCustomers);

            // lblHeaderOverview
            this.lblHeaderOverview.AutoSize = true;
            this.lblHeaderOverview.Location = new System.Drawing.Point(20, 20);
            this.lblHeaderOverview.Size = new System.Drawing.Size(200, 25);
            this.lblHeaderOverview.Text = "Thông tin tổng quan";

            // lblInfoTotalCustomers
            this.lblInfoTotalCustomers.AutoSize = true;
            this.lblInfoTotalCustomers.Location = new System.Drawing.Point(40, 60);
            this.lblInfoTotalCustomers.Size = new System.Drawing.Size(150, 20);
            this.lblInfoTotalCustomers.Text = "Tổng số khách hàng:";

            // lblValueTotalCustomers
            this.lblValueTotalCustomers.AutoSize = true;
            this.lblValueTotalCustomers.Location = new System.Drawing.Point(200, 60);
            this.lblValueTotalCustomers.Size = new System.Drawing.Size(80, 20);
            this.lblValueTotalCustomers.Text = "0";

            // lblInfoTotalPoints
            this.lblInfoTotalPoints.AutoSize = true;
            this.lblInfoTotalPoints.Location = new System.Drawing.Point(40, 90);
            this.lblInfoTotalPoints.Size = new System.Drawing.Size(150, 20);
            this.lblInfoTotalPoints.Text = "Tổng điểm tích lũy:";

            // lblValueTotalPoints
            this.lblValueTotalPoints.AutoSize = true;
            this.lblValueTotalPoints.Location = new System.Drawing.Point(200, 90);
            this.lblValueTotalPoints.Size = new System.Drawing.Size(80, 20);
            this.lblValueTotalPoints.Text = "0";

            // lblInfoTotalSpent
            this.lblInfoTotalSpent.AutoSize = true;
            this.lblInfoTotalSpent.Location = new System.Drawing.Point(40, 120);
            this.lblInfoTotalSpent.Size = new System.Drawing.Size(150, 20);
            this.lblInfoTotalSpent.Text = "Tổng chi tiêu:";

            // lblValueTotalSpent
            this.lblValueTotalSpent.AutoSize = true;
            this.lblValueTotalSpent.Location = new System.Drawing.Point(200, 120);
            this.lblValueTotalSpent.Size = new System.Drawing.Size(80, 20);
            this.lblValueTotalSpent.Text = "0 VNĐ";

            // lblInfoNewCustomers
            this.lblInfoNewCustomers.AutoSize = true;
            this.lblInfoNewCustomers.Location = new System.Drawing.Point(40, 150);
            this.lblInfoNewCustomers.Size = new System.Drawing.Size(150, 20);
            this.lblInfoNewCustomers.Text = "Khách hàng mới (tháng):";

            // lblValueNewCustomers
            this.lblValueNewCustomers.AutoSize = true;
            this.lblValueNewCustomers.Location = new System.Drawing.Point(200, 150);
            this.lblValueNewCustomers.Size = new System.Drawing.Size(80, 20);
            this.lblValueNewCustomers.Text = "0";

            // lblInfoActiveCustomers
            this.lblInfoActiveCustomers.AutoSize = true;
            this.lblInfoActiveCustomers.Location = new System.Drawing.Point(40, 180);
            this.lblInfoActiveCustomers.Size = new System.Drawing.Size(150, 20);
            this.lblInfoActiveCustomers.Text = "Khách hàng tích cực:";

            // lblValueActiveCustomers
            this.lblValueActiveCustomers.AutoSize = true;
            this.lblValueActiveCustomers.Location = new System.Drawing.Point(200, 180);
            this.lblValueActiveCustomers.Size = new System.Drawing.Size(80, 20);
            this.lblValueActiveCustomers.Text = "0";

            // lblInfoInactiveCustomers
            this.lblInfoInactiveCustomers.AutoSize = true;
            this.lblInfoInactiveCustomers.Location = new System.Drawing.Point(40, 210);
            this.lblInfoInactiveCustomers.Size = new System.Drawing.Size(150, 20);
            this.lblInfoInactiveCustomers.Text = "Khách hàng không tích cực:";

            // lblValueInactiveCustomers
            this.lblValueInactiveCustomers.AutoSize = true;
            this.lblValueInactiveCustomers.Location = new System.Drawing.Point(200, 210);
            this.lblValueInactiveCustomers.Size = new System.Drawing.Size(80, 20);
            this.lblValueInactiveCustomers.Text = "0";

            // lblInfoAvgSpent
            this.lblInfoAvgSpent.AutoSize = true;
            this.lblInfoAvgSpent.Location = new System.Drawing.Point(40, 240);
            this.lblInfoAvgSpent.Size = new System.Drawing.Size(150, 20);
            this.lblInfoAvgSpent.Text = "Chi tiêu trung bình:";

            // lblValueAvgSpent
            this.lblValueAvgSpent.AutoSize = true;
            this.lblValueAvgSpent.Location = new System.Drawing.Point(200, 240);
            this.lblValueAvgSpent.Size = new System.Drawing.Size(80, 20);
            this.lblValueAvgSpent.Text = "0 VNĐ";

            // lblHeaderMembership
            this.lblHeaderMembership.AutoSize = true;
            this.lblHeaderMembership.Location = new System.Drawing.Point(400, 20);
            this.lblHeaderMembership.Size = new System.Drawing.Size(200, 25);
            this.lblHeaderMembership.Text = "Phân loại theo cấp độ";

            // lblInfoRegularCustomers
            this.lblInfoRegularCustomers.AutoSize = true;
            this.lblInfoRegularCustomers.Location = new System.Drawing.Point(420, 60);
            this.lblInfoRegularCustomers.Size = new System.Drawing.Size(150, 20);
            this.lblInfoRegularCustomers.Text = "Regular:";

            // lblValueRegularCustomers
            this.lblValueRegularCustomers.AutoSize = true;
            this.lblValueRegularCustomers.Location = new System.Drawing.Point(580, 60);
            this.lblValueRegularCustomers.Size = new System.Drawing.Size(80, 20);
            this.lblValueRegularCustomers.Text = "0";

            // lblInfoSilverCustomers
            this.lblInfoSilverCustomers.AutoSize = true;
            this.lblInfoSilverCustomers.Location = new System.Drawing.Point(420, 90);
            this.lblInfoSilverCustomers.Size = new System.Drawing.Size(150, 20);
            this.lblInfoSilverCustomers.Text = "Silver:";

            // lblValueSilverCustomers
            this.lblValueSilverCustomers.AutoSize = true;
            this.lblValueSilverCustomers.Location = new System.Drawing.Point(580, 90);
            this.lblValueSilverCustomers.Size = new System.Drawing.Size(80, 20);
            this.lblValueSilverCustomers.Text = "0";

            // lblInfoGoldCustomers
            this.lblInfoGoldCustomers.AutoSize = true;
            this.lblInfoGoldCustomers.Location = new System.Drawing.Point(420, 120);
            this.lblInfoGoldCustomers.Size = new System.Drawing.Size(150, 20);
            this.lblInfoGoldCustomers.Text = "Gold:";

            // lblValueGoldCustomers
            this.lblValueGoldCustomers.AutoSize = true;
            this.lblValueGoldCustomers.Location = new System.Drawing.Point(580, 120);
            this.lblValueGoldCustomers.Size = new System.Drawing.Size(80, 20);
            this.lblValueGoldCustomers.Text = "0";

            // lblInfoPlatinumCustomers
            this.lblInfoPlatinumCustomers.AutoSize = true;
            this.lblInfoPlatinumCustomers.Location = new System.Drawing.Point(420, 150);
            this.lblInfoPlatinumCustomers.Size = new System.Drawing.Size(150, 20);
            this.lblInfoPlatinumCustomers.Text = "Platinum:";

            // lblValuePlatinumCustomers
            this.lblValuePlatinumCustomers.AutoSize = true;
            this.lblValuePlatinumCustomers.Location = new System.Drawing.Point(580, 150);
            this.lblValuePlatinumCustomers.Size = new System.Drawing.Size(80, 20);
            this.lblValuePlatinumCustomers.Text = "0";

            // tabPageTopCustomers
            this.tabPageTopCustomers.Text = "Top khách hàng";
            this.tabPageTopCustomers.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageTopCustomers.Controls.Add(this.dgvTopCustomers);

            // dgvTopCustomers
            this.dgvTopCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTopCustomers.Location = new System.Drawing.Point(10, 10);
            this.dgvTopCustomers.Size = new System.Drawing.Size(880, 420);
            this.dgvTopCustomers.ReadOnly = true;
            this.dgvTopCustomers.MultiSelect = false;
            this.dgvTopCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTopCustomers.AllowUserToAddRows = false;
            this.dgvTopCustomers.AllowUserToDeleteRows = false;
            this.dgvTopCustomers.AllowUserToResizeRows = false;

            // tabPageCharts
            this.tabPageCharts.Text = "Biểu đồ";
            this.tabPageCharts.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageCharts.Controls.Add(this.pieChartMembership);
            this.tabPageCharts.Controls.Add(this.pieChartActivity);
            this.tabPageCharts.Controls.Add(this.lblChartMembership);
            this.tabPageCharts.Controls.Add(this.lblChartActivity);

            // lblChartMembership
            this.lblChartMembership.AutoSize = true;
            this.lblChartMembership.Location = new System.Drawing.Point(10, 10);
            this.lblChartMembership.Size = new System.Drawing.Size(200, 20);
            this.lblChartMembership.Text = "Phân bố theo cấp độ thành viên";
            this.lblChartMembership.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblChartMembership.ForeColor = _accentColor;

            // pieChartMembership
            this.pieChartMembership.Location = new System.Drawing.Point(10, 40);
            this.pieChartMembership.Size = new System.Drawing.Size(420, 350);
            this.pieChartMembership.BackColor = Color.White;

            // lblChartActivity
            this.lblChartActivity.AutoSize = true;
            this.lblChartActivity.Location = new System.Drawing.Point(450, 10);
            this.lblChartActivity.Size = new System.Drawing.Size(200, 20);
            this.lblChartActivity.Text = "Phân bố theo hoạt động khách hàng";
            this.lblChartActivity.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblChartActivity.ForeColor = _accentColor;

            // pieChartActivity
            this.pieChartActivity.Location = new System.Drawing.Point(450, 40);
            this.pieChartActivity.Size = new System.Drawing.Size(420, 350);
            this.pieChartActivity.BackColor = Color.White;

            // panelBottom
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 500);
            this.panelBottom.Size = new System.Drawing.Size(900, 60);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnExport);

            // btnExport
            this.btnExport.Location = new System.Drawing.Point(680, 15);
            this.btnExport.Size = new System.Drawing.Size(100, 30);
            this.btnExport.Text = "Xuất báo cáo";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);

            // btnClose
            this.btnClose.Location = new System.Drawing.Point(790, 15);
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // CustomerReportForm
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Báo cáo khách hàng";
        }

        private Panel panelTop;
        private Label lblTitle;
        private TabControl tabControl;
        private TabPage tabPageOverview;
        private TabPage tabPageTopCustomers;
        private TabPage tabPageCharts;
        private Label lblHeaderOverview;
        private Label lblInfoTotalCustomers;
        private Label lblValueTotalCustomers;
        private Label lblInfoTotalPoints;
        private Label lblValueTotalPoints;
        private Label lblInfoTotalSpent;
        private Label lblValueTotalSpent;
        private Label lblInfoNewCustomers;
        private Label lblValueNewCustomers;
        private Label lblInfoActiveCustomers;
        private Label lblValueActiveCustomers;
        private Label lblInfoInactiveCustomers;
        private Label lblValueInactiveCustomers;
        private Label lblInfoAvgSpent;
        private Label lblValueAvgSpent;
        private Label lblHeaderMembership;
        private Label lblInfoRegularCustomers;
        private Label lblValueRegularCustomers;
        private Label lblInfoSilverCustomers;
        private Label lblValueSilverCustomers;
        private Label lblInfoGoldCustomers;
        private Label lblValueGoldCustomers;
        private Label lblInfoPlatinumCustomers;
        private Label lblValuePlatinumCustomers;
        private DataGridView dgvTopCustomers;
        private PieChart pieChartMembership;
        private PieChart pieChartActivity;
        private Label lblChartMembership;
        private Label lblChartActivity;
        private Panel panelBottom;
        private Button btnClose;
        private Button btnExport;
    }
}