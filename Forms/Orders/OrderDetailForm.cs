// File: /Forms/Orders/OrderDetailForm.cs
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Services.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;
using Bunifu.Framework.UI;
using System.Linq;

namespace QuanLyCuaHangTienLoi.Forms.Orders
{
    public partial class OrderDetailForm : Form
    {
        private readonly IOrderService _orderService;
        private readonly int _orderID;
        private Order _order;

        public OrderDetailForm(IOrderService orderService, int orderID)
        {
            InitializeComponent();

            _orderService = orderService;
            _orderID = orderID;

            SetupPinkTheme();
            LoadOrderData();
            InitializeControls();
        }

        private void SetupPinkTheme()
        {
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu hồng nhạt
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Chi tiết đơn hàng";

            // Tiêu đề form
            Label lblTitle = new Label();
            lblTitle.Text = "CHI TIẾT ĐƠN HÀNG";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(231, 62, 151);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(lblTitle);
        }

        private void LoadOrderData()
        {
            try
            {
                // Lấy thông tin đơn hàng từ service
                _order = _orderService.GetOrderByID(_orderID);

                if (_order == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin đơn hàng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin đơn hàng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void InitializeControls()
        {
            // Panel thông tin chung đơn hàng
            Panel orderInfoPanel = new Panel();
            orderInfoPanel.Dock = DockStyle.Top;
            orderInfoPanel.Height = 150;
            orderInfoPanel.Padding = new Padding(10);

            // Thông tin cơ bản đơn hàng
            TableLayoutPanel tlpOrderInfo = new TableLayoutPanel();
            tlpOrderInfo.Dock = DockStyle.Fill;
            tlpOrderInfo.ColumnCount = 4;
            tlpOrderInfo.RowCount = 4;
            tlpOrderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlpOrderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpOrderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlpOrderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));

            // Mã đơn hàng
            Label lblOrderCode = new Label();
            lblOrderCode.Text = "Mã đơn hàng:";
            lblOrderCode.Font = new Font("Segoe UI", 10F);
            lblOrderCode.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblOrderCode, 0, 0);

            Label lblOrderCodeValue = new Label();
            lblOrderCodeValue.Text = _order.OrderCode;
            lblOrderCodeValue.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblOrderCodeValue.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblOrderCodeValue, 1, 0);

            // Ngày đặt
            Label lblOrderDate = new Label();
            lblOrderDate.Text = "Ngày đặt:";
            lblOrderDate.Font = new Font("Segoe UI", 10F);
            lblOrderDate.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblOrderDate, 2, 0);

