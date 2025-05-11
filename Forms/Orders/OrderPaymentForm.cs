// File: /Forms/Orders/OrderPaymentForm.cs
using QuanLyCuaHangTienLoi.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bunifu.Framework.UI;

namespace QuanLyCuaHangTienLoi.Forms.Orders
{
    public partial class OrderPaymentForm : Form
    {
        private readonly PaymentProcessor _paymentProcessor;
        private readonly decimal _totalAmount;

        // Kết quả form
        public int SelectedPaymentMethodID { get; private set; }
        public bool PrintReceipt { get; private set; }

        public OrderPaymentForm(PaymentProcessor paymentProcessor, decimal totalAmount)
        {
            InitializeComponent();

            _paymentProcessor = paymentProcessor;
            _totalAmount = totalAmount;

            SetupPinkTheme();
            InitializeControls();
        }

        private void SetupPinkTheme()
        {
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu hồng nhạt
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Thanh toán";

            // Tiêu đề form
            Label lblTitle = new Label();
            lblTitle.Text = "THANH TOÁN";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(231, 62, 151);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(lblTitle);
        }

        private void InitializeControls()
        {
            // Panel chính
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(20);

            // Panel tổng tiền
            Panel totalPanel = new Panel();
            totalPanel.Height = 60;
            totalPanel.Dock = DockStyle.Top;
            totalPanel.BackColor = Color.FromArgb(231, 62, 151);
            totalPanel.Margin = new Padding(0, 10, 0, 20);

            Label lblTotalText = new Label();
            lblTotalText.Text = "TỔNG TIỀN:";
            lblTotalText.ForeColor = Color.White;
            lblTotalText.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTotalText.Dock = DockStyle.Left;
            lblTotalText.Width = 150;
            lblTotalText.TextAlign = ContentAlignment.MiddleLeft;
            lblTotalText.Padding = new Padding(20, 0, 0, 0);

            Label lblTotal = new Label();
            lblTotal.Text = _totalAmount.ToString("N0") + " VND";
            lblTotal.ForeColor = Color.White;
            lblTotal.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTotal.Dock = DockStyle.Fill;
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            lblTotal.Padding = new Padding(0, 0, 20, 0);

            totalPanel.Controls.Add(lblTotal);
            totalPanel.Controls.Add(lblTotalText);

            // Panel phương thức thanh toán
            Panel methodPanel = new Panel();
            methodPanel.Height = 80;
            methodPanel.Dock = DockStyle.Top;
            methodPanel.Margin = new Padding(0, 20, 0, 0);

            Label lblMethod = new Label();
            lblMethod.Text = "Phương thức thanh toán:";
            lblMethod.Dock = DockStyle.Top;
            lblMethod.Height = 30;
            lblMethod.Font = new Font("Segoe UI", 11F);

            // ComboBox phương thức thanh toán
            BunifuDropdown cboPaymentMethod = new BunifuDropdown();
            cboPaymentMethod.BackColor = Color.Transparent;
            cboPaymentMethod.BorderRadius = 3;
            cboPaymentMethod.DisabledColor = Color.Gray;
            cboPaymentMethod.ForeColor = Color.FromArgb(64, 64, 64);
            cboPaymentMethod.Items = new string[] { "Tiền mặt", "Thẻ ngân hàng", "Ví điện tử", "Chuyển khoản" };
            cboPaymentMethod.Size = new Size(300, 35);
            cboPaymentMethod.Dock = DockStyle.Top;
            cboPaymentMethod.selectedIndex = 0;

            methodPanel.Controls.Add(cboPaymentMethod);
            methodPanel.Controls.Add(lblMethod);

            // Panel thanh toán tiền mặt
            Panel cashPanel = new Panel();
            cashPanel.Height = 140;
            cashPanel.Dock = DockStyle.Top;
            cashPanel.Visible = true; // Mặc định hiển thị panel tiền mặt

            Label lblCash = new Label();
            lblCash.Text = "Tiền khách đưa:";
            lblCash.Dock = DockStyle.Top;
            lblCash.Height = 30;
            lblCash.Font = new Font("Segoe UI", 11F);

            BunifuMaterialTextbox txtCash = new BunifuMaterialTextbox();
            txtCash.Text = _totalAmount.ToString("N0");
            txtCash.Dock = DockStyle.Top;
            txtCash.Height = 35;
            txtCash.Font = new Font("Segoe UI", 12F);
            txtCash.HintText = "Nhập số tiền khách đưa";
            txtCash.Width = 300;
            txtCash.LineThickness = 2;
            txtCash.TextAlign = HorizontalAlignment.Right;
            txtCash.TextChanged += TxtCash_TextChanged;

            Label lblChangeText = new Label();
            lblChangeText.Text = "Tiền thừa:";
            lblChangeText.Dock = DockStyle.Top;
            lblChangeText.Height = 30;
            lblChangeText.Font = new Font("Segoe UI", 11F);
            lblChangeText.Margin = new Padding(0, 10, 0, 0);

            Label lblChange = new Label();
            lblChange.Text = "0 VND";
            lblChange.Dock = DockStyle.Top;
            lblChange.Height = 35;
            lblChange.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblChange.ForeColor = Color.Green;
            lblChange.TextAlign = ContentAlignment.MiddleRight;

            cashPanel.Controls.Add(lblChange);
            cashPanel.Controls.Add(lblChangeText);
            cashPanel.Controls.Add(txtCash);
            cashPanel.Controls.Add(lblCash);

            // Panel thẻ ngân hàng (ẩn mặc định)
            Panel cardPanel = new Panel();
            cardPanel.Height = 140;
            cardPanel.Dock = DockStyle.Top;
            cardPanel.Visible = false;

            Label lblCardNumber = new Label();
            lblCardNumber.Text = "Số thẻ:";
            lblCardNumber.Width = 100;
            lblCardNumber.TextAlign = ContentAlignment.MiddleLeft;
            lblCardNumber.Location = new Point(0, 0);

            BunifuMaterialTextbox txtCardNumber = new BunifuMaterialTextbox();
            txtCardNumber.Width = 300;
            txtCardNumber.Location = new Point(100, 0);
            txtCardNumber.HintText = "Nhập số thẻ";
            txtCardNumber.LineThickness = 2;

            Label lblCardHolder = new Label();
            lblCardHolder.Text = "Chủ thẻ:";
            lblCardHolder.Width = 100;
            lblCardHolder.TextAlign = ContentAlignment.MiddleLeft;
            lblCardHolder.Location = new Point(0, 40);

            BunifuMaterialTextbox txtCardHolder = new BunifuMaterialTextbox();
            txtCardHolder.Width = 300;
            txtCardHolder.Location = new Point(100, 40);
            txtCardHolder.HintText = "Tên chủ thẻ";
            txtCardHolder.LineThickness = 2;

            Label lblExpiry = new Label();
            lblExpiry.Text = "Hết hạn:";
            lblExpiry.Width = 100;
            lblExpiry.TextAlign = ContentAlignment.MiddleLeft;
            lblExpiry.Location = new Point(0, 80);

            BunifuMaterialTextbox txtExpiry = new BunifuMaterialTextbox();
            txtExpiry.Width = 100;
            txtExpiry.Location = new Point(100, 80);
            txtExpiry.HintText = "MM/YY";
            txtExpiry.LineThickness = 2;

            Label lblCVV = new Label();
            lblCVV.Text = "CVV:";
            lblCVV.Width = 50;
            lblCVV.TextAlign = ContentAlignment.MiddleLeft;
            lblCVV.Location = new Point(220, 80);

            BunifuMaterialTextbox txtCVV = new BunifuMaterialTextbox();
            txtCVV.Width = 80;
            txtCVV.Location = new Point(270, 80);
            txtCVV.HintText = "CVV";
            txtCVV.LineThickness = 2;

            cardPanel.Controls.Add(txtCVV);
            cardPanel.Controls.Add(lblCVV);
            cardPanel.Controls.Add(txtExpiry);
            cardPanel.Controls.Add(lblExpiry);
            cardPanel.Controls.Add(txtCardHolder);
            cardPanel.Controls.Add(lblCardHolder);
            cardPanel.Controls.Add(txtCardNumber);
            cardPanel.Controls.Add(lblCardNumber);

            // Checkbox in hóa đơn
            BunifuCheckbox chkPrintReceipt = new BunifuCheckbox();
            chkPrintReceipt.BackColor = Color.FromArgb(231, 62, 151);
            chkPrintReceipt.ChechedOffColor = Color.FromArgb(132, 135, 140);
            chkPrintReceipt.Checked = true;
            chkPrintReceipt.CheckedOnColor = Color.FromArgb(231, 62, 151);
            chkPrintReceipt.Location = new Point(20, 280);
            chkPrintReceipt.Size = new Size(20, 20);

            Label lblPrintReceipt = new Label();
            lblPrintReceipt.Text = "In hóa đơn";
            lblPrintReceipt.Location = new Point(45, 280);
            lblPrintReceipt.Size = new Size(150, 20);
            lblPrintReceipt.TextAlign = ContentAlignment.MiddleLeft;

            // Panel các nút
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 60;
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Padding = new Padding(10);

            BunifuThinButton2 btnCancel = new BunifuThinButton2();
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
            btnCancel.Location = new Point(10, 10);
            btnCancel.Dock = DockStyle.Left;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            BunifuThinButton2 btnPayment = new BunifuThinButton2();
            btnPayment.ActiveBorderThickness = 1;
            btnPayment.ActiveCornerRadius = 20;
            btnPayment.ActiveFillColor = Color.SeaGreen;
            btnPayment.ActiveForecolor = Color.White;
            btnPayment.ActiveLineColor = Color.SeaGreen;
            btnPayment.BackColor = Color.FromArgb(255, 240, 245);
            btnPayment.ButtonText = "Thanh toán";
            btnPayment.Cursor = Cursors.Hand;
            btnPayment.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            btnPayment.ForeColor = Color.SeaGreen;
            btnPayment.IdleBorderThickness = 1;
            btnPayment.IdleCornerRadius = 20;
            btnPayment.IdleFillColor = Color.White;
            btnPayment.IdleForecolor = Color.SeaGreen;
            btnPayment.IdleLineColor = Color.SeaGreen;
            btnPayment.Width = 150;
            btnPayment.Location = new Point(440, 10);
            btnPayment.Dock = DockStyle.Right;
            btnPayment.Click += (s, e) =>
            {
                // Lấy thông tin thanh toán
                SelectedPaymentMethodID = cboPaymentMethod.selectedIndex + 1; // ID bắt đầu từ 1
                PrintReceipt = chkPrintReceipt.Checked;

                // Kiểm tra thông tin thanh toán
                if (SelectedPaymentMethodID == 1) // Tiền mặt
                {
                    decimal cashAmount = 0;
                    if (!decimal.TryParse(txtCash.Text.Replace(",", ""), out cashAmount))
                    {
                        MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (cashAmount < _totalAmount)
                    {
                        MessageBox.Show("Số tiền khách đưa không đủ!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (SelectedPaymentMethodID == 2) // Thẻ ngân hàng
                {
                    if (string.IsNullOrEmpty(txtCardNumber.Text) ||
                        string.IsNullOrEmpty(txtCardHolder.Text) ||
                        string.IsNullOrEmpty(txtExpiry.Text) ||
                        string.IsNullOrEmpty(txtCVV.Text))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin thẻ!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Đóng form với kết quả OK
                this.DialogResult = DialogResult.OK;
            };

            buttonPanel.Controls.Add(btnPayment);
            buttonPanel.Controls.Add(btnCancel);

            // Thêm các control vào panel chính
            mainPanel.Controls.Add(cashPanel);
            mainPanel.Controls.Add(cardPanel);
            mainPanel.Controls.Add(methodPanel);
            mainPanel.Controls.Add(totalPanel);
            mainPanel.Controls.Add(chkPrintReceipt);
            mainPanel.Controls.Add(lblPrintReceipt);

            // Thêm các panel vào form
            this.Controls.Add(buttonPanel);
            this.Controls.Add(mainPanel);

            // File: /Forms/Orders/OrderPaymentForm.cs (tiếp theo)
            // Xử lý sự kiện thay đổi phương thức thanh toán
            cboPaymentMethod.onItemSelected += (sender, e) =>
            {
                // Hiển thị panel tương ứng với phương thức thanh toán
                switch (cboPaymentMethod.selectedIndex)
                {
                    case 0: // Tiền mặt
                        cashPanel.Visible = true;
                        cardPanel.Visible = false;
                        break;
                    case 1: // Thẻ ngân hàng
                        cashPanel.Visible = false;
                        cardPanel.Visible = true;
                        break;
                    case 2: // Ví điện tử
                    case 3: // Chuyển khoản
                        cashPanel.Visible = false;
                        cardPanel.Visible = false;
                        break;
                }
            };

            // Tính tiền thừa ban đầu
            CalculateChange(txtCash, lblChange);
        }

        private void TxtCash_TextChanged(object sender, EventArgs e)
        {
            BunifuMaterialTextbox txtCash = sender as BunifuMaterialTextbox;
            Label lblChange = this.Controls.Find("lblChange", true)[0] as Label;

            CalculateChange(txtCash, lblChange);
        }

        private void CalculateChange(BunifuMaterialTextbox txtCash, Label lblChange)
        {
            try
            {
                // Chuyển đổi chuỗi thành số (bỏ qua dấu phẩy ngăn cách hàng nghìn)
                string cashText = txtCash.Text.Replace(",", "");
                decimal cashAmount = string.IsNullOrEmpty(cashText) ? 0 : decimal.Parse(cashText);

                // Tính tiền thừa
                decimal change = cashAmount - _totalAmount;

                // Hiển thị tiền thừa
                lblChange.Text = change >= 0 ? change.ToString("N0") + " VND" : "Thiếu " + Math.Abs(change).ToString("N0") + " VND";
                lblChange.ForeColor = change >= 0 ? Color.Green : Color.Red;
            }
            catch
            {
                lblChange.Text = "0 VND";
                lblChange.ForeColor = Color.Red;
            }
        }
    }
}