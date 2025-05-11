// Vị trí file: /Forms/Expenses/ExpenseTypeManagementForm.cs

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
    public partial class ExpenseTypeManagementForm : Form
    {
        private readonly IExpenseService _expenseService;
        private readonly Logger _logger;
        private List<ExpenseTypeDTO> _expenseTypes;
        private ExpenseTypeDTO _currentExpenseType;
        private bool _isEditMode = false;

        public ExpenseTypeManagementForm(IExpenseService expenseService, Logger logger)
        {
            InitializeComponent();

            _expenseService = expenseService;
            _logger = logger;

            // Thiết lập màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu lavender blush

            // Thiết lập màu cho các control
            CustomizeControls();
        }

        private void ExpenseTypeManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Thiết lập trạng thái form
                SetFormState(false);

                // Load dữ liệu loại chi phí
                LoadExpenseTypes();
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseTypeManagementForm_Load", ex);
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeControls()
        {
            // Thiết lập màu cho Panel
            pnlList.BackColor = Color.FromArgb(255, 228, 225); // Màu misty rose
            pnlDetail.BackColor = Color.FromArgb(255, 228, 225);

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

            btnSave.BackColor = Color.FromArgb(255, 182, 193);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;

            btnCancel.BackColor = Color.FromArgb(255, 105, 180);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;

            // Thiết lập màu cho Label
            lblExpenseTypes.ForeColor = Color.FromArgb(199, 21, 133); // Màu medium violet red
            lblTypeName.ForeColor = Color.FromArgb(199, 21, 133);
            lblDescription.ForeColor = Color.FromArgb(199, 21, 133);
            lblIsActive.ForeColor = Color.FromArgb(199, 21, 133);
        }

        private void LoadExpenseTypes()
        {
            try
            {
                _expenseTypes = _expenseService.GetAllExpenseTypes();

                // Bind dữ liệu vào ListView
                lvwExpenseTypes.Items.Clear();
                foreach (var expenseType in _expenseTypes)
                {
                    ListViewItem item = new ListViewItem(expenseType.ExpenseTypeID.ToString());
                    item.SubItems.Add(expenseType.TypeName);
                    item.SubItems.Add(expenseType.Description);
                    item.SubItems.Add(expenseType.IsActive ? "Có" : "Không");
                    item.Tag = expenseType;

                    lvwExpenseTypes.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadExpenseTypes", ex);
                MessageBox.Show("Đã xảy ra lỗi khi tải loại chi phí: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetFormState(bool isEditing)
        {
            _isEditMode = isEditing;

            // Thiết lập trạng thái của các control
            txtTypeName.Enabled = isEditing;
            txtDescription.Enabled = isEditing;
            chkIsActive.Enabled = isEditing;

            btnSave.Enabled = isEditing;
            btnCancel.Enabled = isEditing;

            btnAdd.Enabled = !isEditing;
            btnEdit.Enabled = !isEditing && lvwExpenseTypes.SelectedItems.Count > 0;
            btnDelete.Enabled = !isEditing && lvwExpenseTypes.SelectedItems.Count > 0;

            lvwExpenseTypes.Enabled = !isEditing;

            // Clear dữ liệu nếu không phải chế độ sửa
            if (!isEditing)
            {
                ClearFields();
            }
        }

        private void ClearFields()
        {
            txtTypeName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            chkIsActive.Checked = true;
            _currentExpenseType = null;
        }

        private void DisplayExpenseTypeDetails(ExpenseTypeDTO expenseType)
        {
            if (expenseType != null)
            {
                txtTypeName.Text = expenseType.TypeName;
                txtDescription.Text = expenseType.Description;
                chkIsActive.Checked = expenseType.IsActive;
                _currentExpenseType = expenseType;
            }
            else
            {
                ClearFields();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearFields();
            SetFormState(true);
            txtTypeName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvwExpenseTypes.SelectedItems.Count > 0)
            {
                // Lấy loại chi phí được chọn từ ListView
                _currentExpenseType = (ExpenseTypeDTO)lvwExpenseTypes.SelectedItems[0].Tag;
                DisplayExpenseTypeDetails(_currentExpenseType);
                SetFormState(true);
                txtTypeName.Focus();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvwExpenseTypes.SelectedItems.Count > 0)
                {
                    // Lấy loại chi phí được chọn từ ListView
                    ExpenseTypeDTO expenseType = (ExpenseTypeDTO)lvwExpenseTypes.SelectedItems[0].Tag;

                    // Hiển thị hộp thoại xác nhận
                    DialogResult result = MessageBox.Show(
                        $"Bạn có chắc chắn muốn xóa loại chi phí '{expenseType.TypeName}' không?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Thực hiện xóa loại chi phí
                        bool success = _expenseService.DeleteExpenseType(expenseType.ExpenseTypeID);

                        if (success)
                        {
                            MessageBox.Show("Xóa loại chi phí thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadExpenseTypes();
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Xóa loại chi phí thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _logger.LogError("btnDelete_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate dữ liệu
                if (!ValidateData())
                {
                    return;
                }

                // Lấy dữ liệu từ form
                string typeName = txtTypeName.Text.Trim();
                string description = txtDescription.Text.Trim();
                bool isActive = chkIsActive.Checked;

                if (_currentExpenseType != null && _currentExpenseType.ExpenseTypeID > 0)
                {
                    // Cập nhật loại chi phí
                    _currentExpenseType.TypeName = typeName;
                    _currentExpenseType.Description = description;
                    _currentExpenseType.IsActive = isActive;

                    // Gọi service để cập nhật
                    bool success = _expenseService.UpdateExpenseType(_currentExpenseType);

                    if (success)
                    {
                        MessageBox.Show("Cập nhật loại chi phí thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadExpenseTypes();
                        SetFormState(false);
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật loại chi phí thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Tạo đối tượng ExpenseTypeDTO mới
                    ExpenseTypeDTO newExpenseType = new ExpenseTypeDTO
                    {
                        TypeName = typeName,
                        Description = description,
                        IsActive = isActive
                    };

                    // Gọi service để thêm mới
                    int newExpenseTypeID = _expenseService.CreateExpenseType(newExpenseType);

                    if (newExpenseTypeID > 0)
                    {
                        MessageBox.Show("Thêm loại chi phí thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadExpenseTypes();
                        SetFormState(false);
                    }
                    else
                    {
                        MessageBox.Show("Thêm loại chi phí thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("btnSave_Click", ex);
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateData()
        {
            // Kiểm tra tên loại chi phí
            if (string.IsNullOrWhiteSpace(txtTypeName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại chi phí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTypeName.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetFormState(false);

            // Nếu đang chọn một loại chi phí, hiển thị lại thông tin
            if (lvwExpenseTypes.SelectedItems.Count > 0)
            {
                DisplayExpenseTypeDetails((ExpenseTypeDTO)lvwExpenseTypes.SelectedItems[0].Tag);
            }
            else
            {
                ClearFields();
            }
        }

        private void lvwExpenseTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = lvwExpenseTypes.SelectedItems.Count > 0;
            btnDelete.Enabled = lvwExpenseTypes.SelectedItems.Count > 0;

            if (lvwExpenseTypes.SelectedItems.Count > 0)
            {
                DisplayExpenseTypeDetails((ExpenseTypeDTO)lvwExpenseTypes.SelectedItems[0].Tag);
            }
            else
            {
                ClearFields();
            }
        }

        private void lvwExpenseTypes_DoubleClick(object sender, EventArgs e)
        {
            if (lvwExpenseTypes.SelectedItems.Count > 0)
            {
                btnEdit_Click(sender, e);
            }
        }

        #region Designer Generated Code

        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pnlList = new System.Windows.Forms.Panel();
            this.lvwExpenseTypes = new System.Windows.Forms.ListView();
            this.colID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTypeName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colIsActive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblExpenseTypes = new System.Windows.Forms.Label();
            this.pnlListActions = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pnlDetail = new System.Windows.Forms.Panel();
            this.pnlDetailActions = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkIsActive = new System.Windows.Forms.CheckBox();
            this.lblIsActive = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtTypeName = new System.Windows.Forms.TextBox();
            this.lblTypeName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.pnlListActions.SuspendLayout();
            this.pnlDetail.SuspendLayout();
            this.pnlDetailActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.pnlList);
            // Tiếp tục ở phần còn lại của ExpenseTypeManagementForm.cs

            this.splitContainer.Panel1.Controls.Add(this.pnlListActions);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pnlDetail);
            this.splitContainer.Size = new System.Drawing.Size(784, 461);
            this.splitContainer.SplitterDistance = 400;
            this.splitContainer.TabIndex = 0;
            // 
            // pnlList
            // 
            this.pnlList.Controls.Add(this.lvwExpenseTypes);
            this.pnlList.Controls.Add(this.lblExpenseTypes);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlList.Location = new System.Drawing.Point(0, 0);
            this.pnlList.Name = "pnlList";
            this.pnlList.Padding = new System.Windows.Forms.Padding(10);
            this.pnlList.Size = new System.Drawing.Size(400, 411);
            this.pnlList.TabIndex = 1;
            // 
            // lvwExpenseTypes
            // 
            this.lvwExpenseTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colTypeName,
            this.colDescription,
            this.colIsActive});
            this.lvwExpenseTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwExpenseTypes.FullRowSelect = true;
            this.lvwExpenseTypes.HideSelection = false;
            this.lvwExpenseTypes.Location = new System.Drawing.Point(10, 33);
            this.lvwExpenseTypes.MultiSelect = false;
            this.lvwExpenseTypes.Name = "lvwExpenseTypes";
            this.lvwExpenseTypes.Size = new System.Drawing.Size(380, 368);
            this.lvwExpenseTypes.TabIndex = 1;
            this.lvwExpenseTypes.UseCompatibleStateImageBehavior = false;
            this.lvwExpenseTypes.View = System.Windows.Forms.View.Details;
            this.lvwExpenseTypes.SelectedIndexChanged += new System.EventHandler(this.lvwExpenseTypes_SelectedIndexChanged);
            this.lvwExpenseTypes.DoubleClick += new System.EventHandler(this.lvwExpenseTypes_DoubleClick);
            // 
            // colID
            // 
            this.colID.Text = "ID";
            this.colID.Width = 40;
            // 
            // colTypeName
            // 
            this.colTypeName.Text = "Tên loại chi phí";
            this.colTypeName.Width = 150;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Mô tả";
            this.colDescription.Width = 120;
            // 
            // colIsActive
            // 
            this.colIsActive.Text = "Đang sử dụng";
            this.colIsActive.Width = 80;
            // 
            // lblExpenseTypes
            // 
            this.lblExpenseTypes.AutoSize = true;
            this.lblExpenseTypes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblExpenseTypes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExpenseTypes.Location = new System.Drawing.Point(10, 10);
            this.lblExpenseTypes.Name = "lblExpenseTypes";
            this.lblExpenseTypes.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.lblExpenseTypes.Size = new System.Drawing.Size(121, 23);
            this.lblExpenseTypes.TabIndex = 0;
            this.lblExpenseTypes.Text = "Loại chi phí";
            // 
            // pnlListActions
            // 
            this.pnlListActions.Controls.Add(this.btnDelete);
            this.pnlListActions.Controls.Add(this.btnEdit);
            this.pnlListActions.Controls.Add(this.btnAdd);
            this.pnlListActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlListActions.Location = new System.Drawing.Point(0, 411);
            this.pnlListActions.Name = "pnlListActions";
            this.pnlListActions.Size = new System.Drawing.Size(400, 50);
            this.pnlListActions.TabIndex = 0;
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(199, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 26);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Enabled = false;
            this.btnEdit.Location = new System.Drawing.Point(106, 12);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 26);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Sửa";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(13, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 26);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // pnlDetail
            // 
            this.pnlDetail.Controls.Add(this.pnlDetailActions);
            this.pnlDetail.Controls.Add(this.chkIsActive);
            this.pnlDetail.Controls.Add(this.lblIsActive);
            this.pnlDetail.Controls.Add(this.txtDescription);
            this.pnlDetail.Controls.Add(this.lblDescription);
            this.pnlDetail.Controls.Add(this.txtTypeName);
            this.pnlDetail.Controls.Add(this.lblTypeName);
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Padding = new System.Windows.Forms.Padding(10);
            this.pnlDetail.Size = new System.Drawing.Size(380, 461);
            this.pnlDetail.TabIndex = 0;
            // 
            // pnlDetailActions
            // 
            this.pnlDetailActions.Controls.Add(this.btnCancel);
            this.pnlDetailActions.Controls.Add(this.btnSave);
            this.pnlDetailActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDetailActions.Location = new System.Drawing.Point(10, 401);
            this.pnlDetailActions.Name = "pnlDetailActions";
            this.pnlDetailActions.Size = new System.Drawing.Size(360, 50);
            this.pnlDetailActions.TabIndex = 6;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(279, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(186, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 26);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkIsActive
            // 
            this.chkIsActive.AutoSize = true;
            this.chkIsActive.Checked = true;
            this.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsActive.Enabled = false;
            this.chkIsActive.Location = new System.Drawing.Point(111, 160);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Size = new System.Drawing.Size(15, 14);
            this.chkIsActive.TabIndex = 5;
            this.chkIsActive.UseVisualStyleBackColor = true;
            // 
            // lblIsActive
            // 
            this.lblIsActive.AutoSize = true;
            this.lblIsActive.Location = new System.Drawing.Point(13, 160);
            this.lblIsActive.Name = "lblIsActive";
            this.lblIsActive.Size = new System.Drawing.Size(72, 13);
            this.lblIsActive.TabIndex = 4;
            this.lblIsActive.Text = "Đang sử dụng:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Enabled = false;
            this.txtDescription.Location = new System.Drawing.Point(111, 85);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(256, 60);
            this.txtDescription.TabIndex = 3;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(13, 88);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(37, 13);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Mô tả:";
            // 
            // txtTypeName
            // 
            this.txtTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTypeName.Enabled = false;
            this.txtTypeName.Location = new System.Drawing.Point(111, 50);
            this.txtTypeName.Name = "txtTypeName";
            this.txtTypeName.Size = new System.Drawing.Size(256, 20);
            this.txtTypeName.TabIndex = 1;
            // 
            // lblTypeName
            // 
            this.lblTypeName.AutoSize = true;
            this.lblTypeName.Location = new System.Drawing.Point(13, 53);
            this.lblTypeName.Name = "lblTypeName";
            this.lblTypeName.Size = new System.Drawing.Size(70, 13);
            this.lblTypeName.TabIndex = 0;
            this.lblTypeName.Text = "Tên loại chi phí:";
            // 
            // ExpenseTypeManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "ExpenseTypeManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quản lý loại chi phí";
            this.Load += new System.EventHandler(this.ExpenseTypeManagementForm_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.pnlList.PerformLayout();
            this.pnlListActions.ResumeLayout(false);
            this.pnlDetail.ResumeLayout(false);
            this.pnlDetail.PerformLayout();
            this.pnlDetailActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnlListActions;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel pnlList;
        private System.Windows.Forms.ListView lvwExpenseTypes;
        private System.Windows.Forms.ColumnHeader colID;
        private System.Windows.Forms.ColumnHeader colTypeName;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colIsActive;
        private System.Windows.Forms.Label lblExpenseTypes;
        private System.Windows.Forms.Panel pnlDetail;
        private System.Windows.Forms.Panel pnlDetailActions;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkIsActive;
        private System.Windows.Forms.Label lblIsActive;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtTypeName;
        private System.Windows.Forms.Label lblTypeName;
    }
}