            Label lblOrderDateValue = new Label();
            lblOrderDateValue.Text = _order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss");
            lblOrderDateValue.Font = new Font("Segoe UI", 10F);
            lblOrderDateValue.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblOrderDateValue, 3, 0);

            // Khách hàng
            Label lblCustomer = new Label();
            lblCustomer.Text = "Khách hàng:";
            lblCustomer.Font = new Font("Segoe UI", 10F);
            lblCustomer.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblCustomer, 0, 1);

            Label lblCustomerValue = new Label();
            lblCustomerValue.Text = !string.IsNullOrEmpty(_order.CustomerName) ? _order.CustomerName : "Khách lẻ";
            lblCustomerValue.Font = new Font("Segoe UI", 10F);
            lblCustomerValue.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblCustomerValue, 1, 1);

            // Nhân viên
            Label lblEmployee = new Label();
            lblEmployee.Text = "Nhân viên:";
            lblEmployee.Font = new Font("Segoe UI", 10F);
            lblEmployee.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblEmployee, 2, 1);

            Label lblEmployeeValue = new Label();
            lblEmployeeValue.Text = _order.EmployeeName;
            lblEmployeeValue.Font = new Font("Segoe UI", 10F);
            lblEmployeeValue.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblEmployeeValue, 3, 1);

            // Phương thức thanh toán
            Label lblPaymentMethod = new Label();
            lblPaymentMethod.Text = "Phương thức TT:";
            lblPaymentMethod.Font = new Font("Segoe UI", 10F);
            lblPaymentMethod.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblPaymentMethod, 0, 2);

            Label lblPaymentMethodValue = new Label();
            lblPaymentMethodValue.Text = _order.PaymentMethod;
            lblPaymentMethodValue.Font = new Font("Segoe UI", 10F);
            lblPaymentMethodValue.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblPaymentMethodValue, 1, 2);

            // Trạng thái thanh toán
            Label lblPaymentStatus = new Label();
            lblPaymentStatus.Text = "Trạng thái:";
            lblPaymentStatus.Font = new Font("Segoe UI", 10F);
            lblPaymentStatus.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblPaymentStatus, 2, 2);

            Label lblPaymentStatusValue = new Label();
            string status = "";
            switch (_order.PaymentStatus)
            {
                case "Paid":
                    status = "Đã thanh toán";
                    lblPaymentStatusValue.ForeColor = Color.Green;
                    break;
                case "Pending":
                    status = "Chờ thanh toán";
                    lblPaymentStatusValue.ForeColor = Color.Blue;
                    break;
                case "Cancelled":
                    status = "Đã hủy";
                    lblPaymentStatusValue.ForeColor = Color.Red;
                    break;
                default:
                    status = _order.PaymentStatus;
                    break;
            }
            lblPaymentStatusValue.Text = status;
            lblPaymentStatusValue.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPaymentStatusValue.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblPaymentStatusValue, 3, 2);

            // Ghi chú
            Label lblNote = new Label();
            lblNote.Text = "Ghi chú:";
            lblNote.Font = new Font("Segoe UI", 10F);
            lblNote.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblNote, 0, 3);

            Label lblNoteValue = new Label();
            lblNoteValue.Text = !string.IsNullOrEmpty(_order.Note) ? _order.Note : "";
            lblNoteValue.Font = new Font("Segoe UI", 10F);
            lblNoteValue.TextAlign = ContentAlignment.MiddleLeft;
            tlpOrderInfo.Controls.Add(lblNoteValue, 1, 3);

            orderInfoPanel.Controls.Add(tlpOrderInfo);

            // Panel chi tiết đơn hàng
            Panel orderDetailPanel = new Panel();
            orderDetailPanel.Dock = DockStyle.Fill;
            orderDetailPanel.Padding = new Padding(10);

            // Label chi tiết đơn hàng
            Label lblOrderDetail = new Label();
            lblOrderDetail.Text = "CHI TIẾT ĐƠN HÀNG";
            lblOrderDetail.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblOrderDetail.ForeColor = Color.FromArgb(231, 62, 151);
            lblOrderDetail.Dock = DockStyle.Top;
            lblOrderDetail.Height = 30;
            lblOrderDetail.TextAlign = ContentAlignment.MiddleLeft;

            // DataGridView hiển thị chi tiết đơn hàng
            BunifuCustomDataGrid dgvOrderItems = new BunifuCustomDataGrid();
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

            // Thêm các cột cho DataGridView
            dgvOrderItems.Columns.Add("ProductID", "Mã SP");
            dgvOrderItems.Columns.Add("ProductCode", "Mã sản phẩm");
            dgvOrderItems.Columns.Add("ProductName", "Tên sản phẩm");
            dgvOrderItems.Columns.Add("Unit", "ĐVT");
            dgvOrderItems.Columns.Add("Quantity", "Số lượng");
            dgvOrderItems.Columns.Add("UnitPrice", "Đơn giá");
            dgvOrderItems.Columns.Add("Discount", "Giảm giá");
            dgvOrderItems.Columns.Add("TotalPrice", "Thành tiền");

            dgvOrderItems.Columns["ProductID"].Visible = false;
            dgvOrderItems.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvOrderItems.Columns["UnitPrice"].DefaultCellStyle.Format = "N0";
            dgvOrderItems.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvOrderItems.Columns["Discount"].DefaultCellStyle.Format = "N0";
            dgvOrderItems.Columns["Discount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvOrderItems.Columns["TotalPrice"].DefaultCellStyle.Format = "N0";
            dgvOrderItems.Columns["TotalPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Hiển thị dữ liệu chi tiết đơn hàng
            foreach (var item in _order.OrderDetails)
            {
                dgvOrderItems.Rows.Add(
                    item.ProductID,
                    item.ProductCode,
                    item.ProductName,
                    item.Unit,
                    item.Quantity,
                    item.UnitPrice,
                    item.Discount,
                    item.TotalPrice
                );
            }

            // Panel hiển thị tổng tiền
            Panel totalPanel = new Panel();
            totalPanel.Height = 100;
            totalPanel.Dock = DockStyle.Bottom;
            totalPanel.BackColor = Color.FromArgb(255, 230, 240);

            // Panel tổng tiền hàng
            Panel subtotalPanel = new Panel();
            subtotalPanel.Height = 30;
            subtotalPanel.Dock = DockStyle.Top;
            subtotalPanel.Padding = new Padding(10, 5, 10, 5);

            Label lblSubtotalText = new Label();
            lblSubtotalText.Text = "Tổng tiền hàng:";
            lblSubtotalText.Dock = DockStyle.Right;
            lblSubtotalText.Width = 150;
            lblSubtotalText.TextAlign = ContentAlignment.MiddleRight;
            lblSubtotalText.Font = new Font("Segoe UI", 10F);

            Label lblSubtotal = new Label();
            lblSubtotal.Text = _order.TotalAmount.ToString("N0") + " VND";
            lblSubtotal.Dock = DockStyle.Right;
            lblSubtotal.Width = 150;
            lblSubtotal.TextAlign = ContentAlignment.MiddleRight;
            lblSubtotal.Font = new Font("Segoe UI", 10F);

            subtotalPanel.Controls.Add(lblSubtotal);
            subtotalPanel.Controls.Add(lblSubtotalText);

            // Panel thuế
            Panel taxPanel = new Panel();
            taxPanel.Height = 30;
            taxPanel.Dock = DockStyle.Top;
            taxPanel.Padding = new Padding(10, 5, 10, 5);

            Label lblTaxText = new Label();
            lblTaxText.Text = "Thuế:";
            lblTaxText.Dock = DockStyle.Right;
            lblTaxText.Width = 150;
            lblTaxText.TextAlign = ContentAlignment.MiddleRight;
            lblTaxText.Font = new Font("Segoe UI", 10F);

            Label lblTax = new Label();
            lblTax.Text = _order.Tax.ToString("N0") + " VND";
            lblTax.Dock = DockStyle.Right;
            lblTax.Width = 150;
            lblTax.TextAlign = ContentAlignment.MiddleRight;
            lblTax.Font = new Font("Segoe UI", 10F);

            taxPanel.Controls.Add(lblTax);
            taxPanel.Controls.Add(lblTaxText);

            // Panel tổng cộng
            Panel finalPanel = new Panel();
            finalPanel.Height = 40;
            finalPanel.Dock = DockStyle.Top;
            finalPanel.Padding = new Padding(10, 5, 10, 5);
            finalPanel.BackColor = Color.FromArgb(231, 62, 151);

            Label lblTotalText = new Label();
            lblTotalText.Text = "TỔNG CỘNG:";
            lblTotalText.Dock = DockStyle.Right;
            lblTotalText.Width = 150;
            lblTotalText.TextAlign = ContentAlignment.MiddleRight;
            lblTotalText.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTotalText.ForeColor = Color.White;

            Label lblTotal = new Label();
            lblTotal.Text = _order.FinalAmount.ToString("N0") + " VND";
            lblTotal.Dock = DockStyle.Right;
            lblTotal.Width = 150;
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            lblTotal.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTotal.ForeColor = Color.White;

            finalPanel.Controls.Add(lblTotal);
            finalPanel.Controls.Add(lblTotalText);

            totalPanel.Controls.Add(finalPanel);
            totalPanel.Controls.Add(taxPanel);
            totalPanel.Controls.Add(subtotalPanel);

            // Các nút
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 60;
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Padding = new Padding(10);

            // Nút in hóa đơn
            BunifuThinButton2 btnPrint = new BunifuThinButton2();
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
            btnPrint.Width = 120;
            btnPrint.Location = new Point(10, 10);
            btnPrint.TextAlign = ContentAlignment.MiddleCenter;
            btnPrint.Click += (s, e) => PrintReceipt();

            // Nút đóng
            BunifuThinButton2 btnClose = new BunifuThinButton2();
            btnClose.ActiveBorderThickness = 1;
            btnClose.ActiveCornerRadius = 20;
            btnClose.ActiveFillColor = Color.FromArgb(64, 64, 64);
            btnClose.ActiveForecolor = Color.White;
            btnClose.ActiveLineColor = Color.FromArgb(64, 64, 64);
            btnClose.BackColor = Color.FromArgb(255, 240, 245);
            btnClose.ButtonText = "Đóng";
            btnClose.Cursor = Cursors.Hand;
            btnClose.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnClose.ForeColor = Color.FromArgb(64, 64, 64);
            btnClose.IdleBorderThickness = 1;
            btnClose.IdleCornerRadius = 20;
            btnClose.IdleFillColor = Color.White;
            btnClose.IdleForecolor = Color.FromArgb(64, 64, 64);
            btnClose.IdleLineColor = Color.FromArgb(64, 64, 64);
            btnClose.Width = 120;
            btnClose.Location = new Point(this.Width - 130, 10);
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.TextAlign = ContentAlignment.MiddleCenter;
            btnClose.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(btnPrint);
            buttonPanel.Controls.Add(btnClose);

            // Thêm các control vào panel chi tiết đơn hàng
            orderDetailPanel.Controls.Add(dgvOrderItems);
            orderDetailPanel.Controls.Add(lblOrderDetail);

            // Thêm các panel vào form
            this.Controls.Add(buttonPanel);
            this.Controls.Add(totalPanel);
            this.Controls.Add(orderDetailPanel);
            this.Controls.Add(orderInfoPanel);
        }

        private void PrintReceipt()
        {
            try
            {
                // Tạo PaymentProcessor để in hóa đơn
                PaymentProcessor paymentProcessor = new PaymentProcessor();

                // Chuẩn bị dữ liệu cho hóa đơn
                var items = _order.OrderDetails
                    .Select(d => new Tuple<string, int, decimal, decimal>(
                        d.ProductName,
                        d.Quantity,
                        d.UnitPrice,
                        d.TotalPrice))
                    .ToList();

                // Gọi phương thức in hóa đơn
                bool success = paymentProcessor.PrintReceipt(
                    _order.OrderID,
                    _order.OrderCode,
                    _order.OrderDate,
                    _order.CustomerName,
                    _order.EmployeeName,
                    _order.TotalAmount,
                    _order.Tax,
                    _order.Discount,
                    _order.FinalAmount,
                    items,
                    _order.PaymentMethod,
                    0, // Số tiền khách đưa (thông tin này có thể không có)
                    0  // Tiền thừa (thông tin này có thể không có)
                );

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
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi in hóa đơn: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}