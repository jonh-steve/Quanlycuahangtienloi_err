// Vị trí file: /Forms/Expenses/ExpenseListForm.cs

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Forms.Expenses
{
    public partial class ExpenseListForm : Form
    {
        private readonly IExpenseService _expenseService;
        private readonly Logger _logger;
        private List<ExpenseDTO> _expenses;
        private List<ExpenseTypeDTO> _expenseTypes;

        public ExpenseListForm(IExpenseService expenseService, Logger logger)
        {
            InitializeComponent();

            _expenseService = expenseService;
            _logger = logger;

            // Thiết lập màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu lavender blush

            // Thiết lập màu cho các control
            CustomizeControls();
        }

        private void ExpenseListForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Thiết lập giá trị mặc định cho DateTimePicker
                dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtpToDate.Value = DateTime.Now;

                // Load loại chi phí
                LoadExpenseTypes();

                // Load dữ liệu chi phí
                LoadExpenses();

                // Thiết lập DataGridView
                SetupDataGridView();
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseListForm_Load", ex);
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeControls()
        {
            // Thiết lập màu cho Panel
            pnlFilter.BackColor = Color.FromArgb(255, 228, 225); // Màu misty rose

            // Thiết lập màu cho Button
            btnAdd.BackColor = Color.FromArgb(255, 182, 193); // Màu light pink
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;

            btnEdit.BackColor = Color.FromArgb(255, 182, 193);
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.FlatAppearance.BorderSize = 0;

            btnDelete.BackColor = Color.FromArgb(255, 105, 180); // Màu hot pink
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;

            btnSearch.BackColor = Color.FromArgb(255, 182, 193);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;

            btnExportExcel.BackColor = Color.FromArgb(255, 182, 193);
            btnExportExcel.ForeColor = Color.White;
            btnExportExcel.FlatStyle = FlatStyle.Flat;
            btnExportExcel.FlatAppearance.BorderSize = 0;

            btnExportPDF.BackColor = Color.FromArgb(255, 182, 193);
            btnExportPDF.ForeColor = Color.White;
            btnExportPDF.FlatStyle = FlatStyle.Flat;
            btnExportPDF.FlatAppearance.BorderSize = 0;

            // Thiết lập màu cho Label
            lblFromDate.ForeColor = Color.FromArgb(199, 21, 133); // Màu medium violet red
            lblToDate.ForeColor = Color.FromArgb(199, 21, 133);
            lblExpenseType.ForeColor = Color.FromArgb(199, 21, 133);
            lblTotal.ForeColor = Color.FromArgb(199, 21, 133);
            lblTotalAmount.ForeColor = Color.FromArgb(199, 21, 133);
            lblTotalAmount.Font = new Font(lblTotalAmount.Font, FontStyle.Bold);
        }

        private void LoadExpenseTypes()
        {
            try
            {
                _expenseTypes = _expenseService.GetAllExpenseTypes();

                // Thêm item "Tất cả" vào đầu danh sách
                var allItem = new ExpenseTypeDTO { ExpenseTypeID = 0, TypeName = "-- Tất cả --" };
                _expenseTypes.Insert(0, allItem);

                // Bind dữ liệu vào ComboBox
                cboExpenseType.DisplayMember = "TypeName";
                cboExpenseType.ValueMember = "ExpenseTypeID";
                cboExpenseType.DataSource = _expenseTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadExpenseTypes", ex);
                MessageBox.Show("Đã xảy ra lỗi khi tải loại chi phí: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExpenses()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // Lấy tham số tìm kiếm
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // Đến cuối ngày
                int expenseTypeID = Convert.ToInt32(cboExpenseType.SelectedValue);

                // Lấy dữ liệu chi phí
                _expenses = _expenseService.GetExpensesByDateRange(fromDate, toDate);

                // Lọc theo loại chi phí nếu cần
                if (expenseTypeID > 0)
                {
                    _expenses = _expenses.Where(e => e.ExpenseTypeID == expenseTypeID).ToList();
                }

                // Hiển thị dữ liệu
                dgvExpenses.DataSource = _expenses;

                // Cập nhật tổng chi phí
                decimal totalAmount = _expenses.Sum(e => e.Amount);
                lblTotalAmount.Text = totalAmount.ToString("#,##0") + " VNĐ";
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadExpenses", ex);
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu chi phí: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void SetupDataGridView()
        {
            // Thiết lập thuộc tính DataGridView
            dgvExpenses.AutoGenerateColumns = false;
            dgvExpenses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvExpenses.MultiSelect = false;
            dgvExpenses.ReadOnly = true;
            dgvExpenses.AllowUserToAddRows = false;
            dgvExpenses.AllowUserToDeleteRows = false;
            dgvExpenses.AllowUserToResizeRows = false;
            dgvExpenses.RowHeadersVisible = false;
            dgvExpenses.EnableHeadersVisualStyles = false;

            // Thiết lập màu sắc cho DataGridView
            dgvExpenses.BackgroundColor = Color.White;
            dgvExpenses.BorderStyle = BorderStyle.None;
            dgvExpenses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 192, 203); // Màu pink
            dgvExpenses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvExpenses.ColumnHeadersDefaultCellStyle.Font = new Font(dgvExpenses.Font, FontStyle.Bold);
            dgvExpenses.ColumnHeadersHeight = 40;
            dgvExpenses.RowTemplate.Height = 30;
            dgvExpenses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245); // Màu lavender blush
            dgvExpenses.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 182, 193); // Màu light pink
            dgvExpenses.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Xóa tất cả cột
            dgvExpenses.Columns.Clear();

            // Thêm các cột mới
            DataGridViewTextBoxColumn colSTT = new DataGridViewTextBoxColumn();
            colSTT.Name = "colSTT";
            colSTT.HeaderText = "STT";
            colSTT.Width = 50;
            dgvExpenses.Columns.Add(colSTT);

            DataGridViewTextBoxColumn colExpenseTypeName = new DataGridViewTextBoxColumn();
            colExpenseTypeName.Name = "colExpenseTypeName";
            colExpenseTypeName.HeaderText = "Loại chi phí";
            colExpenseTypeName.DataPropertyName = "ExpenseTypeName";
            colExpenseTypeName.Width = 150;
            dgvExpenses.Columns.Add(colExpenseTypeName);

            DataGridViewTextBoxColumn colAmount = new DataGridViewTextBoxColumn();
            colAmount.Name = "colAmount";
            colAmount.HeaderText = "Số tiền";
            colAmount.DataPropertyName = "Amount";
            colAmount.Width = 120;
            colAmount.DefaultCellStyle.Format = "#,##0";
            colAmount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvExpenses.Columns.Add(colAmount);

            DataGridViewTextBoxColumn colExpenseDate = new DataGridViewTextBoxColumn();
            colExpenseDate.Name = "colExpenseDate";
            colExpenseDate.HeaderText = "Ngày chi";
            colExpenseDate.DataPropertyName = "ExpenseDate";
            colExpenseDate.Width = 100;
            colExpenseDate.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvExpenses.Columns.Add(colExpenseDate);

            DataGridViewTextBoxColumn colDescription = new DataGridViewTextBoxColumn();
            colDescription.Name = "colDescription";
            colDescription.HeaderText = "Mô tả";
            colDescription.DataPropertyName = "Description";
            colDescription.Width = 250;
            dgvExpenses.Columns.Add(colDescription);

            DataGridViewTextBoxColumn colEmployeeName = new DataGridViewTextBoxColumn();
            colEmployeeName.Name = "colEmployeeName";
            colEmployeeName.HeaderText = "Người tạo";
            colEmployeeName.DataPropertyName = "EmployeeName";
            colEmployeeName.Width = 150;
            dgvExpenses.Columns.Add(colEmployeeName);

            DataGridViewTextBoxColumn colCreatedDate = new DataGridViewTextBoxColumn();
            colCreatedDate.Name = "colCreatedDate";
            colCreatedDate.HeaderText = "Ngày tạo";
            colCreatedDate.DataPropertyName = "CreatedDate";
            colCreatedDate.Width = 150;
            colCreatedDate.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            dgvExpenses.Columns.Add(colCreatedDate);

            // Ẩn cột ID
            DataGridViewTextBoxColumn colExpenseID = new DataGridViewTextBoxColumn();
            colExpenseID.Name = "colExpenseID";
            colExpenseID.DataPropertyName = "ExpenseID";
            colExpenseID.Visible = false;
            dgvExpenses.Columns.Add(colExpenseID);
        }

        private void dgvExpenses_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Tiếp tục ở phần còn lại của ExpenseListForm.cs

            // Đánh số thứ tự
            if (e.ColumnIndex == dgvExpenses.Columns["colSTT"].Index && e.RowIndex >= 0)
            {
                e.Value = e.RowIndex + 1;
                e.FormattingApplied = true;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadExpenses();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở form thêm chi phí
                ExpenseDetailForm expenseDetailForm = new ExpenseDetailForm(_expenseService, _logger);
                expenseDetailForm.StartPosition = FormStartPosition.CenterParent;

                if (expenseDetailForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu thêm thành công, cập nhật lại danh sách
                    LoadExpenses();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnAdd_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem đã chọn chi phí nào chưa
                if (dgvExpenses.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn chi phí cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lấy ID chi phí được chọn
                int expenseID = Convert.ToInt32(dgvExpenses.CurrentRow.Cells["colExpenseID"].Value);

                // Mở form sửa chi phí
                ExpenseDetailForm expenseDetailForm = new ExpenseDetailForm(_expenseService, _logger, expenseID);
                expenseDetailForm.StartPosition = FormStartPosition.CenterParent;

                if (expenseDetailForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu sửa thành công, cập nhật lại danh sách
                    LoadExpenses();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnEdit_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem đã chọn chi phí nào chưa
                if (dgvExpenses.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn chi phí cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lấy thông tin chi phí được chọn
                int expenseID = Convert.ToInt32(dgvExpenses.CurrentRow.Cells["colExpenseID"].Value);
                string expenseTypeName = dgvExpenses.CurrentRow.Cells["colExpenseTypeName"].Value.ToString();
                decimal amount = Convert.ToDecimal(dgvExpenses.CurrentRow.Cells["colAmount"].Value);

                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa chi phí '{expenseTypeName}' với số tiền {amount.ToString("#,##0")} VNĐ không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Thực hiện xóa chi phí
                    bool success = _expenseService.DeleteExpense(expenseID);

                    if (success)
                    {
                        MessageBox.Show("Xóa chi phí thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadExpenses();
                    }
                    else
                    {
                        MessageBox.Show("Xóa chi phí thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnDelete_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_expenses == null || _expenses.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Xuất báo cáo Excel";
                saveFileDialog.FileName = $"BaoCaoChiPhi_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    // Xuất báo cáo ra Excel
                    string filePath = _expenseService.ExportExpensesToExcel(_expenses, saveFileDialog.FileName);

                    Cursor.Current = Cursors.Default;

                    // Hỏi người dùng có muốn mở file không
                    DialogResult result = MessageBox.Show(
                        "Xuất báo cáo Excel thành công! Bạn có muốn mở file không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                _logger.LogError("btnExportExcel_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_expenses == null || _expenses.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.Title = "Xuất báo cáo PDF";
                saveFileDialog.FileName = $"BaoCaoChiPhi_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    // Xuất báo cáo ra PDF
                    string filePath = _expenseService.ExportExpensesToPDF(_expenses, saveFileDialog.FileName);

                    Cursor.Current = Cursors.Default;

                    // Hỏi người dùng có muốn mở file không
                    DialogResult result = MessageBox.Show(
                        "Xuất báo cáo PDF thành công! Bạn có muốn mở file không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                _logger.LogError("btnExportPDF_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvExpenses_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEdit_Click(sender, e);
            }
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            // Đảm bảo FromDate không lớn hơn ToDate
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                dtpFromDate.Value = dtpToDate.Value;
            }
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            // Đảm bảo ToDate không nhỏ hơn FromDate
            if (dtpToDate.Value < dtpFromDate.Value)
            {
                dtpToDate.Value = dtpFromDate.Value;
            }
        }

        #region Designer Generated Code

        private void InitializeComponent()
        {
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cboExpenseType = new System.Windows.Forms.ComboBox();
            this.lblExpenseType = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnExportPDF = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pnlTotal = new System.Windows.Forms.Panel();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.dgvExpenses = new System.Windows.Forms.DataGridView();
            this.pnlFilter.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.pnlTotal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.btnSearch);
            this.pnlFilter.Controls.Add(this.cboExpenseType);
            this.pnlFilter.Controls.Add(this.lblExpenseType);
            this.pnlFilter.Controls.Add(this.dtpToDate);
            this.pnlFilter.Controls.Add(this.lblToDate);
            this.pnlFilter.Controls.Add(this.dtpFromDate);
            this.pnlFilter.Controls.Add(this.lblFromDate);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(984, 70);
            this.pnlFilter.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(661, 22);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(92, 26);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cboExpenseType
            // 
            this.cboExpenseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExpenseType.FormattingEnabled = true;
            this.cboExpenseType.Location = new System.Drawing.Point(447, 24);
            this.cboExpenseType.Name = "cboExpenseType";
            this.cboExpenseType.Size = new System.Drawing.Size(194, 21);
            this.cboExpenseType.TabIndex = 5;
            // 
            // lblExpenseType
            // 
            this.lblExpenseType.AutoSize = true;
            this.lblExpenseType.Location = new System.Drawing.Point(371, 28);
            this.lblExpenseType.Name = "lblExpenseType";
            this.lblExpenseType.Size = new System.Drawing.Size(70, 13);
            this.lblExpenseType.TabIndex = 4;
            this.lblExpenseType.Text = "Loại chi phí:";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(254, 24);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(97, 20);
            this.dtpToDate.TabIndex = 3;
            this.dtpToDate.ValueChanged += new System.EventHandler(this.dtpToDate_ValueChanged);
            // 
            // lblToDate
            // 
            this.lblToDate.AutoSize = true;
            this.lblToDate.Location = new System.Drawing.Point(191, 28);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(57, 13);
            this.lblToDate.TabIndex = 2;
            this.lblToDate.Text = "Đến ngày:";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(73, 24);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(97, 20);
            this.dtpFromDate.TabIndex = 1;
            this.dtpFromDate.ValueChanged += new System.EventHandler(this.dtpFromDate_ValueChanged);
            // 
            // lblFromDate
            // 
            this.lblFromDate.AutoSize = true;
            this.lblFromDate.Location = new System.Drawing.Point(12, 28);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(49, 13);
            this.lblFromDate.TabIndex = 0;
            this.lblFromDate.Text = "Từ ngày:";
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.btnExportPDF);
            this.pnlActions.Controls.Add(this.btnExportExcel);
            this.pnlActions.Controls.Add(this.btnDelete);
            this.pnlActions.Controls.Add(this.btnEdit);
            this.pnlActions.Controls.Add(this.btnAdd);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 511);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(984, 50);
            this.pnlActions.TabIndex = 1;
            // 
            // btnExportPDF
            // 
            this.btnExportPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportPDF.Location = new System.Drawing.Point(877, 12);
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Size = new System.Drawing.Size(95, 26);
            this.btnExportPDF.TabIndex = 4;
            this.btnExportPDF.Text = "Xuất PDF";
            this.btnExportPDF.UseVisualStyleBackColor = true;
            this.btnExportPDF.Click += new System.EventHandler(this.btnExportPDF_Click);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportExcel.Location = new System.Drawing.Point(776, 12);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(95, 26);
            this.btnExportExcel.TabIndex = 3;
            this.btnExportExcel.Text = "Xuất Excel";
            this.btnExportExcel.UseVisualStyleBackColor = true;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(228, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 26);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(120, 12);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(80, 26);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Sửa";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(80, 26);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // pnlTotal
            // 
            this.pnlTotal.Controls.Add(this.lblTotalAmount);
            this.pnlTotal.Controls.Add(this.lblTotal);
            this.pnlTotal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTotal.Location = new System.Drawing.Point(0, 481);
            this.pnlTotal.Name = "pnlTotal";
            this.pnlTotal.Size = new System.Drawing.Size(984, 30);
            this.pnlTotal.TabIndex = 2;
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Location = new System.Drawing.Point(882, 10);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(43, 13);
            this.lblTotalAmount.TabIndex = 1;
            this.lblTotalAmount.Text = "0 VNĐ";
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(804, 10);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(72, 13);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "Tổng chi phí:";
            // 
            // dgvExpenses
            // 
            this.dgvExpenses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExpenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExpenses.Location = new System.Drawing.Point(0, 70);
            this.dgvExpenses.Name = "dgvExpenses";
            this.dgvExpenses.Size = new System.Drawing.Size(984, 411);
            this.dgvExpenses.TabIndex = 3;
            this.dgvExpenses.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvExpenses_CellDoubleClick);
            this.dgvExpenses.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvExpenses_CellFormatting);
            // 
            // ExpenseListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.dgvExpenses);
            this.Controls.Add(this.pnlTotal);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlFilter);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "ExpenseListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách chi phí";
            this.Load += new System.EventHandler(this.ExpenseListForm_Load);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.pnlTotal.ResumeLayout(false);
            this.pnlTotal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cboExpenseType;
        private System.Windows.Forms.Label lblExpenseType;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnExportPDF;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel pnlTotal;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.DataGridView dgvExpenses;
    }
}