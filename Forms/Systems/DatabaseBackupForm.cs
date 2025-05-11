// File: Forms/System/DatabaseBackupForm.cs
// Mô tả: Form sao lưu và phục hồi cơ sở dữ liệu
// Tác giả: Steve-Thuong_hai
// Ngày tạo: 12/05/2025

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Forms.System
{
    /// <summary>
    /// Form sao lưu và phục hồi cơ sở dữ liệu
    /// </summary>
    public partial class DatabaseBackupForm : Form
    {
        private readonly ISystemConfigService _systemConfigService;
        private readonly AccountDTO _currentUser;
        private List<BackupDTO> _backupHistory;
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Khởi tạo form sao lưu và phục hồi cơ sở dữ liệu
        /// </summary>
        /// <param name="currentUser">Tài khoản đang đăng nhập</param>
        public DatabaseBackupForm(AccountDTO currentUser)
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
            dgvBackups.EnableHeadersVisualStyles = false;
            dgvBackups.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(219, 112, 147); // Màu hồng đậm
            dgvBackups.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBackups.ColumnHeadersDefaultCellStyle.Font = new Font(dgvBackups.Font, FontStyle.Bold);
            dgvBackups.BackgroundColor = Color.FromArgb(255, 240, 245);
            dgvBackups.RowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245);
            dgvBackups.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(253, 233, 242);
            dgvBackups.GridColor = Color.FromArgb(255, 192, 203);

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
                            }
                        }
                    }
                }
            }

            // Màu cho TabControl
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += (sender, e) =>
            {
                var tabPage = tabControl.TabPages[e.Index];
                var tabRect = tabControl.GetTabRect(e.Index);

                using (var brush = new SolidBrush(e.State == DrawItemState.Selected ?
                    Color.FromArgb(255, 240, 245) : Color.FromArgb(219, 112, 147)))
                {
                    e.Graphics.FillRectangle(brush, tabRect);
                }

                var textColor = e.State == DrawItemState.Selected ?
                    Color.FromArgb(219, 112, 147) : Color.White;
                var textBrush = new SolidBrush(textColor);
                var tabFont = e.State == DrawItemState.Selected ?
                    new Font(tabControl.Font, FontStyle.Bold) : tabControl.Font;

                var stringFlags = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                e.Graphics.DrawString(tabPage.Text, tabFont, textBrush, tabRect, stringFlags);
            };
        }

        /// <summary>
        /// Sự kiện khi form tải
        /// </summary>
        private void DatabaseBackupForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Tải đường dẫn sao lưu từ cấu hình
                string defaultBackupPath = _systemConfigService.GetConfigValue("DefaultBackupPath",
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Backups");
                txtBackupPath.Text = defaultBackupPath;

                // Kiểm tra và tạo thư mục sao lưu nếu chưa tồn tại
                if (!Directory.Exists(defaultBackupPath))
                {
                    Directory.CreateDirectory(defaultBackupPath);
                }

                // Tải lịch sử sao lưu
                dtpStartDate.Value = DateTime.Now.AddMonths(-1);
                dtpEndDate.Value = DateTime.Now;
                LoadBackupHistory();

                // Tải cấu hình lập lịch sao lưu
                LoadScheduleSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError("DatabaseBackupForm_Load", ex);
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải lịch sử sao lưu
        /// </summary>
        private void LoadBackupHistory()
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                _backupHistory = _systemConfigService.GetBackupHistory(
                    dtpStartDate.Value.Date, dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1));

                dgvBackups.DataSource = new BindingList<BackupDTO>(_backupHistory);

                // Cấu hình hiển thị cột
                if (dgvBackups.Columns.Count > 0)
                {
                    dgvBackups.Columns["BackupID"].HeaderText = "ID";
                    dgvBackups.Columns["BackupID"].Width = 50;

                    dgvBackups.Columns["BackupName"].HeaderText = "Tên bản sao lưu";
                    dgvBackups.Columns["BackupName"].Width = 200;

                    dgvBackups.Columns["BackupPath"].HeaderText = "Đường dẫn";
                    dgvBackups.Columns["BackupPath"].Width = 300;

                    dgvBackups.Columns["BackupSize"].HeaderText = "Kích thước";
                    dgvBackups.Columns["BackupSize"].DefaultCellStyle.Format = "N0";
                    dgvBackups.Columns["BackupSize"].Width = 100;

                    dgvBackups.Columns["BackupDate"].HeaderText = "Ngày sao lưu";
                    dgvBackups.Columns["BackupDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                    dgvBackups.Columns["BackupDate"].Width = 150;

                    dgvBackups.Columns["Status"].HeaderText = "Trạng thái";
                    dgvBackups.Columns["Status"].Width = 80;

                    dgvBackups.Columns["Note"].HeaderText = "Ghi chú";
                    dgvBackups.Columns["Note"].Width = 200;

                    dgvBackups.Columns["CreatedBy"].HeaderText = "Người tạo";
                    dgvBackups.Columns["CreatedBy"].Width = 120;

                    dgvBackups.Columns["EmployeeName"].HeaderText = "Tên nhân viên";
                    dgvBackups.Columns["EmployeeName"].Width = 150;
                }

                lblTotalBackups.Text = $"Tổng số bản sao lưu: {dgvBackups.RowCount}";
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadBackupHistory", ex);
                MessageBox.Show($"Lỗi khi tải lịch sử sao lưu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Tải cấu hình lập lịch sao lưu
        /// </summary>
        private void LoadScheduleSettings()
        {
            try
            {
                bool isEnabled = _systemConfigService.GetConfigValueBool("BackupSchedule_Enabled", false);
                string frequency = _systemConfigService.GetConfigValue("BackupSchedule_Frequency", "Daily");
                string timeStr = _systemConfigService.GetConfigValue("BackupSchedule_Time", "23:00:00");
                int daysToKeep = _systemConfigService.GetConfigValueInt("BackupSchedule_DaysToKeep", 30);

                chkEnableSchedule.Checked = isEnabled;

                switch (frequency)
                {
                    case "Daily":
                        radDaily.Checked = true;
                        break;
                    case "Weekly":
                        radWeekly.Checked = true;
                        break;
                    case "Monthly":
                        radMonthly.Checked = true;
                        break;
                    default:
                        radDaily.Checked = true;
                        break;
                }

                if (TimeSpan.TryParse(timeStr, out TimeSpan time))
                {
                    dtpBackupTime.Value = DateTime.Today.Add(time);
                }

                numDaysToKeep.Value = daysToKeep;

                // Cập nhật trạng thái kích hoạt
                EnableScheduleControls(isEnabled);
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadScheduleSettings", ex);
                MessageBox.Show($"Lỗi khi tải cấu hình lập lịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Kích hoạt/vô hiệu hóa các điều khiển lập lịch
        /// </summary>
        /// <param name="enabled">Trạng thái kích hoạt</param>
        private void EnableScheduleControls(bool enabled)
        {
            radDaily.Enabled = enabled;
            radWeekly.Enabled = enabled;
            radMonthly.Enabled = enabled;
            dtpBackupTime.Enabled = enabled;
            numDaysToKeep.Enabled = enabled;
            btnSaveSchedule.Enabled = enabled;
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Chọn đường dẫn
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog
                {
                    Description = "Chọn thư mục lưu bản sao lưu",
                    ShowNewFolderButton = true
                };

                if (!string.IsNullOrEmpty(txtBackupPath.Text) && Directory.Exists(txtBackupPath.Text))
                {
                    folderDialog.SelectedPath = txtBackupPath.Text;
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = folderDialog.SelectedPath;

                    // Lưu đường dẫn vào cấu hình
                    _systemConfigService.UpdateConfig(new SystemConfigDTO
                    {
                        ConfigKey = "DefaultBackupPath",
                        ConfigValue = txtBackupPath.Text
                    }, _currentUser.AccountID);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnBrowse_Click", ex);
                MessageBox.Show($"Lỗi khi chọn đường dẫn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Tạo bản sao lưu
        /// </summary>
        private void btnCreateBackup_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtBackupPath.Text))
                {
                    MessageBox.Show("Vui lòng chọn đường dẫn lưu bản sao lưu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(txtBackupPath.Text))
                {
                    Directory.CreateDirectory(txtBackupPath.Text);
                }

                // Tạo tên bản sao lưu mới
                string backupName = $"QuanLyCuaHangTienLoi_Full_{DateTime.Now:yyyyMMdd_HHmmss}";

                Cursor = Cursors.WaitCursor;
                lblStatus.Text = "Đang tạo bản sao lưu...";
                Application.DoEvents();

                // Gọi service để tạo bản sao lưu
                string result = _systemConfigService.CreateDatabaseBackup(
                    txtBackupPath.Text, backupName, _currentUser.AccountID);

                Cursor = Cursors.Default;

                MessageBox.Show(result, "Kết quả sao lưu",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại lịch sử sao lưu
                LoadBackupHistory();
                lblStatus.Text = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                lblStatus.Text = "Sẵn sàng";

                _logger.LogError("btnCreateBackup_Click", ex);
                MessageBox.Show($"Lỗi khi tạo bản sao lưu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Phục hồi
        /// </summary>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBackups.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn bản sao lưu để phục hồi!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                BackupDTO backup = dgvBackups.CurrentRow.DataBoundItem as BackupDTO;

                if (backup == null)
                {
                    MessageBox.Show("Không thể phục hồi bản sao lưu đã chọn!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (backup.Status != "Success")
                {
                    MessageBox.Show("Chỉ có thể phục hồi từ bản sao lưu thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(backup.BackupPath))
                {
                    MessageBox.Show("Không tìm thấy file sao lưu tại đường dẫn chỉ định!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult confirmResult = MessageBox.Show(
                    "CẢNH BÁO: Phục hồi CSDL sẽ ghi đè lên tất cả dữ liệu hiện tại!\n\n" +
                    "Bạn có chắc chắn muốn phục hồi từ bản sao lưu này không?\n\n" +
                    $"Bản sao lưu: {backup.BackupName}\n" +
                    $"Ngày tạo: {backup.BackupDate}\n\n" +
                    "Lưu ý: Ứng dụng sẽ tự động đóng sau khi phục hồi hoàn tất.",
                    "Xác nhận phục hồi CSDL",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    lblStatus.Text = "Đang phục hồi CSDL...";
                    Application.DoEvents();

                    // Gọi service để phục hồi CSDL
                    string result = _systemConfigService.RestoreDatabase(
                        backup.BackupPath, _currentUser.AccountID);

                    Cursor = Cursors.Default;

                    MessageBox.Show(result + "\n\nỨng dụng sẽ đóng sau khi bạn nhấn OK.",
                        "Kết quả phục hồi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Đóng ứng dụng để khởi động lại với CSDL mới
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                lblStatus.Text = "Sẵn sàng";

                _logger.LogError("btnRestore_Click", ex);
                MessageBox.Show($"Lỗi khi phục hồi CSDL: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Xóa
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBackups.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn bản sao lưu để xóa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                BackupDTO backup = dgvBackups.CurrentRow.DataBoundItem as BackupDTO;

                if (backup == null)
                {
                    MessageBox.Show("Không thể xóa bản sao lưu đã chọn!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult confirmResult = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa bản sao lưu này không?\n\n" +
                    $"Bản sao lưu: {backup.BackupName}\n" +
                    $"Ngày tạo: {backup.BackupDate}",
                    "Xác nhận xóa bản sao lưu",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    if (File.Exists(backup.BackupPath))
                    {
                        // Xóa file sao lưu
                        File.Delete(backup.BackupPath);

                        // Xóa bản ghi trong CSDL (nếu có)
                        // (Lưu ý: Bạn cần thêm phương thức DeleteBackup vào SystemConfigService)

                        MessageBox.Show("Xóa bản sao lưu thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Tải lại lịch sử sao lưu
                        LoadBackupHistory();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy file sao lưu tại đường dẫn chỉ định!\n" +
                            "File có thể đã bị xóa từ trước.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Tải lại lịch sử sao lưu
                        LoadBackupHistory();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnDelete_Click", ex);
                MessageBox.Show($"Lỗi khi xóa bản sao lưu: {ex.Message}", "Lỗi",
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
                LoadBackupHistory();
            }
            catch (Exception ex)
            {
                _logger.LogError("btnRefresh_Click", ex);
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Áp dụng ngày
        /// </summary>
        private void btnApplyDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpStartDate.Value > dtpEndDate.Value)
                {
                    MessageBox.Show("Ngày bắt đầu không thể sau ngày kết thúc!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoadBackupHistory();
            }
            catch (Exception ex)
            {
                _logger.LogError("btnApplyDate_Click", ex);
                MessageBox.Show($"Lỗi khi lọc dữ liệu theo ngày: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi thay đổi trạng thái kích hoạt lập lịch
        /// </summary>
        private void chkEnableSchedule_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                EnableScheduleControls(chkEnableSchedule.Checked);
            }
            catch (Exception ex)
            {
                _logger.LogError("chkEnableSchedule_CheckedChanged", ex);
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Lưu lịch
        /// </summary>
        private void btnSaveSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtBackupPath.Text))
                {
                    MessageBox.Show("Vui lòng chọn đường dẫn lưu bản sao lưu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy tần suất sao lưu
                string frequency = "Daily";
                if (radWeekly.Checked)
                    frequency = "Weekly";
                else if (radMonthly.Checked)
                    frequency = "Monthly";

                // Lấy thời gian sao lưu
                TimeSpan time = dtpBackupTime.Value.TimeOfDay;

                // Lấy số ngày giữ lại
                int daysToKeep = (int)numDaysToKeep.Value;

                bool result = _systemConfigService.ScheduleBackup(
                    txtBackupPath.Text, frequency, time, daysToKeep, _currentUser.AccountID);

                if (result)
                {
                    MessageBox.Show("Lưu cấu hình lập lịch sao lưu thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Lưu cấu hình lập lịch sao lưu thất bại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnSaveSchedule_Click", ex);
                MessageBox.Show($"Lỗi khi lưu lịch sao lưu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Xóa bản sao lưu cũ
        /// </summary>
        private void btnDeleteOldBackups_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtDaysToKeep.Text, out int daysToKeep) || daysToKeep < 1)
                {
                    MessageBox.Show("Vui lòng nhập số ngày hợp lệ (lớn hơn 0)!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult confirmResult = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa tất cả bản sao lưu cũ hơn {daysToKeep} ngày không?",
                    "Xác nhận xóa bản sao lưu cũ",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    lblStatus.Text = "Đang xóa bản sao lưu cũ...";
                    Application.DoEvents();

                    string result = _systemConfigService.DeleteOldBackups(
                        daysToKeep, _currentUser.AccountID);

                    Cursor = Cursors.Default;
                    lblStatus.Text = "Sẵn sàng";

                    MessageBox.Show(result, "Kết quả xóa bản sao lưu cũ",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tải lại lịch sử sao lưu
                    LoadBackupHistory();
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                lblStatus.Text = "Sẵn sàng";

                _logger.LogError("btnDeleteOldBackups_Click", ex);
                MessageBox.Show($"Lỗi khi xóa bản sao lưu cũ: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Tối ưu CSDL
        /// </summary>
        private void btnOptimizeDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult confirmResult = MessageBox.Show(
                    "Bạn có muốn tối ưu hóa cơ sở dữ liệu không?\n\n" +
                    "Quá trình này có thể mất vài phút tùy thuộc vào kích thước CSDL.\n" +
                    "Bạn nên tạo bản sao lưu trước khi thực hiện tối ưu hóa.",
                    "Xác nhận tối ưu hóa CSDL",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    lblStatus.Text = "Đang tối ưu hóa CSDL...";
                    Application.DoEvents();

                    string result = _systemConfigService.OptimizeDatabase(_currentUser.AccountID);

                    Cursor = Cursors.Default;
                    lblStatus.Text = "Sẵn sàng";

                    MessageBox.Show(result, "Kết quả tối ưu hóa CSDL",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                lblStatus.Text = "Sẵn sàng";

                _logger.LogError("btnOptimizeDatabase_Click", ex);
                MessageBox.Show($"Lỗi khi tối ưu hóa CSDL: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Khởi tạo các thành phần giao diện
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabBackup = new System.Windows.Forms.TabPage();
            this.btnOptimizeDatabase = new System.Windows.Forms.Button();
            this.btnDeleteOldBackups = new System.Windows.Forms.Button();
            this.txtDaysToKeep = new System.Windows.Forms.TextBox();
            this.lblDaysToKeep = new System.Windows.Forms.Label();
            this.pnlDate = new System.Windows.Forms.Panel();
            this.btnApplyDate = new System.Windows.Forms.Button();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblTotalBackups = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnCreateBackup = new System.Windows.Forms.Button();
            this.dgvBackups = new System.Windows.Forms.DataGridView();
            this.pnlPath = new System.Windows.Forms.Panel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.lblBackupPath = new System.Windows.Forms.Label();
            this.tabSchedule = new System.Windows.Forms.TabPage();
            this.btnSaveSchedule = new System.Windows.Forms.Button();
            this.numDaysToKeep = new System.Windows.Forms.NumericUpDown();
            this.lblScheduleDaysToKeep = new System.Windows.Forms.Label();
            this.dtpBackupTime = new System.Windows.Forms.DateTimePicker();
            this.lblBackupTime = new System.Windows.Forms.Label();
            this.grpFrequency = new System.Windows.Forms.GroupBox();
            this.radMonthly = new System.Windows.Forms.RadioButton();
            this.radWeekly = new System.Windows.Forms.RadioButton();
            this.radDaily = new System.Windows.Forms.RadioButton();
            this.chkEnableSchedule = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            this.tabBackup.SuspendLayout();
            this.pnlDate.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).BeginInit();
            this.pnlPath.SuspendLayout();
            this.tabSchedule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDaysToKeep)).BeginInit();
            this.grpFrequency.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabBackup);
            this.tabControl.Controls.Add(this.tabSchedule);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(804, 602);
            this.tabControl.TabIndex = 0;
            // 
            // tabBackup
            // 
            this.tabBackup.Controls.Add(this.btnOptimizeDatabase);
            this.tabBackup.Controls.Add(this.btnDeleteOldBackups);
            this.tabBackup.Controls.Add(this.txtDaysToKeep);
            this.tabBackup.Controls.Add(this.lblDaysToKeep);
            this.tabBackup.Controls.Add(this.pnlDate);
            this.tabBackup.Controls.Add(this.lblStatus);
            this.tabBackup.Controls.Add(this.lblTotalBackups);
            this.tabBackup.Controls.Add(this.pnlButtons);
            this.tabBackup.Controls.Add(this.dgvBackups);
            this.tabBackup.Controls.Add(this.pnlPath);
            this.tabBackup.Location = new System.Drawing.Point(4, 22);
            this.tabBackup.Name = "tabBackup";
            this.tabBackup.Padding = new System.Windows.Forms.Padding(3);
            this.tabBackup.Size = new System.Drawing.Size(796, 576);
            this.tabBackup.TabIndex = 0;
            this.tabBackup.Text = "Sao lưu và phục hồi";
            this.tabBackup.UseVisualStyleBackColor = true;
            // 
            // btnOptimizeDatabase
            // 
            this.btnOptimizeDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptimizeDatabase.Location = new System.Drawing.Point(700, 538);
            this.btnOptimizeDatabase.Name = "btnOptimizeDatabase";
            this.btnOptimizeDatabase.Size = new System.Drawing.Size(90, 30);
            this.btnOptimizeDatabase.TabIndex = 9;
            this.btnOptimizeDatabase.Text = "Tối ưu CSDL";
            this.btnOptimizeDatabase.UseVisualStyleBackColor = true;
            this.btnOptimizeDatabase.Click += new System.EventHandler(this.btnOptimizeDatabase_Click);
            // 
            // btnDeleteOldBackups
            // 
            this.btnDeleteOldBackups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteOldBackups.Location = new System.Drawing.Point(604, 538);
            this.btnDeleteOldBackups.Name = "btnDeleteOldBackups";
            this.btnDeleteOldBackups.Size = new System.Drawing.Size(90, 30);
            this.btnDeleteOldBackups.TabIndex = 8;
            this.btnDeleteOldBackups.Text = "Xóa cũ hơn";
            this.btnDeleteOldBackups.UseVisualStyleBackColor = true;
            this.btnDeleteOldBackups.Click += new System.EventHandler(this.btnDeleteOldBackups_Click);
            // 
            // txtDaysToKeep
            // 
            this.txtDaysToKeep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDaysToKeep.Location = new System.Drawing.Point(556, 544);
            this.txtDaysToKeep.Name = "txtDaysToKeep";
            this.txtDaysToKeep.Size = new System.Drawing.Size(42, 20);
            this.txtDaysToKeep.TabIndex = 7;
            this.txtDaysToKeep.Text = "30";
            // 
            // lblDaysToKeep
            // 
            this.lblDaysToKeep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDaysToKeep.AutoSize = true;
            this.lblDaysToKeep.Location = new System.Drawing.Point(493, 547);
            this.lblDaysToKeep.Name = "lblDaysToKeep";
            this.lblDaysToKeep.Size = new System.Drawing.Size(57, 13);
            this.lblDaysToKeep.TabIndex = 6;
            this.lblDaysToKeep.Text = "Số ngày:";
            // 
            // pnlDate
            // 
            this.pnlDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDate.Controls.Add(this.btnApplyDate);
            this.pnlDate.Controls.Add(this.dtpEndDate);
            this.pnlDate.Controls.Add(this.lblEndDate);
            this.pnlDate.Controls.Add(this.dtpStartDate);
            this.pnlDate.Controls.Add(this.lblStartDate);
            this.pnlDate.Location = new System.Drawing.Point(8, 45);
            this.pnlDate.Name = "pnlDate";
            this.pnlDate.Size = new System.Drawing.Size(780, 33);
            this.pnlDate.TabIndex = 5;
            // 
            // btnApplyDate
            // 
            this.btnApplyDate.Location = new System.Drawing.Point(492, 5);
            this.btnApplyDate.Name = "btnApplyDate";
            this.btnApplyDate.Size = new System.Drawing.Size(75, 23);
            this.btnApplyDate.TabIndex = 4;
            this.btnApplyDate.Text = "Áp dụng";
            this.btnApplyDate.UseVisualStyleBackColor = true;
            this.btnApplyDate.Click += new System.EventHandler(this.btnApplyDate_Click);
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(349, 6);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(137, 20);
            this.dtpEndDate.TabIndex = 3;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(273, 10);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(74, 13);
            this.lblEndDate.TabIndex = 2;
            this.lblEndDate.Text = "Ngày kết thúc:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(82, 6);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(137, 20);
            this.dtpStartDate.TabIndex = 1;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(3, 10);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(72, 13);
            this.lblStartDate.TabIndex = 0;
            this.lblStartDate.Text = "Ngày bắt đầu:";
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(8, 547);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(55, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Sẵn sàng";
            // 
            // lblTotalBackups
            // 
            this.lblTotalBackups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalBackups.AutoSize = true;
            this.lblTotalBackups.Location = new System.Drawing.Point(8, 492);
            this.lblTotalBackups.Name = "lblTotalBackups";
            this.lblTotalBackups.Size = new System.Drawing.Size(104, 13);
            this.lblTotalBackups.TabIndex = 3;
            this.lblTotalBackups.Text = "Tổng số bản sao lưu: 0";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Controls.Add(this.btnDelete);
            this.pnlButtons.Controls.Add(this.btnRestore);
            this.pnlButtons.Controls.Add(this.btnCreateBackup);
            this.pnlButtons.Location = new System.Drawing.Point(8, 508);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(782, 24);
            this.pnlButtons.TabIndex = 2;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(3, 0);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(165, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestore.Location = new System.Drawing.Point(704, 0);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(75, 23);
            this.btnRestore.TabIndex = 1;
            this.btnRestore.Text = "Phục hồi";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnCreateBackup
            // 
            this.btnCreateBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateBackup.Location = new System.Drawing.Point(623, 0);
            this.btnCreateBackup.Name = "btnCreateBackup";
            this.btnCreateBackup.Size = new System.Drawing.Size(75, 23);
            this.btnCreateBackup.TabIndex = 0;
            this.btnCreateBackup.Text = "Sao lưu";
            this.btnCreateBackup.UseVisualStyleBackColor = true;
            this.btnCreateBackup.Click += new System.EventHandler(this.btnCreateBackup_Click);
            // 
            // dgvBackups
            // 
            this.dgvBackups.AllowUserToAddRows = false;
            this.dgvBackups.AllowUserToDeleteRows = false;
            this.dgvBackups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBackups.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBackups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBackups.Location = new System.Drawing.Point(6, 84);
            this.dgvBackups.MultiSelect = false;
            this.dgvBackups.Name = "dgvBackups";
            this.dgvBackups.ReadOnly = true;
            this.dgvBackups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBackups.Size = new System.Drawing.Size(784, 405);
            this.dgvBackups.TabIndex = 1;
            // 
            // pnlPath
            // 
            this.pnlPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPath.Controls.Add(this.btnBrowse);
            this.pnlPath.Controls.Add(this.txtBackupPath);
            this.pnlPath.Controls.Add(this.lblBackupPath);
            this.pnlPath.Location = new System.Drawing.Point(8, 6);
            this.pnlPath.Name = "pnlPath";
            this.pnlPath.Size = new System.Drawing.Size(780, 33);
            this.pnlPath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(704, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Chọn...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupPath.Location = new System.Drawing.Point(105, 6);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Size = new System.Drawing.Size(593, 20);
            this.txtBackupPath.TabIndex = 1;
            // 
            // lblBackupPath
            // 
            this.lblBackupPath.AutoSize = true;
            this.lblBackupPath.Location = new System.Drawing.Point(3, 9);
            this.lblBackupPath.Name = "lblBackupPath";
            this.lblBackupPath.Size = new System.Drawing.Size(96, 13);
            this.lblBackupPath.TabIndex = 0;
            this.lblBackupPath.Text = "Đường dẫn sao lưu:";
            // 
            // tabSchedule
            // 
            this.tabSchedule.Controls.Add(this.btnSaveSchedule);
            this.tabSchedule.Controls.Add(this.numDaysToKeep);
            this.tabSchedule.Controls.Add(this.lblScheduleDaysToKeep);
            this.tabSchedule.Controls.Add(this.dtpBackupTime);
            this.tabSchedule.Controls.Add(this.lblBackupTime);
            this.tabSchedule.Controls.Add(this.grpFrequency);
            this.tabSchedule.Controls.Add(this.chkEnableSchedule);
            this.tabSchedule.Location = new System.Drawing.Point(4, 22);
            this.tabSchedule.Name = "tabSchedule";
            this.tabSchedule.Padding = new System.Windows.Forms.Padding(3);
            this.tabSchedule.Size = new System.Drawing.Size(796, 576);
            this.tabSchedule.TabIndex = 1;
            this.tabSchedule.Text = "Lập lịch tự động";
            this.tabSchedule.UseVisualStyleBackColor = true;
            // 
            // btnSaveSchedule
            // 
            this.btnSaveSchedule.Location = new System.Drawing.Point(144, 244);
            this.btnSaveSchedule.Name = "btnSaveSchedule";
            this.btnSaveSchedule.Size = new System.Drawing.Size(90, 35);
            this.btnSaveSchedule.TabIndex = 6;
            this.btnSaveSchedule.Text = "Lưu lịch";
            this.btnSaveSchedule.UseVisualStyleBackColor = true;
            this.btnSaveSchedule.Click += new System.EventHandler(this.btnSaveSchedule_Click);
            // 
            // numDaysToKeep
            // 
            this.numDaysToKeep.Location = new System.Drawing.Point(144, 204);
            this.numDaysToKeep.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.numDaysToKeep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDaysToKeep.Name = "numDaysToKeep";
            this.numDaysToKeep.Size = new System.Drawing.Size(80, 20);
            this.numDaysToKeep.TabIndex = 5;
            this.numDaysToKeep.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblScheduleDaysToKeep
            // 
            this.lblScheduleDaysToKeep.AutoSize = true;
            this.lblScheduleDaysToKeep.Location = new System.Drawing.Point(24, 206);
            this.lblScheduleDaysToKeep.Name = "lblScheduleDaysToKeep";
            this.lblScheduleDaysToKeep.Size = new System.Drawing.Size(114, 13);
            this.lblScheduleDaysToKeep.TabIndex = 4;
            this.lblScheduleDaysToKeep.Text = "Chỉ giữ trong (số ngày):";
            // 
            // dtpBackupTime
            // 
            this.dtpBackupTime.CustomFormat = "HH:mm:ss";
            this.dtpBackupTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBackupTime.Location = new System.Drawing.Point(144, 168);
            this.dtpBackupTime.Name = "dtpBackupTime";
            this.dtpBackupTime.ShowUpDown = true;
            this.dtpBackupTime.Size = new System.Drawing.Size(80, 20);
            this.dtpBackupTime.TabIndex = 3;
            this.dtpBackupTime.Value = new System.DateTime(2021, 1, 1, 23, 0, 0, 0);
            // 
            // lblBackupTime
            // 
            this.lblBackupTime.AutoSize = true;
            this.lblBackupTime.Location = new System.Drawing.Point(24, 172);
            this.lblBackupTime.Name = "lblBackupTime";
            this.lblBackupTime.Size = new System.Drawing.Size(85, 13);
            this.lblBackupTime.TabIndex = 2;
            this.lblBackupTime.Text = "Thời gian sao lưu:";
            // 
            // grpFrequency
            // 
            this.grpFrequency.Controls.Add(this.radMonthly);
            this.grpFrequency.Controls.Add(this.radWeekly);
            this.grpFrequency.Controls.Add(this.radDaily);
            this.grpFrequency.Location = new System.Drawing.Point(24, 68);
            this.grpFrequency.Name = "grpFrequency";
            this.grpFrequency.Size = new System.Drawing.Size(327, 84);
            this.grpFrequency.TabIndex = 1;
            this.grpFrequency.TabStop = false;
            this.grpFrequency.Text = "Tần suất";
            // 
            // radMonthly
            // 
            this.radMonthly.AutoSize = true;
            this.radMonthly.Location = new System.Drawing.Point(235, 37);
            this.radMonthly.Name = "radMonthly";
            this.radMonthly.Size = new System.Drawing.Size(70, 17);
            this.radMonthly.TabIndex = 2;
            this.radMonthly.Text = "Hàng tháng";
            this.radMonthly.UseVisualStyleBackColor = true;
            // 
            // radWeekly
            // 
            this.radWeekly.AutoSize = true;
            this.radWeekly.Location = new System.Drawing.Point(130, 37);
            this.radWeekly.Name = "radWeekly";
            this.radWeekly.Size = new System.Drawing.Size(75, 17);
            this.radWeekly.TabIndex = 1;
            this.radWeekly.Text = "Hàng tuần";
            this.radWeekly.UseVisualStyleBackColor = true;
            // 
            // radDaily
            // 
            this.radDaily.AutoSize = true;
            this.radDaily.Checked = true;
            this.radDaily.Location = new System.Drawing.Point(29, 37);
            this.radDaily.Name = "radDaily";
            this.radDaily.Size = new System.Drawing.Size(73, 17);
            this.radDaily.TabIndex = 0;
            this.radDaily.TabStop = true;
            this.radDaily.Text = "Hàng ngày";
            this.radDaily.UseVisualStyleBackColor = true;
            // 
            // chkEnableSchedule
            // 
            this.chkEnableSchedule.AutoSize = true;
            this.chkEnableSchedule.Location = new System.Drawing.Point(24, 27);
            this.chkEnableSchedule.Name = "chkEnableSchedule";
            this.chkEnableSchedule.Size = new System.Drawing.Size(153, 17);
            this.chkEnableSchedule.TabIndex = 0;
            this.chkEnableSchedule.Text = "Kích hoạt lập lịch tự động";
            this.chkEnableSchedule.UseVisualStyleBackColor = true;
            this.chkEnableSchedule.CheckedChanged += new System.EventHandler(this.chkEnableSchedule_CheckedChanged);
            // 
            // DatabaseBackupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 602);
            this.Controls.Add(this.tabControl);
            this.MinimumSize = new System.Drawing.Size(820, 640);
            this.Name = "DatabaseBackupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sao lưu và phục hồi cơ sở dữ liệu";
            this.Load += new System.EventHandler(this.DatabaseBackupForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabBackup.ResumeLayout(false);
            this.tabBackup.PerformLayout();
            this.pnlDate.ResumeLayout(false);
            this.pnlDate.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).EndInit();
            this.pnlPath.ResumeLayout(false);
            this.pnlPath.PerformLayout();
            this.tabSchedule.ResumeLayout(false);
            this.tabSchedule.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDaysToKeep)).EndInit();
            this.grpFrequency.ResumeLayout(false);
            this.grpFrequency.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabBackup;
        private System.Windows.Forms.TabPage tabSchedule;
        private System.Windows.Forms.Panel pnlPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.Label lblBackupPath;
        private System.Windows.Forms.DataGridView dgvBackups;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnCreateBackup;
        private System.Windows.Forms.Label lblTotalBackups;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlDate;
        private System.Windows.Forms.Button btnApplyDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.Button btnDeleteOldBackups;
        private System.Windows.Forms.TextBox txtDaysToKeep;
        private System.Windows.Forms.Label lblDaysToKeep;
        private System.Windows.Forms.Button btnOptimizeDatabase;
        private System.Windows.Forms.CheckBox chkEnableSchedule;
        private System.Windows.Forms.GroupBox grpFrequency;
        private System.Windows.Forms.RadioButton radMonthly;
        private System.Windows.Forms.RadioButton radWeekly;
        private System.Windows.Forms.RadioButton radDaily;
        private System.Windows.Forms.DateTimePicker dtpBackupTime;
        private System.Windows.Forms.Label lblBackupTime;
        private System.Windows.Forms.NumericUpDown numDaysToKeep;
        private System.Windows.Forms.Label lblScheduleDaysToKeep;
        private System.Windows.Forms.Button btnSaveSchedule;
        private System.Windows.Forms.Button btnRefresh;
    }
}