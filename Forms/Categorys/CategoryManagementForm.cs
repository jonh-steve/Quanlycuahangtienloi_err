// File: Forms/Products/CategoryManagementForm.cs (Form - Windows Forms)
using System;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Products
{
    public partial class CategoryManagementForm : Form
    {
        private readonly ICategoryService _categoryService;

        public CategoryManagementForm(ICategoryService categoryService)
        {
            InitializeComponent();
            _categoryService = categoryService;

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = System.Drawing.Color.FromArgb(255, 240, 245);
            btnAdd.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            btnEdit.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            btnDelete.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            btnRefresh.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);

            // Khởi tạo TreeView
            InitializeTreeView();

            // Đăng ký sự kiện
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;
            tvCategories.AfterSelect += TvCategories_AfterSelect;
        }

        private void InitializeTreeView()
        {
            tvCategories.ImageList = new ImageList();
            tvCategories.ImageList.Images.Add("folder", Properties.Resources.folder_icon);
            tvCategories.ImageList.Images.Add("folder_open", Properties.Resources.folder_open_icon);

            tvCategories.NodeMouseClick += (sender, args) => {
                if (args.Button == MouseButtons.Right)
                {
                    tvCategories.SelectedNode = args.Node;
                    cmsCategoryMenu.Show(tvCategories, args.Location);
                }
            };

            // Thêm context menu
            cmsCategoryMenu = new ContextMenuStrip();
            cmsCategoryMenu.Items.Add("Thêm danh mục con", null, (s, e) => {
                AddSubCategory();
            });
            cmsCategoryMenu.Items.Add("Chỉnh sửa", null, (s, e) => {
                BtnEdit_Click(s, e);
            });
            cmsCategoryMenu.Items.Add("Xóa", null, (s, e) => {
                BtnDelete_Click(s, e);
            });
        }

        private void LoadCategories()
        {
            try
            {
                tvCategories.Nodes.Clear();

                var categories = _categoryService.GetAllCategories();

                // Tạo từ điển để theo dõi các node
                var nodeDict = new System.Collections.Generic.Dictionary<int, TreeNode>();

                // Thêm các danh mục gốc (không có danh mục cha)
                foreach (var category in categories)
                {
                    if (!category.ParentCategoryID.HasValue)
                    {
                        var node = new TreeNode(category.CategoryName);
                        node.Tag = category;
                        node.ImageKey = "folder";
                        node.SelectedImageKey = "folder_open";

                        tvCategories.Nodes.Add(node);
                        nodeDict.Add(category.CategoryID, node);
                    }
                }

                // Thêm các danh mục con
                foreach (var category in categories)
                {
                    if (category.ParentCategoryID.HasValue && nodeDict.ContainsKey(category.ParentCategoryID.Value))
                    {
                        var parentNode = nodeDict[category.ParentCategoryID.Value];

                        var node = new TreeNode(category.CategoryName);
                        node.Tag = category;
                        node.ImageKey = "folder";
                        node.SelectedImageKey = "folder_open";

                        parentNode.Nodes.Add(node);
                        nodeDict.Add(category.CategoryID, node);
                    }
                }

                // Mở rộng tất cả các node
                tvCategories.ExpandAll();

                // Cập nhật label tổng số
                lblTotalCategories.Text = $"Tổng số: {categories.Count} danh mục";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new CategoryDetailForm(null, _categoryService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                }
            }
        }

        private void AddSubCategory()
        {
            if (tvCategories.SelectedNode == null) return;

            var parentCategory = tvCategories.SelectedNode.Tag as CategoryDTO;
            if (parentCategory == null) return;

            using (var form = new CategoryDetailForm(null, _categoryService, parentCategory.CategoryID))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (tvCategories.SelectedNode == null) return;

            var category = tvCategories.SelectedNode.Tag as CategoryDTO;
            if (category == null) return;

            using (var form = new CategoryDetailForm(category, _categoryService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (tvCategories.SelectedNode == null) return;

            var category = tvCategories.SelectedNode.Tag as CategoryDTO;
            if (category == null) return;

            // Kiểm tra nếu node có nút con
            if (tvCategories.SelectedNode.Nodes.Count > 0)
            {
                MessageBox.Show("Không thể xóa danh mục này vì nó chứa các danh mục con.",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa danh mục '{category.CategoryName}'?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var result = _categoryService.DeleteCategory(category.CategoryID);

                    if (result)
                    {
                        MessageBox.Show("Xóa danh mục thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa danh mục này vì nó đang được sử dụng.",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa danh mục: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void TvCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var category = e.Node.Tag as CategoryDTO;

            if (category != null)
            {
                // Hiển thị thông tin chi tiết
                txtCategoryName.Text = category.CategoryName;
                txtDescription.Text = category.Description;
                txtParentCategory.Text = category.ParentCategoryName;
                txtDisplayOrder.Text = category.DisplayOrder.ToString();
                chkIsActive.Checked = category.IsActive;

                // Hiển thị số sản phẩm thuộc danh mục
                var productCount = _categoryService.GetProductCountByCategory(category.CategoryID);
                lblProductCount.Text = $"Số sản phẩm: {productCount}";

                // Kích hoạt các nút chức năng
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                ClearCategoryDetails();
            }
        }

        private void ClearCategoryDetails()
        {
            txtCategoryName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtParentCategory.Text = string.Empty;
            txtDisplayOrder.Text = string.Empty;
            chkIsActive.Checked = false;
            lblProductCount.Text = "Số sản phẩm: 0";

            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Load danh mục
            LoadCategories();

            // Vô hiệu hóa các nút chức năng ban đầu
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
        }
    }
}