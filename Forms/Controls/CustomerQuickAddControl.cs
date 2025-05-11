// Steve-Thuong_hai
using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Controls
{
    public partial class CustomerQuickAddControl : UserControl
    {
        private readonly CustomerService _customerService;
        private readonly int _currentEmployeeID;

        // Sự kiện khi chọn khách hàng
        public event EventHandler<int> CustomerSelected;

        // Colors for pink theme
        private readonly Color _primaryColor = Color.FromArgb(255, 192, 203); // Pink
        private readonly Color _secondaryColor = Color.FromArgb(255, 182, 193); // LightPink
        private readonly Color _accentColor = Color.FromArgb(219, 112, 147); // PaleVioletRed
        private readonly Color _textColor = Color.FromArgb(60, 60, 60); // Dark gray for text

        public CustomerQuickAddControl(CustomerService customerService, int currentEmployeeID)
        {
            InitializeComponent();
            _customerService = customerService;
            _currentEmployeeID = currentEmployeeID;

            // Áp dụng theme
            ApplyPinkTheme();
        }

        private void ApplyPinkTheme()
        {
            // Set control properties
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            // Set group box
            groupBoxCustomer.ForeColor = _accentColor;
            groupBoxCustomer.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            // Set labels
            lblPhoneNumber.ForeColor = _textColor;
            lblPhoneNumber.Font = new Font("Segoe UI", 9.5F);

            lblCustomerName.ForeColor = _textColor;
            lblCustomerName.Font = new Font("Segoe UI", 9.5F);

            lblSelectedCustomer.ForeColor = _accentColor;
            lblSelectedCustomer.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            // Set buttons
            btnSearch.BackColor = _accentColor;
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSearch.Cursor = Cursors.Hand;

            btnNew.BackColor = _accentColor;
            btnNew.ForeColor = Color.White;
            btnNew.FlatStyle = FlatStyle.Flat;
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNew.Cursor = Cursors.Hand;

            btnClear.BackColor = Color.Gray;
            btnClear.ForeColor = Color.White;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnClear.Cursor = Cursors.Hand;

            // Set textboxes
            txtPhoneNumber.BorderStyle = BorderStyle.FixedSingle;
            txtPhoneNumber.Font = new Font("Segoe UI", 9.5F);

            txtCustomerName.BorderStyle = BorderStyle.FixedSingle;
            txtCustomerName.Font = new Font("Segoe UI", 9.5F);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string phoneNumber = txtPhoneNumber.Text.Trim();

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại khách hàng", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPhoneNumber.Focus();
                    return;
                }

                // Tìm kiếm khách hàng theo số điện thoại
                var customers = _customerService.SearchCustomers(phoneNumber);

                if (customers.Count == 0)
                {
                    lblSelectedCustomer.Text = "Không tìm thấy khách hàng";
                    lblSelectedCustomer.ForeColor = Color.Red;
                    return;
                }

                // Tìm thấy khách hàng
                var customer = customers[0]; // Lấy khách hàng đầu tiên

                // Hiển thị thông tin
                lblSelectedCustomer.Text = $"{customer.CustomerName} - Điểm: {customer.Points:N0}";
                lblSelectedCustomer.ForeColor = _accentColor;

                // Gọi sự kiện
                CustomerSelected?.Invoke(this, customer.CustomerID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                string customerName = txtCustomerName.Text.Trim();
                string phoneNumber = txtPhoneNumber.Text.Trim();

                // Validate dữ liệu
                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("Vui lòng nhập tên khách hàng", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtCustomerName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPhoneNumber.Focus();
                    return;
                }

                if (!Utils.Validators.IsValidPhoneNumber(phoneNumber))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhoneNumber.Focus();
                    return;
                }

                // Tạo đối tượng Customer
                var customer = new Customer
                {
                    CustomerName = customerName,
                    PhoneNumber = phoneNumber,
                    MembershipLevel = "Regular",
                    Points = 0
                };

                // Gọi service để tạo khách hàng mới
                int newCustomerID = _customerService.CreateCustomer(customer, _currentEmployeeID);

                if (newCustomerID > 0)
                {
                    MessageBox.Show("Thêm khách hàng mới thành công", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Hiển thị thông tin
                    lblSelectedCustomer.Text = $"{customerName} - Điểm: 0";
                    lblSelectedCustomer.ForeColor = _accentColor;

                    // Gọi sự kiện
                    CustomerSelected?.Invoke(this, newCustomerID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Xóa thông tin đã chọn
            txtPhoneNumber.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            lblSelectedCustomer.Text = "Chưa chọn khách hàng";
            lblSelectedCustomer.ForeColor = _textColor;

            // Gọi sự kiện với ID = 0 (không có khách hàng)
            CustomerSelected?.Invoke(this, 0);
        }

        private void txtPhoneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSearch_Click(sender, e);
            }
        }

        // Khởi tạo Designer components
        private void InitializeComponent()
        {
            this.groupBoxCustomer = new System.Windows.Forms.GroupBox();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblSelectedCustomer = new System.Windows.Forms.Label();

            // groupBoxCustomer
            this.groupBoxCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxCustomer.Location = new System.Drawing.Point(0, 0);
            this.groupBoxCustomer.Size = new System.Drawing.Size(350, 150);
            this.groupBoxCustomer.Text = "Thông tin khách hàng";
            this.groupBoxCustomer.Controls.Add(this.lblPhoneNumber);
            this.groupBoxCustomer.Controls.Add(this.txtPhoneNumber);
            this.groupBoxCustomer.Controls.Add(this.btnSearch);
            this.groupBoxCustomer.Controls.Add(this.lblCustomerName);
            this.groupBoxCustomer.Controls.Add(this.txtCustomerName);
            this.groupBoxCustomer.Controls.Add(this.btnNew);
            this.groupBoxCustomer.Controls.Add(this.btnClear);
            this.groupBoxCustomer.Controls.Add(this.lblSelectedCustomer);

            // lblPhoneNumber
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Location = new System.Drawing.Point(10, 30);
            this.lblPhoneNumber.Size = new System.Drawing.Size(100, 20);
            this.lblPhoneNumber.Text = "Số điện thoại:";

            // txtPhoneNumber
            this.txtPhoneNumber.Location = new System.Drawing.Point(120, 30);
            this.txtPhoneNumber.Size = new System.Drawing.Size(140, 25);
            this.txtPhoneNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPhoneNumber_KeyDown);

            // btnSearch
            this.btnSearch.Location = new System.Drawing.Point(270, 30);
            this.btnSearch.Size = new System.Drawing.Size(70, 25);
            this.btnSearch.Text = "Tìm";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // lblCustomerName
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(10, 65);
            this.lblCustomerName.Size = new System.Drawing.Size(100, 20);
            this.lblCustomerName.Text = "Tên khách hàng:";

            // txtCustomerName
            this.txtCustomerName.Location = new System.Drawing.Point(120, 65);
            this.txtCustomerName.Size = new System.Drawing.Size(140, 25);

            // btnNew
            this.btnNew.Location = new System.Drawing.Point(270, 65);
            this.btnNew.Size = new System.Drawing.Size(70, 25);
            this.btnNew.Text = "Thêm";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            // btnClear
            this.btnClear.Location = new System.Drawing.Point(270, 100);
            this.btnClear.Size = new System.Drawing.Size(70, 25);
            this.btnClear.Text = "Xóa";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            // lblSelectedCustomer
            this.lblSelectedCustomer.AutoSize = true;
            this.lblSelectedCustomer.Location = new System.Drawing.Point(10, 100);
            this.lblSelectedCustomer.Size = new System.Drawing.Size(250, 20);
            this.lblSelectedCustomer.Text = "Chưa chọn khách hàng";

            // CustomerQuickAddControl
            this.Size = new System.Drawing.Size(350, 150);
            this.Controls.Add(this.groupBoxCustomer);
        }

        private GroupBox groupBoxCustomer;
        private Label lblPhoneNumber;
        private TextBox txtPhoneNumber;
        private Button btnSearch;
        private Label lblCustomerName;
        private TextBox txtCustomerName;
        private Button btnNew;
        private Button btnClear;
        private Label lblSelectedCustomer;
    }
}