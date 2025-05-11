// Steve-Thuong_hai
using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Customers
{
    public partial class CustomerPointForm : Form
    {
        public enum PointMode
        {
            Add,
            Redeem
        }

        private readonly CustomerPointService _customerPointService;
        private readonly int _customerID;
        private readonly int _currentEmployeeID;
        private readonly PointMode _mode;

        // Colors for pink theme
        private readonly Color _primaryColor = Color.FromArgb(255, 192, 203); // Pink
        private readonly Color _secondaryColor = Color.FromArgb(255, 182, 193); // LightPink
        private readonly Color _accentColor = Color.FromArgb(219, 112, 147); // PaleVioletRed
        private readonly Color _textColor = Color.FromArgb(60, 60, 60); // Dark gray for text

        public CustomerPointForm(CustomerPointService customerPointService, int customerID,
            int currentEmployeeID, PointMode mode)
        {
            InitializeComponent();
            _customerPointService = customerPointService;
            _customerID = customerID;
            _currentEmployeeID = currentEmployeeID;
            _mode = mode;

            // Áp dụng theme
            ApplyPinkTheme();

            // Set up form theo mode
            SetupFormByMode();
        }

        private void ApplyPinkTheme()
        {
            // Set form properties
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.Text = _mode == PointMode.Add ? "Cộng điểm khách hàng" : "Đổi điểm khách hàng";

            // Set panel properties
            panelTop.BackColor = _primaryColor;

            // Set button properties
            btnSave.BackColor = _accentColor;
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSave.Cursor = Cursors.Hand;

            btnCancel.BackColor = Color.Gray;
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCancel.Cursor = Cursors.Hand;

            // Set label properties
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);

            lblPoints.ForeColor = _textColor;
            lblPoints.Font = new Font("Segoe UI", 9.5F);

            lblReason.ForeColor = _textColor;
            lblReason.Font = new Font("Segoe UI", 9.5F);

            // Set input controls
            numPoints.Font = new Font("Segoe UI", 10F);
            txtReason.Font = new Font("Segoe UI", 10F);
            txtReason.BorderStyle = BorderStyle.FixedSingle;
        }

        private void SetupFormByMode()
        {
            if (_mode == PointMode.Add)
            {
                lblTitle.Text = "CỘNG ĐIỂM KHÁCH HÀNG";
                lblPoints.Text = "Số điểm cộng:";
                btnSave.Text = "Cộng điểm";
                numPoints.Minimum = 1;
                numPoints.Maximum = 1000000;
                numPoints.Value = 1;
            }
            else // PointMode.Redeem
            {
                lblTitle.Text = "ĐỔI ĐIỂM KHÁCH HÀNG";
                lblPoints.Text = "Số điểm đổi:";
                btnSave.Text = "Đổi điểm";
                numPoints.Minimum = 1;
                numPoints.Maximum = 1000000;
                numPoints.Value = 1;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy số điểm và lý do
                int points = (int)numPoints.Value;
                string reason = txtReason.Text.Trim();

                if (points <= 0)
                {
                    MessageBox.Show("Số điểm phải lớn hơn 0", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(reason))
                {
                    reason = _mode == PointMode.Add ? "Cộng điểm thủ công" : "Đổi điểm thủ công";
                }

                bool result;

                if (_mode == PointMode.Add)
                {
                    // Gọi service cộng điểm
                    result = _customerPointService.AddPointsFromOrder(_customerID, 0, points * 10000, _currentEmployeeID);
                }
                else // PointMode.Redeem
                {
                    // Gọi service đổi điểm
                    result = _customerPointService.RedeemPoints(_customerID, points, reason, _currentEmployeeID);
                }

                if (result)
                {
                    // Điều chỉnh cấp độ thành viên nếu cần
                    _customerPointService.AdjustMembershipLevel(_customerID, _currentEmployeeID);

                    MessageBox.Show(
                        _mode == PointMode.Add ?
                            $"Đã cộng {points} điểm cho khách hàng" :
                            $"Đã đổi {points} điểm của khách hàng",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Khởi tạo Designer components
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblPoints = new System.Windows.Forms.Label();
            this.numPoints = new System.Windows.Forms.NumericUpDown();
            this.lblReason = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Size = new System.Drawing.Size(400, 60);
            this.panelTop.Controls.Add(this.lblTitle);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Size = new System.Drawing.Size(300, 30);
            this.lblTitle.Text = "CỘNG ĐIỂM KHÁCH HÀNG";

            // panelMain
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 60);
            this.panelMain.Size = new System.Drawing.Size(400, 140);
            this.panelMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelMain.Controls.Add(this.lblPoints);
            this.panelMain.Controls.Add(this.numPoints);
            this.panelMain.Controls.Add(this.lblReason);
            this.panelMain.Controls.Add(this.txtReason);

            // lblPoints
            this.lblPoints.AutoSize = true;
            this.lblPoints.Location = new System.Drawing.Point(20, 20);
            this.lblPoints.Size = new System.Drawing.Size(100, 20);
            this.lblPoints.Text = "Số điểm cộng:";

            // numPoints
            this.numPoints.Location = new System.Drawing.Point(130, 20);
            this.numPoints.Size = new System.Drawing.Size(100, 25);
            this.numPoints.Minimum = 1;
            this.numPoints.Maximum = 1000000;
            this.numPoints.Value = 1;

            // lblReason
            this.lblReason.AutoSize = true;
            this.lblReason.Location = new System.Drawing.Point(20, 60);
            this.lblReason.Size = new System.Drawing.Size(100, 20);
            this.lblReason.Text = "Lý do:";

            // txtReason
            this.txtReason.Location = new System.Drawing.Point(130, 60);
            this.txtReason.Size = new System.Drawing.Size(250, 25);
            this.txtReason.Multiline = true;
            this.txtReason.Height = 60;

            // panelBottom
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 200);
            this.panelBottom.Size = new System.Drawing.Size(400, 60);
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Controls.Add(this.btnSave);

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(180, 15);
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "Cộng điểm";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(290, 15);
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // CustomerPointForm
            this.ClientSize = new System.Drawing.Size(400, 260);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
        }

        private Panel panelTop;
        private Label lblTitle;
        private Panel panelMain;
        private Label lblPoints;
        private NumericUpDown numPoints;
        private Label lblReason;
        private TextBox txtReason;
        private Panel panelBottom;
        private Button btnCancel;
        private Button btnSave;
    }
}