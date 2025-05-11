// File: /Forms/Orders/POSForm.cs
using QuanLyCuaHangTienLoi.Forms.Controls;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Bunifu.Framework.UI;
using log4net;
using QuanLyCuaHangTienLoi.Db.Repositories;

namespace QuanLyCuaHangTienLoi.Forms.Orders
{
    public partial class POSForm : Form
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly PaymentProcessor _paymentProcessor;

        // Các control trên form
        private BunifuCustomDataGrid dgvOrderItems;
        private ProductSearchControl productSearchControl;
        private BunifuMetroTextbox txtBarcode;
        private BunifuMaterialTextbox txtCustomerPhone;
        private Button btnFindCustomer;
        private BunifuMaterialTextbox txtCustomerName;
        private Button btnAddCustomer;
        private Label lblCustomerPoints;
        private Label lblSubtotal;
        private Label lblTax;
        private Label lblTotal;
        private BunifuThinButton2 btnAddProduct;
        private BunifuThinButton2 btnRemoveProduct;
        private BunifuThinButton2 btnClearOrder;
        private BunifuThinButton2 btnPayment;
        private BunifuThinButton2 btnCancel;

        // Thông tin đơn hàng
        private List<OrderDetail> orderItems = new List<OrderDetail>();
        private Customer selectedCustomer;
        private Employee currentEmployee;
        private int orderID = 0;

        public POSForm(
            IOrderService orderService,
            IProductService productService,
            ICustomerService customerService,
            IEmployeeService employeeService)
        {
            InitializeComponent();

            _orderService = orderService;
            _productService = productService;
            _customerService = customerService;
            _employeeService = employeeService;
            _paymentProcessor = new PaymentProcessor();

            // Lấy thông tin nhân viên hiện tại từ session
            currentEmployee = LoginManager.CurrentEmployee;

            SetupPinkTheme();
            InitializeControls();
        }

        private void SetupPinkTheme()
        {
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu hồng nhạt
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Bán hàng";

            // Tiêu đề form
            Label lblTitle = new Label();
            lblTitle.Text = "BÁN HÀNG";
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(231, 62, 151);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(lblTitle);
        }

