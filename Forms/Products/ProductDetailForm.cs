// File: Forms/Products/ProductDetailForm.cs (Form - Windows Forms)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Services;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Forms.Products
{
    public partial class ProductDetailForm : Form
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ProductDTO _product;
        private readonly bool _isEditMode;
        private List<ProductImage> _images = new List<ProductImage>();
        private List<string> _tempImagePaths = new List<string>(); // Đường dẫn tạm thời cho hình ảnh mới

        public ProductDetailForm(ProductDTO product, IProductService productService, ICategoryService categoryService)
        {
            InitializeComponent();
            _productService = productService;
            _categoryService = categoryService;
            _product = product;
            _isEditMode = product != null;

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 240, 245);
            btnSave.BackColor = Color.FromArgb(255, 182, 193);
            btnCancel.BackColor = Color.FromArgb(255, 182, 193);
            btnAddImage.BackColor = Color.FromArgb(255, 182, 193);
            btnDeleteImage.BackColor = Color.FromArgb(255, 182, 193);
            btnSetDefaultImage.BackColor = Color.FromArgb(255, 182, 193);

            // Thiết lập tiêu đề form
            this.Text = _isEditMode ? "Chỉnh sửa sản phẩm" : "Thêm sản phẩm mới";

            // Thiết lập các điều khiển
            txtProductCode.ReadOnly = _isEditMode; // Không cho phép sửa mã sản phẩm nếu đang chỉnh sửa

            // Thiết lập các sự kiện
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            btnAddImage.Click += BtnAddImage_Click;
            btnDeleteImage.Click += BtnDeleteImage_Click;
            btnSetDefaultImage.Click += BtnSetDefaultImage_Click;

            // Sự kiện validate nhập liệu
            txtProductCode.TextChanged += TxtProductCode_TextChanged;
            txtBarcode.TextChanged += TxtBarcode_TextChanged;
            txtCostPrice.TextChanged += TxtCostPrice_TextChanged;
            txtSellPrice.TextChanged += TxtSellPrice_TextChanged;
            txtMinimumStock.TextChanged += TxtMinimumStock_TextChanged;

            // Sự kiện chọn hình ảnh
            lvImages.SelectedIndexChanged += LvImages_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Tải danh sách danh mục
            LoadCategories();

            // Nạp dữ liệu nếu ở chế độ chỉnh sửa
            if (_isEditMode)
            {
                LoadProductData();
            }
            else
            {
                // Giá trị mặc định cho sản phẩm mới
                chkIsActive.Checked = true;
                txtMinimumStock.Text = "0";
            }

            // Cập nhật trạng thái các nút
            UpdateButtonState();
        }

        private void LoadCategories()
        {
            try
            {
                // Lấy danh sách danh mục
                var categories = _categoryService.GetAllCategories();

                cboCategory.DisplayMember = "Text";
                cboCategory.ValueMember = "Value";

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

                // Chọn danh mục đầu tiên nếu không có sản phẩm
                if (cboCategory.Items.Count > 0 && !_isEditMode)
                {
                    cboCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách danh mục: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductData()
        {
            // Thiết lập các trường dữ liệu
            txtProductCode.Text = _product.ProductCode;
            txtBarcode.Text = _product.Barcode;
            txtProductName.Text = _product.ProductName;
            txtDescription.Text = _product.Description;
            txtCostPrice.Text = _product.CostPrice.ToString("N0");
            txtSellPrice.Text = _product.SellPrice.ToString("N0");
            txtUnit.Text = _product.Unit;
            txtMinimumStock.Text = _product.MinimumStock.ToString();
            chkIsActive.Checked = _product.IsActive;

            // Chọn danh mục
            for (int i = 0; i < cboCategory.Items.Count; i++)
            {
                var item = cboCategory.Items[i] as ComboBoxItem;
                if (item != null && item.Value == _product.CategoryID)
                {
                    cboCategory.SelectedIndex = i;
                    break;
                }
            }

            // Tải hình ảnh sản phẩm
            if (_product.Images != null && _product.Images.Count > 0)
            {
                _images = new List<ProductImage>(_product.Images);
                UpdateImageList();
            }
        }

        private void UpdateImageList()
        {
            lvImages.Items.Clear();

            foreach (var image in _images)
            {
                // Tạo thumbnail cho hình ảnh
                Image thumbnail = null;
                try
                {
                    if (File.Exists(image.ImagePath))
                    {
                        using (var img = Image.FromFile(image.ImagePath))
                        {
                            thumbnail = img.GetThumbnailImage(64, 64, null, IntPtr.Zero);
                        }
                    }
                }
                catch
                {
                    // Nếu không tạo được thumbnail, sử dụng hình mặc định
                    thumbnail = Properties.Resources.no_image;
                }

                // Thêm hình ảnh vào ImageList nếu chưa có
                if (!imgThumbnails.Images.ContainsKey(image.ImageID.ToString()))
                {
                    imgThumbnails.Images.Add(image.ImageID.ToString(), thumbnail);
                }

                // Tạo ListViewItem mới
                var item = new ListViewItem
                {
                    Text = image.IsDefault ? "(Mặc định)" : "",
                    ImageKey = image.ImageID.ToString(),
                    Tag = image
                };

                lvImages.Items.Add(item);
            }

            // Cập nhật trạng thái các nút
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            btnDeleteImage.Enabled = lvImages.SelectedItems.Count > 0;
            btnSetDefaultImage.Enabled = lvImages.SelectedItems.Count > 0;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("Vui lòng nhập mã sản phẩm!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }

                if (cboCategory.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn danh mục sản phẩm!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboCategory.Focus();
                    return;
                }

                // Kiểm tra các giá trị số
                decimal costPrice = 0;
                if (!decimal.TryParse(txtCostPrice.Text.Replace(",", ""), out costPrice) || costPrice < 0)
                {
                    MessageBox.Show("Giá nhập không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCostPrice.Focus();
                    return;
                }

                decimal sellPrice = 0;
                if (!decimal.TryParse(txtSellPrice.Text.Replace(",", ""), out sellPrice) || sellPrice < 0)
                {
                    MessageBox.Show("Giá bán không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSellPrice.Focus();
                    return;
                }

                int minimumStock = 0;
                if (!int.TryParse(txtMinimumStock.Text, out minimumStock) || minimumStock < 0)
                {
                    MessageBox.Show("Tồn kho tối thiểu không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMinimumStock.Focus();
                    return;
                }

                // Tạo đối tượng ProductDTO
                var productDTO = new ProductDTO
                {
                    ProductCode = txtProductCode.Text,
                    Barcode = txtBarcode.Text,
                    ProductName = txtProductName.Text,
                    CategoryID = (cboCategory.SelectedItem as ComboBoxItem).Value.Value,
                    Description = txtDescription.Text,
                    CostPrice = costPrice,
                    SellPrice = sellPrice,
                    Unit = txtUnit.Text,
                    MinimumStock = minimumStock,
                    IsActive = chkIsActive.Checked
                };

                // Nếu đang chỉnh sửa, thêm ID vào
                if (_isEditMode)
                {
                    productDTO.ProductID = _product.ProductID;
                    productDTO.CurrentStock = _product.CurrentStock;
                }

                // Lưu sản phẩm
                int productID;
                if (_isEditMode)
                {
                    _productService.UpdateProduct(productDTO, 1); // TODO: Lấy ID người dùng hiện tại
                    productID = _product.ProductID;
                    MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    productID = _productService.CreateProduct(productDTO, 1); // TODO: Lấy ID người dùng hiện tại
                    MessageBox.Show("Thêm sản phẩm mới thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // TODO: Xử lý lưu hình ảnh mới nếu có

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu sản phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // TODO: Xóa các file hình ảnh tạm nếu có

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            // Tạo và hiển thị OpenFileDialog
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn hình ảnh sản phẩm";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in openFileDialog.FileNames)
                    {
                        try
                        {
                            // TODO: Xử lý lưu hình ảnh vào thư mục của ứng dụng
                            // Hiện tại chỉ sử dụng đường dẫn gốc

                            // Tạo đối tượng ProductImage mới
                            var image = new ProductImage
                            {
                                ImageID = -(_images.Count + 1), // ID tạm thời âm
                                ProductID = _isEditMode ? _product.ProductID : 0,
                                ImagePath = fileName,
                                DisplayOrder = _images.Count,
                                IsDefault = _images.Count == 0, // Mặc định nếu là hình đầu tiên
                                CreatedDate = DateTime.Now
                            };

                            // Thêm vào danh sách
                            _images.Add(image);
                            _tempImagePaths.Add(fileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi thêm hình ảnh: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Cập nhật danh sách hình ảnh
                    UpdateImageList();
                }
            }
        }

        private void BtnDeleteImage_Click(object sender, EventArgs e)
        {
            if (lvImages.SelectedItems.Count == 0) return;

            var selectedImage = lvImages.SelectedItems[0].Tag as ProductImage;
            if (selectedImage == null) return;

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa hình ảnh này?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // Nếu là hình ảnh đã lưu trong CSDL
                    if (selectedImage.ImageID > 0 && _isEditMode)
                    {
                        _productService.DeleteProductImage(selectedImage.ImageID);
                    }

                    // Xóa khỏi danh sách
                    _images.Remove(selectedImage);

                    // Nếu xóa hình mặc định, đặt hình đầu tiên làm mặc định
                    if (selectedImage.IsDefault && _images.Count > 0)
                    {
                        _images[0].IsDefault = true;

                        // Cập nhật trong CSDL nếu cần
                        if (_isEditMode && _images[0].ImageID > 0)
                        {
                            _productService.SetDefaultImage(_product.ProductID, _images[0].ImageID);
                        }
                    }

                    // Cập nhật danh sách hình ảnh
                    UpdateImageList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa hình ảnh: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSetDefaultImage_Click(object sender, EventArgs e)
        {
            if (lvImages.SelectedItems.Count == 0) return;

            var selectedImage = lvImages.SelectedItems[0].Tag as ProductImage;
            if (selectedImage == null) return;

            try
            {
                // Đặt hình ảnh khác không còn là mặc định
                foreach (var image in _images)
                {
                    image.IsDefault = (image == selectedImage);
                }

                // Cập nhật trong CSDL nếu cần
                if (_isEditMode && selectedImage.ImageID > 0)
                {
                    _productService.SetDefaultImage(_product.ProductID, selectedImage.ImageID);
                }

                // Cập nhật danh sách hình ảnh
                UpdateImageList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt hình ảnh mặc định: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LvImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonState();

            // Hiển thị hình ảnh được chọn
            if (lvImages.SelectedItems.Count > 0)
            {
                var selectedImage = lvImages.SelectedItems[0].Tag as ProductImage;
                if (selectedImage != null && File.Exists(selectedImage.ImagePath))
                {
                    try
                    {
                        picProductImage.Image = Image.FromFile(selectedImage.ImagePath);
                    }
                    catch
                    {
                        picProductImage.Image = Properties.Resources.no_image;
                    }
                }
                else
                {
                    picProductImage.Image = Properties.Resources.no_image;
                }
            }
            else
            {
                picProductImage.Image = Properties.Resources.no_image;
            }
        }

        private void TxtProductCode_TextChanged(object sender, EventArgs e)
        {
            // Kiểm tra mã sản phẩm hợp lệ
            if (!string.IsNullOrEmpty(txtProductCode.Text) && !Validators.IsValidProductCode(txtProductCode.Text))
            {
                errorProvider.SetError(txtProductCode, "Mã sản phẩm không hợp lệ! Chỉ được chứa chữ cái, số và dấu gạch dưới, tối thiểu 3 ký tự.");
            }
            else
            {
                errorProvider.SetError(txtProductCode, "");
            }
        }

        private void TxtBarcode_TextChanged(object sender, EventArgs e)
        {
            // Kiểm tra barcode hợp lệ
            if (!string.IsNullOrEmpty(txtBarcode.Text) && !Validators.IsValidBarcode(txtBarcode.Text))
            {
                errorProvider.SetError(txtBarcode, "Mã vạch không hợp lệ!");
            }
            else
            {
                errorProvider.SetError(txtBarcode, "");
            }
        }

        private void TxtCostPrice_TextChanged(object sender, EventArgs e)
        {
            // Định dạng giá nhập
            FormatCurrencyTextBox(txtCostPrice);
        }

        private void TxtSellPrice_TextChanged(object sender, EventArgs e)
        {
            // Định dạng giá bán
            FormatCurrencyTextBox(txtSellPrice);
        }

        private void TxtMinimumStock_TextChanged(object sender, EventArgs e)
        {
            // Kiểm tra tồn kho tối thiểu hợp lệ
            int value;
            if (!int.TryParse(txtMinimumStock.Text, out value) || value < 0)
            {
                errorProvider.SetError(txtMinimumStock, "Tồn kho tối thiểu phải là số nguyên không âm!");
            }
            else
            {
                errorProvider.SetError(txtMinimumStock, "");
            }
        }

        private void FormatCurrencyTextBox(TextBox textBox)
        {
            // Lưu vị trí con trỏ
            int cursorPosition = textBox.SelectionStart;
            int textLength = textBox.Text.Length;

            // Xóa các ký tự không phải số
            string text = new string(Array.FindAll(textBox.Text.ToCharArray(), c => Char.IsDigit(c)));

            // Chuyển đổi thành số
            decimal value;
            if (decimal.TryParse(text, out value))
            {
                // Định dạng số
                textBox.Text = value.ToString("N0");

                // Đặt lại vị trí con trỏ
                int newLength = textBox.Text.Length;
                int newPosition = cursorPosition + (newLength - textLength);
                if (newPosition < 0) newPosition = 0;
                if (newPosition > newLength) newPosition = newLength;
                textBox.SelectionStart = newPosition;
            }
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