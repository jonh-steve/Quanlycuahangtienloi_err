// File: /Forms/Orders/OrderListForm.cs
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bunifu.Framework.UI;
using log4net;
using QuanLyCuaHangTienLoi.Services;
using System.Linq;
using QuanLyCuaHangTienLoi.Db.Repositories;

namespace QuanLyCuaHangTienLoi.Forms.Orders
{
    public partial class OrderListForm : Form
    {
        private readonly IOrderService _orderService;
        private List<Order> _orders;

        // Controls
        private BunifuCustomDataGrid dgvOrders;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private BunifuDropdown cboPaymentStatus;
        private BunifuThinButton2 btnSearch;
        private BunifuThinButton2 btnViewDetail;
        private BunifuThinButton2 btnCancel;
        private BunifuThinButton2 btnPrint;
        private BunifuThinButton2 btnNewOrder;

        public OrderListForm(IOrderService orderService)
        {
            InitializeComponent();

            _orderService = orderService;

            SetupPinkTheme();
            InitializeControls();
        }

        private void SetupPinkTheme()
        {
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu hồng nhạt
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Danh sách đơn hàng";

            // Tiêu đề form
            Label lblTitle = new Label();
            lblTitle.Text = "DANH SÁCH ĐƠN HÀNG";
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(231, 62, 151);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(lblTitle);
        }

