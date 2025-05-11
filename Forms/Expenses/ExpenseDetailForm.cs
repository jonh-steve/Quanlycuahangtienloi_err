// Vị trí file: /Forms/Expenses/ExpenseDetailForm.cs

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Forms.Expenses
{
    public partial class ExpenseDetailForm : Form
    {
        private readonly IExpenseService _expenseService;
        private readonly Logger _logger;
        private readonly int _expenseID;
        private ExpenseDTO _expenseDTO;
        private List<ExpenseTypeDTO> _expenseTypes;
        private bool _isEditMode;

        public ExpenseDetailForm(IExpenseService expenseService, Logger logger, int expenseID = 0)
        {
            InitializeComponent();

            _expenseService = expenseService;
            _logger = logger;
            _expenseID = expenseID;
            _isEditMode = (_expenseID > 0);

            // Thiết lập màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu lavender blush

            // Thiết lập màu cho các control
            CustomizeControls();
        }

        private void ExpenseDetailForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Thiết lập tiêu đề form
                if (_isEditMode)
                {
                    this.Text = "Sửa chi phí";
                    btnSave.Text = "Cập nhật";
                }
                else
                {
                    this.Text = "Thêm chi phí mới";
                    btnSave.Text = "Lưu";
                }

                // Load loại chi phí
                LoadExpenseTypes();

                // Load dữ liệu chi phí nếu ở chế độ sửa
                if (_isEditMode)
                {
                    LoadExpenseData();
                }
                else
                {
                    // Đặt giá trị mặc định cho chi phí mới
                    dtpExpenseDate.Value = DateTime.Now;
                    nudAmount.Value = 0;
                    txtDescription.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseDetailForm_Load", ex);
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeControls()
        {
            // Thiết lập màu cho Panel
            pnlMain.BackColor = Color.FromArgb(255, 228, 225); // Màu misty rose

            // Thiết lập màu cho Button
            btnSave.BackColor = Color.FromArgb(255, 182, 193); // Màu light pink
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;

            btnCancel.BackColor = Color.FromArgb(255, 105, 180); // Màu hot pink
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;

            // Thiết lập màu cho Label
            lblExpenseType.ForeColor = Color.FromArgb(199, 21, 133); // Màu medium violet red
            lblAmount.ForeColor = Color.FromArgb(199, 21, 133);
            lblExpenseDate.ForeColor = Color.FromArgb(199, 21, 133);
            lblDescription.ForeColor = Color.FromArgb(199, 21, 133);
        }

        private void LoadExpenseTypes()
        {
            try
            {
                _expenseTypes = _expenseService.GetAllExpenseTypes();

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

        private void LoadExpenseData()
        {
            try
            {
                // Lấy thông tin chi phí từ service
                _expenseDTO = _expenseService.GetExpenseByID(_expenseID);

                if (_expenseDTO == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin chi phí!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                // Hiển thị thông tin chi phí lên form
                cboExpenseType.SelectedValue = _expenseDTO.ExpenseTypeID;
                nudAmount.Value = _expenseDTO.Amount;
                dtpExpenseDate.Value = _expenseDTO.ExpenseDate;
                txtDescription.Text = _expenseDTO.Description;
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadExpenseData", ex);
                // Tiếp tục ở phần còn lại của ExpenseDetailForm.cs

                MessageBox.Show("Đã xảy ra lỗi khi tải thông tin chi phí: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
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
                int expenseTypeID = Convert.ToInt32(cboExpenseType.SelectedValue);
                decimal amount = nudAmount.Value;
                DateTime expenseDate = dtpExpenseDate.Value;
                string description = txtDescription.Text.Trim();

                if (_isEditMode)
                {
                    // Cập nhật thông tin chi phí
                    _expenseDTO.ExpenseTypeID = expenseTypeID;
                    _expenseDTO.Amount = amount;
                    _expenseDTO.ExpenseDate = expenseDate;
                    _expenseDTO.Description = description;

                    // Gọi service để cập nhật
                    bool success = _expenseService.UpdateExpense(_expenseDTO);

                    if (success)
                    {
                        MessageBox.Show("Cập nhật chi phí thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật chi phí thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Tạo đối tượng ExpenseDTO mới
                    ExpenseDTO newExpense = new ExpenseDTO
                    {
                        ExpenseTypeID = expenseTypeID,
                        Amount = amount,
                        ExpenseDate = expenseDate,
                        Description = description,
                        EmployeeID = 1 // Tạm thời hardcode, cần lấy từ người đăng nhập
                    };

                    // Gọi service để thêm mới
                    int newExpenseID = _expenseService.CreateExpense(newExpense);

                    if (newExpenseID > 0)
                    {
                        MessageBox.Show("Thêm chi phí thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Thêm chi phí thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // Kiểm tra loại chi phí
            if (cboExpenseType.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn loại chi phí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboExpenseType.Focus();
                return false;
            }

            // Kiểm tra số tiền
            if (nudAmount.Value <= 0)
            {
                MessageBox.Show("Số tiền chi phí phải lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudAmount.Focus();
                return false;
            }

            // Kiểm tra ngày chi
            if (dtpExpenseDate.Value > DateTime.Now)
            {
                MessageBox.Show("Ngày chi không thể là ngày trong tương lai!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpExpenseDate.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #region Designer Generated Code

        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.dtpExpenseDate = new System.Windows.Forms.DateTimePicker();
            this.lblExpenseDate = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.nudAmount = new System.Windows.Forms.NumericUpDown();
            this.lblAmount = new System.Windows.Forms.Label();
            this.cboExpenseType = new System.Windows.Forms.ComboBox();
            this.lblExpenseType = new System.Windows.Forms.Label();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmount)).BeginInit();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.dtpExpenseDate);
            this.pnlMain.Controls.Add(this.lblExpenseDate);
            this.pnlMain.Controls.Add(this.txtDescription);
            this.pnlMain.Controls.Add(this.lblDescription);
            this.pnlMain.Controls.Add(this.nudAmount);
            this.pnlMain.Controls.Add(this.lblAmount);
            this.pnlMain.Controls.Add(this.cboExpenseType);
            this.pnlMain.Controls.Add(this.lblExpenseType);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(484, 261);
            this.pnlMain.TabIndex = 0;
            // 
            // dtpExpenseDate
            // 
            this.dtpExpenseDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpenseDate.Location = new System.Drawing.Point(121, 92);
            this.dtpExpenseDate.Name = "dtpExpenseDate";
            this.dtpExpenseDate.Size = new System.Drawing.Size(121, 20);
            this.dtpExpenseDate.TabIndex = 7;
            // 
            // lblExpenseDate
            // 
            this.lblExpenseDate.AutoSize = true;
            this.lblExpenseDate.Location = new System.Drawing.Point(24, 96);
            this.lblExpenseDate.Name = "lblExpenseDate";
            this.lblExpenseDate.Size = new System.Drawing.Size(53, 13);
            this.lblExpenseDate.TabIndex = 6;
            this.lblExpenseDate.Text = "Ngày chi:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(121, 124);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(330, 80);
            this.txtDescription.TabIndex = 5;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(24, 127);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(37, 13);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Mô tả:";
            // 
            // nudAmount
            // 
            this.nudAmount.DecimalPlaces = 0;
            this.nudAmount.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudAmount.Location = new System.Drawing.Point(121, 60);
            this.nudAmount.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudAmount.Name = "nudAmount";
            this.nudAmount.Size = new System.Drawing.Size(151, 20);
            this.nudAmount.TabIndex = 3;
            this.nudAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudAmount.ThousandsSeparator = true;
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(24, 64);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(46, 13);
            this.lblAmount.TabIndex = 2;
            this.lblAmount.Text = "Số tiền:";
            // 
            // cboExpenseType
            // 
            this.cboExpenseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExpenseType.FormattingEnabled = true;
            this.cboExpenseType.Location = new System.Drawing.Point(121, 28);
            this.cboExpenseType.Name = "cboExpenseType";
            this.cboExpenseType.Size = new System.Drawing.Size(243, 21);
            this.cboExpenseType.TabIndex = 1;
            // 
            // lblExpenseType
            // 
            this.lblExpenseType.AutoSize = true;
            this.lblExpenseType.Location = new System.Drawing.Point(24, 32);
            this.lblExpenseType.Name = "lblExpenseType";
            this.lblExpenseType.Size = new System.Drawing.Size(70, 13);
            this.lblExpenseType.TabIndex = 0;
            this.lblExpenseType.Text = "Loại chi phí:";
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.btnCancel);
            this.pnlActions.Controls.Add(this.btnSave);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 211);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(484, 50);
            this.pnlActions.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(376, 12);
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
            this.btnSave.Location = new System.Drawing.Point(283, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 26);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ExpenseDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExpenseDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chi tiết chi phí";
            this.Load += new System.EventHandler(this.ExpenseDetailForm_Load);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmount)).EndInit();
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DateTimePicker dtpExpenseDate;
        private System.Windows.Forms.Label lblExpenseDate;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.NumericUpDown nudAmount;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.ComboBox cboExpenseType;
        private System.Windows.Forms.Label lblExpenseType;
    }
}