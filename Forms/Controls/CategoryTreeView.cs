// File: Controls/CategoryTreeView.cs (User Control - Windows Forms)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using QuanLyCuaHangTienLoi.Models.DTO;

namespace QuanLyCuaHangTienLoi.Controls
{
    public partial class CategoryTreeView : UserControl
    {
        private TreeView _treeView;
        private List<CategoryDTO> _categories;
        private bool _allowDragDrop = false;

        public event EventHandler<CategoryEventArgs> CategorySelected;
        public event EventHandler<CategoryEventArgs> CategoryMoved;

        public CategoryTreeView()
        {
            InitializeComponent();

            // Khởi tạo TreeView
            _treeView = new TreeView();
            _treeView.Dock = DockStyle.Fill;
            _treeView.ShowNodeToolTips = true;
            _treeView.HideSelection = false;
            _treeView.BorderStyle = BorderStyle.None;
            _treeView.BackColor = Color.FromArgb(255, 240, 245);

            // Thêm ImageList
            _treeView.ImageList = new ImageList();
            _treeView.ImageList.Images.Add("folder", Properties.Resources.folder_icon);
            _treeView.ImageList.Images.Add("folder_open", Properties.Resources.folder_open_icon);

            // Đăng ký sự kiện
            _treeView.AfterSelect += TreeView_AfterSelect;
            _treeView.ItemDrag += TreeView_ItemDrag;
            _treeView.DragEnter += TreeView_DragEnter;
            _treeView.DragOver += TreeView_DragOver;
            _treeView.DragDrop += TreeView_DragDrop;

            this.Controls.Add(_treeView);
        }

        public bool AllowDragDrop
        {
            get { return _allowDragDrop; }
            set { _allowDragDrop = value; }
        }

        public CategoryDTO SelectedCategory
        {
            get
            {
                if (_treeView.SelectedNode != null)
                {
                    return _treeView.SelectedNode.Tag as CategoryDTO;
                }

                return null;
            }
        }

        public void LoadCategories(List<CategoryDTO> categories)
        {
            _categories = categories;
            _treeView.Nodes.Clear();

            // Tạo từ điển để theo dõi các node
            var nodeDict = new Dictionary<int, TreeNode>();

            // Thêm các danh mục gốc (không có danh mục cha)
            foreach (var category in categories)
            {
                if (!category.ParentCategoryID.HasValue)
                {
                    var node = new TreeNode(category.CategoryName);
                    node.Tag = category;
                    node.ImageKey = "folder";
                    node.SelectedImageKey = "folder_open";
                    node.ToolTipText = !string.IsNullOrEmpty(category.Description)
                        ? category.Description
                        : category.CategoryName;

                    _treeView.Nodes.Add(node);
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
                    node.ToolTipText = !string.IsNullOrEmpty(category.Description)
                        ? category.Description
                        : category.CategoryName;

                    parentNode.Nodes.Add(node);
                    nodeDict.Add(category.CategoryID, node);
                }
            }

            // Sắp xếp theo DisplayOrder
            SortTreeNodesByDisplayOrder(_treeView.Nodes);

            // Mở rộng tất cả các node
            _treeView.ExpandAll();
        }

        private void SortTreeNodesByDisplayOrder(TreeNodeCollection nodes)
        {
            // Sắp xếp mỗi cấp danh mục
            List<TreeNode> nodeList = new List<TreeNode>();
            foreach (TreeNode node in nodes)
            {
                nodeList.Add(node);
                SortTreeNodesByDisplayOrder(node.Nodes);
            }

            nodeList.Sort((x, y) => {
                var categoryX = x.Tag as CategoryDTO;
                var categoryY = y.Tag as CategoryDTO;

                if (categoryX != null && categoryY != null)
                {
                    return categoryX.DisplayOrder.CompareTo(categoryY.DisplayOrder);
                }

                return 0;
            });

            nodes.Clear();
            foreach (TreeNode node in nodeList)
            {
                nodes.Add(node);
            }
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            OnCategorySelected(e.Node.Tag as CategoryDTO);
        }

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (_allowDragDrop)
            {
                _treeView.DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (_allowDragDrop)
            {
                e.Effect = e.AllowedEffect;
            }
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            if (!_allowDragDrop) return;

            // Lấy vị trí hiện tại
            Point pt = _treeView.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = _treeView.GetNodeAt(pt);

            // Không cho phép kéo thả vào chính node đang kéo hoặc con của nó
            TreeNode draggedNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (draggedNode != null && (targetNode == draggedNode || IsDescendantOf(targetNode, draggedNode)))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = e.AllowedEffect;
            _treeView.SelectedNode = targetNode;
        }

        private void TreeView_DragDrop(object sender, DragEventArgs e)
        {
            if (!_allowDragDrop) return;

            // Lấy node đang kéo và node đích
            TreeNode draggedNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (draggedNode == null) return;

            Point pt = _treeView.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = _treeView.GetNodeAt(pt);

            // Không cho phép kéo thả vào chính node đang kéo hoặc con của nó
            if (targetNode == draggedNode || IsDescendantOf(targetNode, draggedNode))
            {
                return;
            }

            // Lấy danh mục tương ứng
            var draggedCategory = draggedNode.Tag as CategoryDTO;
            var targetCategory = targetNode?.Tag as CategoryDTO;

            // Tạo bản sao của node đang kéo
            TreeNode newNode = (TreeNode)draggedNode.Clone();

            // Thêm node vào node đích hoặc root nếu không có node đích
            if (targetNode != null)
            {
                targetNode.Nodes.Add(newNode);
                targetNode.Expand();
            }
            else
            {
                _treeView.Nodes.Add(newNode);
            }

            // Xóa node cũ
            draggedNode.Remove();

            // Thông báo sự kiện di chuyển danh mục
            OnCategoryMoved(draggedCategory, targetCategory);
        }

        // Kiểm tra xem một node có phải là con (hoặc cháu) của một node khác không
        private bool IsDescendantOf(TreeNode node, TreeNode potentialAncestor)
        {
            if (node == null || potentialAncestor == null)
            {
                return false;
            }

            if (node.Parent == potentialAncestor)
            {
                return true;
            }

            return IsDescendantOf(node.Parent, potentialAncestor);
        }

        protected virtual void OnCategorySelected(CategoryDTO category)
        {
            CategorySelected?.Invoke(this, new CategoryEventArgs(category));
        }

        protected virtual void OnCategoryMoved(CategoryDTO category, CategoryDTO newParent)
        {
            CategoryMoved?.Invoke(this, new CategoryMovedEventArgs(category, newParent));
        }

        public class CategoryEventArgs : EventArgs
        {
            public CategoryDTO Category { get; private set; }

            public CategoryEventArgs(CategoryDTO category)
            {
                Category = category;
            }
        }

        public class CategoryMovedEventArgs : CategoryEventArgs
        {
            public CategoryDTO NewParent { get; private set; }

            public CategoryMovedEventArgs(CategoryDTO category, CategoryDTO newParent)
                : base(category)
            {
                NewParent = newParent;
            }
        }
    }
}