        private void InitializeControls()
        {
            // Panel tìm kiếm
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 80;
            searchPanel.Padding = new Padding(10);

            // Label ngày bắt đầu
            Label lblStartDate = new Label();
            lblStartDate.Text = "Từ ngày:";
            lblStartDate.Width = 80;
            lblStartDate.TextAlign = ContentAlignment.MiddleLeft;
            lblStartDate.Location = new Point(10, 15);

            // DateTimePicker ngày bắt đầu
            dtpStartDate = new DateTimePicker();
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.Width = 120;
            dtpStartDate.Location = new Point(90, 15);
            dtpStartDate.Value = DateTime.Today.AddDays(-7);

            // Label ngày kết thúc
            Label lblEndDate = new Label();
            lblEndDate.Text = "Đến ngày:";
            lblEndDate.Width = 80;
            lblEndDate.TextAlign = ContentAlignment.MiddleLeft;
            lblEndDate.Location = new Point(230, 15);

            // DateTimePicker ngày kết thúc
            dtpEndDate = new DateTimePicker();
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Width = 120;
            dtpEndDate.Location = new Point(310, 15);
            dtpEndDate.Value = DateTime.Today;

            // Label trạng thái
            Label lblStatus = new Label();
            lblStatus.Text = "Trạng thái:";
            lblStatus.Width = 80;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.Location = new Point(450, 15);

            // Dropdown trạng thái
            cboPaymentStatus = new BunifuDropdown();
            cboPaymentStatus.BackColor = Color.Transparent;
            cboPaymentStatus.BorderRadius = 3;
            cboPaymentStatus.DisabledColor = Color.Gray;
            cboPaymentStatus.ForeColor = Color.FromArgb(64, 64, 64);
            cboPaymentStatus.Items = new string[] { "Tất cả", "Đã thanh toán", "Chờ thanh toán", "Đã hủy" };
            cboPaymentStatus.Size = new Size(150, 35);
            cboPaymentStatus.Location = new Point(530, 15);
            cboPaymentStatus.selectedIndex = 0;

            // Nút tìm kiếm
            btnSearch = new BunifuThinButton2();
            btnSearch.ActiveBorderThickness = 1;
            btnSearch.ActiveCornerRadius = 20;
            btnSearch.ActiveFillColor = Color.FromArgb(231, 62, 151);
            btnSearch.ActiveForecolor = Color.White;
            btnSearch.ActiveLineColor = Color.FromArgb(231, 62, 151);
            btnSearch.BackColor = Color.FromArgb(255, 240, 245);
            btnSearch.ButtonText = "Tìm kiếm";
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnSearch.ForeColor = Color.FromArgb(231, 62, 151);
            btnSearch.IdleBorderThickness = 1;
            btnSearch.IdleCornerRadius = 20;
            btnSearch.IdleFillColor = Color.White;
            btnSearch.IdleForecolor = Color.FromArgb(231, 62, 151);
            btnSearch.IdleLineColor = Color.FromArgb(231, 62, 151);
            btnSearch.Size = new Size(120, 40);
            btnSearch.Location = new Point(700, 12);
            btnSearch.TextAlign = ContentAlignment.MiddleCenter;
            btnSearch.Click += BtnSearch_Click;

            // Thêm các control vào panel tìm kiếm
            searchPanel.Controls.Add(btnSearch);
            searchPanel.Controls.Add(cboPaymentStatus);
            searchPanel.Controls.Add(lblStatus);
            searchPanel.Controls.Add(dtpEndDate);
            searchPanel.Controls.Add(lblEndDate);
            searchPanel.Controls.Add(dtpStartDate);
            searchPanel.Controls.Add(lblStartDate);

            // Panel nút chức năng
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            // Nút xem chi tiết
            btnViewDetail = new BunifuThinButton2();
            btnViewDetail.ActiveBorderThickness = 1;
            btnViewDetail.ActiveCornerRadius = 20;
            btnViewDetail.ActiveFillColor = Color.FromArgb(231, 62, 151);
            btnViewDetail.ActiveForecolor = Color.White;
            btnViewDetail.ActiveLineColor = Color.FromArgb(231, 62, 151);
            btnViewDetail.BackColor = Color.FromArgb(255, 240, 245);
            btnViewDetail.ButtonText = "Xem chi tiết";
            btnViewDetail.Cursor = Cursors.Hand;
            btnViewDetail.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnViewDetail.ForeColor = Color.FromArgb(231, 62, 151);
            btnViewDetail.IdleBorderThickness = 1;
            btnViewDetail.IdleCornerRadius = 20;
            btnViewDetail.IdleFillColor = Color.White;
            btnViewDetail.IdleForecolor = Color.FromArgb(231, 62, 151);
            btnViewDetail.IdleLineColor = Color.FromArgb(231, 62, 151);
            btnViewDetail.Size = new Size(120, 40);
            btnViewDetail.Location = new Point(10, 10);
            btnViewDetail.TextAlign = ContentAlignment.MiddleCenter;
            btnViewDetail.Click += BtnViewDetail_Click;

            // Nút hủy đơn hàng
            btnCancel = new BunifuThinButton2();
            btnCancel.ActiveBorderThickness = 1;
            btnCancel.ActiveCornerRadius = 20;
            btnCancel.ActiveFillColor = Color.FromArgb(255, 128, 128);
            btnCancel.ActiveForecolor = Color.White;
            btnCancel.ActiveLineColor = Color.FromArgb(255, 128, 128);
            btnCancel.BackColor = Color.FromArgb(255, 240, 245);
            btnCancel.ButtonText = "Hủy đơn";
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnCancel.ForeColor = Color.FromArgb(255, 128, 128);
            btnCancel.IdleBorderThickness = 1;
            btnCancel.IdleCornerRadius = 20;
            btnCancel.IdleFillColor = Color.White;
            btnCancel.IdleForecolor = Color.FromArgb(255, 128, 128);
            btnCancel.IdleLineColor = Color.FromArgb(255, 128, 128);
            btnCancel.Size = new Size(120, 40);
            btnCancel.Location = new Point(140, 10);
            btnCancel.TextAlign = ContentAlignment.MiddleCenter;
            btnCancel.Click += BtnCancel_Click;

            // Nút in hóa đơn
            btnPrint = new BunifuThinButton2();
            btnPrint.ActiveBorderThickness = 1;
            btnPrint.ActiveCornerRadius = 20;
            btnPrint.ActiveFillColor = Color.FromArgb(231, 62, 151);
            btnPrint.ActiveForecolor = Color.White;
            btnPrint.ActiveLineColor = Color.FromArgb(231, 62, 151);
            btnPrint.BackColor = Color.FromArgb(255, 240, 245);
            btnPrint.ButtonText = "In hóa đơn";
            btnPrint.Cursor = Cursors.Hand;
            btnPrint.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnPrint.ForeColor = Color.FromArgb(231, 62, 151);
            btnPrint.IdleBorderThickness = 1;
            btnPrint.IdleCornerRadius = 20;
            btnPrint.IdleFillColor = Color.White;
            btnPrint.IdleForecolor = Color.FromArgb(231, 62, 151);
            btnPrint.IdleLineColor = Color.FromArgb(231, 62, 151);
            btnPrint.Size = new Size(120, 40);
            btnPrint.Location = new Point(270, 10);
            btnPrint.TextAlign = ContentAlignment.MiddleCenter;
            btnPrint.Click += BtnPrint_Click;

            // Nút tạo đơn mới
            btnNewOrder = new BunifuThinButton2();
            btnNewOrder.ActiveBorderThickness = 1;
            btnNewOrder.ActiveCornerRadius = 20;
            btnNewOrder.ActiveFillColor = Color.SeaGreen;
            btnNewOrder.ActiveForecolor = Color.White;
            btnNewOrder.ActiveLineColor = Color.SeaGreen;
            btnNewOrder.BackColor = Color.FromArgb(255, 240, 245);
            btnNewOrder.ButtonText = "Tạo đơn mới";
            btnNewOrder.Cursor = Cursors.Hand;
            btnNewOrder.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            btnNewOrder.ForeColor = Color.SeaGreen;
            btnNewOrder.IdleBorderThickness = 1;
            btnNewOrder.IdleCornerRadius = 20;
            btnNewOrder.IdleFillColor = Color.White;
            btnNewOrder.IdleForecolor = Color.SeaGreen;
            btnNewOrder.IdleLineColor = Color.SeaGreen;
            btnNewOrder.Size = new Size(150, 40);
            btnNewOrder.Location = new Point(this.Width - 160, 10);
            btnNewOrder.TextAlign = ContentAlignment.MiddleCenter;
            btnNewOrder.Click += BtnNewOrder_Click;

            // Thêm các nút vào panel
            buttonPanel.Controls.Add(btnNewOrder);
            buttonPanel.Controls.Add(btnPrint);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnViewDetail);

