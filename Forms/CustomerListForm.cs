// Steve-Thuong_hai
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Customers
{
    public partial class CustomerListForm : Form
    {
        private readonly CustomerService _customerService;
        private List<CustomerDTO> _customers;
        private int _currentEmployeeID;

        // Colors for pink theme
        private readonly Color _primaryColor = Color.FromArgb(255, 192, 203); // Pink
        private readonly Color _secondaryColor = Color.FromArgb(255, 182, 193); // LightPink
        private readonly Color _accentColor = Color.FromArgb(219, 112, 147); // PaleVioletRed
        private readonly Color _textColor = Color.FromArgb(60, 60, 60); // Dark gray for text

        public CustomerListForm(CustomerService customerService, int employeeID)
        {
            InitializeComponent();
            _customerService = customerService;
            _currentEmployeeID = employeeID;

            // Áp dụng theme màu hồng
            ApplyPinkTheme();

            // Khởi tạo DataGridView
            SetupDataGridView();

            // Load dữ liệu
            LoadCustomers();
        }

        private void ApplyPinkTheme()
        {
            // Set form properties
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.Text = "Quản lý khách hàng";
            this.Icon = Properties.Resources.CustomerIcon;

            // Set panel properties
            panelTop.BackColor = _primaryColor;
            panelSearch.BackColor = Color.White;
            panelBottom.BackColor = _primaryColor;

            // Set button properties
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = _accentColor;
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                    button.Cursor = Cursors.Hand;
                }
            }

            // Các button cụ thể
            btnAdd.BackColor = _accentColor;
            btnAdd.ForeColor = Color.White;
            btnEdit.BackColor = _accentColor;
            btnEdit.ForeColor = Color.White;
            btnDelete.BackColor = _accentColor;
            btnDelete.ForeColor = Color.White;
            btnSearch.BackColor = _accentColor;
            btnSearch.ForeColor = Color.White;
            btnViewDetail.BackColor = _accentColor;
            btnViewDetail.ForeColor = Color.White;
            btnRefresh.BackColor = _accentColor;
            btnRefresh.ForeColor = Color.White;

            // Set TextBox properties
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("Segoe UI", 10F);

            // Set label properties
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblSearch.ForeColor = _textColor;
            lblSearch.Font = new Font("Segoe UI", 10F);
            lblTotal.ForeColor = _textColor;
            lblTotal.Font = new Font("Segoe UI", 10F);
        }

        private void SetupDataGridView()
        {
            // Thiết lập thuộc tính cho DataGridView
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            dgvCustomers.AllowUserToResizeRows = false;
            dgvCustomers.BackgroundColor = Color.White;
            dgvCustomers.BorderStyle = BorderStyle.None;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.EnableHeadersVisualStyles = false;

            // Thiết lập màu cho header
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = _secondaryColor;
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = _textColor;
            dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvCustomers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomers.ColumnHeadersHeight = 40;

            // Thiết lập màu cho các hàng
            dgvCustomers.RowsDefaultCellStyle.BackColor = Color.White;
            dgvCustomers.RowsDefaultCellStyle.ForeColor = _textColor;
            dgvCustomers.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvCustomers.RowsDefaultCellStyle.SelectionBackColor = _primaryColor;
            dgvCustomers.RowsDefaultCellStyle.SelectionForeColor = _textColor;
            dgvCustomers.RowTemplate.Height = 35;
            dgvCustomers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Thiết lập các cột
            dgvCustomers.Columns.Clear();
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerID",
                HeaderText = "Mã KH",
                DataPropertyName = "CustomerID",
                Width = 70
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Tên khách hàng",
                DataPropertyName = "CustomerName",
                Width = 200
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PhoneNumber",
                HeaderText = "Số điện thoại",
                DataPropertyName = "PhoneNumber",
                Width = 120
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                DataPropertyName = "Email",
                Width = 180
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MembershipLevel",
                HeaderText = "Hạng thành viên",
                DataPropertyName = "MembershipLevel",
                Width = 120
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Points",
                HeaderText = "Điểm tích lũy",
                DataPropertyName = "Points",
                Width = 100
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OrderCount",
                HeaderText = "Số đơn hàng",
                DataPropertyName = "OrderCount",
                Width = 100
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalSpent",
                HeaderText = "Tổng chi tiêu",
                DataPropertyName = "TotalSpent",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LastOrderDate",
                HeaderText = "Mua hàng gần nhất",
                DataPropertyName = "LastOrderDate",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy HH:mm"
                }
            });
        }

        private void LoadCustomers()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Lấy danh sách khách hàng
                _customers = _customerService.GetAllCustomers();

                // Hiển thị dữ liệu lên DataGridView
                dgvCustomers.DataSource = null;
                dgvCustomers.DataSource = _customers;

                // Cập nhật số lượng khách hàng
                lblTotal.Text = $"Tổng số: {_customers.Count} khách hàng";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    LoadCustomers();
                    return;
                }

                // Tìm kiếm khách hàng
                _customers = _customerService.SearchCustomers(searchTerm);

                // Hiển thị kết quả
                dgvCustomers.DataSource = null;
                dgvCustomers.DataSource = _customers;

                // Cập nhật số lượng
                lblTotal.Text = $"Kết quả: {_customers.Count} khách hàng";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở form thêm mới khách hàng
                var detailForm = new CustomerDetailForm(_customerService, _currentEmployeeID);

                if (detailForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thêm khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã chọn khách hàng chưa
                if (dgvCustomers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần chỉnh sửa", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lấy ID khách hàng đã chọn
                int customerID = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["CustomerID"].Value);

                // Mở form chỉnh sửa
                var detailForm = new CustomerDetailForm(_customerService, _currentEmployeeID, customerID);

                if (detailForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã chọn khách hàng chưa
                if (dgvCustomers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần xóa", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lấy thông tin khách hàng đã chọn
                int customerID = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["CustomerID"].Value);
                string customerName = dgvCustomers.SelectedRows[0].Cells["CustomerName"].Value.ToString();

                // Xác nhận xóa
                DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa khách hàng '{customerName}'?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Gọi service để xóa
                    // Note: Chức năng xóa khách hàng chưa được phát triển trong DB, có thể đánh dấu không hoạt động
                    MessageBox.Show("Chức năng xóa khách hàng sẽ được phát triển trong phiên bản tiếp theo.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // LoadCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewDetail_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã chọn khách hàng chưa
                if (dgvCustomers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng để xem chi tiết", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lấy ID khách hàng đã chọn
                int customerID = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["CustomerID"].Value);

                // Mở form chi tiết
                var customerInfoForm = new CustomerInfoForm(_customerService, customerID, _currentEmployeeID);
                customerInfoForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form xem chi tiết khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadCustomers();
        }

        private void dgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnViewDetail_Click(sender, e);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(sender, e);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        // Khởi tạo Designer components
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.dgvCustomers = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnViewDetail = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Size = new System.Drawing.Size(1000, 60);
            this.panelTop.Controls.Add(this.lblTitle);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Size = new System.Drawing.Size(250, 30);
            this.lblTitle.Text = "QUẢN LÝ KHÁCH HÀNG";

            // panelSearch
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 60);
            this.panelSearch.Size = new System.Drawing.Size(1000, 60);
            this.panelSearch.Controls.Add(this.btnRefresh);
            this.panelSearch.Controls.Add(this.btnSearch);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.lblSearch);

            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(20, 20);
            this.lblSearch.Size = new System.Drawing.Size(80, 20);
            this.lblSearch.Text = "Tìm kiếm:";

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(110, 17);
            this.txtSearch.Size = new System.Drawing.Size(300, 25);

            // btnSearch
            this.btnSearch.Location = new System.Drawing.Point(420, 17);
            this.btnSearch.Size = new System.Drawing.Size(100, 30);
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(530, 17);
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // dgvCustomers
            this.dgvCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomers.Location = new System.Drawing.Point(0, 120);
            this.dgvCustomers.Size = new System.Drawing.Size(1000, 380);
            this.dgvCustomers.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomers_CellDoubleClick);

            // panelBottom
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 500);
            this.panelBottom.Size = new System.Drawing.Size(1000, 60);
            this.panelBottom.Controls.Add(this.lblTotal);
            this.panelBottom.Controls.Add(this.btnViewDetail);
            this.panelBottom.Controls.Add(this.btnDelete);
            this.panelBottom.Controls.Add(this.btnEdit);
            this.panelBottom.Controls.Add(this.btnAdd);

            // lblTotal
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(20, 20);
            this.lblTotal.Size = new System.Drawing.Size(150, 20);
            this.lblTotal.Text = "Tổng số: 0 khách hàng";

            // btnAdd
            this.btnAdd.Location = new System.Drawing.Point(460, 15);
            this.btnAdd.Size = new System.Drawing.Size(120, 30);
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // btnEdit
            this.btnEdit.Location = new System.Drawing.Point(590, 15);
            this.btnEdit.Size = new System.Drawing.Size(120, 30);
            this.btnEdit.Text = "Chỉnh sửa";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(720, 15);
            this.btnDelete.Size = new System.Drawing.Size(120, 30);
            this.btnDelete.Text = "Xóa";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // btnViewDetail
            this.btnViewDetail.Location = new System.Drawing.Point(850, 15);
            this.btnViewDetail.Size = new System.Drawing.Size(120, 30);
            this.btnViewDetail.Text = "Xem chi tiết";
            this.btnViewDetail.Click += new System.EventHandler(this.btnViewDetail_Click);

            // CustomerListForm
            this.ClientSize = new System.Drawing.Size(1000, 560);
            this.Controls.Add(this.dgvCustomers);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.panelTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.Text = "Quản lý khách hàng";
        }

        private Panel panelTop;
        private Label lblTitle;
        private Panel panelSearch;
        private Button btnRefresh;
        private Button btnSearch;
        private TextBox txtSearch;
        private Label lblSearch;
        private DataGridView dgvCustomers;
        private Panel panelBottom;
        private Label lblTotal;
        private Button btnViewDetail;
        private Button btnDelete;
        private Button btnEdit;
        private Button btnAdd;
    }
}