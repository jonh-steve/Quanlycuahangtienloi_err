// File: /Forms/Inventory/InventoryAdjustmentForm.cs
// Mô tả: Form điều chỉnh số lượng tồn kho
// Author: Steve-Thuong_hai

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Inventory
{
    public partial class InventoryAdjustmentForm : Form
    {
        private readonly InventoryDTO _inventory;
        private readonly IInventoryService _inventoryService;

        public InventoryAdjustmentForm(InventoryDTO inventory, IInventoryService inventoryService)
        {
            InitializeComponent();
            _inventory = inventory;
            _inventoryService = inventoryService;

            // Thiết lập màu hồng
            SetPinkTheme();

            // Hiển thị thông tin sản phẩm
            DisplayProductInfo();
        }

        private void SetPinkTheme()
        {
            // Màu hồng cho form
            this.BackColor = Color.FromArgb(255, 240, 245);

            // Màu hồng cho panel
            panelTop.BackColor = Color.FromArgb(255, 182, 193);
            lblTitle.ForeColor = Color.White;

            // Màu hồng cho các button
            btnSave.BackColor = Color.FromArgb(255, 182, 193);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;

            btnCancel.BackColor = Color.Silver;
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;

            // Màu hồng cho GroupBox
            groupProductInfo.ForeColor = Color.FromArgb(219, 112, 147);
            groupProductInfo.Font = new Font(groupProductInfo.Font, FontStyle.Bold);

            groupAdjustment.ForeColor = Color.FromArgb(219, 112, 147);
            groupAdjustment.Font = new Font(groupAdjustment.Font, FontStyle.Bold);
        }

        private void DisplayProductInfo()
        {
            // Hiển thị thông tin sản phẩm
            lblProductCode.Text = _inventory.ProductCode;
            lblProductName.Text = _inventory.ProductName;
            lblCategory.Text = _inventory.CategoryName;
            lblCurrentQuantity.Text = _inventory.Quantity.ToString();
            lblUnit.Text = _inventory.Unit;

            // Hiển thị số lượng tồn kho tối thiểu
            lblMinimumStock.Text = _inventory.MinimumStock.ToString();

            // Hiển thị trạng thái tồn kho
            if (_inventory.IsLowStock)
            {
                lblStockStatus.Text = "SẮP HẾT";
                lblStockStatus.ForeColor = Color.Red;
            }
            else if (_inventory.Quantity <= 0)
            {
                lblStockStatus.Text = "HẾT HÀNG";
                lblStockStatus.ForeColor = Color.Red;
            }
            else
            {
                lblStockStatus.Text = "CÒN HÀNG";
                lblStockStatus.ForeColor = Color.Green;
            }

            // Thiết lập giá trị ban đầu cho spinner
            nudNewQuantity.Value = _inventory.Quantity;
            nudNewQuantity.Minimum = 0;
            nudNewQuantity.Maximum = 999999;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có thay đổi số lượng không
            if (nudNewQuantity.Value == _inventory.Quantity)
            {
                MessageBox.Show("Số lượng tồn kho không thay đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Kiểm tra lý do điều chỉnh
            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                MessageBox.Show("Vui lòng nhập lý do điều chỉnh", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReason.Focus();
                return;
            }

            try
            {
                // Điều chỉnh tồn kho
                bool result = _inventoryService.AdjustInventory(
                    _inventory.ProductID,
                    (int)nudNewQuantity.Value,
                    txtReason.Text,
                    1 // Tạm thời sử dụng EmployeeID = 1
                );

                if (result)
                {
                    MessageBox.Show("Điều chỉnh tồn kho thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Điều chỉnh tồn kho thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void nudNewQuantity_ValueChanged(object sender, EventArgs e)
        {
            // Tính toán độ chênh lệch
            int difference = (int)nudNewQuantity.Value - _inventory.Quantity;

            // Hiển thị thông tin chênh lệch
            if (difference > 0)
            {
                lblDifference.Text = "+" + difference.ToString();
                lblDifference.ForeColor = Color.Green;
            }
            else if (difference < 0)
            {
                lblDifference.Text = difference.ToString();
                lblDifference.ForeColor = Color.Red;
            }
            else
            {
                lblDifference.Text = "0";
                lblDifference.ForeColor = Color.Black;
            }
        }

        #region Designer Generated Code

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupProductInfo = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLabelProductCode = new System.Windows.Forms.Label();
            this.lblLabelProductName = new System.Windows.Forms.Label();
            this.lblLabelCategory = new System.Windows.Forms.Label();
            this.lblLabelCurrentQuantity = new System.Windows.Forms.Label();
            this.lblLabelUnit = new System.Windows.Forms.Label();
            this.lblLabelMinimumStock = new System.Windows.Forms.Label();
            this.lblLabelStockStatus = new System.Windows.Forms.Label();
            this.lblProductCode = new System.Windows.Forms.Label();
            this.lblProductName = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lblCurrentQuantity = new System.Windows.Forms.Label();
            this.lblUnit = new System.Windows.Forms.Label();
            this.lblMinimumStock = new System.Windows.Forms.Label();
            this.lblStockStatus = new System.Windows.Forms.Label();
            this.groupAdjustment = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLabelNewQuantity = new System.Windows.Forms.Label();
            this.lblLabelDifference = new System.Windows.Forms.Label();
            this.lblLabelReason = new System.Windows.Forms.Label();
            this.nudNewQuantity = new System.Windows.Forms.NumericUpDown();
            this.lblDifference = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(500, 50);
            this.panelTop.TabIndex = 0;

            // lblTitle
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ĐIỀU CHỈNH TỒN KHO";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // groupProductInfo
            this.groupProductInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupProductInfo.Controls.Add(this.tableLayoutPanel1);
            this.groupProductInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupProductInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(112)))), ((int)(((byte)(147)))));
            this.groupProductInfo.Location = new System.Drawing.Point(12, 56);
            this.groupProductInfo.Name = "groupProductInfo";
            this.groupProductInfo.Size = new System.Drawing.Size(476, 230);
            this.groupProductInfo.TabIndex = 1;
            this.groupProductInfo.TabStop = false;
            this.groupProductInfo.Text = "Thông tin sản phẩm";

            // tableLayoutPanel1
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Controls.Add(this.lblLabelProductCode, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblLabelProductName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblLabelCategory, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblLabelCurrentQuantity, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblLabelUnit, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblLabelMinimumStock, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblLabelStockStatus, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblProductCode, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblProductName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblCategory, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblCurrentQuantity, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblUnit, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblMinimumStock, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblStockStatus, 1, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(470, 208);
            this.tableLayoutPanel1.TabIndex = 0;

            // lblLabelProductCode
            this.lblLabelProductCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelProductCode.AutoSize = true;
            this.lblLabelProductCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelProductCode.ForeColor = System.Drawing.Color.Black;
            this.lblLabelProductCode.Location = new System.Drawing.Point(3, 6);
            this.lblLabelProductCode.Name = "lblLabelProductCode";
            this.lblLabelProductCode.Size = new System.Drawing.Size(51, 15);
            this.lblLabelProductCode.TabIndex = 0;
            this.lblLabelProductCode.Text = "Mã SP:";

            // lblLabelProductName
            this.lblLabelProductName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelProductName.AutoSize = true;
            this.lblLabelProductName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelProductName.ForeColor = System.Drawing.Color.Black;
            this.lblLabelProductName.Location = new System.Drawing.Point(3, 35);
            this.lblLabelProductName.Name = "lblLabelProductName";
            this.lblLabelProductName.Size = new System.Drawing.Size(87, 15);
            this.lblLabelProductName.TabIndex = 1;
            this.lblLabelProductName.Text = "Tên sản phẩm:";

            // lblLabelCategory
            this.lblLabelCategory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelCategory.AutoSize = true;
            this.lblLabelCategory.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelCategory.ForeColor = System.Drawing.Color.Black;
            this.lblLabelCategory.Location = new System.Drawing.Point(3, 64);
            this.lblLabelCategory.Name = "lblLabelCategory";
            this.lblLabelCategory.Size = new System.Drawing.Size(67, 15);
            this.lblLabelCategory.TabIndex = 2;
            this.lblLabelCategory.Text = "Danh mục:";

            // lblLabelCurrentQuantity
            this.lblLabelCurrentQuantity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelCurrentQuantity.AutoSize = true;
            this.lblLabelCurrentQuantity.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelCurrentQuantity.ForeColor = System.Drawing.Color.Black;
            this.lblLabelCurrentQuantity.Location = new System.Drawing.Point(3, 93);
            this.lblLabelCurrentQuantity.Name = "lblLabelCurrentQuantity";
            this.lblLabelCurrentQuantity.Size = new System.Drawing.Size(102, 15);
            this.lblLabelCurrentQuantity.TabIndex = 3;
            this.lblLabelCurrentQuantity.Text = "Số lượng hiện tại:";
            // lblLabelUnit
            this.lblLabelUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelUnit.AutoSize = true;
            this.lblLabelUnit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelUnit.ForeColor = System.Drawing.Color.Black;
            this.lblLabelUnit.Location = new System.Drawing.Point(3, 122);
            this.lblLabelUnit.Name = "lblLabelUnit";
            this.lblLabelUnit.Size = new System.Drawing.Size(48, 15);
            this.lblLabelUnit.TabIndex = 4;
            this.lblLabelUnit.Text = "Đơn vị:";

            // lblLabelMinimumStock
            this.lblLabelMinimumStock.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelMinimumStock.AutoSize = true;
            this.lblLabelMinimumStock.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelMinimumStock.ForeColor = System.Drawing.Color.Black;
            this.lblLabelMinimumStock.Location = new System.Drawing.Point(3, 151);
            this.lblLabelMinimumStock.Name = "lblLabelMinimumStock";
            this.lblLabelMinimumStock.Size = new System.Drawing.Size(95, 15);
            this.lblLabelMinimumStock.TabIndex = 5;
            this.lblLabelMinimumStock.Text = "Tồn kho tối thiểu:";

            // lblLabelStockStatus
            this.lblLabelStockStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelStockStatus.AutoSize = true;
            this.lblLabelStockStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelStockStatus.ForeColor = System.Drawing.Color.Black;
            this.lblLabelStockStatus.Location = new System.Drawing.Point(3, 181);
            this.lblLabelStockStatus.Name = "lblLabelStockStatus";
            this.lblLabelStockStatus.Size = new System.Drawing.Size(96, 15);
            this.lblLabelStockStatus.TabIndex = 6;
            this.lblLabelStockStatus.Text = "Trạng thái tồn kho:";

            // lblProductCode
            this.lblProductCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblProductCode.AutoSize = true;
            this.lblProductCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProductCode.ForeColor = System.Drawing.Color.Black;
            this.lblProductCode.Location = new System.Drawing.Point(191, 6);
            this.lblProductCode.Name = "lblProductCode";
            this.lblProductCode.Size = new System.Drawing.Size(49, 15);
            this.lblProductCode.TabIndex = 7;
            this.lblProductCode.Text = "[MaSP]";

            // lblProductName
            this.lblProductName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblProductName.AutoSize = true;
            this.lblProductName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProductName.ForeColor = System.Drawing.Color.Black;
            this.lblProductName.Location = new System.Drawing.Point(191, 35);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(100, 15);
            this.lblProductName.TabIndex = 8;
            this.lblProductName.Text = "[Tên sản phẩm]";

            // lblCategory
            this.lblCategory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCategory.ForeColor = System.Drawing.Color.Black;
            this.lblCategory.Location = new System.Drawing.Point(191, 64);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(78, 15);
            this.lblCategory.TabIndex = 9;
            this.lblCategory.Text = "[Danh mục]";

            // lblCurrentQuantity
            this.lblCurrentQuantity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCurrentQuantity.AutoSize = true;
            this.lblCurrentQuantity.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCurrentQuantity.ForeColor = System.Drawing.Color.Black;
            this.lblCurrentQuantity.Location = new System.Drawing.Point(191, 93);
            this.lblCurrentQuantity.Name = "lblCurrentQuantity";
            this.lblCurrentQuantity.Size = new System.Drawing.Size(17, 15);
            this.lblCurrentQuantity.TabIndex = 10;
            this.lblCurrentQuantity.Text = "0";

            // lblUnit
            this.lblUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblUnit.AutoSize = true;
            this.lblUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUnit.ForeColor = System.Drawing.Color.Black;
            this.lblUnit.Location = new System.Drawing.Point(191, 122);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(57, 15);
            this.lblUnit.TabIndex = 11;
            this.lblUnit.Text = "[Đơn vị]";

            // lblMinimumStock
            this.lblMinimumStock.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMinimumStock.AutoSize = true;
            this.lblMinimumStock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMinimumStock.ForeColor = System.Drawing.Color.Black;
            this.lblMinimumStock.Location = new System.Drawing.Point(191, 151);
            this.lblMinimumStock.Name = "lblMinimumStock";
            this.lblMinimumStock.Size = new System.Drawing.Size(17, 15);
            this.lblMinimumStock.TabIndex = 12;
            this.lblMinimumStock.Text = "0";

            // lblStockStatus
            this.lblStockStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStockStatus.AutoSize = true;
            this.lblStockStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStockStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStockStatus.Location = new System.Drawing.Point(191, 181);
            this.lblStockStatus.Name = "lblStockStatus";
            this.lblStockStatus.Size = new System.Drawing.Size(80, 15);
            this.lblStockStatus.TabIndex = 13;
            this.lblStockStatus.Text = "CÒN HÀNG";

            // groupAdjustment
            this.groupAdjustment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupAdjustment.Controls.Add(this.tableLayoutPanel2);
            this.groupAdjustment.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupAdjustment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(112)))), ((int)(((byte)(147)))));
            this.groupAdjustment.Location = new System.Drawing.Point(12, 292);
            this.groupAdjustment.Name = "groupAdjustment";
            this.groupAdjustment.Size = new System.Drawing.Size(476, 117);
            this.groupAdjustment.TabIndex = 2;
            this.groupAdjustment.TabStop = false;
            this.groupAdjustment.Text = "Điều chỉnh tồn kho";

            // tableLayoutPanel2
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.Controls.Add(this.lblLabelNewQuantity, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblLabelDifference, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblLabelReason, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.nudNewQuantity, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblDifference, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtReason, 1, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(470, 95);
            this.tableLayoutPanel2.TabIndex = 0;

            // lblLabelNewQuantity
            this.lblLabelNewQuantity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelNewQuantity.AutoSize = true;
            this.lblLabelNewQuantity.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelNewQuantity.ForeColor = System.Drawing.Color.Black;
            this.lblLabelNewQuantity.Location = new System.Drawing.Point(3, 8);
            this.lblLabelNewQuantity.Name = "lblLabelNewQuantity";
            this.lblLabelNewQuantity.Size = new System.Drawing.Size(87, 15);
            this.lblLabelNewQuantity.TabIndex = 0;
            this.lblLabelNewQuantity.Text = "Số lượng mới:";

            // lblLabelDifference
            this.lblLabelDifference.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelDifference.AutoSize = true;
            this.lblLabelDifference.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelDifference.ForeColor = System.Drawing.Color.Black;
            this.lblLabelDifference.Location = new System.Drawing.Point(3, 39);
            this.lblLabelDifference.Name = "lblLabelDifference";
            this.lblLabelDifference.Size = new System.Drawing.Size(69, 15);
            this.lblLabelDifference.TabIndex = 1;
            this.lblLabelDifference.Text = "Chênh lệch:";

            // lblLabelReason
            this.lblLabelReason.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLabelReason.AutoSize = true;
            this.lblLabelReason.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLabelReason.ForeColor = System.Drawing.Color.Black;
            this.lblLabelReason.Location = new System.Drawing.Point(3, 71);
            this.lblLabelReason.Name = "lblLabelReason";
            this.lblLabelReason.Size = new System.Drawing.Size(87, 15);
            this.lblLabelReason.TabIndex = 2;
            this.lblLabelReason.Text = "Lý do điều chỉnh:";

            // nudNewQuantity
            this.nudNewQuantity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudNewQuantity.Location = new System.Drawing.Point(191, 4);
            this.nudNewQuantity.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nudNewQuantity.Name = "nudNewQuantity";
            this.nudNewQuantity.Size = new System.Drawing.Size(120, 23);
            this.nudNewQuantity.TabIndex = 3;
            this.nudNewQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNewQuantity.ValueChanged += new System.EventHandler(this.nudNewQuantity_ValueChanged);

            // lblDifference
            this.lblDifference.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDifference.AutoSize = true;
            this.lblDifference.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDifference.ForeColor = System.Drawing.Color.Green;
            this.lblDifference.Location = new System.Drawing.Point(191, 39);
            this.lblDifference.Name = "lblDifference";
            this.lblDifference.Size = new System.Drawing.Size(17, 15);
            this.lblDifference.TabIndex = 4;
            this.lblDifference.Text = "0";

            // txtReason
            this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReason.Location = new System.Drawing.Point(191, 67);
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(276, 23);
            this.txtReason.TabIndex = 5;

            // panelButtons
            this.panelButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnSave);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 429);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(500, 50);
            this.panelButtons.TabIndex = 3;

            // btnCancel
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(413, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // btnSave
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(332, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // InventoryAdjustmentForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 479);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.groupAdjustment);
            this.Controls.Add(this.groupProductInfo);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InventoryAdjustmentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Điều chỉnh tồn kho";
            this.panelTop.ResumeLayout(false);
            this.groupProductInfo.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupAdjustment.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNewQuantity)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupProductInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblLabelProductCode;
        private System.Windows.Forms.Label lblLabelProductName;
        private System.Windows.Forms.Label lblLabelCategory;
        private System.Windows.Forms.Label lblLabelCurrentQuantity;
        private System.Windows.Forms.Label lblLabelUnit;
        private System.Windows.Forms.Label lblLabelMinimumStock;
        private System.Windows.Forms.Label lblLabelStockStatus;
        private System.Windows.Forms.Label lblProductCode;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblCurrentQuantity;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.Label lblMinimumStock;
        private System.Windows.Forms.Label lblStockStatus;
        private System.Windows.Forms.GroupBox groupAdjustment;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblLabelNewQuantity;
        private System.Windows.Forms.Label lblLabelDifference;
        private System.Windows.Forms.Label lblLabelReason;
        private System.Windows.Forms.NumericUpDown nudNewQuantity;
        private System.Windows.Forms.Label lblDifference;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}