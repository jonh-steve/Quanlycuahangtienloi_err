// File: /Forms/Controls/ProductSearchControl.cs
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bunifu.Framework.UI;
using QuanLyCuaHangTienLoi.Services;
using System.Linq;

namespace QuanLyCuaHangTienLoi.Forms.Controls
{
    public partial class ProductSearchControl : UserControl
    {
        private readonly IProductService _productService;
        private List<Product> _products;

        // Event để thông báo khi sản phẩm được chọn
        public event EventHandler<Product> ProductSelected;

        public ProductSearchControl(IProductService productService)
        {
            InitializeComponent();
            _productService = productService;

            // Thiết lập kiểu giao diện với màu hồng
            SetupPinkTheme();
        }

        private void SetupPinkTheme()
        {
            this.BackColor = Color.FromArgb(255, 240, 245); // Màu hồng nhạt

            // Thiết lập TextBox tìm kiếm
            txtSearch = new BunifuMetroTextbox();
            txtSearch.BorderColorFocused = Color.FromArgb(231, 62, 151); // Màu hồng đậm khi focus
            txtSearch.BorderColorIdle = Color.FromArgb(242, 145, 190); // Màu hồng vừa phải khi idle
            txtSearch.BorderColorMouseHover = Color.FromArgb(231, 62, 151); // Màu hồng đậm khi hover
            txtSearch.BorderThickness = 2;
            txtSearch.Dock = DockStyle.Top;
            txtSearch.Font = new Font("Segoe UI", 11F);
            txtSearch.ForeColor = Color.FromArgb(64, 64, 64);
            txtSearch.Margin = new Padding(3, 4, 3, 4);
            txtSearch.Padding = new Padding(5, 0, 5, 0);
            txtSearch.Size = new Size(400, 35);
            txtSearch.TextAlign = HorizontalAlignment.Left;
            txtSearch.TextMarginBottom = 0;
            txtSearch.TextMarginLeft = 5;
            txtSearch.TextMarginTop = 0;
            txtSearch.TextPlaceholder = "Nhập mã hoặc tên sản phẩm để tìm kiếm...";
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Thiết lập DataGridView hiển thị sản phẩm
            dgvProducts = new DataGridView();
            dgvProducts.BackgroundColor = Color.FromArgb(255, 240, 245);
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(231, 62, 151);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 62, 151);
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvProducts.ColumnHeadersHeight = 35;
            dgvProducts.DefaultCellStyle.BackColor = Color.White;
            dgvProducts.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 200, 230);
            dgvProducts.DefaultCellStyle.SelectionForeColor = Color.FromArgb(64, 64, 64);
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.GridColor = Color.FromArgb(242, 145, 190);
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;

            // Thêm các control vào panel
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Controls.Add(dgvProducts);
            panel.Controls.Add(txtSearch);

            Controls.Add(panel);
        }

        // Load dữ liệu sản phẩm
        public void LoadProducts()
        {
            try
            {
                // Lấy danh sách sản phẩm có trạng thái "Đang bán" và còn hàng trong kho
                _products = _productService.GetActiveProductsWithStock();

                // Thiết lập cột cho DataGridView
                dgvProducts.DataSource = null;
                dgvProducts.Columns.Clear();

                dgvProducts.Columns.Add("ProductID", "Mã SP");
                dgvProducts.Columns.Add("Barcode", "Mã vạch");
                dgvProducts.Columns.Add("ProductName", "Tên sản phẩm");
                dgvProducts.Columns.Add("Category", "Danh mục");
                dgvProducts.Columns.Add("SellPrice", "Giá bán");
                dgvProducts.Columns.Add("Quantity", "Tồn kho");

                // Thiết lập định dạng cho cột giá
                dgvProducts.Columns["SellPrice"].DefaultCellStyle.Format = "N0";

                // Hiển thị dữ liệu
                DisplayProducts(_products);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu sản phẩm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hiển thị danh sách sản phẩm
        private void DisplayProducts(List<Product> products)
        {
            dgvProducts.Rows.Clear();

            foreach (var product in products)
            {
                dgvProducts.Rows.Add(
                    product.ProductID,
                    product.Barcode,
                    product.ProductName,
                    product.CategoryName,
                    product.SellPrice,
                    product.CurrentStock
                );
            }
        }

        // Xử lý sự kiện khi người dùng nhập vào ô tìm kiếm
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                DisplayProducts(_products);
                return;
            }

            // Lọc sản phẩm theo từ khóa tìm kiếm
            var filteredProducts = _products.Where(p =>
                p.ProductCode.ToLower().Contains(searchText) ||
                p.Barcode?.ToLower().Contains(searchText) == true ||
                p.ProductName.ToLower().Contains(searchText)
            ).ToList();

            DisplayProducts(filteredProducts);
        }

        // Xử lý sự kiện khi người dùng double-click vào một sản phẩm
        private void DgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int productId = Convert.ToInt32(dgvProducts.Rows[e.RowIndex].Cells["ProductID"].Value);
                var selectedProduct = _products.FirstOrDefault(p => p.ProductID == productId);

                if (selectedProduct != null)
                {
                    // Gọi event để thông báo sản phẩm đã được chọn
                    ProductSelected?.Invoke(this, selectedProduct);
                }
            }
        }

        // Tìm kiếm sản phẩm theo mã vạch
        public Product FindProductByBarcode(string barcode)
        {
            return _products.FirstOrDefault(p => p.Barcode == barcode);
        }
    }
}