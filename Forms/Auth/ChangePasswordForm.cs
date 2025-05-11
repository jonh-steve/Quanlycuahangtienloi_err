// Mã gợi ý cho ChangePasswordForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Auth
{
    public partial class ChangePasswordForm : Form
    {
        private IAccountService _accountService;
        private int _accountId;

        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnChange;
        private Button btnCancel;
        private Label lblTitle;
        private Panel panelHeader;

        // Màu sắc hồng dễ thương
        private Color pinkLight = Color.FromArgb(255, 192, 203); // Pink
        private Color pinkLighter = Color.FromArgb(255, 228, 225); // MistyRose
        private Color pinkDark = Color.FromArgb(255, 105, 180); // HotPink

        public ChangePasswordForm(int accountId)
        {
            _accountId = accountId;
            _accountService = AccountService.Instance;
            InitializeComponent();
            ConfigureForm();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtCurrentPassword = new System.Windows.Forms.TextBox();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.btnChange = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = this.pinkDark;
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(400, 80);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(117, 25);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(166, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Đổi Mật Khẩu";
            // 
            // txtCurrentPassword
            // 
            this.txtCurrentPassword.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrentPassword.Location = new System.Drawing.Point(70, 120);
            this.txtCurrentPassword.Name = "txtCurrentPassword";
            this.txtCurrentPassword.PasswordChar = '●';
            this.txtCurrentPassword.Size = new System.Drawing.Size(260, 27);
            this.txtCurrentPassword.TabIndex = 1;
            this.txtCurrentPassword.PlaceholderText = "Mật khẩu hiện tại";
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewPassword.Location = new System.Drawing.Point(70, 170);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '●';
            this.txtNewPassword.Size = new System.Drawing.Size(260, 27);
            this.txtNewPassword.TabIndex = 2;
            this.txtNewPassword.PlaceholderText = "Mật khẩu mới";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfirmPassword.Location = new System.Drawing.Point(70, 220);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '●';
            this.txtConfirmPassword.Size = new System.Drawing.Size(260, 27);
            this.txtConfirmPassword.TabIndex = 3;
            this.txtConfirmPassword.PlaceholderText = "Xác nhận mật khẩu mới";
            // 
            // btnChange
            // 
            this.btnChange.BackColor = this.pinkDark;
            this.btnChange.FlatAppearance.BorderSize = 0;
            this.btnChange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChange.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChange.ForeColor = System.Drawing.Color.White;
            this.btnChange.Location = new System.Drawing.Point(70, 280);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(120, 35);
            this.btnChange.TabIndex = 4;
            this.btnChange.Text = "Đổi Mật Khẩu";
            this.btnChange.UseVisualStyleBackColor = false;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(210, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ChangePasswordForm
            // 
            this.AcceptButton = this.btnChange;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = this.pinkLighter;
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnChange);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.txtCurrentPassword);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đổi Mật Khẩu - Quản Lý Cửa Hàng Tiện Lợi";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ConfigureForm()
        {
            // Tùy chỉnh thêm giao diện
            try
            {
                this.Icon = Properties.Resources.AppIcon;
            }
            catch
            {
                // Xử lý nếu chưa có icon
            }

            // Tạo hiệu ứng đổ bóng cho button
            btnChange.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, btnChange.ClientRectangle,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid);
            };

            btnCancel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, btnCancel.ClientRectangle,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid);
            };
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Kiểm tra dữ liệu nhập
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Clear();
                txtConfirmPassword.Focus();
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            // Hiển thị loading
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            try
            {
                bool result = _accountService.ChangePassword(_accountId, currentPassword, newPassword);

                if (result)
                {
                    MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Mật khẩu hiện tại không đúng hoặc có lỗi xảy ra!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCurrentPassword.Clear();
                    txtCurrentPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}