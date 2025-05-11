// File: Forms/System/SystemConfigForm.cs
// Mô tả: Form quản lý cấu hình hệ thống
// Tác giả: Steve-Thuong_hai
// Ngày tạo: 12/05/2025

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Forms.System
{
    /// <summary>
    /// Form quản lý cấu hình hệ thống
    /// </summary>
    public partial class SystemConfigForm : Form
    {
        private readonly ISystemConfigService _systemConfigService;
        private readonly AccountDTO _currentUser;
        private List<SystemConfigDTO> _allConfigs;
        private SystemConfigDTO _selectedConfig;
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Khởi tạo form quản lý cấu hình hệ thống
        /// </summary>
        /// <param name="currentUser">Tài khoản đang đăng nhập</param>
        public SystemConfigForm(AccountDTO currentUser)
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
            dgvConfigs.EnableHeadersVisualStyles = false;
            dgvConfigs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(219, 112, 147); // Màu hồng đậm
            dgvConfigs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvConfigs.ColumnHeadersDefaultCellStyle.Font = new Font(dgvConfigs.Font, FontStyle.Bold);
            dgvConfigs.BackgroundColor = Color.FromArgb(255, 240, 245);
            dgvConfigs.RowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245);
            dgvConfigs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(253, 233, 242);
            dgvConfigs.GridColor = Color.FromArgb(255, 192, 203);

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
                        else if (gbControl is ComboBox comboBox)
                        {
                            comboBox.BackColor = Color.FromArgb(255, 250, 250);
                            comboBox.FlatStyle = FlatStyle.Flat;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sự kiện khi form tải
        /// </summary>
        private void SystemConfigForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Tải danh sách cấu hình
                LoadConfigurations();

                // Thiết lập danh sách loại dữ liệu
                cboDataType.Items.Clear();
                cboDataType.Items.Add("");
                cboDataType.Items.Add("String");
                cboDataType.Items.Add("Number");
                cboDataType.Items.Add("Boolean");
                cboDataType.Items.Add("DateTime");

                // Thiết lập danh sách bộ lọc
                cboFilter.Items.Clear();
                cboFilter.Items.Add("Tất cả cấu hình");
                cboFilter.Items.Add("Cấu hình hệ thống");
                cboFilter.Items.Add("Cấu hình giao diện");
                cboFilter.Items.Add("Cấu hình sao lưu");
                cboFilter.Items.Add("Cấu hình báo cáo");
                cboFilter.SelectedIndex = 0;

                // Thiết lập trạng thái ban đầu
                EnableConfigControls(false);
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigForm_Load", ex);
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải danh sách cấu hình
        /// </summary>
        private void LoadConfigurations()
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                _allConfigs = _systemConfigService.GetAllConfigs();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadConfigurations", ex);
                MessageBox.Show($"Lỗi khi tải cấu hình: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Áp dụng bộ lọc
        /// </summary>
        private void ApplyFilter()
        {
            try
            {
                if (_allConfigs == null)
                    return;

                IEnumerable<SystemConfigDTO> filteredConfigs = _allConfigs;

                // Áp dụng bộ lọc thể loại
                switch (cboFilter.SelectedIndex)
                {
                    case 1: // Cấu hình hệ thống
                        filteredConfigs = filteredConfigs.Where(c => c.ConfigKey.StartsWith("System"));
                        break;
                    case 2: // Cấu hình giao diện
                        filteredConfigs = filteredConfigs.Where(c => c.ConfigKey.StartsWith("UI") || c.ConfigKey.StartsWith("Theme"));
                        break;
                    case 3: // Cấu hình sao lưu
                        filteredConfigs = filteredConfigs.Where(c => c.ConfigKey.StartsWith("Backup") || c.ConfigKey.Contains("Backup"));
                        break;
                    case 4: // Cấu hình báo cáo
                        filteredConfigs = filteredConfigs.Where(c => c.ConfigKey.StartsWith("Report") || c.ConfigKey.Contains("Report"));
                        break;
                }

                // Áp dụng tìm kiếm
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    filteredConfigs = filteredConfigs.Where(c =>
                        c.ConfigKey.ToLower().Contains(search) ||
                        c.ConfigValue.ToLower().Contains(search) ||
                        (c.Description != null && c.Description.ToLower().Contains(search))
                    );
                }

                // Hiển thị kết quả
                dgvConfigs.DataSource = new BindingList<SystemConfigDTO>(filteredConfigs.ToList());

                // Cấu hình hiển thị cột
                if (dgvConfigs.Columns.Count > 0)
                {
                    dgvConfigs.Columns["ConfigID"].HeaderText = "ID";
                    dgvConfigs.Columns["ConfigID"].Width = 50;

                    dgvConfigs.Columns["ConfigKey"].HeaderText = "Khóa";
                    dgvConfigs.Columns["ConfigKey"].Width = 200;

                    dgvConfigs.Columns["ConfigValue"].HeaderText = "Giá trị";
                    dgvConfigs.Columns["ConfigValue"].Width = 200;

                    dgvConfigs.Columns["Description"].HeaderText = "Mô tả";
                    dgvConfigs.Columns["Description"].Width = 250;

                    dgvConfigs.Columns["DataType"].HeaderText = "Loại dữ liệu";
                    dgvConfigs.Columns["DataType"].Width = 100;

                    dgvConfigs.Columns["IsReadOnly"].HeaderText = "Chỉ đọc";
                    dgvConfigs.Columns["IsReadOnly"].Width = 70;

                    dgvConfigs.Columns["CreatedDate"].HeaderText = "Ngày tạo";
                    dgvConfigs.Columns["CreatedDate"].Width = 120;
                    dgvConfigs.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                    dgvConfigs.Columns["ModifiedDate"].HeaderText = "Ngày sửa";
                    dgvConfigs.Columns["ModifiedDate"].Width = 120;
                    dgvConfigs.Columns["ModifiedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }

                lblTotalRows.Text = $"Tổng số cấu hình: {dgvConfigs.RowCount}";
            }
            catch (Exception ex)
            {
                _logger.LogError("ApplyFilter", ex);
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Kích hoạt/vô hiệu hóa các điều khiển cấu hình
        /// </summary>
        /// <param name="enabled">Trạng thái kích hoạt</param>
        private void EnableConfigControls(bool enabled)
        {
            // Các điều khiển nhập liệu
            txtConfigKey.Enabled = enabled && _selectedConfig == null;
            txtConfigValue.Enabled = enabled && (_selectedConfig == null || !_selectedConfig.IsReadOnly);
            txtDescription.Enabled = enabled;
            cboDataType.Enabled = enabled && _selectedConfig == null;
            chkReadOnly.Enabled = enabled && _selectedConfig == null;

            // Các nút tác vụ
            btnSave.Enabled = enabled;
            btnCancel.Enabled = enabled;

            // Các nút chính
            btnAdd.Enabled = !enabled;
            btnEdit.Enabled = !enabled && _selectedConfig != null;
            btnDelete.Enabled = !enabled && _selectedConfig != null && !_selectedConfig.IsReadOnly;
            btnRefresh.Enabled = !enabled;
            btnExport.Enabled = !enabled;
            btnImport.Enabled = !enabled;
        }

        /// <summary>
        /// Xóa trắng các điều khiển nhập liệu
        /// </summary>
        private void ClearConfigControls()
        {
            txtConfigKey.Text = string.Empty;
            txtConfigValue.Text = string.Empty;
            txtDescription.Text = string.Empty;
            cboDataType.SelectedIndex = -1;
            chkReadOnly.Checked = false;
        }

        /// <summary>
        /// Hiển thị thông tin cấu hình đã chọn
        /// </summary>
        private void DisplaySelectedConfig()
        {
            if (_selectedConfig == null)
            {
                ClearConfigControls();
                return;
            }

            txtConfigKey.Text = _selectedConfig.ConfigKey;
            txtConfigValue.Text = _selectedConfig.ConfigValue;
            txtDescription.Text = _selectedConfig.Description;
            cboDataType.Text = _selectedConfig.DataType;
            chkReadOnly.Checked = _selectedConfig.IsReadOnly;
        }

        /// <summary>
        /// Sự kiện khi chọn một dòng trong DataGridView
        /// </summary>
        private void dgvConfigs_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvConfigs.CurrentRow != null)
            {
                _selectedConfig = dgvConfigs.CurrentRow.DataBoundItem as SystemConfigDTO;
                DisplaySelectedConfig();

                // Cập nhật trạng thái các nút
                btnEdit.Enabled = _selectedConfig != null;
                btnDelete.Enabled = _selectedConfig != null && !_selectedConfig.IsReadOnly;
            }
            else
            {
                _selectedConfig = null;
                ClearConfigControls();
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Thêm
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                _selectedConfig = null;
                ClearConfigControls();
                EnableConfigControls(true);
                txtConfigKey.Focus();
            }
            catch (Exception ex)
            {
                _logger.LogError("btnAdd_Click", ex);
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Sửa
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedConfig == null)
                {
                    MessageBox.Show("Vui lòng chọn cấu hình cần sửa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                EnableConfigControls(true);
                txtConfigValue.Focus();
            }
            catch (Exception ex)
            {
                _logger.LogError("btnEdit_Click", ex);
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
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
                if (_selectedConfig == null)
                {
                    MessageBox.Show("Vui lòng chọn cấu hình cần xóa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_selectedConfig.IsReadOnly)
                {
                    MessageBox.Show("Không thể xóa cấu hình chỉ đọc!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa cấu hình '{_selectedConfig.ConfigKey}'?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _systemConfigService.DeleteConfig(_selectedConfig.ConfigKey, _currentUser.AccountID);

                    if (success)
                    {
                        MessageBox.Show("Xóa cấu hình thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadConfigurations();
                    }
                    else
                    {
                        MessageBox.Show("Xóa cấu hình thất bại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnDelete_Click", ex);
                MessageBox.Show($"Lỗi khi xóa cấu hình: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Lưu
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (string.IsNullOrWhiteSpace(txtConfigKey.Text))
                {
                    MessageBox.Show("Vui lòng nhập khóa cấu hình!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfigKey.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtConfigValue.Text))
                {
                    MessageBox.Show("Vui lòng nhập giá trị cấu hình!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfigValue.Focus();
                    return;
                }

                // Thêm mới hoặc cập nhật
                if (_selectedConfig == null)
                {
                    // Kiểm tra khóa cấu hình đã tồn tại chưa
                    if (_allConfigs.Any(c => c.ConfigKey == txtConfigKey.Text))
                    {
                        MessageBox.Show("Khóa cấu hình đã tồn tại!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtConfigKey.Focus();
                        return;
                    }

                    // Tạo đối tượng cấu hình mới
                    SystemConfigDTO newConfig = new SystemConfigDTO
                    {
                        ConfigKey = txtConfigKey.Text,
                        ConfigValue = txtConfigValue.Text,
                        Description = txtDescription.Text,
                        DataType = cboDataType.Text,
                        IsReadOnly = chkReadOnly.Checked
                    };

                    // Gọi service tạo mới
                    int configId = _systemConfigService.CreateConfig(newConfig, _currentUser.AccountID);

                    if (configId > 0)
                    {
                        MessageBox.Show("Thêm cấu hình thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadConfigurations();
                        EnableConfigControls(false);
                    }
                    else
                    {
                        MessageBox.Show("Thêm cấu hình thất bại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Cập nhật đối tượng cấu hình
                    SystemConfigDTO updatedConfig = new SystemConfigDTO
                    {
                        ConfigID = _selectedConfig.ConfigID,
                        ConfigKey = txtConfigKey.Text,
                        ConfigValue = txtConfigValue.Text
                    };

                    // Gọi service cập nhật
                    bool success = _systemConfigService.UpdateConfig(updatedConfig, _currentUser.AccountID);

                    if (success)
                    {
                        MessageBox.Show("Cập nhật cấu hình thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadConfigurations();
                        EnableConfigControls(false);
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật cấu hình thất bại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnSave_Click", ex);
                MessageBox.Show($"Lỗi khi lưu cấu hình: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Hủy
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị lại thông tin cấu hình đã chọn (nếu có)
                DisplaySelectedConfig();
                EnableConfigControls(false);
            }
            catch (Exception ex)
            {
                _logger.LogError("btnCancel_Click", ex);
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
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
                LoadConfigurations();
                _selectedConfig = null;
                ClearConfigControls();
                EnableConfigControls(false);
            }
            catch (Exception ex)
            {
                _logger.LogError("btnRefresh_Click", ex);
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi thay đổi giá trị bộ lọc
        /// </summary>
        private void cboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyFilter();
            }
            catch (Exception ex)
            {
                _logger.LogError("cboFilter_SelectedIndexChanged", ex);
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi",
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
                ApplyFilter();
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
                    ApplyFilter();
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
        /// Sự kiện khi nhấn nút Xuất
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json",
                    Title = "Xuất cấu hình hệ thống",
                    DefaultExt = "json",
                    FileName = $"SystemConfig_Export_{DateTime.Now:yyyyMMdd}"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = _systemConfigService.ExportConfigurations(saveFileDialog.FileName);

                    MessageBox.Show($"Đã xuất cấu hình thành công!\nĐường dẫn: {filePath}",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnExport_Click", ex);
                MessageBox.Show($"Lỗi khi xuất cấu hình: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Nhập
        /// </summary>
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json",
                    Title = "Nhập cấu hình hệ thống",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DialogResult confirmResult = MessageBox.Show(
                        "Cấu hình hiện tại có thể bị ghi đè. Bạn có muốn tiếp tục?",
                        "Xác nhận nhập cấu hình", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        string result = _systemConfigService.ImportConfigurations(
                            openFileDialog.FileName, _currentUser.AccountID);

                        MessageBox.Show(result, "Kết quả nhập cấu hình",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadConfigurations();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnImport_Click", ex);
                MessageBox.Show($"Lỗi khi nhập cấu hình: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi nhấn đúp vào một cấu hình
        /// </summary>
        private void dgvConfigs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    _selectedConfig = dgvConfigs.Rows[e.RowIndex].DataBoundItem as SystemConfigDTO;

                    if (_selectedConfig != null)
                    {
                        DisplaySelectedConfig();
                        btnEdit_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("dgvConfigs_CellDoubleClick", ex);
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Khởi tạo các thành phần giao diện
        /// </summary>
        private void InitializeComponent()
        {
            this.grpConfigs = new System.Windows.Forms.GroupBox();
            this.dgvConfigs = new System.Windows.Forms.DataGridView();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkReadOnly = new System.Windows.Forms.CheckBox();
            this.cboDataType = new System.Windows.Forms.ComboBox();
            this.lblDataType = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtConfigValue = new System.Windows.Forms.TextBox();
            this.lblConfigValue = new System.Windows.Forms.Label();
            this.txtConfigKey = new System.Windows.Forms.TextBox();
            this.lblConfigKey = new System.Windows.Forms.Label();
            this.pnlTools = new System.Windows.Forms.Panel();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.lblTotalRows = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.cboFilter = new System.Windows.Forms.ComboBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.grpConfigs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConfigs)).BeginInit();
            this.grpActions.SuspendLayout();
            this.pnlTools.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConfigs
            // 
            this.grpConfigs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConfigs.Controls.Add(this.dgvConfigs);
            this.grpConfigs.Location = new System.Drawing.Point(12, 60);
            this.grpConfigs.Name = "grpConfigs";
            this.grpConfigs.Size = new System.Drawing.Size(780, 300);
            this.grpConfigs.TabIndex = 0;
            this.grpConfigs.TabStop = false;
            this.grpConfigs.Text = "Danh sách cấu hình";
            // 
            // dgvConfigs
            // 
            this.dgvConfigs.AllowUserToAddRows = false;
            this.dgvConfigs.AllowUserToDeleteRows = false;
            this.dgvConfigs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvConfigs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvConfigs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConfigs.Location = new System.Drawing.Point(6, 19);
            this.dgvConfigs.MultiSelect = false;
            this.dgvConfigs.Name = "dgvConfigs";
            this.dgvConfigs.ReadOnly = true;
            this.dgvConfigs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConfigs.Size = new System.Drawing.Size(768, 275);
            this.dgvConfigs.TabIndex = 0;
            this.dgvConfigs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConfigs_CellDoubleClick);
            this.dgvConfigs.SelectionChanged += new System.EventHandler(this.dgvConfigs_SelectionChanged);
            // 
            // grpActions
            // 
            this.grpActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpActions.Controls.Add(this.btnCancel);
            this.grpActions.Controls.Add(this.btnSave);
            this.grpActions.Controls.Add(this.chkReadOnly);
            this.grpActions.Controls.Add(this.cboDataType);
            this.grpActions.Controls.Add(this.lblDataType);
            this.grpActions.Controls.Add(this.txtDescription);
            this.grpActions.Controls.Add(this.lblDescription);
            this.grpActions.Controls.Add(this.txtConfigValue);
            this.grpActions.Controls.Add(this.lblConfigValue);
            this.grpActions.Controls.Add(this.txtConfigKey);
            this.grpActions.Controls.Add(this.lblConfigKey);
            this.grpActions.Location = new System.Drawing.Point(12, 415);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(780, 175);
            this.grpActions.TabIndex = 1;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Thông tin cấu hình";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(683, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(587, 130);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 35);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkReadOnly
            // 
            this.chkReadOnly.AutoSize = true;
            this.chkReadOnly.Location = new System.Drawing.Point(349, 97);
            this.chkReadOnly.Name = "chkReadOnly";
            this.chkReadOnly.Size = new System.Drawing.Size(66, 17);
            this.chkReadOnly.TabIndex = 8;
            this.chkReadOnly.Text = "Chỉ đọc";
            this.chkReadOnly.UseVisualStyleBackColor = true;
            // 
            // cboDataType
            // 
            this.cboDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDataType.FormattingEnabled = true;
            this.cboDataType.Location = new System.Drawing.Point(110, 95);
            this.cboDataType.Name = "cboDataType";
            this.cboDataType.Size = new System.Drawing.Size(121, 21);
            this.cboDataType.TabIndex = 7;
            // 
            // lblDataType
            // 
            this.lblDataType.AutoSize = true;
            this.lblDataType.Location = new System.Drawing.Point(6, 98);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(65, 13);
            this.lblDataType.TabIndex = 6;
            this.lblDataType.Text = "Loại dữ liệu:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(110, 69);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(664, 20);
            this.txtDescription.TabIndex = 5;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 72);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(37, 13);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Mô tả:";
            // 
            // txtConfigValue
            // 
            this.txtConfigValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfigValue.Location = new System.Drawing.Point(110, 43);
            this.txtConfigValue.Name = "txtConfigValue";
            this.txtConfigValue.Size = new System.Drawing.Size(664, 20);
            this.txtConfigValue.TabIndex = 3;
            // 
            // lblConfigValue
            // 
            this.lblConfigValue.AutoSize = true;
            this.lblConfigValue.Location = new System.Drawing.Point(6, 46);
            this.lblConfigValue.Name = "lblConfigValue";
            this.lblConfigValue.Size = new System.Drawing.Size(36, 13);
            this.lblConfigValue.TabIndex = 2;
            this.lblConfigValue.Text = "Giá trị:";
            // 
            // txtConfigKey
            // 
            this.txtConfigKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfigKey.Location = new System.Drawing.Point(110, 17);
            this.txtConfigKey.Name = "txtConfigKey";
            this.txtConfigKey.Size = new System.Drawing.Size(664, 20);
            this.txtConfigKey.TabIndex = 1;
            // 
            // lblConfigKey
            // 
            this.lblConfigKey.AutoSize = true;
            this.lblConfigKey.Location = new System.Drawing.Point(6, 20);
            this.lblConfigKey.Name = "lblConfigKey";
            this.lblConfigKey.Size = new System.Drawing.Size(35, 13);
            this.lblConfigKey.TabIndex = 0;
            this.lblConfigKey.Text = "Khóa:";
            // 
            // pnlTools
            // 
            this.pnlTools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTools.Controls.Add(this.btnImport);
            this.pnlTools.Controls.Add(this.btnExport);
            this.pnlTools.Controls.Add(this.btnRefresh);
            this.pnlTools.Controls.Add(this.btnDelete);
            this.pnlTools.Controls.Add(this.btnEdit);
            this.pnlTools.Controls.Add(this.btnAdd);
            this.pnlTools.Location = new System.Drawing.Point(12, 366);
            this.pnlTools.Name = "pnlTools";
            this.pnlTools.Size = new System.Drawing.Size(780, 43);
            this.pnlTools.TabIndex = 2;
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(696, 4);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(77, 35);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "Nhập";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(613, 4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(77, 35);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Xuất";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(530, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(77, 35);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(170, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(77, 35);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(87, 4);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(77, 35);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Sửa";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(4, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(77, 35);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Thêm";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // pnlSearch
            // 
            this.pnlSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSearch.Controls.Add(this.lblTotalRows);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Controls.Add(this.cboFilter);
            this.pnlSearch.Controls.Add(this.lblFilter);
            this.pnlSearch.Location = new System.Drawing.Point(12, 12);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(780, 42);
            this.pnlSearch.TabIndex = 3;
            // 
            // lblTotalRows
            // 
            this.lblTotalRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalRows.Location = new System.Drawing.Point(595, 13);
            this.lblTotalRows.Name = "lblTotalRows";
            this.lblTotalRows.Size = new System.Drawing.Size(178, 13);
            this.lblTotalRows.TabIndex = 5;
            this.lblTotalRows.Text = "Tổng số cấu hình: 0";
            this.lblTotalRows.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(526, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(63, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(342, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(178, 20);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(284, 13);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(52, 13);
            this.lblSearch.TabIndex = 2;
            this.lblSearch.Text = "Tìm kiếm:";
            // 
            // cboFilter
            // 
            this.cboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilter.FormattingEnabled = true;
            this.cboFilter.Location = new System.Drawing.Point(57, 10);
            this.cboFilter.Name = "cboFilter";
            this.cboFilter.Size = new System.Drawing.Size(221, 21);
            this.cboFilter.TabIndex = 1;
            this.cboFilter.SelectedIndexChanged += new System.EventHandler(this.cboFilter_SelectedIndexChanged);
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(3, 13);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(38, 13);
            this.lblFilter.TabIndex = 0;
            this.lblFilter.Text = "Bộ lọc:";
            // 
            // SystemConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 602);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.pnlTools);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpConfigs);
            this.MinimumSize = new System.Drawing.Size(820, 640);
            this.Name = "SystemConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý cấu hình hệ thống";
            this.Load += new System.EventHandler(this.SystemConfigForm_Load);
            this.grpConfigs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConfigs)).EndInit();
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.pnlTools.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox grpConfigs;
        private System.Windows.Forms.DataGridView dgvConfigs;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Label lblConfigKey;
        private System.Windows.Forms.TextBox txtConfigKey;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkReadOnly;
        private System.Windows.Forms.ComboBox cboDataType;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtConfigValue;
        private System.Windows.Forms.Label lblConfigValue;
        private System.Windows.Forms.Panel pnlTools;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.ComboBox cboFilter;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.Label lblTotalRows;
    }
}