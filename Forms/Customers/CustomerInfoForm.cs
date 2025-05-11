// Steve-Thuong_hai
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Customers
{
    public partial class CustomerInfoForm : Form
    {
        private readonly CustomerService _customerService;
        private readonly CustomerPointService _customerPointService;
        private readonly int _customerID;
        private readonly int _currentEmployeeID;
        private CustomerDTO _customerInfo;

        // Colors for pink theme
        private readonly Color _primaryColor = Color.FromArgb(255, 192, 203); // Pink
        private readonly Color _secondaryColor = Color.FromArgb(255, 182, 193); // LightPink
        private readonly Color _accentColor = Color.FromArgb(219, 112, 147); // PaleVioletRed
        private readonly Color _textColor = Color.FromArgb(60, 60, 60); // Dark gray for text

        public CustomerInfoForm(CustomerService customerService, int customerID, int currentEmployeeID)
        {
            InitializeComponent();
            _customerService = customerService;
            // Tạm thời dùng CustomerRepository giống CustomerService
            _customerPointService = new CustomerPointService(
                new Db.Repositories.CustomerRepository(new Db.ConnectionManager()), customerService);
            _customerID = customerID;
            _currentEmployeeID = currentEmployeeID;

            // Áp dụng theme
            ApplyPinkTheme();

            // Load dữ liệu
            LoadCustomerInfo();
            LoadPointHistory();
        }

        private void ApplyPinkTheme()
        {
            // Set form properties
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.Text = "Thông tin khách hàng";

            // Set panel properties
            panelTop.BackColor = _primaryColor;
            panelInfo.BackColor = Color.White;
            panelPoints.BackColor = Color.White;

            // Set button properties
            btnEdit.BackColor = _accentColor;
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnEdit.Cursor = Cursors.Hand;

            btnClose.BackColor = Color.Gray;
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;

            btnAddPoints.BackColor = _accentColor;
            btnAddPoints.ForeColor = Color.White;
            btnAddPoints.FlatStyle = FlatStyle.Flat;
            btnAddPoints.FlatAppearance.BorderSize = 0;
            btnAddPoints.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnAddPoints.Cursor = Cursors.Hand;

            btnRedeemPoints.BackColor = _accentColor;
            btnRedeemPoints.ForeColor = Color.White;
            btnRedeemPoints.FlatStyle = FlatStyle.Flat;
            btnRedeemPoints.FlatAppearance.BorderSize = 0;
            btnRedeemPoints.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRedeemPoints.Cursor = Cursors.Hand;

            // Set label properties
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCustomerName.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCustomerName.ForeColor = _accentColor;

            lblMembershipLevel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMembershipLevel.ForeColor = _accentColor;

            // Set các label thông tin
            foreach (Control control in panelInfo.Controls)
            {
                if (control is Label label)
                {
                    if (label.Name.StartsWith("lblInfo"))
                    {
                        label.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                        label.ForeColor = _textColor;
                    }
                    else if (label.Name.StartsWith("lblValue"))
                    {
                        label.Font = new Font("Segoe UI", 9.5F);
                        label.ForeColor = _textColor;
                    }
                }
            }

            // Set DataGridView
            dgvPointHistory.BackgroundColor = Color.White;
            dgvPointHistory.BorderStyle = BorderStyle.None;
            dgvPointHistory.RowHeadersVisible = false;
            dgvPointHistory.EnableHeadersVisualStyles = false;
            dgvPointHistory.ColumnHeadersDefaultCellStyle.BackColor = _secondaryColor;
            dgvPointHistory.ColumnHeadersDefaultCellStyle.ForeColor = _textColor;
            dgvPointHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvPointHistory.ColumnHeadersHeight = 40;
            dgvPointHistory.RowsDefaultCellStyle.BackColor = Color.White;
            dgvPointHistory.RowsDefaultCellStyle.ForeColor = _textColor;
            dgvPointHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvPointHistory.RowTemplate.Height = 35;

            // Set GroupBox
            groupBoxPointHistory.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBoxPointHistory.ForeColor = _accentColor;
        }

        private void LoadCustomerInfo()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Lấy thông tin khách hàng
                _customerInfo = _customerService.GetCustomerByID(_customerID);

                if (_customerInfo == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin khách hàng", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Hiển thị thông tin lên form
                lblCustomerName.Text = _customerInfo.CustomerName;
                lblMembershipLevel.Text = _customerInfo.MembershipLevel;

                lblValueID.Text = _customerInfo.CustomerID.ToString();
                lblValuePhone.Text = string.IsNullOrEmpty(_customerInfo.PhoneNumber) ? "Chưa cập nhật" : _customerInfo.PhoneNumber;
                lblValueEmail.Text = string.IsNullOrEmpty(_customerInfo.Email) ? "Chưa cập nhật" : _customerInfo.Email;
                lblValueAddress.Text = string.IsNullOrEmpty(_customerInfo.Address) ? "Chưa cập nhật" : _customerInfo.Address;
                lblValuePoints.Text = _customerInfo.Points.ToString("N0");

                // Tính giảm giá theo cấp độ thành viên
                decimal discount = _customerService.GetMembershipDiscount(_customerInfo.MembershipLevel) * 100;
                lblValueDiscount.Text = $"{discount:N0}%";

                lblValueTotalOrders.Text = _customerInfo.OrderCount.ToString("N0");
                lblValueTotalSpent.Text = _customerInfo.TotalSpent.ToString("N0") + " VNĐ";
                lblValueLastOrder.Text = _customerInfo.LastOrderDate.HasValue ?
                    _customerInfo.LastOrderDate.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa có đơn hàng";

                // Hiển thị ngày tạo
                lblValueCreatedDate.Text = _customerInfo.CreatedDate.ToString("dd/MM/yyyy");

                // Đổi màu label cấp độ theo cấp độ thành viên
                switch (_customerInfo.MembershipLevel)
                {
                    case "Regular":
                        lblMembershipLevel.ForeColor = Color.Gray;
                        break;
                    case "Silver":
                        lblMembershipLevel.ForeColor = Color.Silver;
                        break;
                    case "Gold":
                        lblMembershipLevel.ForeColor = Color.Gold;
                        break;
                    case "Platinum":
                        lblMembershipLevel.ForeColor = Color.DarkSlateBlue;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LoadPointHistory()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Thiết lập cột cho DataGridView
                dgvPointHistory.AutoGenerateColumns = false;
                dgvPointHistory.Columns.Clear();

                dgvPointHistory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TransactionDate",
                    HeaderText = "Ngày giao dịch",
                    DataPropertyName = "TransactionDate",
                    Width = 130,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "dd/MM/yyyy HH:mm"
                    }
                });

                dgvPointHistory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Points",
                    HeaderText = "Điểm",
                    DataPropertyName = "Points",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "+0;-0",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                });

                dgvPointHistory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PointType",
                    HeaderText = "Loại",
                    DataPropertyName = "PointType",
                    Width = 100
                });

                dgvPointHistory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Mô tả",
                    DataPropertyName = "Description",
                    Width = 200
                });

                dgvPointHistory.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CreatedByName",
                    HeaderText = "Người thực hiện",
                    DataPropertyName = "CreatedByName",
                    Width = 150
                });

                // Lấy lịch sử điểm
                var pointHistory = _customerService.GetCustomerPointHistory(_customerID);

                // Hiển thị dữ liệu
                dgvPointHistory.DataSource = pointHistory;

                // Tô màu cột điểm
                dgvPointHistory.CellFormatting += (sender, e) =>
                {
                    if (e.ColumnIndex == dgvPointHistory.Columns["Points"].Index && e.RowIndex >= 0)
                    {
                        int points = Convert.ToInt32(dgvPointHistory.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                        if (points > 0)
                        {
                            e.CellStyle.ForeColor = Color.Green;
                        }
                        else if (points < 0)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử điểm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở form chỉnh sửa
                var editForm = new CustomerDetailForm(_customerService, _currentEmployeeID, _customerID);

                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload thông tin
                    LoadCustomerInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddPoints_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị dialog nhập điểm
                using (var pointForm = new CustomerPointForm(_customerPointService, _customerID,
                    _currentEmployeeID, CustomerPointForm.PointMode.Add))
                {
                    if (pointForm.ShowDialog() == DialogResult.OK)
                    {
                        // Reload thông tin
                        LoadCustomerInfo();
                        LoadPointHistory();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm điểm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRedeemPoints_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị dialog đổi điểm
                using (var pointForm = new CustomerPointForm(_customerPointService, _customerID,
                    _currentEmployeeID, CustomerPointForm.PointMode.Redeem))
                {
                    if (pointForm.ShowDialog() == DialogResult.OK)
                    {
                        // Reload thông tin
                        LoadCustomerInfo();
                        LoadPointHistory();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đổi điểm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Khởi tạo Designer components
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.lblMembershipLevel = new System.Windows.Forms.Label();
            this.lblInfoID = new System.Windows.Forms.Label();
            this.lblValueID = new System.Windows.Forms.Label();
            this.lblInfoPhone = new System.Windows.Forms.Label();
            this.lblValuePhone = new System.Windows.Forms.Label();
            this.lblInfoEmail = new System.Windows.Forms.Label();
            this.lblValueEmail = new System.Windows.Forms.Label();
            this.lblInfoAddress = new System.Windows.Forms.Label();
            this.lblValueAddress = new System.Windows.Forms.Label();
            this.lblInfoPoints = new System.Windows.Forms.Label();
            this.lblValuePoints = new System.Windows.Forms.Label();
            this.lblInfoDiscount = new System.Windows.Forms.Label();
            this.lblValueDiscount = new System.Windows.Forms.Label();
            this.lblInfoTotalOrders = new System.Windows.Forms.Label();
            this.lblValueTotalOrders = new System.Windows.Forms.Label();
            this.lblInfoTotalSpent = new System.Windows.Forms.Label();
            this.lblValueTotalSpent = new System.Windows.Forms.Label();
            this.lblInfoLastOrder = new System.Windows.Forms.Label();
            this.lblValueLastOrder = new System.Windows.Forms.Label();
            this.lblInfoCreatedDate = new System.Windows.Forms.Label();
            this.lblValueCreatedDate = new System.Windows.Forms.Label();
            this.panelPoints = new System.Windows.Forms.Panel();
            this.groupBoxPointHistory = new System.Windows.Forms.GroupBox();
            this.dgvPointHistory = new System.Windows.Forms.DataGridView();
            this.btnAddPoints = new System.Windows.Forms.Button();
            this.btnRedeemPoints = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Size = new System.Drawing.Size(800, 60);
            this.panelTop.Controls.Add(this.lblTitle);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Size = new System.Drawing.Size(350, 30);
            this.lblTitle.Text = "THÔNG TIN KHÁCH HÀNG";

            // panelInfo
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelInfo.Location = new System.Drawing.Point(0, 60);
            this.panelInfo.Size = new System.Drawing.Size(350, 440);
            this.panelInfo.Padding = new System.Windows.Forms.Padding(20);
            this.panelInfo.Controls.Add(this.lblCustomerName);
            this.panelInfo.Controls.Add(this.lblMembershipLevel);
            this.panelInfo.Controls.Add(this.lblInfoID);
            this.panelInfo.Controls.Add(this.lblValueID);
            this.panelInfo.Controls.Add(this.lblInfoPhone);
            this.panelInfo.Controls.Add(this.lblValuePhone);
            this.panelInfo.Controls.Add(this.lblInfoEmail);
            this.panelInfo.Controls.Add(this.lblValueEmail);
            this.panelInfo.Controls.Add(this.lblInfoAddress);
            this.panelInfo.Controls.Add(this.lblValueAddress);
            this.panelInfo.Controls.Add(this.lblInfoPoints);
            this.panelInfo.Controls.Add(this.lblValuePoints);
            this.panelInfo.Controls.Add(this.lblInfoDiscount);
            this.panelInfo.Controls.Add(this.lblValueDiscount);
            this.panelInfo.Controls.Add(this.lblInfoTotalOrders);
            this.panelInfo.Controls.Add(this.lblValueTotalOrders);
            this.panelInfo.Controls.Add(this.lblInfoTotalSpent);
            this.panelInfo.Controls.Add(this.lblValueTotalSpent);
            this.panelInfo.Controls.Add(this.lblInfoLastOrder);
            this.panelInfo.Controls.Add(this.lblValueLastOrder);
            this.panelInfo.Controls.Add(this.lblInfoCreatedDate);
            this.panelInfo.Controls.Add(this.lblValueCreatedDate);

            // lblCustomerName
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(20, 20);
            this.lblCustomerName.Size = new System.Drawing.Size(300, 30);
            this.lblCustomerName.Text = "Tên Khách Hàng";

            // lblMembershipLevel
            this.lblMembershipLevel.AutoSize = true;
            this.lblMembershipLevel.Location = new System.Drawing.Point(20, 50);
            this.lblMembershipLevel.Size = new System.Drawing.Size(150, 20);
            this.lblMembershipLevel.Text = "Regular";

            // lblInfoID
            this.lblInfoID.AutoSize = true;
            this.lblInfoID.Location = new System.Drawing.Point(20, 90);
            this.lblInfoID.Size = new System.Drawing.Size(100, 20);
            this.lblInfoID.Text = "Mã khách hàng:";

            // lblValueID
            this.lblValueID.AutoSize = true;
            this.lblValueID.Location = new System.Drawing.Point(150, 90);
            this.lblValueID.Size = new System.Drawing.Size(100, 20);
            this.lblValueID.Text = "1";

            // lblInfoPhone
            this.lblInfoPhone.AutoSize = true;
            this.lblInfoPhone.Location = new System.Drawing.Point(20, 120);
            this.lblInfoPhone.Size = new System.Drawing.Size(100, 20);
            this.lblInfoPhone.Text = "Số điện thoại:";

            // lblValuePhone
            this.lblValuePhone.AutoSize = true;
            this.lblValuePhone.Location = new System.Drawing.Point(150, 120);
            this.lblValuePhone.Size = new System.Drawing.Size(150, 20);
            this.lblValuePhone.Text = "0123456789";

            // lblInfoEmail
            this.lblInfoEmail.AutoSize = true;
            this.lblInfoEmail.Location = new System.Drawing.Point(20, 150);
            this.lblInfoEmail.Size = new System.Drawing.Size(100, 20);
            this.lblInfoEmail.Text = "Email:";

            // lblValueEmail
            this.lblValueEmail.AutoSize = true;
            this.lblValueEmail.Location = new System.Drawing.Point(150, 150);
            this.lblValueEmail.Size = new System.Drawing.Size(150, 20);
            this.lblValueEmail.Text = "email@example.com";

            // lblInfoAddress
            this.lblInfoAddress.AutoSize = true;
            this.lblInfoAddress.Location = new System.Drawing.Point(20, 180);
            this.lblInfoAddress.Size = new System.Drawing.Size(100, 20);
            this.lblInfoAddress.Text = "Địa chỉ:";

            // lblValueAddress
            this.lblValueAddress.AutoSize = true;
            this.lblValueAddress.Location = new System.Drawing.Point(150, 180);
            this.lblValueAddress.Size = new System.Drawing.Size(150, 20);
            this.lblValueAddress.MaximumSize = new System.Drawing.Size(180, 0);
            this.lblValueAddress.Text = "Địa chỉ khách hàng";

            // lblInfoPoints
            this.lblInfoPoints.AutoSize = true;
            this.lblInfoPoints.Location = new System.Drawing.Point(20, 230);
            this.lblInfoPoints.Size = new System.Drawing.Size(100, 20);
            this.lblInfoPoints.Text = "Điểm tích lũy:";

            // lblValuePoints
            this.lblValuePoints.AutoSize = true;
            this.lblValuePoints.Location = new System.Drawing.Point(150, 230);
            this.lblValuePoints.Size = new System.Drawing.Size(100, 20);
            this.lblValuePoints.Text = "0";

            // lblInfoDiscount
            this.lblInfoDiscount.AutoSize = true;
            this.lblInfoDiscount.Location = new System.Drawing.Point(20, 260);
            this.lblInfoDiscount.Size = new System.Drawing.Size(100, 20);
            this.lblInfoDiscount.Text = "Giảm giá:";

            // lblValueDiscount
            this.lblValueDiscount.AutoSize = true;
            this.lblValueDiscount.Location = new System.Drawing.Point(150, 260);
            this.lblValueDiscount.Size = new System.Drawing.Size(100, 20);
            this.lblValueDiscount.Text = "0%";

            // lblInfoTotalOrders
            this.lblInfoTotalOrders.AutoSize = true;
            this.lblInfoTotalOrders.Location = new System.Drawing.Point(20, 290);
            this.lblInfoTotalOrders.Size = new System.Drawing.Size(100, 20);
            this.lblInfoTotalOrders.Text = "Số đơn hàng:";

            // lblValueTotalOrders
            this.lblValueTotalOrders.AutoSize = true;
            this.lblValueTotalOrders.Location = new System.Drawing.Point(150, 290);
            this.lblValueTotalOrders.Size = new System.Drawing.Size(100, 20);
            this.lblValueTotalOrders.Text = "0";

            // lblInfoTotalSpent
            this.lblInfoTotalSpent.AutoSize = true;
            this.lblInfoTotalSpent.Location = new System.Drawing.Point(20, 320);
            this.lblInfoTotalSpent.Size = new System.Drawing.Size(100, 20);
            this.lblInfoTotalSpent.Text = "Tổng chi tiêu:";

            // lblValueTotalSpent
            this.lblValueTotalSpent.AutoSize = true;
            this.lblValueTotalSpent.Location = new System.Drawing.Point(150, 320);
            this.lblValueTotalSpent.Size = new System.Drawing.Size(100, 20);
            this.lblValueTotalSpent.Text = "0 VNĐ";

            // lblInfoLastOrder
            this.lblInfoLastOrder.AutoSize = true;
            this.lblInfoLastOrder.Location = new System.Drawing.Point(20, 350);
            this.lblInfoLastOrder.Size = new System.Drawing.Size(120, 20);
            this.lblInfoLastOrder.Text = "Mua hàng gần nhất:";

            // lblValueLastOrder
            this.lblValueLastOrder.AutoSize = true;
            this.lblValueLastOrder.Location = new System.Drawing.Point(150, 350);
            this.lblValueLastOrder.Size = new System.Drawing.Size(150, 20);
            this.lblValueLastOrder.Text = "Chưa có đơn hàng";

            // lblInfoCreatedDate
            this.lblInfoCreatedDate.AutoSize = true;
            this.lblInfoCreatedDate.Location = new System.Drawing.Point(20, 380);
            this.lblInfoCreatedDate.Size = new System.Drawing.Size(100, 20);
            this.lblInfoCreatedDate.Text = "Ngày tạo:";

            // lblValueCreatedDate
            this.lblValueCreatedDate.AutoSize = true;
            this.lblValueCreatedDate.Location = new System.Drawing.Point(150, 380);
            this.lblValueCreatedDate.Size = new System.Drawing.Size(100, 20);
            this.lblValueCreatedDate.Text = "01/01/2025";

            // panelPoints
            this.panelPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPoints.Location = new System.Drawing.Point(350, 60);
            this.panelPoints.Size = new System.Drawing.Size(450, 440);
            this.panelPoints.Padding = new System.Windows.Forms.Padding(10);
            this.panelPoints.Controls.Add(this.groupBoxPointHistory);
            this.panelPoints.Controls.Add(this.btnAddPoints);
            this.panelPoints.Controls.Add(this.btnRedeemPoints);

            // groupBoxPointHistory
            this.groupBoxPointHistory.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxPointHistory.Location = new System.Drawing.Point(10, 10);
            this.groupBoxPointHistory.Size = new System.Drawing.Size(430, 350);
            this.groupBoxPointHistory.Text = "Lịch sử điểm";
            this.groupBoxPointHistory.Controls.Add(this.dgvPointHistory);

            // dgvPointHistory
            this.dgvPointHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPointHistory.Location = new System.Drawing.Point(3, 23);
            this.dgvPointHistory.Size = new System.Drawing.Size(424, 324);
            this.dgvPointHistory.ReadOnly = true;
            this.dgvPointHistory.MultiSelect = false;
            this.dgvPointHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPointHistory.AllowUserToAddRows = false;
            this.dgvPointHistory.AllowUserToDeleteRows = false;
            this.dgvPointHistory.AllowUserToResizeRows = false;

            // btnAddPoints
            this.btnAddPoints.Location = new System.Drawing.Point(240, 370);
            this.btnAddPoints.Size = new System.Drawing.Size(100, 30);
            this.btnAddPoints.Text = "Cộng điểm";
            this.btnAddPoints.Click += new System.EventHandler(this.btnAddPoints_Click);

            // btnRedeemPoints
            this.btnRedeemPoints.Location = new System.Drawing.Point(350, 370);
            this.btnRedeemPoints.Size = new System.Drawing.Size(100, 30);
            this.btnRedeemPoints.Text = "Đổi điểm";
            this.btnRedeemPoints.Click += new System.EventHandler(this.btnRedeemPoints_Click);

            // panelBottom
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 500);
            this.panelBottom.Size = new System.Drawing.Size(800, 60);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnEdit);

            // btnEdit
            this.btnEdit.Location = new System.Drawing.Point(580, 15);
            this.btnEdit.Size = new System.Drawing.Size(100, 30);
            this.btnEdit.Text = "Chỉnh sửa";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);

            // btnClose
            this.btnClose.Location = new System.Drawing.Point(690, 15);
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // CustomerInfoForm
            this.ClientSize = new System.Drawing.Size(800, 560);
            this.Controls.Add(this.panelPoints);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thông tin khách hàng";
        }

        private Panel panelTop;
        private Label lblTitle;
        private Panel panelInfo;
        private Label lblCustomerName;
        private Label lblMembershipLevel;
        private Label lblInfoID;
        private Label lblValueID;
        private Label lblInfoPhone;
        private Label lblValuePhone;
        private Label lblInfoEmail;
        private Label lblValueEmail;
        private Label lblInfoAddress;
        private Label lblValueAddress;
        private Label lblInfoPoints;
        private Label lblValuePoints;
        private Label lblInfoDiscount;
        private Label lblValueDiscount;
        private Label lblInfoTotalOrders;
        private Label lblValueTotalOrders;
        private Label lblInfoTotalSpent;
        private Label lblValueTotalSpent;
        private Label lblInfoLastOrder;
        private Label lblValueLastOrder;
        private Label lblInfoCreatedDate;
        private Label lblValueCreatedDate;
        private Panel panelPoints;
        private GroupBox groupBoxPointHistory;
        private DataGridView dgvPointHistory;
        private Button btnAddPoints;
        private Button btnRedeemPoints;
        private Panel panelBottom;
        private Button btnClose;
        private Button btnEdit;
    }
}