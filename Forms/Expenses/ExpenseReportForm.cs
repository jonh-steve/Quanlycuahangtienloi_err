// Vị trí file: /Forms/Expenses/ExpenseReportForm.cs

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using LiveCharts.Wpf;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Forms.Expenses
{
    public partial class ExpenseReportForm : Form
    {
        private readonly IExpenseService _expenseService;
        private readonly Logger _logger;
        private List<ExpenseDTO> _expenses;
        private Dictionary<string, decimal> _expensesByType;
        private Dictionary<DateTime, decimal> _expensesByDate;

        public ExpenseReportForm(IExpenseService expenseService, Logger logger)
        {
            InitializeComponent();

            _expenseService = expenseService;
            _logger = logger;

            // Thiết lập màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu lavender blush

            // Thiết lập màu cho các control
            CustomizeControls();
        }

        private void ExpenseReportForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Thiết lập giá trị mặc định cho DateTimePicker
                dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtpToDate.Value = DateTime.Now;

                // Thiết lập giá trị mặc định cho GroupBy
                cboGroupBy.Items.Add("Ngày");
                cboGroupBy.Items.Add("Tuần");
                cboGroupBy.Items.Add("Tháng");
                cboGroupBy.SelectedIndex = 0;

                // Thiết lập Chart
                SetupCharts();

                // Tải dữ liệu báo cáo
                LoadReport();
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseReportForm_Load", ex);
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeControls()
        {
            // Thiết lập màu cho Panel
            pnlFilter.BackColor = Color.FromArgb(255, 228, 225); // Màu misty rose

            // Thiết lập màu cho Button
            btnRefresh.BackColor = Color.FromArgb(255, 182, 193); // Màu light pink
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;

            btnExportExcel.BackColor = Color.FromArgb(255, 182, 193);
            btnExportExcel.ForeColor = Color.White;
            btnExportExcel.FlatStyle = FlatStyle.Flat;
            btnExportExcel.FlatAppearance.BorderSize = 0;

            btnExportPDF.BackColor = Color.FromArgb(255, 182, 193);
            btnExportPDF.ForeColor = Color.White;
            btnExportPDF.FlatStyle = FlatStyle.Flat;
            btnExportPDF.FlatAppearance.BorderSize = 0;

            // Thiết lập màu cho Label
            lblFromDate.ForeColor = Color.FromArgb(199, 21, 133); // Màu medium violet red
            lblToDate.ForeColor = Color.FromArgb(199, 21, 133);
            lblGroupBy.ForeColor = Color.FromArgb(199, 21, 133);
            lblTotalAmount.ForeColor = Color.FromArgb(199, 21, 133);
            lblTotalAmount.Font = new Font(lblTotalAmount.Font, FontStyle.Bold);
            lblTotalExpense.ForeColor = Color.FromArgb(199, 21, 133);
            lblTotalExpense.Font = new Font(lblTotalExpense.Font, FontStyle.Bold);

            // Thiết lập màu cho TabControl
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += (sender, e) => {
                Graphics g = e.Graphics;
                TabPage tp = tabControl.TabPages[e.Index];
                Rectangle tabBounds = tabControl.GetTabRect(e.Index);

                Brush textBrush;
                if (e.State == DrawItemState.Selected)
                {
                    // Tab được chọn
                    textBrush = new SolidBrush(Color.FromArgb(199, 21, 133)); // Màu medium violet red
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, 228, 225)), e.Bounds); // Màu misty rose
                }
                else
                {
                    // Tab không được chọn
                    textBrush = new SolidBrush(Color.FromArgb(255, 105, 180)); // Màu hot pink
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, 240, 245)), e.Bounds); // Màu lavender blush
                }

                // Vẽ tên tab
                Font font = e.Index == tabControl.SelectedIndex ? new Font(tabControl.Font, FontStyle.Bold) : tabControl.Font;
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(tp.Text, font, textBrush, tabBounds, sf);
            };
        }

        private void SetupCharts()
        {
            // Thiết lập chart chi phí theo loại
            chartByType.Series.Clear();
            Series series1 = new Series("Chi phí theo loại");
            series1.ChartType = SeriesChartType.Pie;
            series1.IsValueShownAsLabel = true;
            series1.LabelFormat = "{0:N0} VNĐ";
            series1.Font = new Font("Arial", 9, FontStyle.Bold);
            chartByType.Series.Add(series1);

            // Thiết lập chart chi phí theo thời gian
            chartByDate.Series.Clear();
            Series series2 = new Series("Chi phí theo thời gian");
            series2.ChartType = SeriesChartType.Column;
            series2.IsValueShownAsLabel = true;
            series2.LabelFormat = "{0:N0}";
            series2.Font = new Font("Arial", 8, FontStyle.Regular);
            series2.Color = Color.FromArgb(255, 105, 180); // Màu hot pink
            chartByDate.Series.Add(series2);

            // Thiết lập màu nền cho chart
            chartByType.BackColor = Color.FromArgb(255, 240, 245); // Màu lavender blush
            chartByDate.BackColor = Color.FromArgb(255, 240, 245);

            // Thiết lập hiển thị tiêu đề
            chartByType.Titles.Clear();
            Title title1 = new Title("Chi phí theo loại", Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.FromArgb(199, 21, 133));
            chartByType.Titles.Add(title1);

            chartByDate.Titles.Clear();
            Title title2 = new Title("Chi phí theo thời gian", Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.FromArgb(199, 21, 133));
            chartByDate.Titles.Add(title2);

            // Thiết lập palette màu cho chart pie
            chartByType.Palette = ChartColorPalette.Berry;
        }

        private void LoadReport()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // Lấy tham số báo cáo
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // Đến cuối ngày
                string groupBy = GetGroupByValue();

                // Lấy dữ liệu chi phí
                _expenses = _expenseService.GetExpensesByDateRange(fromDate, toDate);

                // Tính tổng chi phí
                decimal totalAmount = _expenses.Sum(e => e.Amount);
                lblTotalAmount.Text = totalAmount.ToString("#,##0") + " VNĐ";

                // Lấy dữ liệu thống kê theo loại chi phí
                _expensesByType = _expenseService.GetExpenseSummaryByType(fromDate, toDate);

                // Lấy dữ liệu thống kê theo thời gian
                _expensesByDate = _expenseService.GetExpenseSummaryByDate(fromDate, toDate, groupBy);

                // Cập nhật chart
                UpdateCharts();

                // Cập nhật DataGridView
                UpdateDataGridView();
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadReport", ex);
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private string GetGroupByValue()
        {
            switch (cboGroupBy.SelectedIndex)
            {
                case 0: return "Day";
                case 1: return "Week";
                case 2: return "Month";
                default: return "Day";
            }
        }

        private void UpdateCharts()
        {
            // Cập nhật chart chi phí theo loại
            chartByType.Series[0].Points.Clear();
            foreach (var item in _expensesByType)
            {
                int point = chartByType.Series[0].Points.AddY(item.Value);
                chartByType.Series[0].Points[point].LegendText = item.Key;
                chartByType.Series[0].Points[point].Label = item.Value.ToString("#,##0");
            }

            // Cập nhật chart chi phí theo thời gian
            chartByDate.Series[0].Points.Clear();
            chartByDate.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartByDate.ChartAreas[0].AxisX.Interval = 1;
            chartByDate.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 8);
            chartByDate.ChartAreas[0].AxisY.LabelStyle.Format = "{0:N0}";

            foreach (var item in _expensesByDate.OrderBy(x => x.Key))
            {
                string dateLabel = FormatDateLabel(item.Key);
                int point = chartByDate.Series[0].Points.AddXY(dateLabel, item.Value);
                chartByDate.Series[0].Points[point].Label = item.Value.ToString("#,##0");
            }
        }

        private string FormatDateLabel(DateTime date)
        {
            switch (cboGroupBy.SelectedIndex)
            {
                case 0: // Ngày
                    return date.ToString("dd/MM");
                case 1: // Tuần
                    return $"Tuần {GetWeekNumber(date)}";
                case 2: // Tháng
                    return date.ToString("MM/yyyy");
                default:
                    return date.ToString("dd/MM");
            }
        }

        private int GetWeekNumber(DateTime date)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        private void UpdateDataGridView()
        {
            // Cập nhật DataGridView chi phí
            dgvExpenses.DataSource = null;
            dgvExpenses.DataSource = _expenses;

            // Thiết lập hiển thị cột
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            // Thiết lập thuộc tính DataGridView
            dgvExpenses.AutoGenerateColumns = false;
            dgvExpenses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvExpenses.MultiSelect = false;
            dgvExpenses.ReadOnly = true;
            dgvExpenses.AllowUserToAddRows = false;
            dgvExpenses.AllowUserToDeleteRows = false;
            dgvExpenses.AllowUserToResizeRows = false;
            dgvExpenses.RowHeadersVisible = false;
            dgvExpenses.EnableHeadersVisualStyles = false;

            // Thiết lập màu sắc cho DataGridView
            dgvExpenses.BackgroundColor = Color.White;
            dgvExpenses.BorderStyle = BorderStyle.None;
            dgvExpenses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 192, 203); // Màu pink
            dgvExpenses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvExpenses.ColumnHeadersDefaultCellStyle.Font = new Font(dgvExpenses.Font, FontStyle.Bold);
            dgvExpenses.ColumnHeadersHeight = 40;
            dgvExpenses.RowTemplate.Height = 30;
            dgvExpenses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245); // Màu lavender blush
            dgvExpenses.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 182, 193); // Màu light pink
            dgvExpenses.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Xóa tất cả cột
            if (dgvExpenses.Columns.Count == 0)
            {
                // Thêm các cột mới
                DataGridViewTextBoxColumn colSTT = new DataGridViewTextBoxColumn();
                colSTT.Name = "colSTT";
                colSTT.HeaderText = "STT";
                colSTT.Width = 50;
                dgvExpenses.Columns.Add(colSTT);

                // Tiếp tục ở phần còn lại của ExpenseReportForm.cs

                DataGridViewTextBoxColumn colAmount = new DataGridViewTextBoxColumn();
                colAmount.Name = "colAmount";
                colAmount.HeaderText = "Số tiền";
                colAmount.DataPropertyName = "Amount";
                colAmount.Width = 120;
                colAmount.DefaultCellStyle.Format = "#,##0";
                colAmount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvExpenses.Columns.Add(colAmount);

                DataGridViewTextBoxColumn colExpenseDate = new DataGridViewTextBoxColumn();
                colExpenseDate.Name = "colExpenseDate";
                colExpenseDate.HeaderText = "Ngày chi";
                colExpenseDate.DataPropertyName = "ExpenseDate";
                colExpenseDate.Width = 100;
                colExpenseDate.DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvExpenses.Columns.Add(colExpenseDate);

                DataGridViewTextBoxColumn colDescription = new DataGridViewTextBoxColumn();
                colDescription.Name = "colDescription";
                colDescription.HeaderText = "Mô tả";
                colDescription.DataPropertyName = "Description";
                colDescription.Width = 250;
                dgvExpenses.Columns.Add(colDescription);

                DataGridViewTextBoxColumn colEmployeeName = new DataGridViewTextBoxColumn();
                colEmployeeName.Name = "colEmployeeName";
                colEmployeeName.HeaderText = "Người tạo";
                colEmployeeName.DataPropertyName = "EmployeeName";
                colEmployeeName.Width = 150;
                dgvExpenses.Columns.Add(colEmployeeName);

                DataGridViewTextBoxColumn colCreatedDate = new DataGridViewTextBoxColumn();
                colCreatedDate.Name = "colCreatedDate";
                colCreatedDate.HeaderText = "Ngày tạo";
                colCreatedDate.DataPropertyName = "CreatedDate";
                colCreatedDate.Width = 150;
                colCreatedDate.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dgvExpenses.Columns.Add(colCreatedDate);

                // Ẩn cột ID
                DataGridViewTextBoxColumn colExpenseID = new DataGridViewTextBoxColumn();
                colExpenseID.Name = "colExpenseID";
                colExpenseID.DataPropertyName = "ExpenseID";
                colExpenseID.Visible = false;
                dgvExpenses.Columns.Add(colExpenseID);
            }
        }

        private void dgvExpenses_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Đánh số thứ tự
            if (e.ColumnIndex == dgvExpenses.Columns["colSTT"].Index && e.RowIndex >= 0)
            {
                e.Value = e.RowIndex + 1;
                e.FormattingApplied = true;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_expenses == null || _expenses.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Xuất báo cáo Excel";
                saveFileDialog.FileName = $"BaoCaoChiPhi_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    // Xuất báo cáo ra Excel
                    string filePath = _expenseService.ExportExpensesToExcel(_expenses, saveFileDialog.FileName);

                    Cursor.Current = Cursors.Default;

                    // Hỏi người dùng có muốn mở file không
                    DialogResult result = MessageBox.Show(
                        "Xuất báo cáo Excel thành công! Bạn có muốn mở file không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                _logger.LogError("btnExportExcel_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_expenses == null || _expenses.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.Title = "Xuất báo cáo PDF";
                saveFileDialog.FileName = $"BaoCaoChiPhi_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    // Xuất báo cáo ra PDF
                    string filePath = _expenseService.ExportExpensesToPDF(_expenses, saveFileDialog.FileName);

                    Cursor.Current = Cursors.Default;

                    // Hỏi người dùng có muốn mở file không
                    DialogResult result = MessageBox.Show(
                        "Xuất báo cáo PDF thành công! Bạn có muốn mở file không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                _logger.LogError("btnExportPDF_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            // Đảm bảo FromDate không lớn hơn ToDate
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                dtpFromDate.Value = dtpToDate.Value;
            }
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            // Đảm bảo ToDate không nhỏ hơn FromDate
            if (dtpToDate.Value < dtpFromDate.Value)
            {
                dtpToDate.Value = dtpFromDate.Value;
            }
        }

        private void cboGroupBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Tải lại báo cáo khi thay đổi loại nhóm
            LoadReport();
        }

        #region Designer Generated Code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.pnlTotalInfo = new System.Windows.Forms.Panel();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblTotalExpense = new System.Windows.Forms.Label();
            this.btnExportPDF = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cboGroupBy = new System.Windows.Forms.ComboBox();
            this.lblGroupBy = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCharts = new System.Windows.Forms.TabPage();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.chartByType = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartByDate = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabDetails = new System.Windows.Forms.TabPage();
            this.dgvExpenses = new System.Windows.Forms.DataGridView();
            this.pnlFilter.SuspendLayout();
            this.pnlTotalInfo.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartByType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartByDate)).BeginInit();
            this.tabDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.pnlTotalInfo);
            this.pnlFilter.Controls.Add(this.btnExportPDF);
            this.pnlFilter.Controls.Add(this.btnExportExcel);
            this.pnlFilter.Controls.Add(this.btnRefresh);
            this.pnlFilter.Controls.Add(this.cboGroupBy);
            this.pnlFilter.Controls.Add(this.lblGroupBy);
            this.pnlFilter.Controls.Add(this.dtpToDate);
            this.pnlFilter.Controls.Add(this.lblToDate);
            this.pnlFilter.Controls.Add(this.dtpFromDate);
            this.pnlFilter.Controls.Add(this.lblFromDate);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(984, 70);
            this.pnlFilter.TabIndex = 0;
            // 
            // pnlTotalInfo
            // 
            this.pnlTotalInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTotalInfo.Controls.Add(this.lblTotalAmount);
            this.pnlTotalInfo.Controls.Add(this.lblTotalExpense);
            this.pnlTotalInfo.Location = new System.Drawing.Point(735, 10);
            this.pnlTotalInfo.Name = "pnlTotalInfo";
            this.pnlTotalInfo.Size = new System.Drawing.Size(237, 50);
            this.pnlTotalInfo.TabIndex = 9;
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Location = new System.Drawing.Point(134, 19);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(43, 13);
            this.lblTotalAmount.TabIndex = 1;
            this.lblTotalAmount.Text = "0 VNĐ";
            // 
            // lblTotalExpense
            // 
            this.lblTotalExpense.AutoSize = true;
            this.lblTotalExpense.Location = new System.Drawing.Point(16, 19);
            this.lblTotalExpense.Name = "lblTotalExpense";
            this.lblTotalExpense.Size = new System.Drawing.Size(112, 13);
            this.lblTotalExpense.TabIndex = 0;
            this.lblTotalExpense.Text = "Tổng chi phí trong kỳ:";
            // 
            // btnExportPDF
            // 
            this.btnExportPDF.Location = new System.Drawing.Point(634, 22);
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Size = new System.Drawing.Size(92, 26);
            this.btnExportPDF.TabIndex = 8;
            this.btnExportPDF.Text = "Xuất PDF";
            this.btnExportPDF.UseVisualStyleBackColor = true;
            this.btnExportPDF.Click += new System.EventHandler(this.btnExportPDF_Click);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Location = new System.Drawing.Point(536, 22);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(92, 26);
            this.btnExportExcel.TabIndex = 7;
            this.btnExportExcel.Text = "Xuất Excel";
            this.btnExportExcel.UseVisualStyleBackColor = true;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(455, 22);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 26);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cboGroupBy
            // 
            this.cboGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGroupBy.FormattingEnabled = true;
            this.cboGroupBy.Location = new System.Drawing.Point(354, 25);
            this.cboGroupBy.Name = "cboGroupBy";
            this.cboGroupBy.Size = new System.Drawing.Size(95, 21);
            this.cboGroupBy.TabIndex = 5;
            this.cboGroupBy.SelectedIndexChanged += new System.EventHandler(this.cboGroupBy_SelectedIndexChanged);
            // 
            // lblGroupBy
            // 
            this.lblGroupBy.AutoSize = true;
            this.lblGroupBy.Location = new System.Drawing.Point(283, 29);
            this.lblGroupBy.Name = "lblGroupBy";
            this.lblGroupBy.Size = new System.Drawing.Size(65, 13);
            this.lblGroupBy.TabIndex = 4;
            this.lblGroupBy.Text = "Nhóm theo:";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(180, 25);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(97, 20);
            this.dtpToDate.TabIndex = 3;
            this.dtpToDate.ValueChanged += new System.EventHandler(this.dtpToDate_ValueChanged);
            // 
            // lblToDate
            // 
            this.lblToDate.AutoSize = true;
            this.lblToDate.Location = new System.Drawing.Point(117, 29);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(57, 13);
            this.lblToDate.TabIndex = 2;
            this.lblToDate.Text = "Đến ngày:";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(14, 25);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(97, 20);
            this.dtpFromDate.TabIndex = 1;
            this.dtpFromDate.ValueChanged += new System.EventHandler(this.dtpFromDate_ValueChanged);
            // 
            // lblFromDate
            // 
            this.lblFromDate.AutoSize = true;
            this.lblFromDate.Location = new System.Drawing.Point(12, 9);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(49, 13);
            this.lblFromDate.TabIndex = 0;
            this.lblFromDate.Text = "Từ ngày:";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabCharts);
            this.tabControl.Controls.Add(this.tabDetails);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 70);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(984, 491);
            this.tabControl.TabIndex = 1;
            // 
            // tabCharts
            // 
            this.tabCharts.Controls.Add(this.splitContainer);
            this.tabCharts.Location = new System.Drawing.Point(4, 22);
            this.tabCharts.Name = "tabCharts";
            this.tabCharts.Padding = new System.Windows.Forms.Padding(3);
            this.tabCharts.Size = new System.Drawing.Size(976, 465);
            this.tabCharts.TabIndex = 0;
            this.tabCharts.Text = "Biểu đồ";
            this.tabCharts.UseVisualStyleBackColor = true;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(3, 3);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.chartByType);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.chartByDate);
            this.splitContainer.Size = new System.Drawing.Size(970, 459);
            this.splitContainer.SplitterDistance = 485;
            this.splitContainer.TabIndex = 0;
            // 
            // chartByType
            // 
            chartArea1.Name = "ChartArea1";
            this.chartByType.ChartAreas.Add(chartArea1);
            this.chartByType.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartByType.Legends.Add(legend1);
            this.chartByType.Location = new System.Drawing.Point(0, 0);
            this.chartByType.Name = "chartByType";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartByType.Series.Add(series1);
            this.chartByType.Size = new System.Drawing.Size(485, 459);
            this.chartByType.TabIndex = 0;
            this.chartByType.Text = "Chi phí theo loại";
            // 
            // chartByDate
            // 
            chartArea2.Name = "ChartArea1";
            this.chartByDate.ChartAreas.Add(chartArea2);
            this.chartByDate.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.chartByDate.Legends.Add(legend2);
            this.chartByDate.Location = new System.Drawing.Point(0, 0);
            this.chartByDate.Name = "chartByDate";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartByDate.Series.Add(series2);
            this.chartByDate.Size = new System.Drawing.Size(481, 459);
            this.chartByDate.TabIndex = 0;
            this.chartByDate.Text = "Chi phí theo thời gian";
            // 
            // tabDetails
            // 
            this.tabDetails.Controls.Add(this.dgvExpenses);
            this.tabDetails.Location = new System.Drawing.Point(4, 22);
            this.tabDetails.Name = "tabDetails";
            this.tabDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabDetails.Size = new System.Drawing.Size(976, 465);
            this.tabDetails.TabIndex = 1;
            this.tabDetails.Text = "Chi tiết";
            this.tabDetails.UseVisualStyleBackColor = true;
            // 
            // dgvExpenses
            // 
            this.dgvExpenses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExpenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExpenses.Location = new System.Drawing.Point(3, 3);
            this.dgvExpenses.Name = "dgvExpenses";
            this.dgvExpenses.Size = new System.Drawing.Size(970, 459);
            this.dgvExpenses.TabIndex = 0;
            this.dgvExpenses.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvExpenses_CellFormatting);
            // 
            // ExpenseReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlFilter);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "ExpenseReportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Báo cáo chi phí";
            this.Load += new System.EventHandler(this.ExpenseReportForm_Load);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlTotalInfo.ResumeLayout(false);
            this.pnlTotalInfo.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabCharts.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartByType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartByDate)).EndInit();
            this.tabDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Panel pnlTotalInfo;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblTotalExpense;
        private System.Windows.Forms.Button btnExportPDF;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cboGroupBy;
        private System.Windows.Forms.Label lblGroupBy;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabCharts;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartByType;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartByDate;
        private System.Windows.Forms.TabPage tabDetails;
        private System.Windows.Forms.DataGridView dgvExpenses;
    }
}