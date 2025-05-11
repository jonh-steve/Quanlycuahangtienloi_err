// File: Forms/System/SystemLogViewerForm.cs
// Mô tả: Form xem nhật ký hệ thống
// Tác giả: Steve-Thuong_hai
// Ngày tạo: 12/05/2025

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Forms.System
{
    /// <summary>
    /// Form xem nhật ký hệ thống
    /// </summary>
    public partial class SystemLogViewerForm : Form
    {
        private readonly ISystemConfigService _systemConfigService;
        private readonly AccountDTO _currentUser;
        private List<SystemLogDTO> _systemLogs;
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Khởi tạo form xem nhật ký hệ thống
        /// </summary>
        /// <param name="currentUser">Tài khoản đang đăng nhập</param>
        public SystemLogViewerForm(AccountDTO currentUser)
        {
            InitializeComponent();
            _systemConfigService = new SystemConfigService();
            _currentUser = currentUser;

            // Kiểm tra quyền truy cập
            if (_currentUser == null || _currentUser.RoleID != 1) // Giả sử RoleID 1 là Admin
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            // Áp dụng theme màu hồng dễ thương
            ApplyPinkTheme();
        }

        /// <summary>
        /// Áp dụng theme màu hồng dễ thương
        /// </summary>
        private void ApplyPinkTheme()
        {
            BackColor = Color.FromArgb(255, 240, 245); // Màu nền hồng nhạt

            // Màu cho DataGridView
            dgvLogs.EnableHeadersVisualStyles = false;
            dgvLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(219, 112, 147); // Màu hồng đậm
            dgvLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLogs.ColumnHeadersDefaultCellStyle.Font = new Font(dgvLogs.Font, FontStyle.Bold);
            dgvLogs.BackgroundColor = Color.FromArgb(255, 240, 245);
            dgvLogs.RowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245);
            dgvLogs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(253, 233, 242);
            dgvLogs.GridColor = Color.FromArgb(255, 192, 203);

            // Style cho các button
            foreach (Control control in Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = Color.FromArgb(219, 112, 147);
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = Color.FromArgb(255, 182, 193);
                    button.Font = new Font(button.Font, FontStyle.Bold);
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.ForeColor = Color.FromArgb(219, 112, 147);
                    groupBox.Font = new Font(groupBox.Font, FontStyle.Bold);

                    foreach (Control gbControl in groupBox.Controls)
                    {
                        if (gbControl is Button gbButton)
                        {
                            gbButton.BackColor = Color.FromArgb(219, 112, 147);
                            gbButton.ForeColor = Color.White;
                            gbButton.FlatStyle = FlatStyle.Flat;
                            gbButton.FlatAppearance.BorderColor = Color.FromArgb(255, 182, 193);
                            gbButton.Font = new Font(gbButton.Font, FontStyle.Bold);
                        }
                        else if (gbControl is Label label)
                        {
                            label.ForeColor = Color.FromArgb(199, 21, 133);
                        }
                        else if (gbControl is TextBox textBox)
                        {
                            textBox.BorderStyle = BorderStyle.FixedSingle;
                            textBox.BackColor = Color.FromArgb(255, 250, 250);
                        }
                        else if (gbControl is Panel panel)
                        {
                            foreach (Control pnlControl in panel.Controls)
                            {
                                if (pnlControl is Button pnlButton)
                                {
                                    pnlButton.BackColor = Color.FromArgb(219, 112, 147);
                                    pnlButton.ForeColor = Color.White;
                                    pnlButton.FlatStyle = FlatStyle.Flat;
                                    pnlButton.FlatAppearance.BorderColor = Color.FromArgb(255, 182, 193);
                                    pnlButton.Font = new Font(pnlButton.Font, FontStyle.Bold);
                                }
                                else if (pnlControl is DateTimePicker picker)
                                {
                                    picker.CalendarForeColor = Color.FromArgb(199, 21, 133);
                                    picker.CalendarTitleBackColor = Color.FromArgb(219, 112, 147);
                                    picker.CalendarTitleForeColor = Color.White;
                                }
                                else if (pnlControl is ComboBox combo)
                                {
                                    combo.BackColor = Color.FromArgb(255, 250, 250);
                                    combo.FlatStyle = FlatStyle.Flat;
                                }
                            }
                        }
                    }
                }
            }

            // Thêm màu sắc cho richTextBox
            rtbLogDetails.BackColor = Color.FromArgb(255, 250, 250);
            rtbLogDetails.BorderStyle = BorderStyle.FixedSingle;
        }

        /// <summary>
        /// Sự kiện khi form tải
        /// </summary>
        private void SystemLogViewerForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Thiết lập thanh tìm kiếm
                cboLogLevel.Items.Clear();
                cboLogLevel.Items.Add("Tất cả");
                cboLogLevel.Items.Add("Info");
                cboLogLevel.Items.Add("Warning");
                cboLogLevel.Items.Add("Error");
                cboLogLevel.Items.Add("Debug");
                cboLogLevel.SelectedIndex = 0;

                // Thiết lập giá trị mặc định cho số dòng
                numMaxRows.Value = 1000;

                // Thiết lập giá trị mặc định cho ngày
                dtpStartDate.Value = DateTime.Now.AddDays(-7);
                dtpEndDate.Value = DateTime.Now;

                // Tải log
                LoadSystemLogs();
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemLogViewerForm_Load", ex);
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải log hệ thống
        /// </summary>
        private void LoadSystemLogs()
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                string logLevel = cboLogLevel.SelectedIndex > 0 ? cboLogLevel.Text : null;
                int maxRows = (int)numMaxRows.Value;

                _systemLogs = _systemConfigService.GetSystemLogs(
                    dtpStartDate.Value, dtpEndDate.Value.AddDays(1).AddSeconds(-1), logLevel, maxRows);

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    string searchText = txtSearch.Text.ToLower();
                    _systemLogs = _systemLogs.Where(l =>
                        (l.Message != null && l.Message.ToLower().Contains(searchText)) ||
                        (l.Source != null && l.Source.ToLower().Contains(searchText)) ||
                        (l.Exception != null && l.Exception.ToLower().Contains(searchText)) ||
                        (l.Username != null && l.Username.ToLower().Contains(searchText))
                    ).ToList();
                }

                dgvLogs.DataSource = new BindingList<SystemLogDTO>(_systemLogs);

                // Cấu hình hiển thị cột
                if (dgvLogs.Columns.Count > 0)
                {
                    dgvLogs.Columns["LogID"].HeaderText = "ID";
                    dgvLogs.Columns["LogID"].Width = 50;

                    dgvLogs.Columns["LogLevel"].HeaderText = "Mức độ";
                    dgvLogs.Columns["LogLevel"].Width = 70;

                    dgvLogs.Columns["Message"].HeaderText = "Thông báo";
                    dgvLogs.Columns["Message"].Width = 300;

                    dgvLogs.Columns["Source"].HeaderText = "Nguồn";
                    dgvLogs.Columns["Source"].Width = 150;

                    dgvLogs.Columns["LogDate"].HeaderText = "Ngày ghi";
                    dgvLogs.Columns["LogDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                    dgvLogs.Columns["LogDate"].Width = 150;

                    dgvLogs.Columns["Username"].HeaderText = "Người dùng";
                    dgvLogs.Columns["Username"].Width = 100;

                    dgvLogs.Columns["IPAddress"].HeaderText = "Địa chỉ IP";
                    dgvLogs.Columns["IPAddress"].Width = 100;

                    // Ẩn một số cột không cần thiết trong DataGridView
                    dgvLogs.Columns["Exception"].Visible = false;
                    dgvLogs.Columns["StackTrace"].Visible = false;
                    dgvLogs.Columns["AccountID"].Visible = false;

                    // Thiết lập màu sắc cho các mức độ log
                    dgvLogs.CellFormatting += (sender, e) =>
                    {
                        if (e.ColumnIndex == dgvLogs.Columns["LogLevel"].Index && e.Value != null)
                        {
                            string level = e.Value.ToString();
                            if (level == "Error")
                            {
                                e.CellStyle.ForeColor = Color.White;
                                e.CellStyle.BackColor = Color.FromArgb(220, 53, 69);
                                e.CellStyle.Font = new Font(dgvLogs.Font, FontStyle.Bold);
                            }
                            else if (level == "Warning")
                            {
                                e.CellStyle.ForeColor = Color.Black;
                                e.CellStyle.BackColor = Color.FromArgb(255, 193, 7);
                                e.CellStyle.Font = new Font(dgvLogs.Font, FontStyle.Bold);
                            }
                            else if (level == "Info")
                            {
                                e.CellStyle.ForeColor = Color.White;
                                e.CellStyle.BackColor = Color.FromArgb(0, 123, 255);
                            }
                            else if (level == "Debug")
                            {
                                e.CellStyle.ForeColor = Color.White;
                                e.CellStyle.BackColor = Color.FromArgb(108, 117, 125);
                            }
                        }
                    };
                }

                lblTotalLogs.Text = $"Tổng số bản ghi: {dgvLogs.RowCount}";

                // Xóa chi tiết log
                rtbLogDetails.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadSystemLogs", ex);
                MessageBox.Show($"Lỗi khi tải log hệ thống: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Hiển thị chi tiết log được chọn
        /// </summary>
        private void DisplayLogDetails()
        {
            try
            {
                if (dgvLogs.CurrentRow == null)
                {
                    rtbLogDetails.Clear();
                    return;
                }

                SystemLogDTO log = dgvLogs.CurrentRow.DataBoundItem as SystemLogDTO;

                if (log == null)
                {
                    rtbLogDetails.Clear();
                    return;
                }

                rtbLogDetails.Clear();

                // Tạo nội dung chi tiết log với định dạng màu sắc
                rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                rtbLogDetails.SelectionColor = Color.Blue;
                rtbLogDetails.AppendText("ID: ");
                rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);
                rtbLogDetails.SelectionColor = Color.Black;
                rtbLogDetails.AppendText($"{log.LogID}\n");

                rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                rtbLogDetails.SelectionColor = Color.Blue;
                rtbLogDetails.AppendText("Mức độ: ");
                rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);

                // Màu sắc tương ứng với mức độ log
                switch (log.LogLevel)
                {
                    case "Error":
                        rtbLogDetails.SelectionColor = Color.Red;
                        rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                        break;
                    case "Warning":
                        rtbLogDetails.SelectionColor = Color.Orange;
                        rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                        break;
                    case "Info":
                        rtbLogDetails.SelectionColor = Color.Blue;
                        break;
                    case "Debug":
                        rtbLogDetails.SelectionColor = Color.Gray;
                        break;
                    default:
                        rtbLogDetails.SelectionColor = Color.Black;
                        break;
                }

                rtbLogDetails.AppendText($"{log.LogLevel}\n");

                rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                rtbLogDetails.SelectionColor = Color.Blue;
                rtbLogDetails.AppendText("Ngày ghi: ");
                rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);
                rtbLogDetails.SelectionColor = Color.Black;
                rtbLogDetails.AppendText($"{log.LogDate:dd/MM/yyyy HH:mm:ss}\n");

                rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                rtbLogDetails.SelectionColor = Color.Blue;
                rtbLogDetails.AppendText("Thông báo: ");
                rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);
                rtbLogDetails.SelectionColor = Color.Black;
                rtbLogDetails.AppendText($"{log.Message}\n");

                if (!string.IsNullOrEmpty(log.Source))
                {
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                    rtbLogDetails.SelectionColor = Color.Blue;
                    rtbLogDetails.AppendText("Nguồn: ");
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);
                    rtbLogDetails.SelectionColor = Color.Black;
                    rtbLogDetails.AppendText($"{log.Source}\n");
                }

                if (log.AccountID.HasValue)
                {
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                    rtbLogDetails.SelectionColor = Color.Blue;
                    rtbLogDetails.AppendText("Người dùng: ");
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);
                    rtbLogDetails.SelectionColor = Color.Black;
                    rtbLogDetails.AppendText($"{log.Username} (ID: {log.AccountID})\n");
                }

                if (!string.IsNullOrEmpty(log.IPAddress))
                {
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                    rtbLogDetails.SelectionColor = Color.Blue;
                    rtbLogDetails.AppendText("Địa chỉ IP: ");
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);
                    rtbLogDetails.SelectionColor = Color.Black;
                    rtbLogDetails.AppendText($"{log.IPAddress}\n");
                }

                if (!string.IsNullOrEmpty(log.Exception))
                {
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                    rtbLogDetails.SelectionColor = Color.Red;
                    rtbLogDetails.AppendText("\nNgoại lệ:\n");
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);
                    rtbLogDetails.SelectionColor = Color.Red;
                    rtbLogDetails.AppendText($"{log.Exception}\n");
                }

                if (!string.IsNullOrEmpty(log.StackTrace))
                {
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Bold);
                    rtbLogDetails.SelectionColor = Color.Gray;
                    rtbLogDetails.AppendText("\nStack Trace:\n");
                    rtbLogDetails.SelectionFont = new Font(rtbLogDetails.Font, FontStyle.Regular);
                    rtbLogDetails.SelectionColor = Color.Gray;
                    rtbLogDetails.AppendText($"{log.StackTrace}\n");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("DisplayLogDetails", ex);
                MessageBox.Show($"Lỗi khi hiển thị chi tiết log: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi chọn một dòng trong DataGridView
        /// </summary>
        private void dgvLogs_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DisplayLogDetails();
            }
            catch (Exception ex)
            {
                _logger.LogError("dgvLogs_SelectionChanged", ex);
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Áp dụng
        /// </summary>
        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpStartDate.Value > dtpEndDate.Value)
                {
                    MessageBox.Show("Ngày bắt đầu không thể sau ngày kết thúc!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoadSystemLogs();
            }
            catch (Exception ex)
            {
                _logger.LogError("btnApply_Click", ex);
                MessageBox.Show($"Lỗi khi áp dụng bộ lọc: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Làm mới
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSystemLogs();
            }
            catch (Exception ex)
            {
                _logger.LogError("btnRefresh_Click", ex);
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Xóa tìm kiếm
        /// </summary>
        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearch.Text = string.Empty;
                cboLogLevel.SelectedIndex = 0;
                LoadSystemLogs();
            }
            catch (Exception ex)
            {
                _logger.LogError("btnClearSearch_Click", ex);
                MessageBox.Show($"Lỗi khi xóa tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Tìm kiếm
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSystemLogs();
            }
            catch (Exception ex)
            {
                _logger.LogError("btnSearch_Click", ex);
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn phím Enter trong ô tìm kiếm
        /// </summary>
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    e.Handled = true;
                    LoadSystemLogs();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("txtSearch_KeyPress", ex);
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Xuất log
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (_systemLogs == null || _systemLogs.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt",
                    Title = "Xuất log hệ thống",
                    DefaultExt = "csv",
                    FileName = $"SystemLog_{DateTime.Now:yyyyMMdd}"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;

                    // Tạo nội dung file
                    StringBuilder content = new StringBuilder();

                    // Tiêu đề cột
                    content.AppendLine("LogID,LogLevel,LogDate,Message,Source,AccountID,Username,IPAddress,Exception,StackTrace");

                    // Dữ liệu
                    foreach (var log in _systemLogs)
                    {
                        content.AppendLine(string.Join(",",
                            log.LogID,
                            EscapeCsvField(log.LogLevel),
                            log.LogDate,
                            EscapeCsvField(log.Message),
                            EscapeCsvField(log.Source),
                            log.AccountID,
                            EscapeCsvField(log.Username),
                            EscapeCsvField(log.IPAddress),
                            EscapeCsvField(log.Exception),
                            EscapeCsvField(log.StackTrace)
                        ));
                    }

                    // Ghi file
                    File.WriteAllText(saveFileDialog.FileName, content.ToString(), Encoding.UTF8);

                    Cursor = Cursors.Default;

                    MessageBox.Show($"Xuất log thành công!\nĐường dẫn: {saveFileDialog.FileName}",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;

                _logger.LogError("btnExport_Click", ex);
                MessageBox.Show($"Lỗi khi xuất log: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Escape chuỗi cho định dạng CSV
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // Bọc trong dấu ngoặc kép và escape các dấu ngoặc kép trong chuỗi
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Xóa log
        /// </summary>
        private void btnDeleteLogs_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpStartDate.Value > dtpEndDate.Value)
                {
                    MessageBox.Show("Ngày bắt đầu không thể sau ngày kết thúc!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa tất cả log từ {dtpStartDate.Value:dd/MM/yyyy} đến {dtpEndDate.Value:dd/MM/yyyy}?",
                    "Xác nhận xóa log",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Xóa log (cần thêm phương thức DeleteSystemLogs vào SystemConfigService)
                    // int deletedCount = _systemConfigService.DeleteSystemLogs(
                    //    dtpStartDate.Value, dtpEndDate.Value.AddDays(1).AddSeconds(-1), 
                    //    cboLogLevel.SelectedIndex > 0 ? cboLogLevel.Text : null);

                    int deletedCount = 0; // Tạm thời chưa có phương thức xóa

                    MessageBox.Show($"Đã xóa {deletedCount} bản ghi log.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tải lại log
                    LoadSystemLogs();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnDeleteLogs_Click", ex);
                MessageBox.Show($"Lỗi khi xóa log: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Khởi tạo các thành phần giao diện
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.btnDeleteLogs = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.cboLogLevel = new System.Windows.Forms.ComboBox();
            this.lblLogLevel = new System.Windows.Forms.Label();
            this.numMaxRows = new System.Windows.Forms.NumericUpDown();
            this.lblMaxRows = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvLogs = new System.Windows.Forms.DataGridView();
            this.lblTotalLogs = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.rtbLogDetails = new System.Windows.Forms.RichTextBox();
            this.lblDetails = new System.Windows.Forms.Label();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlFilter
            // 
            this.pnlFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFilter.Controls.Add(this.btnDeleteLogs);
            this.pnlFilter.Controls.Add(this.btnExport);
            this.pnlFilter.Controls.Add(this.btnClearSearch);
            this.pnlFilter.Controls.Add(this.btnSearch);
            this.pnlFilter.Controls.Add(this.txtSearch);
            this.pnlFilter.Controls.Add(this.lblSearch);
            this.pnlFilter.Controls.Add(this.cboLogLevel);
            this.pnlFilter.Controls.Add(this.lblLogLevel);
            this.pnlFilter.Controls.Add(this.numMaxRows);
            this.pnlFilter.Controls.Add(this.lblMaxRows);
            this.pnlFilter.Controls.Add(this.btnApply);
            this.pnlFilter.Controls.Add(this.dtpEndDate);
            this.pnlFilter.Controls.Add(this.lblEndDate);
            this.pnlFilter.Controls.Add(this.dtpStartDate);
            this.pnlFilter.Controls.Add(this.lblStartDate);
            this.pnlFilter.Location = new System.Drawing.Point(12, 12);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(980, 69);
            this.pnlFilter.TabIndex = 0;
            // 
            // btnDeleteLogs
            // 
            this.btnDeleteLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteLogs.Location = new System.Drawing.Point(902, 37);
            this.btnDeleteLogs.Name = "btnDeleteLogs";
            this.btnDeleteLogs.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteLogs.TabIndex = 14;
            this.btnDeleteLogs.Text = "Xóa log";
            this.btnDeleteLogs.UseVisualStyleBackColor = true;
            this.btnDeleteLogs.Click += new System.EventHandler(this.btnDeleteLogs_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(821, 37);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 13;
            this.btnExport.Text = "Xuất log";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearSearch.Location = new System.Drawing.Point(740, 37);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(75, 23);
            this.btnClearSearch.TabIndex = 12;
            this.btnClearSearch.Text = "Xóa tìm kiếm";
            this.btnClearSearch.UseVisualStyleBackColor = true;
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(655, 37);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(347, 39);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(302, 20);
            this.txtSearch.TabIndex = 10;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(288, 42);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(53, 13);
            this.lblSearch.TabIndex = 9;
            this.lblSearch.Text = "Tìm kiếm:";
            // 
            // cboLogLevel
            // 
            this.cboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLogLevel.FormattingEnabled = true;
            this.cboLogLevel.Location = new System.Drawing.Point(156, 39);
            this.cboLogLevel.Name = "cboLogLevel";
            this.cboLogLevel.Size = new System.Drawing.Size(121, 21);
            this.cboLogLevel.TabIndex = 8;
            // 
            // lblLogLevel
            // 
            this.lblLogLevel.AutoSize = true;
            this.lblLogLevel.Location = new System.Drawing.Point(102, 42);
            this.lblLogLevel.Name = "lblLogLevel";
            this.lblLogLevel.Size = new System.Drawing.Size(48, 13);
            this.lblLogLevel.TabIndex = 7;
            this.lblLogLevel.Text = "Mức độ:";
            // 
            // numMaxRows
            // 
            this.numMaxRows.Location = new System.Drawing.Point(68, 40);
            this.numMaxRows.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numMaxRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxRows.Name = "numMaxRows";
            this.numMaxRows.Size = new System.Drawing.Size(69, 20);
            this.numMaxRows.TabIndex = 6;
            this.numMaxRows.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblMaxRows
            // 
            this.lblMaxRows.AutoSize = true;
            this.lblMaxRows.Location = new System.Drawing.Point(3, 42);
            this.lblMaxRows.Name = "lblMaxRows";
            this.lblMaxRows.Size = new System.Drawing.Size(59, 13);
            this.lblMaxRows.TabIndex = 5;
            this.lblMaxRows.Text = "Số bản ghi:";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(902, 8);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Áp dụng";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndDate.Location = new System.Drawing.Point(588, 10);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(180, 20);
            this.dtpEndDate.TabIndex = 3;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(512, 13);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(74, 13);
            this.lblEndDate.TabIndex = 2;
            this.lblEndDate.Text = "Ngày kết thúc:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDate.Location = new System.Drawing.Point(331, 10);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(180, 20);
            this.dtpStartDate.TabIndex = 1;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(255, 13);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(72, 13);
            this.lblStartDate.TabIndex = 0;
            this.lblStartDate.Text = "Ngày bắt đầu:";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 87);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dgvLogs);
            this.splitContainer.Panel1.Controls.Add(this.lblTotalLogs);
            this.splitContainer.Panel1.Controls.Add(this.btnRefresh);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.rtbLogDetails);
            this.splitContainer.Panel2.Controls.Add(this.lblDetails);
            this.splitContainer.Size = new System.Drawing.Size(980, 503);
            this.splitContainer.SplitterDistance = 322;
            this.splitContainer.TabIndex = 1;
            // 
            // dgvLogs
            // 
            this.dgvLogs.AllowUserToAddRows = false;
            this.dgvLogs.AllowUserToDeleteRows = false;
            this.dgvLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLogs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogs.Location = new System.Drawing.Point(3, 29);
            this.dgvLogs.MultiSelect = false;
            this.dgvLogs.Name = "dgvLogs";
            this.dgvLogs.ReadOnly = true;
            this.dgvLogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLogs.Size = new System.Drawing.Size(974, 290);
            this.dgvLogs.TabIndex = 0;
            this.dgvLogs.SelectionChanged += new System.EventHandler(this.dgvLogs_SelectionChanged);
            // 
            // lblTotalLogs
            // 
            this.lblTotalLogs.AutoSize = true;
            this.lblTotalLogs.Location = new System.Drawing.Point(3, 9);
            this.lblTotalLogs.Name = "lblTotalLogs";
            this.lblTotalLogs.Size = new System.Drawing.Size(81, 13);
            this.lblTotalLogs.TabIndex = 2;
            this.lblTotalLogs.Text = "Tổng số bản ghi: 0";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(902, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // rtbLogDetails
            // 
            this.rtbLogDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLogDetails.Location = new System.Drawing.Point(3, 23);
            this.rtbLogDetails.Name = "rtbLogDetails";
            this.rtbLogDetails.ReadOnly = true;
            this.rtbLogDetails.Size = new System.Drawing.Size(974, 151);
            this.rtbLogDetails.TabIndex = 1;
            this.rtbLogDetails.Text = "";
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(3, 7);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(42, 13);
            this.lblDetails.TabIndex = 0;
            this.lblDetails.Text = "Chi tiết:";
            // 
            // SystemLogViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 602);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.pnlFilter);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "SystemLogViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xem nhật ký hệ thống";
            this.Load += new System.EventHandler(this.SystemLogViewerForm_Load);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxRows)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.Button