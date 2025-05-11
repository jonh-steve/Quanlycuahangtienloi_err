// File: /Forms/Inventory/ImportListForm.cs
// Mô tả: Form hiển thị danh sách phiếu nhập hàng
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
    public partial class ImportListForm : Form
    {
        private readonly IImportService _importService;
        private readonly ISupplierService _supplierService;
        private List<ImportDTO> _importList;

        public ImportListForm(IImportService importService, ISupplierService supplierService)
        {
            InitializeComponent();
            _importService = importService;
            _supplierService = supplierService;

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

            // Màu hồng cho panel
            panelTop.BackColor = Color.FromArgb(255, 182, 193);
            lblTitle.ForeColor = Color.White;

            // Màu hồng cho DataGridView
            if (dgvImports != null)
            {
                dgvImports.EnableHeadersVisualStyles = false;
                dgvImports.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 182, 193);
                dgvImports.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvImports.ColumnHeadersDefaultCellStyle.Font = new Font(dgvImports.Font, FontStyle.Bold);

                dgvImports.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 112, 147);
                dgvImports.DefaultCellStyle.SelectionForeColor = Color.White;

                dgvImports.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245);
            }
        }

        private void ImportListForm_Load(object sender, EventArgs e)
        {
            // Thiết lập DateTimePicker
            dtpFrom.Value = DateTime.Now.AddMonths(-1);
            dtpTo.Value = DateTime.Now;

            // Setup ComboBox Supplier
            LoadSuppliers();

            // Setup ComboBox Status
            cboStatus.Items.Clear();
            cboStatus.Items.Add("Tất cả");
            cboStatus.Items.Add("Đang xử lý");
            cboStatus.Items.Add("Hoàn thành");
            cboStatus.Items.Add("Đã hủy");
            cboStatus.SelectedIndex = 0;

            // Setup DataGridView
            SetupDataGridView();

            // Load data
            LoadImportData();
        }

        private void LoadSuppliers()
        {
            try
            {
                // TODO: Lấy danh sách nhà cung cấp từ SupplierService

                // Tạm thời thêm các item mẫu
                cboSupplier.Items.Clear();
                cboSupplier.Items.Add("Tất cả");
                cboSupplier.Items.Add("Công ty TNHH Thực phẩm ABC");
                cboSupplier.Items.Add("Công ty CP Đồ uống XYZ");
                cboSupplier.Items.Add("Nhà phân phối Hàng tiêu dùng DEF");
                cboSupplier.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhà cung cấp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            // Thiết lập các cột cho DataGridView
            dgvImports.AutoGenerateColumns = false;

            // Thêm các cột
            if (dgvImports.Columns.Count == 0)
            {
                dgvImports.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ImportID",
                    HeaderText = "ID",
                    DataPropertyName = "ImportID",
                    Width = 50,
                    Visible = false
                });

                dgvImports.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ImportCode",
                    HeaderText = "Mã phiếu",
                    DataPropertyName = "ImportCode",
                    Width = 100
                });

                dgvImports.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ImportDate",
                    HeaderText = "Ngày nhập",
                    DataPropertyName = "ImportDate",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
                });

                dgvImports.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierName",
                    HeaderText = "Nhà cung cấp",
                    DataPropertyName = "SupplierName",
                    Width = 200
                });

                dgvImports.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ItemCount",
                    HeaderText = "Số SP",
                    DataPropertyName = "ItemCount",
                    Width = 60,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                });

                dgvImports.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalAmount",
                    HeaderText = "Tổng tiền",
                    DataPropertyName = "TotalAmount",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight,
                        Format = "N0"
                    }
                });

                dgvImports.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Trạng thái",
                    DataPropertyName = "Status",
                    Width = 100
                });

                dgvImports.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "EmployeeName",
                    HeaderText = "Người tạo",
                    DataPropertyName = "EmployeeName",
                    Width = 150
                });
            }

            // Tùy chỉnh appearance của DataGridView
            dgvImports.RowHeadersVisible = false;
            dgvImports.AllowUserToAddRows = false;
            dgvImports.AllowUserToDeleteRows = false;
            dgvImports.AllowUserToOrderColumns = true;
            dgvImports.ReadOnly = true;
            dgvImports.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Thêm sự kiện CellFormatting để hiển thị màu cho trạng thái
            dgvImports.CellFormatting += DgvImports_CellFormatting;
        }

        private void DgvImports_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Nếu là cột Status
            if (dgvImports.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                string status = e.Value.ToString();

                switch (status)
                {
                    case "Completed":
                        e.Value = "Hoàn thành";
                        e.CellStyle.ForeColor = Color.Green;
                        break;
                    case "Pending":
                        e.Value = "Đang xử lý";
                        e.CellStyle.ForeColor = Color.Blue;
                        break;
                    case "Cancelled":
                        e.Value = "Đã hủy";
                        e.CellStyle.ForeColor = Color.Red;
                        break;
                }
            }
        }

        private void LoadImportData()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Lấy giá trị lọc
                DateTime fromDate = dtpFrom.Value.Date;
                DateTime toDate = dtpTo.Value.Date.AddDays(1).AddSeconds(-1); // Đến cuối ngày

                int? supplierID = null;
                if (cboSupplier.SelectedIndex > 0)
                {
                    // TODO: Lấy supplierID từ item được chọn
                    // Tạm thời gán giá trị mẫu
                    supplierID = cboSupplier.SelectedIndex;
                }

                string status = null;
                switch (cboStatus.SelectedIndex)
                {
                    case 1:
                        status = "Pending";
                        break;
                    case 2:
                        status = "Completed";
                        break;
                    case 3:
                        status = "Cancelled";
                        break;
                }

                // Lấy danh sách phiếu nhập
                _importList = _importService.GetImportList(fromDate, toDate, supplierID);

                // Lọc theo status nếu có
                if (!string.IsNullOrEmpty(status))
                {
                    _importList = _importList.Where(i => i.Status == status).ToList();
                }

                // Lọc theo từ khóa tìm kiếm nếu có
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    string keyword = txtSearch.Text.Trim().ToLower();
                    _importList = _importList.Where(i =>
                        i.ImportCode.ToLower().Contains(keyword) ||
                        i.SupplierName.ToLower().Contains(keyword) ||
                        i.EmployeeName.ToLower().Contains(keyword)
                    ).ToList();
                }

                // Hiển thị dữ liệu
                dgvImports.DataSource = null;
                dgvImports.DataSource = _importList;

                // Cập nhật label tổng số
                lblTotalImports.Text = $"Tổng số: {_importList.Count} phiếu nhập";

                // Cập nhật tổng giá trị
                decimal totalValue = _importList.Sum(i => i.TotalAmount);
                lblTotalValue.Text = $"Tổng giá trị: {totalValue:N0} VNĐ";
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadImportData();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            // TODO: Mở form tạo phiếu nhập mới
            // using (var form = new ImportDetailForm(_importService, _supplierService))
            // {
            //     if (form.ShowDialog() == DialogResult.OK)
            //     {
            //         LoadImportData();
            //     }
            // }

            // Tạm thời hiển thị thông báo
            MessageBox.Show("Chức năng tạo phiếu nhập mới đang được phát triển", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (dgvImports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phiếu nhập để xem chi tiết", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy phiếu nhập được chọn
            ImportDTO selectedImport = dgvImports.SelectedRows[0].DataBoundItem as ImportDTO;

            if (selectedImport != null)
            {
                // TODO: Mở form xem chi tiết phiếu nhập
                // using (var form = new ImportDetailForm(_importService, _supplierService, selectedImport.ImportID))
                // {
                //     form.ShowDialog();
                // }

                // Tạm thời hiển thị thông báo
                MessageBox.Show($"Xem chi tiết phiếu nhập: {selectedImport.ImportCode}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (dgvImports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phiếu nhập để hoàn thành", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy phiếu nhập được chọn
            ImportDTO selectedImport = dgvImports.SelectedRows[0].DataBoundItem as ImportDTO;

            if (selectedImport != null)
            {
                // Kiểm tra trạng thái phiếu nhập
                if (selectedImport.Status != "Pending")
                {
                    MessageBox.Show("Chỉ có thể hoàn thành phiếu nhập có trạng thái 'Đang xử lý'", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận hoàn thành
                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn hoàn thành phiếu nhập {selectedImport.ImportCode}?\nHành động này sẽ cập nhật tồn kho và không thể hoàn tác.",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool success = _importService.CompleteImport(selectedImport.ImportID, 1); // Tạm thời sử dụng EmployeeID = 1

                        if (success)
                        {
                            MessageBox.Show("Hoàn thành phiếu nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadImportData();
                        }
                        else
                        {
                            MessageBox.Show("Hoàn thành phiếu nhập thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (dgvImports.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phiếu nhập để hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy phiếu nhập được chọn
            ImportDTO selectedImport = dgvImports.SelectedRows[0].DataBoundItem as ImportDTO;

            if (selectedImport != null)
            {
                // Kiểm tra trạng thái phiếu nhập
                if (selectedImport.Status != "Pending")
                {
                    MessageBox.Show("Chỉ có thể hủy phiếu nhập có trạng thái 'Đang xử lý'", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị form nhập lý do hủy
                using (var form = new InputBoxForm("Lý do hủy phiếu nhập", "Vui lòng nhập lý do hủy phiếu nhập:"))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        string reason = form.InputText;

                        if (string.IsNullOrWhiteSpace(reason))
                        {
                            MessageBox.Show("Vui lòng nhập lý do hủy phiếu nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        try
                        {
                            bool success = _importService.CancelImport(selectedImport.ImportID, reason, 1); // Tạm thời sử dụng EmployeeID = 1

                            if (success)
                            {
                                MessageBox.Show("Hủy phiếu nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadImportData();
                            }
                            else
                            {
                                MessageBox.Show("Hủy phiếu nhập thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // TODO: Implement export to Excel/PDF
            MessageBox.Show("Chức năng xuất báo cáo đang được phát triển", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadImportData();
        }

        #region Designer Generated Code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportListForm));
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboSupplier = new System.Windows.Forms.ComboBox();
            this.lblSupplier = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFrom = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.dgvImports = new System.Windows.Forms.DataGridView();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnComplete = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.lblTotalImports = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();

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
            this.lblTitle.Text = "QUẢN LÝ NHẬP HÀNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // panelFilter
            this.panelFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.panelFilter.Controls.Add(this.btnSearch);
            this.panelFilter.Controls.Add(this.cboStatus);
            this.panelFilter.Controls.Add(this.lblStatus);
            this.panelFilter.Controls.Add(this.cboSupplier);
            this.panelFilter.Controls.Add(this.lblSupplier);
            this.panelFilter.Controls.Add(this.dtpTo);
            this.panelFilter.Controls.Add(this.lblTo);
            this.panelFilter.Controls.Add(this.dtpFrom);
            this.panelFilter.Controls.Add(this.lblFrom);
            this.panelFilter.Controls.Add(this.txtSearch);
            this.panelFilter.Controls.Add(this.lblSearch);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 60);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1024, 80);
            this.panelFilter.TabIndex = 1;

            // btnSearch
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(912, 45);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 28);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // cboStatus
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Location = new System.Drawing.Point(612, 47);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(150, 23);
            this.cboStatus.TabIndex = 9;

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(542, 50);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(64, 15);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Trạng thái:";

            // cboSupplier
            this.cboSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSupplier.FormattingEnabled = true;
            this.cboSupplier.Location = new System.Drawing.Point(362, 47);
            this.cboSupplier.Name = "cboSupplier";
            this.cboSupplier.Size = new System.Drawing.Size(150, 23);
            this.cboSupplier.TabIndex = 7;

            // lblSupplier
            this.lblSupplier.AutoSize = true;
            this.lblSupplier.Location = new System.Drawing.Point(277, 50);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(79, 15);
            this.lblSupplier.TabIndex = 6;
            this.lblSupplier.Text = "Nhà cung cấp:";

            // dtpTo
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(181, 47);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(90, 23);
            this.dtpTo.TabIndex = 5;

            // lblTo
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(151, 50);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(24, 15);
            this.lblTo.TabIndex = 4;
            this.lblTo.Text = "đến";

            // dtpFrom
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(55, 47);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(90, 23);
            this.dtpFrom.TabIndex = 3;

            // lblFrom
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(12, 50);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(37, 15);
            this.lblFrom.TabIndex = 2;
            this.lblFrom.Text = "Từ ngày:";

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(74, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 18);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(56, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Tìm kiếm:";

            // dgvImports
            this.dgvImports.AllowUserToAddRows = false;
            this.dgvImports.AllowUserToDeleteRows = false;
            this.dgvImports.BackgroundColor = System.Drawing.Color.White;
            this.dgvImports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvImports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvImports.Location = new System.Drawing.Point(0, 140);
            this.dgvImports.Name = "dgvImports";
            this.dgvImports.ReadOnly = true;
            this.dgvImports.RowTemplate.Height = 25;
            this.dgvImports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvImports.Size = new System.Drawing.Size(1024, 380);
            this.dgvImports.TabIndex = 2;

            // panelButtons
            this.panelButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.panelButtons.Controls.Add(this.btnClose);
            this.panelButtons.Controls.Add(this.btnExport);
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnComplete);
            this.panelButtons.Controls.Add(this.btnView);
            this.panelButtons.Controls.Add(this.btnNew);
            this.panelButtons.Controls.Add(this.lblTotalImports);
            this.panelButtons.Controls.Add(this.lblTotalValue);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 520);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(1024, 70);
            this.panelButtons.TabIndex = 3;

            // btnClose
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(912, 21);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 28);
            this.btnClose.TabIndex = 7;
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
            this.btnExport.Location = new System.Drawing.Point(806, 21);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 28);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Xuất báo cáo";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);

            // btnCancel
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(700, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Hủy phiếu";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // btnComplete
            this.btnComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnComplete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnComplete.FlatAppearance.BorderSize = 0;
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComplete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnComplete.ForeColor = System.Drawing.Color.White;
            this.btnComplete.Location = new System.Drawing.Point(594, 21);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(100, 28);
            this.btnComplete.TabIndex = 4;
            this.btnComplete.Text = "Hoàn thành";
            this.btnComplete.UseVisualStyleBackColor = false;
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);

            // btnView
            this.btnView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnView.FlatAppearance.BorderSize = 0;
            this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnView.ForeColor = System.Drawing.Color.White;
            this.btnView.Location = new System.Drawing.Point(488, 21);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(100, 28);
            this.btnView.TabIndex = 3;
            this.btnView.Text = "Xem chi tiết";
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);

            // btnNew
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(182)))), ((int)(((byte)(193)))));
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(382, 21);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(100, 28);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "Tạo mới";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            // lblTotalImports
            this.lblTotalImports.AutoSize = true;
            this.lblTotalImports.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalImports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(112)))), ((int)(((byte)(147)))));
            this.lblTotalImports.Location = new System.Drawing.Point(12, 14);
            this.lblTotalImports.Name = "lblTotalImports";
            this.lblTotalImports.Size = new System.Drawing.Size(99, 15);
            this.lblTotalImports.TabIndex = 0;
            this.lblTotalImports.Text = "Tổng số: 0 phiếu";

            // lblTotalValue
            this.lblTotalValue.AutoSize = true;
            this.lblTotalValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(112)))), ((int)(((byte)(147)))));
            this.lblTotalValue.Location = new System.Drawing.Point(12, 34);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(116, 15);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "Tổng giá trị: 0 VNĐ";

            // ImportListForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1024, 590);
            this.Controls.Add(this.dgvImports);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý nhập hàng";
            this.Load += new System.EventHandler(this.ImportListForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImports)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboSupplier;
        private System.Windows.Forms.Label lblSupplier;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvImports;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Label lblTotalImports;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClose;
    }
}