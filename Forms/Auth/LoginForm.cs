// Mã gợi ý cho LoginForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Auth
{
    public partial class LoginForm : Form
    {
        private IAccountService _accountService;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private CheckBox chkRemember;
        private Label lblTitle;
        private Panel panelHeader;
        private PictureBox picLogo;

        // Màu sắc hồng dễ thương
        private Color pinkLight = Color.FromArgb(255, 192, 203); // Pink
        private Color pinkLighter = Color.FromArgb(255, 228, 225); // MistyRose
        private Color pinkDark = Color.FromArgb(255, 105, 180); // HotPink

        public AccountDTO LoggedInAccount { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            _accountService = AccountService.Instance;
            ConfigureForm();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = this.pinkDark;
            this.panelHeader.Controls.Add(this.picLogo);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(400, 100);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(100, 35);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(231, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Đăng Nhập Hệ Thống";
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(25, 25);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(50, 50);
            this.picLogo.TabIndex = 1;
            this.picLogo.TabStop = false;
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(70, 150);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(260, 29);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.PlaceholderText = "Tên đăng nhập";
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(70, 200);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(260, 29);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.PlaceholderText = "Mật khẩu";
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRemember.Location = new System.Drawing.Point(70, 250);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(122, 21);
            this.chkRemember.TabIndex = 3;
            this.chkRemember.Text = "Nhớ đăng nhập";
            this.chkRemember.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = this.pinkDark;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(70, 300);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(260, 40);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Đăng Nhập";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = this.pinkLighter;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.chkRemember);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng Nhập - Quản Lý Cửa Hàng Tiện Lợi";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ConfigureForm()
        {
            // Tùy chỉnh thêm giao diện
            // Thêm icon cho form nếu có
            try
            {
                this.Icon = Properties.Resources.AppIcon;
                this.picLogo.Image = Properties.Resources.Logo;
                this.picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch
            {
                // Xử lý nếu chưa có icon
            }

            // Tạo hiệu ứng đổ bóng cho button
            btnLogin.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, btnLogin.ClientRectangle,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(200, 182, 182, 182), 1, ButtonBorderStyle.Solid);
            };

            // Load thông tin ghi nhớ đăng nhập nếu có
            try
            {
                string savedUsername = Properties.Settings.Default.RememberedUsername;
                if (!string.IsNullOrEmpty(savedUsername))
                {
                    txtUsername.Text = savedUsername;
                    chkRemember.Checked = true;
                    txtPassword.Focus();
                }
            }
            catch
            {
                // Xử lý nếu chưa có settings
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Có thể thêm xử lý khi form load
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị loading
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            try
            {
                LoggedInAccount = _accountService.Login(username, password);

                if (LoggedInAccount != null)
                {
                    // Lưu thông tin ghi nhớ đăng nhập nếu được chọn
                    try
                    {
                        Properties.Settings.Default.RememberedUsername = chkRemember.Checked ? username : string.Empty;
                        Properties.Settings.Default.Save();
                    }
                    catch
                    {
                        // Xử lý nếu chưa có settings
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
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