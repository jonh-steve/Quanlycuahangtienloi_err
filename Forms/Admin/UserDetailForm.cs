// File: Forms/Admin/UserDetailForm.cs (Form - Windows Forms)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Forms.Admin
{
    public partial class UserDetailForm : Form
    {
        private readonly IAccountService _accountService;
        private readonly AccountDTO _account;
        private readonly bool _isEditMode;
        private List<RoleDTO> _roles;

        public UserDetailForm(AccountDTO account, IAccountService accountService)
        {
            InitializeComponent();
            _accountService = accountService;
            _account = account;
            _isEditMode = account != null;

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = System.Drawing.Color.FromArgb(255, 240, 245);
            btnSave.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            btnCancel.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);

            // Thiết lập tiêu đề form
            this.Text = _isEditMode ? "Chỉnh sửa người dùng" : "Thêm người dùng mới";

            // Đăng ký sự kiện
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Load danh sách vai trò
            LoadRoles();

            // Ẩn trường mật khẩu khi chỉnh sửa
            lblPassword.Visible = !_isEditMode;
            txtPassword.Visible = !_isEditMode;
            chkShowPassword.Visible = !_isEditMode;

            // Thêm trường Reset mật khẩu khi chỉnh sửa
            if (_isEditMode)
            {
                chkResetPassword.Visible = true;
                chkResetPassword.CheckedChanged += ChkResetPassword_CheckedChanged;
            }
            else
            {
                chkResetPassword.Visible = false;
            }

            // Nạp dữ liệu nếu ở chế độ chỉnh sửa
            if (_isEditMode)
            {
                txtUsername.Text = _account.Username;
                txtUsername.ReadOnly = true; // Không cho phép sửa tên đăng nhập
                txtEmail.Text = _account.Email;
                chkIsActive.Checked = _account.IsActive;

                // Chọn vai trò
                foreach (RoleDTO role in cboRole.Items)
                {
                    if (role.RoleID == _account.RoleID)
                    {
                        cboRole.SelectedItem = role;
                        break;
                    }
                }
            }
            else
            {
                chkIsActive.Checked = true;
            }
        }

        private void LoadRoles()
        {
            try
            {
                _roles = _accountService.GetAllRoles();

                cboRole.Items.Clear();
                cboRole.DisplayMember = "RoleName";
                cboRole.ValueMember = "RoleID";

                foreach (var role in _roles)
                {
                    cboRole.Items.Add(role);
                }

                if (cboRole.Items.Count > 0)
                {
                    cboRole.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vai trò: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return;
                }

                if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (_isEditMode && chkResetPassword.Checked && string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (cboRole.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn vai trò!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboRole.Focus();
                    return;
                }

                // Kiểm tra tính hợp lệ của email
                if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !Validators.IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                // Lưu dữ liệu
                if (_isEditMode)
                {
                    var updatedAccount = new AccountDTO
                    {
                        AccountID = _account.AccountID,
                        Username = txtUsername.Text,
                        Email = txtEmail.Text,
                        RoleID = (cboRole.SelectedItem as RoleDTO).RoleID,
                        IsActive = chkIsActive.Checked
                    };

                    if (chkResetPassword.Checked)
                    {
                        _accountService.ResetPassword(_account.AccountID, txtPassword.Text);
                    }

                    _accountService.UpdateAccount(updatedAccount);
                    MessageBox.Show("Cập nhật người dùng thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var newAccount = new AccountDTO
                    {
                        Username = txtUsername.Text,
                        Email = txtEmail.Text,
                        RoleID = (cboRole.SelectedItem as RoleDTO).RoleID,
                        IsActive = chkIsActive.Checked
                    };

                    _accountService.CreateAccount(newAccount, txtPassword.Text);
                    MessageBox.Show("Thêm người dùng mới thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void ChkResetPassword_CheckedChanged(object sender, EventArgs e)
        {
            lblPassword.Visible = chkResetPassword.Checked;
            txtPassword.Visible = chkResetPassword.Checked;
            chkShowPassword.Visible = chkResetPassword.Checked;

            if (chkResetPassword.Checked)
            {
                txtPassword.Focus();
            }
        }
    }
}