// File: /Forms/Inventory/InventoryListForm.cs
// Mô tả: Form hiển thị danh sách tồn kho
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Inventory
{
    public partial class InventoryListForm : Form
    {
        private readonly IInventoryService _inventoryService;
        private List<InventoryDTO> _inventoryList;
        private string _currentFilter = "ALL";

        public InventoryListForm(IInventoryService inventoryService)
        {
            InitializeComponent();
            _inventoryService = inventoryService;

            // Đặt màu hồng nhẹ cho form
            this.BackColor = Color.FromArgb(255, 240, 245);

            // Custommize các control với màu hồng
            SetPinkTheme();
        }

        private void SetPinkTheme()
        {
            // Màu hồng cho các button
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = Color.FromArgb(255, 182, 193); // Light pink
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = Color.FromArgb(219, 112, 147); // Medium pink
                }
            }

            // Màu hồng cho DataGridView
            if (dgvInventory != null)
            {
                dgvInventory.EnableHeadersVisualStyles = false;
                dgvInventory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 182, 193);
                dgvInventory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvInventory.ColumnHeadersDefaultCellStyle.Font = new Font(dgvInventory.Font, FontStyle.Bold);

                dgvInventory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 112, 147);
                dgvInventory.DefaultCellStyle.SelectionForeColor = Color.White;

                dgvInventory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245);
            }

            // Màu hồng cho các GroupBox
            foreach (Control control in this.Controls)
            {
                if (control is GroupBox groupBox)
                {
                    groupBox.ForeColor = Color.FromArgb(219, 112, 147);
                    groupBox.Font = new Font(groupBox.Font, FontStyle.Bold);
                }
            }
        }

        private void InventoryListForm_Load(object sender, EventArgs e)
        {
            LoadInventoryData();
            SetupDataGridView();
            SetupComboBoxes();
        }

        private void LoadInventoryData()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _inventoryList = _inventoryService.GetInventoryList();

                // Lọc dữ liệu theo bộ lọc hiện tại
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void SetupDataGridView()
        {
            // Thiết lập các cột cho DataGridView
            dgvInventory.AutoGenerateColumns = false;

            // Thêm các cột
            if (dgvInventory.Columns.Count == 0)
            {
                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductCode",
                    HeaderText = "Mã SP",
                    DataPropertyName = "ProductCode",
                    Width = 80
                });

                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Tên sản phẩm",
                    DataPropertyName = "ProductName",
                    Width = 200
                });

                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CategoryName",
                    HeaderText = "Danh mục",
                    DataPropertyName = "CategoryName",
                    Width = 120
                });

                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Số lượng",
                    DataPropertyName = "Quantity",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                });

                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Unit",
                    HeaderText = "Đơn vị",
                    DataPropertyName = "Unit",
                    Width = 60
                });

                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CostPrice",
                    HeaderText = "Giá nhập",
                    DataPropertyName = "CostPrice",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight,
                        Format = "N0"
                    }
                });

                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalCostValue",
                    HeaderText = "Giá trị tồn",
                    DataPropertyName = "TotalCostValue",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight,
                        Format = "N0"
                    }
                });

                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "MinimumStock",
                    HeaderText = "Tồn tối thiểu",
                    DataPropertyName = "MinimumStock",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                });

                dgvInventory.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    Name = "IsLowStock",
                    HeaderText = "Sắp hết",
                    DataPropertyName = "IsLowStock",
                    Width = 60
                });

                dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastUpdated",
                    HeaderText = "Cập nhật lúc",
                    DataPropertyName = "LastUpdated",
                    Width = 150,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
                });
            }

            // Tùy chỉnh appearance của DataGridView
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.AllowUserToAddRows = false;
            dgvInventory.AllowUserToDeleteRows = false;
            dgvInventory.AllowUserToOrderColumns = true;
            dgvInventory.ReadOnly = true;
            dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Thêm sự kiện CellFormatting để hiển thị màu đỏ cho hàng tồn kho thấp
            dgvInventory.CellFormatting += DgvInventory_CellFormatting;
        }

        private void DgvInventory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Nếu là cột IsLowStock và giá trị là true
            if (dgvInventory.Columns[e.ColumnIndex].Name == "IsLowStock" && e.Value != null &&
                Convert.ToBoolean(e.Value) == true)
            {
                // Đặt màu nền cho toàn bộ hàng
                dgvInventory.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                dgvInventory.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
            }
        }

        private void SetupComboBoxes()
        {
            // Thiết lập ComboBox Categories
            cboFilter.Items.Clear();
            cboFilter.Items.Add("Tất cả sản phẩm");
            cboFilter.Items.Add("Sản phẩm sắp hết");
            cboFilter.Items.Add("Sản phẩm hết hàng");
            cboFilter.SelectedIndex = 0;
        }

        private void ApplyFilter()
        {
            if (_inventoryList == null)
                return;

            List<InventoryDTO> filteredList;

            switch (_currentFilter)
            {
                case "LOW":
                    filteredList = _inventoryList.Where(i => i.IsLowStock).ToList();
                    break;
                case "ZERO":
                    filteredList = _inventoryList.Where(i => i.Quantity <= 0).ToList();
                    break;
                default:
                    filteredList = _inventoryList;
                    break;
            }

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string keyword = txtSearch.Text.ToLower();
                filteredList = filteredList.Where(i =>
                    i.ProductCode.ToLower().Contains(keyword) ||
                    i.ProductName.ToLower().Contains(keyword) ||
                    i.CategoryName.ToLower().Contains(keyword)
                ).ToList();
            }

            // Hiển thị dữ liệu lên DataGridView
            dgvInventory.DataSource = null;
            dgvInventory.DataSource = filteredList;

            // Cập nhật label tổng số
            lblTotalItems.Text = $"Tổng số: {filteredList.Count} sản phẩm";

            // Cập nhật label tổng giá trị tồn kho
            decimal totalValue = filteredList.Sum(i => i.TotalCostValue);
            lblTotalValue.Text = $"Tổng giá trị: {totalValue:N0} VNĐ";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInventoryData();
        }

        private void btnAdjust_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (dgvInventory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để điều chỉnh tồn kho", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy sản phẩm được chọn
            InventoryDTO selectedInventory = dgvInventory.SelectedRows[0].DataBoundItem as InventoryDTO;

            if (selectedInventory != null)
            {
                // Mở form điều chỉnh tồn kho
                using (var form = new InventoryAdjustmentForm(selectedInventory, _inventoryService))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Nếu điều chỉnh thành công, tải lại dữ liệu
                        LoadInventoryData();
                    }
                }
            }
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (dgvInventory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xem lịch sử tồn kho", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy sản phẩm được chọn
            InventoryDTO selectedInventory = dgvInventory.SelectedRows[0].DataBoundItem as InventoryDTO;

            if (selectedInventory != null)
            {
                // Mở form lịch sử tồn kho
                // using (var form = new InventoryHistoryForm(selectedInventory.ProductID, selectedInventory.ProductName, _inventoryService))
                // {
                //     form.ShowDialog();
                // }

                // Tạm thời hiển thị thông báo
                MessageBox.Show($"Xem lịch sử tồn kho của sản phẩm: {selectedInventory.ProductName}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // TODO: Implement export to Excel/PDF
            MessageBox.Show("Chức năng xuất báo cáo đang được phát triển", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void cboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cập nhật bộ lọc hiện tại dựa trên lựa chọn
            switch (cboFilter.SelectedIndex)
            {
                case 1:
                    _currentFilter = "LOW";
                    break;
                case 2:
                    _currentFilter = "ZERO";
                    break;
                default:
                    _currentFilter = "ALL";
                    break;
            }

            // Áp dụng bộ lọc
            ApplyFilter();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Designer Generated Code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InventoryListForm));
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.cboFilter = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgvInventory = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblTotalItems = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnViewHistory = new System.Windows.Forms.Button();
            this.btnAdjust = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1024, 60);
            this.panelTop.TabIndex = 0;

            // lblTitle
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1024, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "QUẢN LÝ TỒN KHO";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // panelFilter
            this.panelFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.panelFilter.Controls.Add(this.lblSearch);
            this.panelFilter.Controls.Add(this.txtSearch);
            this.panelFilter.Controls.Add(this.lblFilter);
            this.panelFilter.Controls.Add(this.cboFilter);
            this.panelFilter.Controls.Add(this.btnRefresh);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 60);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1024, 50);
            this.panelFilter.TabIndex = 1;

            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 17);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(62, 16);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Tìm kiếm:";

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(80, 14);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // lblFilter
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(350, 17);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(56, 16);
            this.lblFilter.TabIndex = 2;
            this.lblFilter.Text = "Hiển thị:";

            // cboFilter
            this.cboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilter.FormattingEnabled = true;
            this.cboFilter.Location = new System.Drawing.Point(412, 14);
            this.cboFilter.Name = "cboFilter";
            this.cboFilter.Size = new System.Drawing.Size(200, 24);
            this.cboFilter.TabIndex = 3;
            this.cboFilter.SelectedIndexChanged += new System.EventHandler(this.cboFilter_SelectedIndexChanged);

            // btnRefresh
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(632, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 28);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // dgvInventory
            this.dgvInventory.AllowUserToAddRows = false;
            this.dgvInventory.AllowUserToDeleteRows = false;
            this.dgvInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInventory.BackgroundColor = System.Drawing.Color.White;
            this.dgvInventory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInventory.Location = new System.Drawing.Point(0, 110);
            this.dgvInventory.MultiSelect = false;
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.ReadOnly = true;
            this.dgvInventory.RowHeadersVisible = false;
            this.dgvInventory.RowTemplate.Height = 28;
            this.dgvInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInventory.Size = new System.Drawing.Size(1024, 420);
            this.dgvInventory.TabIndex = 2;

            // panelBottom
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.panelBottom.Controls.Add(this.lblTotalItems);
            this.panelBottom.Controls.Add(this.lblTotalValue);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnExport);
            this.panelBottom.Controls.Add(this.btnViewHistory);
            this.panelBottom.Controls.Add(this.btnAdjust);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 530);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1024, 60);
            this.panelBottom.TabIndex = 3;

            // lblTotalItems
            this.lblTotalItems.AutoSize = true;
            this.lblTotalItems.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalItems.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(112)))), ((int)(((byte)(147)))));
            this.lblTotalItems.Location = new System.Drawing.Point(12, 22);
            this.lblTotalItems.Name = "lblTotalItems";
            this.lblTotalItems.Size = new System.Drawing.Size(109, 16);
            this.lblTotalItems.TabIndex = 0;
            this.lblTotalItems.Text = "Tổng số: 0 sản phẩm";

            // lblTotalValue
            this.lblTotalValue.AutoSize = true;
            this.lblTotalValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(112)))), ((int)(((byte)(147)))));
            this.lblTotalValue.Location = new System.Drawing.Point(200, 22);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(136, 16);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "Tổng giá trị: 0 VNĐ";

            // btnClose
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(912, 16);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 28);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // btnExport
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(806, 16);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 28);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Xuất báo cáo";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);

            // btnViewHistory
            this.btnViewHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnViewHistory.FlatAppearance.BorderSize = 0;
            this.btnViewHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewHistory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnViewHistory.ForeColor = System.Drawing.Color.White;
            this.btnViewHistory.Location = new System.Drawing.Point(700, 16);
            this.btnViewHistory.Name = "btnViewHistory";
            this.btnViewHistory.Size = new System.Drawing.Size(100, 28);
            this.btnViewHistory.TabIndex = 3;
            this.btnViewHistory.Text = "Xem lịch sử";
            this.btnViewHistory.UseVisualStyleBackColor = false;
            this.btnViewHistory.Click += new System.EventHandler(this.btnViewHistory_Click);

            // btnAdjust
            this.btnAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdjust.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnAdjust.FlatAppearance.BorderSize = 0;
            this.btnAdjust.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdjust.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAdjust.ForeColor = System.Drawing.Color.White;
            this.btnAdjust.Location = new System.Drawing.Point(594, 16);
            this.btnAdjust.Name = "btnAdjust";
            this.btnAdjust.Size = new System.Drawing.Size(100, 28);
            this.btnAdjust.TabIndex = 2;
            this.btnAdjust.Text = "Điều chỉnh";
            this.btnAdjust.UseVisualStyleBackColor = false;
            this.btnAdjust.Click += new System.EventHandler(this.btnAdjust_Click);

            // InventoryListForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1024, 590);
            this.Controls.Add(this.dgvInventory);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InventoryListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý tồn kho";
            this.Load += new System.EventHandler(this.InventoryListForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.ComboBox cboFilter;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblTotalItems;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Button btnAdjust;
        private System.Windows.Forms.Button btnViewHistory;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClose;
    }
}