        private void InitializeControls()
        {
            // Panel trái chứa danh sách sản phẩm
            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = this.Width / 2;
            leftPanel.Padding = new Padding(10);

            // Panel phải chứa thông tin đơn hàng
            Panel rightPanel = new Panel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Padding = new Padding(10);

            // Tạo control tìm kiếm sản phẩm
            productSearchControl = new ProductSearchControl(_productService);
            productSearchControl.Dock = DockStyle.Fill;
            productSearchControl.ProductSelected += ProductSearchControl_ProductSelected;

            // Tạo TextBox nhập mã vạch
            Panel barcodePanel = new Panel();
            barcodePanel.Height = 60;
            barcodePanel.Dock = DockStyle.Top;
            barcodePanel.Padding = new Padding(5);

            Label lblBarcode = new Label();
            lblBarcode.Text = "Quét mã vạch:";
            lblBarcode.Width = 100;
            lblBarcode.TextAlign = ContentAlignment.MiddleLeft;
            lblBarcode.Dock = DockStyle.Left;

            txtBarcode = new BunifuMetroTextbox();
            txtBarcode.BorderColorFocused = Color.FromArgb(231, 62, 151);
            txtBarcode.BorderColorIdle = Color.FromArgb(242, 145, 190);
            txtBarcode.BorderColorMouseHover = Color.FromArgb(231, 62, 151);
            txtBarcode.BorderThickness = 2;
            txtBarcode.Dock = DockStyle.Fill;
            txtBarcode.Font = new Font("Segoe UI", 12F);
            txtBarcode.ForeColor = Color.FromArgb(64, 64, 64);
            txtBarcode.KeyDown += TxtBarcode_KeyDown;

            barcodePanel.Controls.Add(txtBarcode);
            barcodePanel.Controls.Add(lblBarcode);

            // Thêm các control vào panel trái
            leftPanel.Controls.Add(productSearchControl);
            leftPanel.Controls.Add(barcodePanel);

            // Tạo panel thông tin khách hàng
            Panel customerPanel = new Panel();
            customerPanel.Height = 150;
            customerPanel.Dock = DockStyle.Top;
            customerPanel.Padding = new Padding(5);
            customerPanel.BackColor = Color.FromArgb(255, 230, 240);

            Label lblCustomerInfo = new Label();
            lblCustomerInfo.Text = "THÔNG TIN KHÁCH HÀNG";
            lblCustomerInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCustomerInfo.ForeColor = Color.FromArgb(231, 62, 151);
            lblCustomerInfo.Dock = DockStyle.Top;
            lblCustomerInfo.Height = 30;
            lblCustomerInfo.TextAlign = ContentAlignment.MiddleCenter;

            Panel phonePanel = new Panel();
            phonePanel.Height = 40;
            phonePanel.Dock = DockStyle.Top;
            phonePanel.Margin = new Padding(0, 10, 0, 0);

            Label lblPhone = new Label();
            lblPhone.Text = "Số điện thoại:";
            lblPhone.Width = 120;
            lblPhone.TextAlign = ContentAlignment.MiddleLeft;
            lblPhone.Dock = DockStyle.Left;

            txtCustomerPhone = new BunifuMaterialTextbox();
            txtCustomerPhone.Dock = DockStyle.Fill;
            txtCustomerPhone.HintText = "Nhập số điện thoại khách hàng";
            txtCustomerPhone.LineThickness = 2;
            txtCustomerPhone.Font = new Font("Segoe UI", 10F);

            btnFindCustomer = new Button();
            btnFindCustomer.Text = "Tìm";
            btnFindCustomer.BackColor = Color.FromArgb(231, 62, 151);
            btnFindCustomer.ForeColor = Color.White;
            btnFindCustomer.Width = 80;
            btnFindCustomer.Dock = DockStyle.Right;
            btnFindCustomer.FlatStyle = FlatStyle.Flat;
            btnFindCustomer.Click += BtnFindCustomer_Click;

            phonePanel.Controls.Add(txtCustomerPhone);
            phonePanel.Controls.Add(lblPhone);
            phonePanel.Controls.Add(btnFindCustomer);

            Panel namePanel = new Panel();
            namePanel.Height = 40;
            namePanel.Dock = DockStyle.Top;
            namePanel.Margin = new Padding(0, 10, 0, 0);

            Label lblName = new Label();
            lblName.Text = "Tên khách hàng:";
            lblName.Width = 120;
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            lblName.Dock = DockStyle.Left;

            txtCustomerName = new BunifuMaterialTextbox();
            txtCustomerName.Dock = DockStyle.Fill;
            txtCustomerName.HintText = "Tên khách hàng";
            txtCustomerName.LineThickness = 2;
            txtCustomerName.Font = new Font("Segoe UI", 10F);
            txtCustomerName.Enabled = false;

            btnAddCustomer = new Button();
            btnAddCustomer.Text = "Thêm mới";
            btnAddCustomer.BackColor = Color.FromArgb(231, 62, 151);
            btnAddCustomer.ForeColor = Color.White;
            btnAddCustomer.Width = 80;
            btnAddCustomer.Dock = DockStyle.Right;
            btnAddCustomer.FlatStyle = FlatStyle.Flat;
            btnAddCustomer.Click += BtnAddCustomer_Click;

            namePanel.Controls.Add(txtCustomerName);
            namePanel.Controls.Add(lblName);
            namePanel.Controls.Add(btnAddCustomer);

            Panel pointsPanel = new Panel();
            pointsPanel.Height = 40;
            pointsPanel.Dock = DockStyle.Top;

            Label lblPoints = new Label();
            lblPoints.Text = "Điểm tích lũy:";
            lblPoints.Width = 120;
            lblPoints.TextAlign = ContentAlignment.MiddleLeft;
            lblPoints.Dock = DockStyle.Left;

            lblCustomerPoints = new Label();
            lblCustomerPoints.Text = "0";
            lblCustomerPoints.Dock = DockStyle.Fill;
            lblCustomerPoints.TextAlign = ContentAlignment.MiddleLeft;
            lblCustomerPoints.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            pointsPanel.Controls.Add(lblCustomerPoints);
            pointsPanel.Controls.Add(lblPoints);

            customerPanel.Controls.Add(pointsPanel);
            customerPanel.Controls.Add(namePanel);
            customerPanel.Controls.Add(phonePanel);
            customerPanel.Controls.Add(lblCustomerInfo);

            // Tạo DataGridView hiển thị danh sách sản phẩm trong đơn hàng
            dgvOrderItems = new BunifuCustomDataGrid();
            dgvOrderItems.BackgroundColor = Color.White;
            dgvOrderItems.BorderStyle = BorderStyle.None;
            dgvOrderItems.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvOrderItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(231, 62, 151);
            dgvOrderItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvOrderItems.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 62, 151);
            dgvOrderItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvOrderItems.ColumnHeadersHeight = 35;
            dgvOrderItems.DefaultCellStyle.BackColor = Color.White;
            dgvOrderItems.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvOrderItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 200, 230);
            dgvOrderItems.DefaultCellStyle.SelectionForeColor = Color.FromArgb(64, 64, 64);
            dgvOrderItems.EnableHeadersVisualStyles = false;
            dgvOrderItems.GridColor = Color.FromArgb(242, 145, 190);
            dgvOrderItems.RowHeadersVisible = false;
            dgvOrderItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrderItems.Dock = DockStyle.Fill;
            dgvOrderItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvOrderItems.Columns.Add("ProductID", "Mã SP");
            dgvOrderItems.Columns.Add("ProductName", "Tên sản phẩm");
            dgvOrderItems.Columns.Add("Quantity", "Số lượng");
            dgvOrderItems.Columns.Add("UnitPrice", "Đơn giá");
            dgvOrderItems.Columns.Add("Discount", "Giảm giá");
            dgvOrderItems.Columns.Add("TotalPrice", "Thành tiền");

            dgvOrderItems.Columns["ProductID"].Visible = false;
            dgvOrderItems.Columns["UnitPrice"].DefaultCellStyle.Format = "N0";
            dgvOrderItems.Columns["Discount"].DefaultCellStyle.Format = "N0";
            dgvOrderItems.Columns["TotalPrice"].DefaultCellStyle.Format = "N0";

            // Tạo panel tổng tiền
            Panel totalPanel = new Panel();
            totalPanel.Height = 150;
            totalPanel.Dock = DockStyle.Bottom;
            totalPanel.BackColor = Color.FromArgb(255, 230, 240);

            // Panel tổng tiền hàng
            Panel subtotalPanel = new Panel();
            subtotalPanel.Height = 40;
            subtotalPanel.Dock = DockStyle.Top;
            subtotalPanel.Padding = new Padding(10, 5, 10, 5);

            Label lblSubtotalText = new Label();
            lblSubtotalText.Text = "Tổng tiền hàng:";
            lblSubtotalText.Dock = DockStyle.Left;
            lblSubtotalText.Width = 150;
            lblSubtotalText.TextAlign = ContentAlignment.MiddleLeft;
            lblSubtotalText.Font = new Font("Segoe UI", 11F);

            lblSubtotal = new Label();
            lblSubtotal.Text = "0";
            lblSubtotal.Dock = DockStyle.Right;
            lblSubtotal.Width = 200;
            lblSubtotal.TextAlign = ContentAlignment.MiddleRight;
            lblSubtotal.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            subtotalPanel.Controls.Add(lblSubtotal);
            subtotalPanel.Controls.Add(lblSubtotalText);

            // Panel thuế
            Panel taxPanel = new Panel();
            taxPanel.Height = 40;
            taxPanel.Dock = DockStyle.Top;
            taxPanel.Padding = new Padding(10, 5, 10, 5);

            Label lblTaxText = new Label();
            lblTaxText.Text = "Thuế (10%):";
            lblTaxText.Dock = DockStyle.Left;
            lblTaxText.Width = 150;
            lblTaxText.TextAlign = ContentAlignment.MiddleLeft;
            lblTaxText.Font = new Font("Segoe UI", 11F);

            lblTax = new Label();
            lblTax.Text = "0";
            lblTax.Dock = DockStyle.Right;
            lblTax.Width = 200;
            lblTax.TextAlign = ContentAlignment.MiddleRight;
            lblTax.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            taxPanel.Controls.Add(lblTax);
            taxPanel.Controls.Add(lblTaxText);

            // Panel tổng cộng
            Panel finalPanel = new Panel();
            finalPanel.Height = 50;
            finalPanel.Dock = DockStyle.Top;
            finalPanel.Padding = new Padding(10, 5, 10, 5);
            finalPanel.BackColor = Color.FromArgb(231, 62, 151);

            Label lblTotalText = new Label();
            lblTotalText.Text = "TỔNG CỘNG:";
            lblTotalText.Dock = DockStyle.Left;
            lblTotalText.Width = 150;
            lblTotalText.TextAlign = ContentAlignment.MiddleLeft;
            lblTotalText.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTotalText.ForeColor = Color.White;

            lblTotal = new Label();
            lblTotal.Text = "0";
            lblTotal.Dock = DockStyle.Right;
            lblTotal.Width = 200;
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            lblTotal.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTotal.ForeColor = Color.White;

            finalPanel.Controls.Add(lblTotal);
            finalPanel.Controls.Add(lblTotalText);

            totalPanel.Controls.Add(finalPanel);
            totalPanel.Controls.Add(taxPanel);
            totalPanel.Controls.Add(subtotalPanel);

            // Tạo panel chứa các button
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 60;
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Padding = new Padding(10, 10, 10, 10);

            btnAddProduct = new BunifuThinButton2();
            btnAddProduct.ActiveBorderThickness = 1;
            btnAddProduct.ActiveCornerRadius = 20;
            btnAddProduct.ActiveFillColor = Color.FromArgb(231, 62, 151);
            btnAddProduct.ActiveForecolor = Color.White;
            btnAddProduct.ActiveLineColor = Color.FromArgb(231, 62, 151);
            btnAddProduct.BackColor = Color.FromArgb(255, 240, 245);
            btnAddProduct.ButtonText = "Thêm";
            btnAddProduct.Cursor = Cursors.Hand;
            btnAddProduct.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnAddProduct.ForeColor = Color.FromArgb(231, 62, 151);
            btnAddProduct.IdleBorderThickness = 1;
            btnAddProduct.IdleCornerRadius = 20;
            btnAddProduct.IdleFillColor = Color.White;
            btnAddProduct.IdleForecolor = Color.FromArgb(231, 62, 151);
            btnAddProduct.IdleLineColor = Color.FromArgb(231, 62, 151);
            btnAddProduct.Width = 120;
            btnAddProduct.Dock = DockStyle.Left;
            btnAddProduct.Click += BtnAddProduct_Click;

            btnRemoveProduct = new BunifuThinButton2();
            btnRemoveProduct.ActiveBorderThickness = 1;
            btnRemoveProduct.ActiveCornerRadius = 20;
            btnRemoveProduct.ActiveFillColor = Color.FromArgb(231, 62, 151);
            btnRemoveProduct.ActiveForecolor = Color.White;
            btnRemoveProduct.ActiveLineColor = Color.FromArgb(231, 62, 151);
            btnRemoveProduct.BackColor = Color.FromArgb(255, 240, 245);
            btnRemoveProduct.ButtonText = "Xóa";
            btnRemoveProduct.Cursor = Cursors.Hand;
            btnRemoveProduct.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnRemoveProduct.ForeColor = Color.FromArgb(231, 62, 151);
            btnRemoveProduct.IdleBorderThickness = 1;
            btnRemoveProduct.IdleCornerRadius = 20;
            btnRemoveProduct.IdleFillColor = Color.White;
            btnRemoveProduct.IdleForecolor = Color.FromArgb(231, 62, 151);
            btnRemoveProduct.IdleLineColor = Color.FromArgb(231, 62, 151);
            btnRemoveProduct.Width = 120;
            btnRemoveProduct.Margin = new Padding(10, 0, 0, 0);
            btnRemoveProduct.Dock = DockStyle.Left;
            btnRemoveProduct.Click += BtnRemoveProduct_Click;

            btnClearOrder = new BunifuThinButton2();
            btnClearOrder.ActiveBorderThickness = 1;
            btnClearOrder.ActiveCornerRadius = 20;
            btnClearOrder.ActiveFillColor = Color.FromArgb(231, 62, 151);
            btnClearOrder.ActiveForecolor = Color.White;
            btnClearOrder.ActiveLineColor = Color.FromArgb(231, 62, 151);
            btnClearOrder.BackColor = Color.FromArgb(255, 240, 245);
            btnClearOrder.ButtonText = "Làm mới";
            btnClearOrder.Cursor = Cursors.Hand;
            btnClearOrder.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnClearOrder.ForeColor = Color.FromArgb(231, 62, 151);
            btnClearOrder.IdleBorderThickness = 1;
            btnClearOrder.IdleCornerRadius = 20;
            btnClearOrder.IdleFillColor = Color.White;
            btnClearOrder.IdleForecolor = Color.FromArgb(231, 62, 151);
            btnClearOrder.IdleLineColor = Color.FromArgb(231, 62, 151);
            btnClearOrder.Width = 120;
            btnClearOrder.Margin = new Padding(10, 0, 0, 0);
            btnClearOrder.Dock = DockStyle.Left;
            btnClearOrder.Click += BtnClearOrder_Click;

            btnPayment = new BunifuThinButton2();
            btnPayment.ActiveBorderThickness = 1;
            btnPayment.ActiveCornerRadius = 20;
            btnPayment.ActiveFillColor = Color.SeaGreen;
            btnPayment.ActiveForecolor = Color.White;
            btnPayment.ActiveLineColor = Color.SeaGreen;
            btnPayment.BackColor = Color.FromArgb(255, 240, 245);
            btnPayment.ButtonText = "Thanh toán";
            btnPayment.Cursor = Cursors.Hand;
            btnPayment.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            btnPayment.ForeColor = Color.SeaGreen;
            btnPayment.IdleBorderThickness = 1;
            btnPayment.IdleCornerRadius = 20;
            btnPayment.IdleFillColor = Color.White;
            btnPayment.IdleForecolor = Color.SeaGreen;
            btnPayment.IdleLineColor = Color.SeaGreen;
            btnPayment.Width = 150;
            btnPayment.Dock = DockStyle.Right;
            btnPayment.Click += BtnPayment_Click;

            btnCancel = new BunifuThinButton2();
            btnCancel.ActiveBorderThickness = 1;
            btnCancel.ActiveCornerRadius = 20;
            btnCancel.ActiveFillColor = Color.FromArgb(255, 128, 128);
            btnCancel.ActiveForecolor = Color.White;
            btnCancel.ActiveLineColor = Color.FromArgb(255, 128, 128);
            btnCancel.BackColor = Color.FromArgb(255, 240, 245);
            btnCancel.ButtonText = "Hủy";
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnCancel.ForeColor = Color.FromArgb(255, 128, 128);
            btnCancel.IdleBorderThickness = 1;
            btnCancel.IdleCornerRadius = 20;
            btnCancel.IdleFillColor = Color.White;
            btnCancel.IdleForecolor = Color.FromArgb(255, 128, 128);
            btnCancel.IdleLineColor = Color.FromArgb(255, 128, 128);
            btnCancel.Width = 120;
            btnCancel.Margin = new Padding(0, 0, 10, 0);
            btnCancel.Dock = DockStyle.Right;
            btnCancel.Click += BtnCancel_Click;

            buttonPanel.Controls.Add(btnPayment);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnClearOrder);
            buttonPanel.Controls.Add(btnRemoveProduct);
            buttonPanel.Controls.Add(btnAddProduct);

            // Thêm các control vào panel phải
            rightPanel.Controls.Add(dgvOrderItems);
            rightPanel.Controls.Add(customerPanel);
            rightPanel.Controls.Add(totalPanel);
            rightPanel.Controls.Add(buttonPanel);

            // Thêm các panel vào form
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);

            // Load dữ liệu sản phẩm
            productSearchControl.LoadProducts();

            // Làm mới đơn hàng
            ClearOrder();
        }

        private void TxtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string barcode = txtBarcode.Text.Trim();
                if (!string.IsNullOrEmpty(barcode))
                {
                    // Tìm sản phẩm theo mã vạch
                    var product = productSearchControl.FindProductByBarcode(barcode);

                    if (product != null)
                    {
                        // Thêm sản phẩm vào đơn hàng
                        AddProductToOrder(product);
                        txtBarcode.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm với mã vạch này!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ProductSearchControl_ProductSelected(object sender, Product product)
        {
            AddProductToOrder(product);
        }

        private void BtnFindCustomer_Click(object sender, EventArgs e)
        {
            string phoneNumber = txtCustomerPhone.Text.Trim();

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                try
                {
                    // Tìm khách hàng theo số điện thoại
                    var customer = _customerService.GetCustomerByPhone(phoneNumber);

                    if (customer != null)
                    {
                        // Hiển thị thông tin khách hàng
                        selectedCustomer = customer;
                        txtCustomerName.Text = customer.CustomerName;
                        lblCustomerPoints.Text = customer.Points.ToString("N0");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khách hàng với số điện thoại này!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Xóa thông tin khách hàng hiện tại
                        selectedCustomer = null;
                        txtCustomerName.Text = "";
                        lblCustomerPoints.Text = "0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tìm khách hàng: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            // Mở form thêm khách hàng mới
            using (CustomerDetailForm customerForm = new CustomerDetailForm(_customerService))
            {
                customerForm.StartPosition = FormStartPosition.CenterParent;

                if (customerForm.ShowDialog() == DialogResult.OK)
                {
                    // Lấy khách hàng vừa thêm
                    selectedCustomer = customerForm.Customer;

                    // Hiển thị thông tin khách hàng
                    txtCustomerPhone.Text = selectedCustomer.PhoneNumber;
                    txtCustomerName.Text = selectedCustomer.CustomerName;
                    lblCustomerPoints.Text = selectedCustomer.Points.ToString("N0");
                }
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            // Mở form tìm kiếm sản phẩm
            using (ProductSearchForm searchForm = new ProductSearchForm(_productService))
            {
                searchForm.StartPosition = FormStartPosition.CenterParent;
                searchForm.ProductSelected += SearchForm_ProductSelected;

                searchForm.ShowDialog();
            }
        }

        private void SearchForm_ProductSelected(object sender, Product product)
        {
            AddProductToOrder(product);
        }

        private void BtnRemoveProduct_Click(object sender, EventArgs e)
        {
            if (dgvOrderItems.SelectedRows.Count > 0)
            {
                int index = dgvOrderItems.SelectedRows[0].Index;

                // Xóa sản phẩm khỏi danh sách
                orderItems.RemoveAt(index);

                // Cập nhật lại hiển thị
                DisplayOrderItems();
                UpdateTotals();
            }
        }

        private void BtnClearOrder_Click(object sender, EventArgs e)
        {
            // Xác nhận trước khi làm mới đơn hàng
            if (orderItems.Count > 0)
            {
                DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa tất cả sản phẩm khỏi đơn hàng này?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ClearOrder();
                }
            }
            else
            {
                ClearOrder();
            }
        }

        private void BtnPayment_Click(object sender, EventArgs e)
        {
            if (orderItems.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm sản phẩm vào đơn hàng trước khi thanh toán!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tính tổng tiền đơn hàng
            decimal subtotal = orderItems.Sum(i => i.Quantity * i.UnitPrice - i.Discount);
            decimal tax = _orderService.CalculateTax(subtotal);
            decimal total = subtotal + tax;

            // Mở form thanh toán
            using (OrderPaymentForm paymentForm = new OrderPaymentForm(_paymentProcessor, total))
            {
                paymentForm.StartPosition = FormStartPosition.CenterParent;

                if (paymentForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Tạo đơn hàng mới
                        Order order = new Order
                        {
                            CustomerID = selectedCustomer?.CustomerID,
                            EmployeeID = currentEmployee.EmployeeID,
                            TotalAmount = subtotal,
                            Tax = tax,
                            Discount = 0,
                            FinalAmount = total,
                            PaymentMethodID = paymentForm.SelectedPaymentMethodID,
                            PaymentStatus = "Paid",
                            Note = ""
                        };

                        // Lưu đơn hàng vào CSDL
                        orderID = _orderService.CreateOrder(order, orderItems);

                        if (orderID > 0)
                        {
                            // Hiển thị thông tin đơn hàng vừa tạo
                            MessageBox.Show($"Đơn hàng đã được tạo thành công!\nMã đơn hàng: {order.OrderCode}\nTổng tiền: {order.FinalAmount:N0} VND",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // In hóa đơn
                            if (paymentForm.PrintReceipt)
                            {
                                PrintReceipt(order);
                            }

                            // Làm mới đơn hàng
                            ClearOrder();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tạo đơn hàng: " + ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Xác nhận trước khi đóng form
            if (orderItems.Count > 0)
            {
                DialogResult result = MessageBox.Show("Bạn có chắc muốn hủy đơn hàng này?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void AddProductToOrder(Product product)
        {
            // Kiểm tra sản phẩm đã có trong đơn hàng chưa
            var existingItem = orderItems.FirstOrDefault(i => i.ProductID == product.ProductID);

            if (existingItem != null)
            {
                // Nếu đã có, tăng số lượng
                existingItem.Quantity++;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice - existingItem.Discount;
            }
            else
            {
                // Nếu chưa có, thêm mới
                OrderDetail orderDetail = new OrderDetail
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Quantity = 1,
                    UnitPrice = product.SellPrice,
                    Discount = 0,
                    TotalPrice = product.SellPrice
                };

                orderItems.Add(orderDetail);
            }

            // Cập nhật hiển thị
            DisplayOrderItems();
            UpdateTotals();
        }

        private void DisplayOrderItems()
        {
            dgvOrderItems.Rows.Clear();

            foreach (var item in orderItems)
            {
                dgvOrderItems.Rows.Add(
                    item.ProductID,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.Discount,
                    item.TotalPrice
                );
            }
        }

        private void UpdateTotals()
        {
            decimal subtotal = orderItems.Sum(i => i.Quantity * i.UnitPrice - i.Discount);
            decimal tax = _orderService.CalculateTax(subtotal);
            decimal total = subtotal + tax;

            lblSubtotal.Text = subtotal.ToString("N0") + " VND";
            lblTax.Text = tax.ToString("N0") + " VND";
            lblTotal.Text = total.ToString("N0") + " VND";
        }

        private void ClearOrder()
        {
            // Xóa danh sách sản phẩm
            orderItems.Clear();
            dgvOrderItems.Rows.Clear();

            // Xóa thông tin khách hàng
            selectedCustomer = null;
            txtCustomerPhone.Text = "";
            txtCustomerName.Text = "";
            lblCustomerPoints.Text = "0";

            // Cập nhật tổng tiền
            UpdateTotals();

            // Focus vào ô nhập mã vạch
            txtBarcode.Focus();
        }

        private void PrintReceipt(Order order)
        {
            try
            {
                // Lấy thông tin đơn hàng đầy đủ
                Order fullOrder = _orderService.GetOrderByID(order.OrderID);

                if (fullOrder != null)
                {
                    // Chuẩn bị dữ liệu cho hóa đơn
                    List<Tuple<string, int, decimal, decimal>> items = fullOrder.OrderDetails
                        .Select(d => new Tuple<string, int, decimal, decimal>(
                            d.ProductName,
                            d.Quantity,
                            d.UnitPrice,
                            d.TotalPrice))
                        .ToList();

                    // Gọi phương thức in hóa đơn
                    _paymentProcessor.PrintReceipt(
                        fullOrder.OrderID,
                        fullOrder.OrderCode,
                        fullOrder.OrderDate,
                        fullOrder.CustomerName,
                        fullOrder.EmployeeName,
                        fullOrder.TotalAmount,
                        fullOrder.Tax,
                        fullOrder.Discount,
                        fullOrder.FinalAmount,
                        items,
                        fullOrder.PaymentMethod,
                        0, // Số tiền khách đưa (thông tin này có thể không có)
                        0  // Tiền thừa (thông tin này có thể không có)
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi in hóa đơn: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}