            // DataGridView hiển thị danh sách đơn hàng
            dgvOrders = new BunifuCustomDataGrid();
            dgvOrders.BackgroundColor = Color.White;
            dgvOrders.BorderStyle = BorderStyle.None;
            dgvOrders.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(231, 62, 151);
            dgvOrders.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvOrders.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 62, 151);
            dgvOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvOrders.ColumnHeadersHeight = 35;
            dgvOrders.DefaultCellStyle.BackColor = Color.White;
            dgvOrders.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvOrders.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 200, 230);
            dgvOrders.DefaultCellStyle.SelectionForeColor = Color.FromArgb(64, 64, 64);
            dgvOrders.EnableHeadersVisualStyles = false;
            dgvOrders.GridColor = Color.FromArgb(242, 145, 190);
            dgvOrders.RowHeadersVisible = false;
            dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrders.Dock = DockStyle.Fill;
            dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrders.CellDoubleClick += DgvOrders_CellDoubleClick;

            // Thêm các cột cho DataGridView
            dgvOrders.Columns.Add("OrderID", "Mã ĐH");
            dgvOrders.Columns.Add("OrderCode", "Mã đơn hàng");
            dgvOrders.Columns.Add("OrderDate", "Ngày đặt");
            dgvOrders.Columns.Add("CustomerName", "Khách hàng");
            dgvOrders.Columns.Add("EmployeeName", "Nhân viên");
            dgvOrders.Columns.Add("ItemCount", "SL mặt hàng");
            dgvOrders.Columns.Add("TotalAmount", "Tổng tiền");
            dgvOrders.Columns.Add("PaymentMethod", "Phương thức TT");
            dgvOrders.Columns.Add("PaymentStatus", "Trạng thái");

            dgvOrders.Columns["OrderID"].Visible = false;
            dgvOrders.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvOrders.Columns["TotalAmount"].DefaultCellStyle.Format = "N0";
            dgvOrders.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvOrders.Columns["ItemCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Thêm các control vào form
            this.Controls.Add(dgvOrders);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(searchPanel);

            // Load dữ liệu đơn hàng
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                // Lấy thông tin tìm kiếm
                DateTime startDate = dtpStartDate.Value.Date;
                DateTime endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1);
                string paymentStatus = null;

                // Xử lý trạng thái thanh toán
                switch (cboPaymentStatus.selectedIndex)
                {
                    case 1: // Đã thanh toán
                        paymentStatus = "Paid";
                        break;
                    case 2: // Chờ thanh toán
                        paymentStatus = "Pending";
                        break;
                    case 3: // Đã hủy
                        paymentStatus = "Cancelled";
                        break;
                }

                // Lấy danh sách đơn hàng từ service
                _orders = _orderService.GetAllOrders(startDate, endDate, null, paymentStatus);

                // Hiển thị danh sách đơn hàng
                DisplayOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu đơn hàng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayOrders()
        {
            dgvOrders.Rows.Clear();

            foreach (var order in _orders)
            {
                // Xử lý trạng thái thanh toán
                string status = order.PaymentStatus;
                switch (status)
                {
                    case "Paid":
                        status = "Đã thanh toán";
                        break;
                    case "Pending":
                        status = "Chờ thanh toán";
                        break;
                    case "Cancelled":
                        status = "Đã hủy";
                        break;
                }

                dgvOrders.Rows.Add(
                    order.OrderID,
                    order.OrderCode,
                    order.OrderDate,
                    order.CustomerName,
                    order.EmployeeName,
                    order.ItemCount,
                    order.FinalAmount,
                    order.PaymentMethod,
                    status
                );

                // Đổi màu dòng theo trạng thái
                int rowIndex = dgvOrders.Rows.Count - 1;
                if (order.PaymentStatus == "Cancelled")
                {
                    dgvOrders.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                    dgvOrders.Rows[rowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Strikeout);
                }
                else if (order.PaymentStatus == "Paid")
                {
                    dgvOrders.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Green;
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void BtnViewDetail_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count > 0)
            {
                int orderID = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);

                // Mở form chi tiết đơn hàng
                using (OrderDetailForm detailForm = new OrderDetailForm(_orderService, orderID))
                {
                    detailForm.StartPosition = FormStartPosition.CenterParent;
                    detailForm.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xem chi tiết!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count > 0)
            {
                int orderID = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
                string orderCode = dgvOrders.SelectedRows[0].Cells["OrderCode"].Value.ToString();
                string status = dgvOrders.SelectedRows[0].Cells["PaymentStatus"].Value.ToString();

                // Kiểm tra trạng thái đơn hàng
                if (status == "Đã hủy")
                {
                    MessageBox.Show("Đơn hàng này đã bị hủy trước đó!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Xác nhận hủy đơn hàng
                DialogResult result = MessageBox.Show($"Bạn có chắc muốn hủy đơn hàng {orderCode}?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Nhập lý do hủy đơn
                    string reason = "";
                    using (InputBoxForm inputBox = new InputBoxForm("Hủy đơn hàng", "Nhập lý do hủy đơn hàng:"))
                    {
                        inputBox.StartPosition = FormStartPosition.CenterParent;

                        if (inputBox.ShowDialog() == DialogResult.OK)
                        {
                            reason = inputBox.InputText;

                            try
                            {
                                // Gọi service để hủy đơn hàng
                                bool success = _orderService.CancelOrder(orderID, reason, LoginManager.CurrentEmployee.EmployeeID);

                                if (success)
                                {
                                    MessageBox.Show("Đơn hàng đã được hủy thành công!", "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Tải lại danh sách đơn hàng
                                    LoadOrders();
                                }
                                else
                                {
                                    MessageBox.Show("Không thể hủy đơn hàng này!", "Lỗi",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi hủy đơn hàng: " + ex.Message, "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần hủy!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count > 0)
            {
                int orderID = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
                string orderCode = dgvOrders.SelectedRows[0].Cells["OrderCode"].Value.ToString();

                try
                {
                    // Lấy thông tin đơn hàng từ service
                    Order order = _orderService.GetOrderByID(orderID);

                    if (order != null)
                    {
                        // Tạo PaymentProcessor để in hóa đơn
                        PaymentProcessor paymentProcessor = new PaymentProcessor();

                        // Chuẩn bị dữ liệu cho hóa đơn
                        List<Tuple<string, int, decimal, decimal>> items = order.OrderDetails
                            .Select(d => new Tuple<string, int, decimal, decimal>(
                                d.ProductName,
                                d.Quantity,
                                d.UnitPrice,
                                d.TotalPrice))
                            .ToList();

                        // Gọi phương thức in hóa đơn
                        bool success = paymentProcessor.PrintReceipt(
                            order.OrderID,
                            order.OrderCode,
                            order.OrderDate,
                            order.CustomerName,
                            order.EmployeeName,
                            order.TotalAmount,
                            order.Tax,
                            order.Discount,
                            order.FinalAmount,
                            items,
                            order.PaymentMethod,
                            0, // Số tiền khách đưa (thông tin này có thể không có)
                            0  // Tiền thừa (thông tin này có thể không có)
                        );

                        // File: /Forms/Orders/OrderListForm.cs (tiếp theo)
                        if (success)
                        {
                            MessageBox.Show("Hóa đơn đã được in thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi xảy ra khi in hóa đơn!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi in hóa đơn: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần in hóa đơn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnNewOrder_Click(object sender, EventArgs e)
        {
            // Mở form bán hàng mới
            POSForm posForm = new POSForm(_orderService, _productService, _customerService, _employeeService);
            posForm.ShowDialog();

            // Sau khi tạo đơn hàng mới, tải lại danh sách đơn hàng
            LoadOrders();
        }

        private void DgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int orderID = Convert.ToInt32(dgvOrders.Rows[e.RowIndex].Cells["OrderID"].Value);

                // Mở form chi tiết đơn hàng
                using (OrderDetailForm detailForm = new OrderDetailForm(_orderService, orderID))
                {
                    detailForm.StartPosition = FormStartPosition.CenterParent;
                    detailForm.ShowDialog();
                }
            }
        }
    }
}