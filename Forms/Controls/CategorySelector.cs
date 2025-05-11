// Mã gợi ý cho CategorySelector.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Controls
{
    public partial class CategorySelector : UserControl
    {
        private ICategoryService _categoryService;
        private TreeView treeView;
        private Label lblTitle;

        // Sự kiện khi chọn danh mục
        public event EventHandler<CategorySelectedEventArgs> CategorySelected;

        // Thuộc tính danh mục đã chọn
        public Category SelectedCategory { get; private set; }

        // Thuộc tính tiêu đề
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        // Thuộc tính cho phép chọn danh mục gốc
        public bool AllowRootSelection { get; set; } = true;

        // Thuộc tính cho phép chọn null
        public bool AllowNullSelection { get; set; } = true;

        public CategorySelector()
        {
            InitializeComponent();
            _categoryService = CategoryService.Instance;
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.treeView = new System.Windows.Forms.TreeView();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.Location = new System.Drawing.Point(3, 25);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(244, 222);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(3, 7);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(66, 15);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Danh mục:";
            // 
            // CategorySelector
            // 
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.treeView);
            this.Name = "CategorySelector";
            this.Size = new System.Drawing.Size(250, 250);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadCategories()
        {
            treeView.Nodes.Clear();

            try
            {
                // Thêm node gốc (Không chọn) nếu AllowNullSelection = true
                if (AllowNullSelection)
                {
                    TreeNode rootNode = new TreeNode("-- Không chọn --");
                    rootNode.Tag = null;
                    treeView.Nodes.Add(rootNode);
                }

                // Lấy danh sách danh mục gốc
                List<Category> rootCategories = _categoryService.GetRootCategories();

                foreach (var category in rootCategories)
                {
                    TreeNode node = new TreeNode(category.CategoryName);
                    node.Tag = category;

                    // Đệ quy thêm các danh mục con
                    AddChildNodes(node, category.CategoryID);

                    treeView.Nodes.Add(node);
                }

                // Mở rộng tất cả các node
                treeView.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddChildNodes(TreeNode parentNode, int parentCategoryId)
        {
            List<Category> childCategories = _categoryService.GetChildCategories(parentCategoryId);

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.CategoryName);
                childNode.Tag = childCategory;

                // Đệ quy thêm các danh mục con của con
                AddChildNodes(childNode, childCategory.CategoryID);

                parentNode.Nodes.Add(childNode);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Không cho phép chọn danh mục gốc nếu AllowRootSelection = false
            if (!AllowRootSelection && e.Node.Level == 0 && e.Node.Tag != null)
            {
                MessageBox.Show("Vui lòng chọn danh mục con!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                treeView.SelectedNode = null;
                return;
            }

            // Lưu danh mục đã chọn
            SelectedCategory = e.Node.Tag as Category;

            // Gọi sự kiện CategorySelected
            CategorySelected?.Invoke(this, new CategorySelectedEventArgs(SelectedCategory));
        }

        // Reload danh mục
        public void ReloadCategories()
        {
            LoadCategories();
        }

        // Chọn danh mục theo ID
        public void SelectCategory(int? categoryId)
        {
            if (categoryId == null)
            {
                // Chọn node "Không chọn" nếu có
                if (AllowNullSelection && treeView.Nodes.Count > 0 && treeView.Nodes[0].Tag == null)
                {
                    treeView.SelectedNode = treeView.Nodes[0];
                }
                return;
            }

            // Tìm node có Tag.CategoryID = categoryId
            SelectNodeByCategory(treeView.Nodes, categoryId.Value);
        }

        private bool SelectNodeByCategory(TreeNodeCollection nodes, int categoryId)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && node.Tag is Category category && category.CategoryID == categoryId)
                {
                    treeView.SelectedNode = node;
                    return true;
                }

                if (node.Nodes.Count > 0)
                {
                    if (SelectNodeByCategory(node.Nodes, categoryId))
                        return true;
                }
            }

            return false;
        }
    }

    // Class cho sự kiện chọn danh mục
    public class CategorySelectedEventArgs : EventArgs
    {
        public Category SelectedCategory { get; private set; }

        public CategorySelectedEventArgs(Category selectedCategory)
        {
            SelectedCategory = selectedCategory;
        }
    }
}