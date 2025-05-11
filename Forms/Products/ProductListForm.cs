// File: Forms/Products/ProductListForm.cs (Form - Windows Forms)
using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Products
{
    public partial class ProductListForm : Form
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private ProductSearchResultDTO _searchResult;
        private int _currentPage = 1;
        private int _pageSize = 20;
        private string _searchTerm = string.Empty;
        private int? _categoryID = null;
        private bool? _isActive = true;

      
        public ProductListForm(IProductService productService, ICategoryService categoryService)
        {
            InitializeComponent();
            _productService = productService;
            _categoryService = categoryService;

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 240, 245);
            btnAdd.BackColor = Color.FromArgb(255, 182, 193);
            btnEdit.BackColor = Color.FromArgb(255, 182, 193);
            btnDelete.BackColor = Color.FromArgb(255, 182, 193);
            btnRefresh.BackColor = Color.FromArgb(255, 182, 193);
            btnSearch.BackColor = Color.FromArgb(255, 182, 193);
            btnClearFilter.BackColor = Color.FromArgb(255, 182, 193);
            btnExport.BackColor = Color.FromArgb(255, 182, 193);
            btnImport.BackColor = Color.FromArgb(255, 182, 193);

            // Khởi tạo DataGridView
            InitializeDataGridView();

            // Đăng ký sự kiện
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnSearch.Click += BtnSearch_Click;
            btnClearFilter.Click += BtnClearFilter_Click;
            btnExport.Click += BtnExport_Click;
            btnImport.Click += BtnImport_Click;
            btnFirst.Click += BtnFirst_Click;
            btnPrevious.Click += BtnPrevious_Click;
            btnNext.Click += BtnNext_Click;
            btnLast.Click += BtnLast_Click;
            dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;

            // Tự động tìm kiếm khi thay đổi bộ lọc
            cboCategory.SelectedIndexChanged += Filter_Changed;
            chkShowInactive.CheckedChanged += Filter_Changed;
        }

        private void InitializeDataGridView()
        {
            dgvProducts.AutoGenerateColumns = false;

            // Thêm các cột
            var colID = new DataGridViewTextBoxColumn
            {
                Name = "colProductID",
                DataPropertyName = "ProductID",
                HeaderText = "ID",
                Width = 50,
                ReadOnly = true
            };

            var colCode = new DataGridViewTextBoxColumn
            {
                Name = "colProductCode",
                DataPropertyName = "ProductCode",
                HeaderText = "Mã SP",
                Width = 80,
                ReadOnly = true
            };

            var colName = new DataGridViewTextBoxColumn
            {
                Name = "colProductName",
                DataPropertyName = "ProductName",
                HeaderText = "Tên sản phẩm",
                Width = 200,
                ReadOnly = true
            };

            var colCategory = new DataGridViewTextBoxColumn
            {
                Name = "colCategoryName",
                DataPropertyName = "CategoryName",
                HeaderText = "Danh mục",
                Width = 120,
                ReadOnly = true
            };

            var colCostPrice = new DataGridViewTextBoxColumn
            {
                Name = "colCostPrice",
                DataPropertyName = "CostPrice",
                HeaderText = "Giá nhập",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            };

            var colSellPrice = new DataGridViewTextBoxColumn
            {
                Name = "colSellPrice",
                DataPropertyName = "SellPrice",
                HeaderText = "Giá bán",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            };

            var colStock = new DataGridViewTextBoxColumn
            {
                Name = "colCurrentStock",
                DataPropertyName = "CurrentStock",
                HeaderText = "Tồn kho",
                Width = 80,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            var colUnit = new DataGridViewTextBoxColumn
            {
                Name = "colUnit",
                DataPropertyName = "Unit",
                HeaderText = "Đơn vị",
                Width = 60,
                ReadOnly = true
            };

            var colStatus = new DataGridViewCheckBoxColumn
            {
                Name = "colIsActive",
                DataPropertyName = "IsActive",
                HeaderText = "Kích hoạt",
                Width = 70,
                ReadOnly = true
            };

            // Thêm cột vào DataGridView
            dgvProducts.Columns.Add(colID);
            dgvProducts.Columns.Add(colCode);
            dgvProducts.Columns.Add(colName);
            dgvProducts.Columns.Add(colCategory);
            dgvProducts.Columns.Add(colCostPrice);
            dgvProducts.Columns.Add(colSellPrice);
            dgvProducts.Columns.Add(colStock);
            dgvProducts.Columns.Add(colUnit);
            dgvProducts.Columns.Add(colStatus);

            // Định dạng DataGridView
            dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245);
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 182, 193);
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProducts.RowHeadersVisible = false;

            // Tùy chỉnh cột tồn kho
            dgvProducts.CellFormatting += (sender, e) => {
                if (e.ColumnIndex == colStock.Index && e.RowIndex >= 0)
                {
                    var product = dgvProducts.Rows[e.RowIndex].DataBoundItem as ProductDTO;
                    if (product != null && product.IsLowStock)
                    {
                        e.CellStyle.ForeColor = Color.Red;
                        e.CellStyle.Font = new Font(dgvProducts.Font, FontStyle.Bold);
                    }
                }
            };
        }

        private void LoadCategories()
        {
            try
            {
                // Lấy danh sách danh mục
                var categories = _categoryService.GetAllCategories();

                cboCategory.DisplayMember = "Text";
                cboCategory.ValueMember = "Value";

                // Thêm tùy chọn "Tất cả danh mục"
                cboCategory.Items.Add(new ComboBoxItem { Text = "-- Tất cả danh mục --", Value = null });

                // Thêm các danh mục
                foreach (var category in categories)
                {
                    string prefix = string.Empty;
                    if (category.Level > 0)
                    {
                        prefix = new string(' ', category.Level * 2) + "└─ ";
                    }

                    cboCategory.Items.Add(new ComboBoxItem
                    {
                        Text = prefix + category.CategoryName,
                        Value = category.CategoryID
                    });
                }

                // Chọn tùy chọn đầu tiên
                cboCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                // Tìm kiếm sản phẩm
                _searchResult = _productService.SearchProducts(
                    _searchTerm,
                    _categoryID,
                    _isActive,
                    _currentPage,
                    _pageSize
                );

                // Hiển thị dữ liệu
                dgvProducts.DataSource = _searchResult.Products;

                // Cập nhật thông tin phân trang
                lblPageInfo.Text = $"Trang {_currentPage}/{_searchResult.TotalPages}";
                btnFirst.Enabled = _currentPage > 1;
                btnPrevious.Enabled = _currentPage > 1;
                btnNext.Enabled = _currentPage < _searchResult.TotalPages;
                btnLast.Enabled = _currentPage < _searchResult.TotalPages;

                // Cập nhật tổng số sản phẩm
                lblTotalProducts.Text = $"Tổng số: {_searchResult.TotalProducts} sản phẩm";

                // Kích hoạt/vô hiệu hóa các nút chức năng
                btnEdit.Enabled = dgvProducts.SelectedRows.Count > 0;
                btnDelete.Enabled = dgvProducts.SelectedRows.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sản phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ProductDetailForm(null, _productService, _categoryService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0) return;

            var selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as ProductDTO;
            if (selectedProduct == null) return;

            // Lấy thông tin chi tiết sản phẩm (bao gồm hình ảnh và lịch sử giá)
            var product = _productService.GetProductByID(selectedProduct.ProductID);

            using (var form = new ProductDetailForm(product, _productService, _categoryService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0) return;

            var selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as ProductDTO;
            if (selectedProduct == null) return;

            if (MessageBox.Show($"Bạn có chắc chắn muốn vô hiệu hóa sản phẩm '{selectedProduct.ProductName}'?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _productService.DeactivateProduct(selectedProduct.ProductID);
                    MessageBox.Show("Vô hiệu hóa sản phẩm thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi vô hiệu hóa sản phẩm: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            _searchTerm = txtSearch.Text.Trim();
            _currentPage = 1;
            LoadProducts();
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            cboCategory.SelectedIndex = 0;
            chkShowInactive.Checked = false;
            _searchTerm = string.Empty;
            _categoryID = null;
            _isActive = true;
            _currentPage = 1;
            LoadProducts();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            // TODO: Implement export to Excel
            MessageBox.Show("Chức năng xuất Excel sẽ được phát triển sau!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            // TODO: Implement import from Excel
            MessageBox.Show("Chức năng nhập Excel sẽ được phát triển sau!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnFirst_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            LoadProducts();
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadProducts();
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _searchResult.TotalPages)
            {
                _currentPage++;
                LoadProducts();
            }
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            _currentPage = _searchResult.TotalPages;
            LoadProducts();
        }

        private void DgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnEdit_Click(sender, EventArgs.Empty);
            }
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            // Cập nhật bộ lọc
            _categoryID = (cboCategory.SelectedItem as ComboBoxItem)?.Value;
            _isActive = chkShowInactive.Checked ? (bool?)null : true;
            _currentPage = 1;

            // Tải lại dữ liệu
            LoadProducts();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Tải danh sách danh mục
            LoadCategories();

            // Tải danh sách sản phẩm
            LoadProducts();
        }

        // Lớp hỗ trợ cho ComboBox
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public int? Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}