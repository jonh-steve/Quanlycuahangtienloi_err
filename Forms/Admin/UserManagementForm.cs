// File: Forms/Admin/UserManagementForm.cs (Form - Windows Forms)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Admin
{
    public partial class UserManagementForm : Form
    {
        private readonly IAccountService _accountService;
        private List<AccountDTO> _accounts;
        private int _currentPage = 1;
        private int _pageSize = 15;
        private int _totalPages = 1;

        public UserManagementForm(IAccountService accountService)
        {
            InitializeComponent();
            _accountService = accountService;

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = System.Drawing.Color.FromArgb(255, 240, 245);
            btnAdd.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            btnEdit.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            btnDelete.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            btnRefresh.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);

            // Khởi tạo DataGridView
            InitializeDataGridView();

            // Load dữ liệu
            LoadAccounts();

            // Đăng ký sự kiện
            dgvAccounts.CellClick += DgvAccounts_CellClick;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;
            cboRoleFilter.SelectedIndexChanged += CboRoleFilter_SelectedIndexChanged;
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void InitializeDataGridView()
        {
            dgvAccounts.AutoGenerateColumns = false;

            // Thêm các cột
            dgvAccounts.Columns.Add("colAccountID", "ID");
            dgvAccounts.Columns["colAccountID"].DataPropertyName = "AccountID";
            dgvAccounts.Columns["colAccountID"].Width = 50;

            dgvAccounts.Columns.Add("colUsername", "Tên đăng nhập");
            dgvAccounts.Columns["colUsername"].DataPropertyName = "Username";
            dgvAccounts.Columns["colUsername"].Width = 150;

            dgvAccounts.Columns.Add("colEmail", "Email");
            dgvAccounts.Columns["colEmail"].DataPropertyName = "Email";
            dgvAccounts.Columns["colEmail"].Width = 200;

            dgvAccounts.Columns.Add("colRoleName", "Vai trò");
            dgvAccounts.Columns["colRoleName"].DataPropertyName = "RoleName";
            dgvAccounts.Columns["colRoleName"].Width = 120;

            dgvAccounts.Columns.Add("colLastLogin", "Đăng nhập cuối");
            dgvAccounts.Columns["colLastLogin"].DataPropertyName = "LastLogin";
            dgvAccounts.Columns["colLastLogin"].Width = 150;

            dgvAccounts.Columns.Add("colStatus", "Trạng thái");
            dgvAccounts.Columns["colStatus"].DataPropertyName = "IsActiveText";
            dgvAccounts.Columns["colStatus"].Width = 100;

            // Định dạng DataGridView
            dgvAccounts.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 240, 245);
            dgvAccounts.EnableHeadersVisualStyles = false;
            dgvAccounts.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            dgvAccounts.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dgvAccounts.BorderStyle = BorderStyle.None;
            dgvAccounts.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAccounts.RowHeadersVisible = false;
        }

        private void LoadAccounts()
        {
            try
            {
                // Lấy thông tin lọc
                string searchText = txtSearch.Text.Trim();
                int? roleID = (cboRoleFilter.SelectedItem as ComboBoxItem)?.Value;

                // Lấy danh sách tài khoản từ service
                var result = _accountService.GetAccounts(searchText, roleID, _currentPage, _pageSize);
                _accounts = result.Accounts;
                _totalPages = result.TotalPages;

                // Cập nhật UI
                dgvAccounts.DataSource = null;
                dgvAccounts.DataSource = _accounts;

                lblPageInfo.Text = $"Trang {_currentPage}/{_totalPages}";
                btnPrev.Enabled = _currentPage > 1;
                btnNext.Enabled = _currentPage < _totalPages;

                lblTotalRecords.Text = $"Tổng số: {result.TotalRecords} tài khoản";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách tài khoản: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new UserDetailForm(null, _accountService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAccounts();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.CurrentRow == null) return;

            var selectedAccount = dgvAccounts.CurrentRow.DataBoundItem as AccountDTO;
            if (selectedAccount == null) return;

            using (var form = new UserDetailForm(selectedAccount, _accountService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAccounts();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.CurrentRow == null) return;

            var selectedAccount = dgvAccounts.CurrentRow.DataBoundItem as AccountDTO;
            if (selectedAccount == null) return;

            if (MessageBox.Show($"Bạn có chắc chắn muốn vô hiệu hóa tài khoản '{selectedAccount.Username}'?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _accountService.DeactivateAccount(selectedAccount.AccountID);
                    MessageBox.Show("Vô hiệu hóa tài khoản thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAccounts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi vô hiệu hóa tài khoản: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboRoleFilter.SelectedIndex = 0;
            _currentPage = 1;
            LoadAccounts();
        }

        private void CboRoleFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentPage = 1;
            LoadAccounts();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Sử dụng timer để tránh search liên tục khi gõ
            if (_searchTimer != null)
            {
                _searchTimer.Stop();
                _searchTimer.Dispose();
            }

            _searchTimer = new Timer();
            _searchTimer.Interval = 500;
            _searchTimer.Tick += (s, args) =>
            {
                _searchTimer.Stop();
                _currentPage = 1;
                LoadAccounts();
            };
            _searchTimer.Start();
        }

        private Timer _searchTimer;

        private void DgvAccounts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEdit.Enabled = dgvAccounts.CurrentRow != null;
            btnDelete.Enabled = dgvAccounts.CurrentRow != null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Load danh sách vai trò cho ComboBox
            LoadRoles();
        }

        private void LoadRoles()
        {
            try
            {
                var roles = _accountService.GetAllRoles();

                cboRoleFilter.Items.Clear();
                cboRoleFilter.Items.Add(new ComboBoxItem { Text = "-- Tất cả vai trò --", Value = null });

                foreach (var role in roles)
                {
                    cboRoleFilter.Items.Add(new ComboBoxItem { Text = role.RoleName, Value = role.RoleID });
                }

                cboRoleFilter.SelectedIndex = 0;
                cboRoleFilter.DisplayMember = "Text";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vai trò: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Lớp hỗ trợ cho ComboBox
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public int? Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}