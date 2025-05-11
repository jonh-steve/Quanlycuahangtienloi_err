using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Controls
{
    public partial class CustomerSelectorControl : UserControl
    {
        private readonly ICustomerService _customerService;
        private List<CustomerDTO> _customers;

        public event EventHandler<CustomerDTO> CustomerSelected;

        // Thuộc tính khách hàng đã chọn
        private CustomerDTO _selectedCustomer;
        public CustomerDTO SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                DisplaySelectedCustomer();
            }
        }

        public CustomerSelectorControl(ICustomerService customerService)
        {
            InitializeComponent();
            _customerService = customerService;

            // Thiết lập màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 230, 240);
            btnSelect.BackColor = Color.FromArgb(252, 157, 192);
            btnClear.BackColor = Color.FromArgb(252, 157, 192);

            // Thiết lập sự kiện
            this.Load += CustomerSelectorControl_Load;
            btnSelect.Click += BtnSelect_Click;
            btnClear.Click += BtnClear_Click;

            // Thiết lập kích thước mặc định
            this.Size = new Size(400, 120);
        }

        private void CustomerSelectorControl_Load(object sender, EventArgs e)
        {
            try
            {
                // Tải danh sách khách hàng
                _customers = _customerService.GetAllCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khách hàng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            using (var frm = new CustomerSearchForm(_customerService, _customers))
            {
                if (frm.ShowDialog() == DialogResult.OK && frm.SelectedCustomer != null)
                {
                    _selectedCustomer = frm.SelectedCustomer;
                    DisplaySelectedCustomer();

                    // Kích hoạt sự kiện
                    CustomerSelected?.Invoke(this, _selectedCustomer);
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            _selectedCustomer = null;
            DisplaySelectedCustomer();

            // Kích hoạt sự kiện với tham số null
            CustomerSelected?.Invoke(this, null);
        }

        private void DisplaySelectedCustomer()
        {
            if (_selectedCustomer != null)
            {
                txtCustomerName.Text = _selectedCustomer.CustomerName;
                txtPhoneNumber.Text = _selectedCustomer.PhoneNumber;
                txtMembership.Text = _selectedCustomer.MembershipLevel;
                txtPoints.Text = _selectedCustomer.Points.ToString("N0");

                // Hiển thị panel thông tin
                panelInfo.Visible = true;

                // Làm mờ nút chọn và hiện nút xóa
                btnClear.Visible = true;
            }
            else
            {
                txtCustomerName.Text = string.Empty;
                txtPhoneNumber.Text = string.Empty;
                txtMembership.Text = string.Empty;
                txtPoints.Text = string.Empty;

                // Ẩn panel thông tin
                panelInfo.Visible = false;

                // Hiện nút chọn và ẩn nút xóa
                btnClear.Visible = false;
            }
        }

        private void InitializeComponent()
        {
            // Code tạo các control cho user control
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();

            this.panelInfo = new System.Windows.Forms.Panel();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.lblMembership = new System.Windows.Forms.Label();
            this.txtMembership = new System.Windows.Forms.TextBox();
            this.lblPoints = new System.Windows.Forms.Label();
            this.txtPoints = new System.Windows.Forms.TextBox();

            // Thiết lập các thuộc tính cho control
            // ...

            // Thiết lập các text box là chỉ đọc
            this.txtCustomerName.ReadOnly = true;
            this.txtPhoneNumber.ReadOnly = true;
            this.txtMembership.ReadOnly = true;
            this.txtPoints.ReadOnly = true;

            // Thiết lập màu nền cho text box
            this.txtCustomerName.BackColor = Color.FromArgb(255, 245, 250);
            this.txtPhoneNumber.BackColor = Color.FromArgb(255, 245, 250);
            this.txtMembership.BackColor = Color.FromArgb(255, 245, 250);
            this.txtPoints.BackColor = Color.FromArgb(255, 245, 250);
        }
    }
}