// File: Forms/Products/CategoryDetailForm.cs (Form - Windows Forms)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services;

namespace QuanLyCuaHangTienLoi.Forms.Products
{
    public partial class CategoryDetailForm : Form
    {
        private readonly ICategoryService _categoryService;
        private readonly CategoryDTO _category;
        private readonly bool _isEditMode;
        private readonly int? _defaultParentID;
        private List<CategoryDTO> _categories;

        public CategoryDetailForm(CategoryDTO category, ICategoryService categoryService, int? defaultParentID = null)
        {
            InitializeComponent();
            _categoryService = categoryService;
            _category = category;
            _isEditMode = category != null;
            _defaultParentID = defaultParentID;

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = System.Drawing.Color.FromArgb(255, 240, 245);
            btnSave.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);
            btnCancel.BackColor = System.Drawing.Color.FromArgb(255, 182, 193);

            // Thiết lập tiêu đề form
            this.Text = _isEditMode ? "Chỉnh sửa danh mục" : "Thêm danh mục mới";

            // Đăng ký sự kiện
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            nudDisplayOrder.ValueChanged += NudDisplayOrder_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Load danh sách danh mục (không bao gồm danh mục hiện tại nếu đang chỉnh sửa)
            LoadCategories();

            // Nạp dữ liệu nếu ở chế độ chỉnh sửa
            if (_isEditMode)
            {
                txtCategoryName.Text = _category.CategoryName;
                txtDescription.Text = _category.Description;
                nudDisplayOrder.Value = _category.DisplayOrder;
                chkIsActive.Checked = _category.IsActive;

                // Chọn danh mục cha
                if (_category.ParentCategoryID.HasValue)
                {
                    for (int i = 0; i < cboParentCategory.Items.Count; i++)
                    {
                        var item = cboParentCategory.Items[i] as ComboBoxItem;
                        if (item != null && item.Value.HasValue && item.Value.Value == _category.ParentCategoryID.Value)
                        {
                            cboParentCategory.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    cboParentCategory.SelectedIndex = 0; // "Không có"
                }
            }
            else
            {
                // Thiết lập giá trị mặc định cho danh mục mới
                chkIsActive.Checked = true;
                nudDisplayOrder.Value = 0;

                // Nếu được chỉ định danh mục cha mặc định
                if (_defaultParentID.HasValue)
                {
                    for (int i = 0; i < cboParentCategory.Items.Count; i++)
                    {
                        var item = cboParentCategory.Items[i] as ComboBoxItem;
                        if (item != null && item.Value.HasValue && item.Value.Value == _defaultParentID.Value)
                        {
                            cboParentCategory.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    cboParentCategory.SelectedIndex = 0; // "Không có"
                }
            }
        }

        private void LoadCategories()
        {
            try
            {
                // Lấy tất cả danh mục
                _categories = _categoryService.GetAllCategories();

                cboParentCategory.Items.Clear();
                cboParentCategory.DisplayMember = "Text";
                cboParentCategory.ValueMember = "Value";

                // Thêm lựa chọn "Không có"
                cboParentCategory.Items.Add(new ComboBoxItem { Text = "-- Không có --", Value = null });

                // Thêm các danh mục khác
                foreach (var category in _categories)
                {
                    // Không hiển thị danh mục hiện tại (nếu đang chỉnh sửa) và các danh mục con của nó
                    if (_isEditMode &&
                        (category.CategoryID == _category.CategoryID || IsDescendantOf(category.CategoryID, _category.CategoryID)))
                    {
                        continue;
                    }

                    string indent = string.Empty;
                    if (category.Level > 0)
                    {
                        indent = new string(' ', category.Level * 4) + "└─ ";
                    }

                    cboParentCategory.Items.Add(new ComboBoxItem
                    {
                        Text = indent + category.CategoryName,
                        Value = category.CategoryID
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Kiểm tra xem một danh mục có phải là con (hoặc cháu) của một danh mục khác không
        private bool IsDescendantOf(int categoryID, int ancestorID)
        {
            var category = _categories.Find(c => c.CategoryID == categoryID);
            if (category == null || !category.ParentCategoryID.HasValue)
            {
                return false;
            }

            if (category.ParentCategoryID.Value == ancestorID)
            {
                return true;
            }

            return IsDescendantOf(category.ParentCategoryID.Value, ancestorID);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên danh mục!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCategoryName.Focus();
                    return;
                }

                // Lấy danh mục cha được chọn
                int? parentCategoryID = null;
                var selectedItem = cboParentCategory.SelectedItem as ComboBoxItem;
                if (selectedItem != null && selectedItem.Value.HasValue)
                {
                    parentCategoryID = selectedItem.Value.Value;
                }

                // Lưu dữ liệu
                if (_isEditMode)
                {
                    var updatedCategory = new CategoryDTO
                    {
                        CategoryID = _category.CategoryID,
                        CategoryName = txtCategoryName.Text,
                        Description = txtDescription.Text,
                        ParentCategoryID = parentCategoryID,
                        DisplayOrder = (int)nudDisplayOrder.Value,
                        IsActive = chkIsActive.Checked
                    };

                    _categoryService.UpdateCategory(updatedCategory);
                    MessageBox.Show("Cập nhật danh mục thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var newCategory = new CategoryDTO
                    {
                        CategoryName = txtCategoryName.Text,
                        Description = txtDescription.Text,
                        ParentCategoryID = parentCategoryID,
                        DisplayOrder = (int)nudDisplayOrder.Value,
                        IsActive = chkIsActive.Checked
                    };

                    _categoryService.CreateCategory(newCategory);
                    MessageBox.Show("Thêm danh mục mới thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void NudDisplayOrder_ValueChanged(object sender, EventArgs e)
        {
            lblDisplayOrderHint.Text = $"Thứ tự hiển thị: {nudDisplayOrder.Value} (số nhỏ hiển thị trước)";
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