
using QuanLyCuaHangTienLoi.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Forms.Products;

namespace QuanLyCuaHangTienLoi
{
    public partial class MainForm : BaseForm
    {
        private AccountSessionDTO _currentUser;
        private AccountDTO _currentAccount;
        private Panel pnlMenu;
        private Panel pnlContent;
        private IAccountService _accountService;
        
        // Menu items
        private ToolStripMenuItem _menuLogin;
        private ToolStripMenuItem _menuLogout;
        private ToolStripMenuItem _menuChangePassword;
        private ToolStripMenuItem _menuUserManagement;
        private ToolStripMenuItem _menuCategoryManagement;
        private ToolStripMenuItem _menuProductManagement;
        
        // Status strip items
        private ToolStripStatusLabel _statusUser;
        
        public MainForm()
        {
            InitializeComponent();
            this.Text = AppSettings.Instance.AppName;
            this.WindowState = FormWindowState.Maximized;
            _accountService = new AccountService(); // Giả định có service này
        }

        public MainForm(AccountSessionDTO currentUser) : this()
        {
            _currentUser = currentUser;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Size = new System.Drawing.Size(1200, 800);
            this.Text = "Quản Lý Cửa Hàng Tiện Lợi";
            this.IsMdiContainer = true; // Cho phép chứa các form con

            // Main menu strip
            MainMenuStrip = new MenuStrip
            {
                BackColor = Constants.Colors.LightPink,
                ForeColor = Constants.Colors.DarkPink,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Dock = DockStyle.Top
            };
            
            // Thêm các menu items
            AddMenuItems();

            // Menu panel
            pnlMenu = new Panel
            {
                BackColor = Constants.Colors.DarkPink,
                Dock = DockStyle.Left,
                Width = 220
            };

            // Logo panel
            Panel pnlLogo = new Panel
            {
                BackColor = Constants.Colors.DarkPink,
                Dock = DockStyle.Top,
                Height = 80
            };

            // Logo label
            Label lblLogo = new Label
            {
                Text = "QUẢN LÝ\r\nCỬA HÀNG TIỆN LỢI",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Constants.Colors.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            pnlLogo.Controls.Add(lblLogo);
            pnlMenu.Controls.Add(pnlLogo);

            // User info panel
            Panel pnlUserInfo = new Panel
            {
                BackColor = Constants.Colors.PrimaryPink,
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            Label lblUserName = new Label
            {
                Text = "Xin chào, Khách",
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = Constants.Colors.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button btnLogout = new Button
            {
                Text = "Đăng xuất",
                Font = new Font("Arial", 8, FontStyle.Regular),
                ForeColor = Constants.Colors.White,
                BackColor = Constants.Colors.DarkPink,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 30),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Enabled = false // Ban đầu disable
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;

            pnlUserInfo.Controls.Add(lblUserName);
            pnlUserInfo.Controls.Add(btnLogout);

            // Main content panel
            pnlContent = new Panel
            {
                BackColor = Constants.Colors.LightGray,
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Status strip
            StatusStrip statusStrip = new StatusStrip();
            ToolStripStatusLabel lblStatus = new ToolStripStatusLabel
            {
                Text = $"Phiên bản: {AppSettings.Instance.Version} | Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}"
            };
            
            _statusUser = new ToolStripStatusLabel
            {
                Text = "Chưa đăng nhập",
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };
            
            statusStrip.Items.Add(lblStatus);
            statusStrip.Items.Add(_statusUser);

            // Add menu items
            CreateMenuItems();

            // Add controls to form
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlUserInfo);
            this.Controls.Add(pnlMenu);
            this.Controls.Add(MainMenuStrip);
            this.Controls.Add(statusStrip);

            // Timer for updating time
            Timer timer = new Timer(this.components);
            timer.Interval = 60000; // 1 phút
            timer.Tick += (s, e) =>
            {
                lblStatus.Text = $"Phiên bản: {AppSettings.Instance.Version} | Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}";
            };
            timer.Start();
        }

        private void AddMenuItems()
        {
            // Menu Hệ thống
            ToolStripMenuItem menuSystem = new ToolStripMenuItem("Hệ thống");
            menuSystem.ForeColor = Constants.Colors.DarkPink;
            
            ToolStripMenuItem menuLogin = new ToolStripMenuItem("Đăng nhập");
            menuLogin.Click += MenuLogin_Click;
            menuLogin.Image = Properties.Resources.LoginIcon; // Giả định có resource này

            ToolStripMenuItem menuLogout = new ToolStripMenuItem("Đăng xuất");
            menuLogout.Click += MenuLogout_Click;
            menuLogout.Image = Properties.Resources.LogoutIcon;
            menuLogout.Enabled = false; // Ban đầu disable

            ToolStripMenuItem menuChangePassword = new ToolStripMenuItem("Đổi mật khẩu");
            menuChangePassword.Click += MenuChangePassword_Click;
            menuChangePassword.Image = Properties.Resources.PasswordIcon;
            menuChangePassword.Enabled = false; // Ban đầu disable

            ToolStripMenuItem menuExit = new ToolStripMenuItem("Thoát");
            menuExit.Click += MenuExit_Click;
            menuExit.Image = Properties.Resources.ExitIcon;

            menuSystem.DropDownItems.AddRange(new ToolStripItem[] {
                menuLogin, menuLogout, new ToolStripSeparator(),
                menuChangePassword, new ToolStripSeparator(), menuExit
            });

            // Menu Quản lý
            ToolStripMenuItem menuManagement = new ToolStripMenuItem("Quản lý");
            menuManagement.ForeColor = Constants.Colors.DarkPink;
            
            ToolStripMenuItem menuUserManagement = new ToolStripMenuItem("Quản lý người dùng");
            menuUserManagement.Click += MenuUserManagement_Click;
            menuUserManagement.Image = Properties.Resources.UserManagementIcon;
            menuUserManagement.Enabled = false; // Ban đầu disable

            ToolStripMenuItem menuCategoryManagement = new ToolStripMenuItem("Quản lý danh mục");
            menuCategoryManagement.Click += MenuCategoryManagement_Click;
            menuCategoryManagement.Image = Properties.Resources.CategoryIcon;
            menuCategoryManagement.Enabled = false; // Ban đầu disable

            ToolStripMenuItem menuProductManagement = new ToolStripMenuItem("Quản lý sản phẩm");
            menuProductManagement.Click += MenuProductManagement_Click;
            menuProductManagement.Image = Properties.Resources.ProductIcon;
            menuProductManagement.Enabled = false; // Ban đầu disable

            menuManagement.DropDownItems.AddRange(new ToolStripItem[] {
                menuUserManagement, menuCategoryManagement, menuProductManagement
            });

            // Thêm vào MainMenuStrip
            MainMenuStrip.Items.AddRange(new ToolStripItem[] {
                menuSystem, menuManagement
            });

            // Lưu tham chiếu để có thể truy cập sau này
            _menuLogin = menuLogin;
            _menuLogout = menuLogout;
            _menuChangePassword = menuChangePassword;
            _menuUserManagement = menuUserManagement;
            _menuCategoryManagement = menuCategoryManagement;
            _menuProductManagement = menuProductManagement;
        }

        private void CreateMenuItems()
        {
            // Tạo menu items dựa trên vai trò người dùng
            AddMenuItem("Trang chủ", "home", MenuItem_Click);
            AddMenuItem("Sản phẩm", "product", MenuItem_Click);
            AddMenuItem("Nhân viên", "employee", MenuItem_Click);
            AddMenuItem("Khách hàng", "customer", MenuItem_Click);
            AddMenuItem("Đơn hàng", "order", MenuItem_Click);
            AddMenuItem("Báo cáo", "report", MenuItem_Click);
            AddMenuItem("Hệ thống", "system", MenuItem_Click);
        }

        private void AddMenuItem(string text, string name, EventHandler clickEvent)
        {
            Button btn = new Button
            {
                Text = text,
                Name = "btn" + name,
                Tag = name,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                Height = 50,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Constants.Colors.White,
                BackColor = Constants.Colors.DarkPink,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Click += clickEvent;

            pnlMenu.Controls.Add(btn);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string tag = btn.Tag.ToString();

            // Mở form tương ứng dựa trên tag
            switch (tag)
            {
                case "home":
                    ShowMessage("Chào mừng đến với trang chủ!");
                    break;
                case "product":
                    OpenChildForm(new ProductManagementForm());
                    break;
                case "employee":
                    OpenChildForm(new EmployeeManagementForm());
                    break;
                case "customer":
                    OpenChildForm(new CustomerManagementForm());
                    break;
                case "order":
                    OpenChildForm(new OrderManagementForm());
                    break;
                case "report":
                    OpenChildForm(new ReportForm());
                    break;
                case "system":
                    ShowMessage("Cài đặt hệ thống");
                    break;
                default:
                    ShowMessage($"Bạn đã chọn chức năng: {btn.Text}");
                    break;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            // Kiểm tra xem form đã mở chưa
            Form existingForm = IsFormOpen(childForm.GetType());
            if (existingForm != null)
            {
                // Đã mở, kích hoạt form
                existingForm.Activate();
            }
            else
            {
                // Chưa mở, tạo form mới
                childForm.MdiParent = this;
                childForm.Show();
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            MenuLogout_Click(sender, e);
        }

        private void MenuLogin_Click(object sender, EventArgs e)
        {
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Lưu thông tin tài khoản đăng nhập
                    _currentAccount = loginForm.LoggedInAccount;
                    
                    // Cập nhật UI sau khi đăng nhập
                    UpdateUIAfterLogin();
                    
                    // Hiển thị thông báo
                    MessageBox.Show($"Chào mừng {_currentAccount.Username} đăng nhập hệ thống!",
                        "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void MenuLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                // Đăng xuất
                _accountService.Logout();
                _currentAccount = null;
                
                // Cập nhật UI sau khi đăng xuất
                UpdateUIAfterLogout();
                
                // Đóng tất cả form con
                foreach (Form childForm in MdiChildren)
                {
                    childForm.Close();
                }
            }
        }

        private void MenuChangePassword_Click(object sender, EventArgs e)
        {
            if (_currentAccount != null)
            {
                using (ChangePasswordForm changePasswordForm = new ChangePasswordForm(_currentAccount.AccountID))
                {
                    changePasswordForm.ShowDialog();
                }
            }
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng?",
                "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void MenuUserManagement_Click(object sender, EventArgs e)
        {
            OpenChildForm(new UserManagementForm());
        }

        private void MenuCategoryManagement_Click(object sender, EventArgs e)
        {
            OpenChildForm(new CategoryManagementForm());
        }

        private void MenuProductManagement_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ProductManagementForm());
        }

        // Kiểm tra form đã mở chưa
        private Form IsFormOpen(Type formType)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == formType)
                {
                    return form;
                }
            }
            return null;
        }

        // Cập nhật UI sau khi đăng nhập
        private void UpdateUIAfterLogin()
        {
            // Cập nhật menu
            _menuLogin.Enabled = false;
            _menuLogout.Enabled = true;
            _menuChangePassword.Enabled = true;
            
            // Phân quyền menu
            bool isAdmin = _accountService.HasPermission("admin");
            bool isManager = _accountService.HasPermission("manager");
            
            _menuUserManagement.Enabled = isAdmin;
            _menuCategoryManagement.Enabled = isAdmin || isManager;
            _menuProductManagement.Enabled = isAdmin || isManager;
            
            // Cập nhật thanh trạng thái
            _statusUser.Text = $"Đăng nhập: {_currentAccount.Username} ({_currentAccount.RoleName})";
            
            // Cập nhật label người dùng
            foreach (Control control in this.Controls)
            {
                if (control is Panel && control.Dock == DockStyle.Top)
                {
                    foreach (Control c in control.Controls)
                    {
                        if (c is Label)
                        {
                            ((Label)c).Text = $"Xin chào, {_currentAccount.Username}";
                            break;
                        }
                    }
                    
                    // Kích hoạt nút đăng xuất
                    foreach (Control c in control.Controls)
                    {
                        if (c is Button && c.Text == "Đăng xuất")
                        {
                            c.Enabled = true;
                            break;
                        }
                    }
                    
                    break;
                }
            }
        }

        // Cập nhật UI sau khi đăng xuất
        private void UpdateUIAfterLogout()
        {
            // Cập nhật menu
            _menuLogin.Enabled = true;
            _menuLogout.Enabled = false;
            _menuChangePassword.Enabled = false;
            
            // Vô hiệu hóa các menu quản lý
            _menuUserManagement.Enabled = false;
            _menuCategoryManagement.Enabled = false;
            _menuProductManagement.Enabled = false;
            
            // Cập nhật thanh trạng thái
            _statusUser.Text = "Chưa đăng nhập";
            
            // Cập nhật label người dùng
            foreach (Control control in this.Controls)
            {
                if (control is Panel && control.Dock == DockStyle.Top)
                {
                    foreach (Control c in control.Controls)
                    {
                        if (c is Label)
                        {
                            ((Label)c).Text = "Xin chào, Khách";
                            break;
                        }
                    }
                    
                    // Vô hiệu hóa nút đăng xuất
                    foreach (Control c in control.Controls)
                    {
                        if (c is Button && c.Text == "Đăng xuất")
                        {
                            c.Enabled = false;
                            break;
                        }
                    }
                    
                    break;
                }
            }
        }
    }
}
