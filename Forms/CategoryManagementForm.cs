// Mã gợi ý cho CategoryManagementForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Products
{
    public partial class CategoryManagementForm : Form
    {
        private ICategoryService _categoryService;

        private TreeView treeCategories;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelDetail;
        private Label lblCategoryInfo;

        // Màu sắc hồng dễ thương
        private Color pinkLight = Color.FromArgb(255, 192, 203); // Pink
        private Color pinkLighter = Color.FromArgb(255, 228, 225); // MistyRose
        private Color pinkDark = Color.FromArgb(255, 105, 180); // HotPink

        public CategoryManagementForm()
        {
            _categoryService = CategoryService.Instance;
            InitializeComponent();
            ConfigureForm();
            LoadCategories();
        }

        // Mã InitializeComponent, ConfigureForm, LoadCategories, và các hàm khác sẽ được thêm sau
        // Đây là form tương đối phức tạp với TreeView hiển thị cấu trúc cây danh mục
    }
}