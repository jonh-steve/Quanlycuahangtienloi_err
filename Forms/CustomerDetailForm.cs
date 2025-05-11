// Steve-Thuong_hai
using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Customers
{
    public partial class CustomerDetailForm : Form
    {
        private readonly CustomerService _customerService;
        private readonly int _currentEmployeeID;
        private readonly int _customerID;
        private bool _isEditMode;
        private Customer _currentCustomer;

        // Colors for pink theme
        private readonly Color _primaryColor = Color.FromArgb(255, 192, 203); // Pink
        private readonly Color _secondaryColor = Color.FromArgb(255, 182, 193); // LightPink
        private readonly Color _accentColor = Color.FromArgb(219, 112, 147); // PaleVioletRed
        private readonly Color _textColor = Color.FromArgb(60, 60, 60); // Dark gray for text

        // Constructor cho trường hợp thêm mới
        public CustomerDetailForm(CustomerService customerService, int currentEmployeeID)
        {
            InitializeComponent();
            _customerService = customerService;
            _currentEmployeeID = currentEmployeeID;
            _customerID = 0;
            _isEditMode = false;

            // Áp dụng theme
            ApplyPinkTheme();

            // Set up controls
            SetupForm();
        }

        // Constructor cho trường hợp chỉnh sửa
        public CustomerDetailForm(CustomerService customerService, int currentEmployeeID, int customerID)
        {
            InitializeComponent();
            _customerService = customerService;
            _currentEmployeeID = currentEmployeeID;
            _customerID = customerID;
            _isEditMode = true;

            // Áp dụng theme
            ApplyPinkTheme();

            // Set up controls
            SetupForm();

            // Load dữ liệu khách hàng
            LoadCustomerData();
        }

        private void ApplyPinkTheme()
        {
            // Set form properties
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            // Set panel properties
            panelTop.BackColor = _primaryColor;

            // Set button properties
            btnSave.BackColor = _accentColor;
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSave.Cursor = Cursors.Hand;

            btnCancel.BackColor = Color.Gray;
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCancel.Cursor = Cursors.Hand;

            // Set label properties
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);

            foreach (Control control in panelMain.Controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = _textColor;
                    label.Font = new Font("Segoe UI", 9.5F);
                }
                else if (control is TextBox textBox)
                {
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    textBox.Font = new Font("Segoe UI", 10F);
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.Font = new Font("Segoe UI", 10F);
                    comboBox.FlatStyle = FlatStyle.Flat;
                    comboBox.BackColor = Color.White;
                }
            }
        }

        private void SetupForm()
        {
            // Cập nhật tiêu đề form
            this.Text = _isEditMode ? "Chỉnh sửa thông tin khách hàng" : "Thêm khách hàng mới";
            lblTitle.Text = _isEditMode ? "CHỈNH SỬA THÔNG TIN KHÁCH HÀNG" : "THÊM KHÁCH HÀNG MỚI";

            // Thiết lập ComboBox cấp độ thành viên
            SetupMembershipComboBox();
        }

        private void SetupMembershipComboBox()
        {
            cboMembershipLevel.Items.Clear();
            foreach (var level in MembershipLevel.DefaultLevels)
            {
                cboMembershipLevel.Items.Add(level.LevelName);
            }
            cboMembershipLevel.SelectedIndex = 0; // Default: Regular
        }

        private void LoadCustomerData()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Lấy thông tin khách hàng
                var customerDTO = _customerService.GetCustomerByID(_customerID);

                if (customerDTO == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin khách hàng", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                // Lưu thông tin khách hàng hiện tại
                _currentCustomer = new Customer
                {
                    CustomerID = customerDTO.CustomerID,
                    CustomerName = customerDTO.CustomerName,
                    PhoneNumber = customerDTO.PhoneNumber,
                    Email = customerDTO.Email,
                    Address = customerDTO.Address,
                    MembershipLevel = customerDTO.MembershipLevel,
                    Points = customerDTO.Points,
                    CreatedDate = customerDTO.CreatedDate,
                    ModifiedDate = customerDTO.ModifiedDate
                };

                // Hiển thị thông tin lên form
                txtCustomerName.Text = customerDTO.CustomerName;
                txtPhoneNumber.Text = customerDTO.PhoneNumber;
                txtEmail.Text = customerDTO.Email;
                txtAddress.Text = customerDTO.Address;

                // Chọn cấp độ thành viên
                int levelIndex = 0;
                for (int i = 0; i < cboMembershipLevel.Items.Count; i++)
                {
                    if (cboMembershipLevel.Items[i].ToString() == customerDTO.MembershipLevel)
                    {
                        levelIndex = i;
                        break;
                    }
                }
                cboMembershipLevel.SelectedIndex = levelIndex;

                // Hiển thị điểm
                numPoints.Value = customerDTO.Points;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu nhập
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên khách hàng", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomerName.Focus();
                    return;
                }

                // Kiểm tra số điện thoại
                if (!string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
                {
                    if (!Utils.Validators.IsValidPhoneNumber(txtPhoneNumber.Text))
                    {
                        MessageBox.Show("Số điện thoại không hợp lệ", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPhoneNumber.Focus();
                        return;
                    }
                }

                // Kiểm tra email
                if (!string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    if (!Utils.Validators.IsValidEmail(txtEmail.Text))
                    {
                        MessageBox.Show("Email không hợp lệ", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtEmail.Focus();
                        return;
                    }
                }

                // Tạo đối tượng Customer từ dữ liệu form
                var customer = new Customer
                {
                    CustomerName = txtCustomerName.Text.Trim(),
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    MembershipLevel = cboMembershipLevel.SelectedItem.ToString(),
                    Points = (int)numPoints.Value
                };

                // Nếu là chỉnh sửa, thêm ID
                if (_isEditMode)
                {
                    customer.CustomerID = _customerID;
                }

                // Gọi service để lưu
                if (_isEditMode)
                {
                    bool result = _customerService.UpdateCustomer(customer, _currentEmployeeID);
                    if (result)
                    {
                        MessageBox.Show("Cập nhật thông tin khách hàng thành công", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    int newCustomerID = _customerService.CreateCustomer(customer, _currentEmployeeID);
                    if (newCustomerID > 0)
                    {
                        MessageBox.Show("Thêm khách hàng mới thành công", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Khởi tạo Designer components
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblMembershipLevel = new System.Windows.Forms.Label();
            this.cboMembershipLevel = new System.Windows.Forms.ComboBox();
            this.lblPoints = new System.Windows.Forms.Label();
            this.numPoints = new System.Windows.Forms.NumericUpDown();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Size = new System.Drawing.Size(600, 60);
            this.panelTop.Controls.Add(this.lblTitle);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Size = new System.Drawing.Size(350, 30);
            this.lblTitle.Text = "THÊM KHÁCH HÀNG MỚI";

            // panelMain
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 60);
            this.panelMain.Size = new System.Drawing.Size(600, 290);
            this.panelMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelMain.Controls.Add(this.lblCustomerName);
            this.panelMain.Controls.Add(this.txtCustomerName);
            this.panelMain.Controls.Add(this.lblPhoneNumber);
            this.panelMain.Controls.Add(this.txtPhoneNumber);
            this.panelMain.Controls.Add(this.lblEmail);
            this.panelMain.Controls.Add(this.txtEmail);
            this.panelMain.Controls.Add(this.lblAddress);
            this.panelMain.Controls.Add(this.txtAddress);
            this.panelMain.Controls.Add(this.lblMembershipLevel);
            this.panelMain.Controls.Add(this.cboMembershipLevel);
            this.panelMain.Controls.Add(this.lblPoints);
            this.panelMain.Controls.Add(this.numPoints);

            // lblCustomerName
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(20, 20);
            this.lblCustomerName.Size = new System.Drawing.Size(120, 20);
            this.lblCustomerName.Text = "Tên khách hàng:";

            // txtCustomerName
            this.txtCustomerName.Location = new System.Drawing.Point(150, 20);
            this.txtCustomerName.Size = new System.Drawing.Size(410, 25);

            // lblPhoneNumber
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Location = new System.Drawing.Point(20, 60);
            this.lblPhoneNumber.Size = new System.Drawing.Size(120, 20);
            this.lblPhoneNumber.Text = "Số điện thoại:";

            // txtPhoneNumber
            this.txtPhoneNumber.Location = new System.Drawing.Point(150, 60);
            this.txtPhoneNumber.Size = new System.Drawing.Size(200, 25);

            // lblEmail
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(20, 100);
            this.lblEmail.Size = new System.Drawing.Size(120, 20);
            this.lblEmail.Text = "Email:";

            // txtEmail
            this.txtEmail.Location = new System.Drawing.Point(150, 100);
            this.txtEmail.Size = new System.Drawing.Size(300, 25);

            // lblAddress
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(20, 140);
            this.lblAddress.Size = new System.Drawing.Size(120, 20);
            this.lblAddress.Text = "Địa chỉ:";

            // txtAddress
            this.txtAddress.Location = new System.Drawing.Point(150, 140);
            this.txtAddress.Size = new System.Drawing.Size(410, 25);
            this.txtAddress.Multiline = true;
            this.txtAddress.Height = 60;

            // lblMembershipLevel
            this.lblMembershipLevel.AutoSize = true;
            this.lblMembershipLevel.Location = new System.Drawing.Point(20, 220);
            this.lblMembershipLevel.Size = new System.Drawing.Size(120, 20);
            this.lblMembershipLevel.Text = "Hạng thành viên:";

            // cboMembershipLevel
            this.cboMembershipLevel.Location = new System.Drawing.Point(150, 220);
            this.cboMembershipLevel.Size = new System.Drawing.Size(200, 25);
            this.cboMembershipLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // lblPoints
            this.lblPoints.AutoSize = true;
            this.lblPoints.Location = new System.Drawing.Point(20, 260);
            this.lblPoints.Size = new System.Drawing.Size(120, 20);
            this.lblPoints.Text = "Điểm tích lũy:";

            // numPoints
            this.numPoints.Location = new System.Drawing.Point(150, 260);
            this.numPoints.Size = new System.Drawing.Size(120, 25);
            this.numPoints.Maximum = 1000000;
            this.numPoints.Minimum = 0;
            this.numPoints.Value = 0;

            // panelBottom
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 350);
            this.panelBottom.Size = new System.Drawing.Size(600, 60);
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Controls.Add(this.btnSave);

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(350, 15);
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "Lưu";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(460, 15);
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // CustomerDetailForm
            this.ClientSize = new System.Drawing.Size(600, 410);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
        }

        private Panel panelTop;
        private Label lblTitle;
        private Panel panelMain;
        private Label lblCustomerName;
        private TextBox txtCustomerName;
        private Label lblPhoneNumber;
        private TextBox txtPhoneNumber;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblAddress;
        private TextBox txtAddress;
        private Label lblMembershipLevel;
        private ComboBox cboMembershipLevel;
        private Label lblPoints;
        private NumericUpDown numPoints;
        private Panel panelBottom;
        private Button btnCancel;
        private Button btnSave;
    }
}