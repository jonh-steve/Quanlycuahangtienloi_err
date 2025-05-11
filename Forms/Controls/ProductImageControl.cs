// File: Controls/ProductImageControl.cs (User Control - Windows Forms)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Controls
{
    public partial class ProductImageControl : UserControl
    {
        private List<ProductImage> _images = new List<ProductImage>();
        private List<string> _tempImagePaths = new List<string>();

        public event EventHandler<ProductImageEventArgs> ImageAdded;
        public event EventHandler<ProductImageEventArgs> ImageDeleted;
        public event EventHandler<ProductImageEventArgs> DefaultImageChanged;

        public ProductImageControl()
        {
            InitializeComponent();

            // Thiết lập giao diện màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 240, 245);
            btnAddImage.BackColor = Color.FromArgb(255, 182, 193);
            btnDeleteImage.BackColor = Color.FromArgb(255, 182, 193);
            btnSetDefaultImage.BackColor = Color.FromArgb(255, 182, 193);

            // Khởi tạo ListView
            lvImages.View = View.LargeIcon;
            lvImages.LargeImageList = new ImageList();
            lvImages.LargeImageList.ImageSize = new Size(64, 64);

            // Thiết lập các sự kiện
            btnAddImage.Click += BtnAddImage_Click;
            btnDeleteImage.Click += BtnDeleteImage_Click;
            btnSetDefaultImage.Click += BtnSetDefaultImage_Click;
            lvImages.SelectedIndexChanged += LvImages_SelectedIndexChanged;
        }

        #region Public Properties and Methods

        public List<ProductImage> Images
        {
            get { return _images; }
            set
            {
                _images = value ?? new List<ProductImage>();
                UpdateImageList();
            }
        }

        public List<string> TempImagePaths
        {
            get { return _tempImagePaths; }
        }

        public void ClearImages()
        {
            _images.Clear();
            _tempImagePaths.Clear();
            UpdateImageList();
        }

        #endregion

        #region Private Methods

        private void UpdateImageList()
        {
            lvImages.Items.Clear();
            lvImages.LargeImageList.Images.Clear();

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

                // Thêm hình ảnh vào ImageList
                string key = image.ImageID.ToString();
                lvImages.LargeImageList.Images.Add(key, thumbnail);

                // Tạo ListViewItem mới
                var item = new ListViewItem
                {
                    Text = image.IsDefault ? "(Mặc định)" : "",
                    ImageKey = key,
                    Tag = image
                };

                lvImages.Items.Add(item);
            }

            // Cập nhật trạng thái các nút
            UpdateButtonState();

            // Cập nhật hình ảnh được chọn
            if (lvImages.Items.Count > 0)
            {
                lvImages.Items[0].Selected = true;
            }
            else
            {
                UpdateSelectedImage();
            }
        }

        private void UpdateButtonState()
        {
            btnDeleteImage.Enabled = lvImages.SelectedItems.Count > 0;
            btnSetDefaultImage.Enabled = lvImages.SelectedItems.Count > 0;
        }

        private void UpdateSelectedImage()
        {
            // Hiển thị hình ảnh được chọn
            if (lvImages.SelectedItems.Count > 0)
            {
                var selectedImage = lvImages.SelectedItems[0].Tag as ProductImage;
                if (selectedImage != null && File.Exists(selectedImage.ImagePath))
                {
                    try
                    {
                        picSelectedImage.Image = Image.FromFile(selectedImage.ImagePath);
                    }
                    catch
                    {
                        picSelectedImage.Image = Properties.Resources.no_image;
                    }
                }
                else
                {
                    picSelectedImage.Image = Properties.Resources.no_image;
                }
            }
            else
            {
                picSelectedImage.Image = Properties.Resources.no_image;
            }
        }

        #endregion

        #region Event Handlers

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
                            // Kiểm tra nếu file hình ảnh hợp lệ
                            using (var img = Image.FromFile(fileName))
                            {
                                // Tạo đối tượng ProductImage mới
                                var image = new ProductImage
                                {
                                    ImageID = -(_images.Count + 1), // ID tạm thời âm
                                    ProductID = 0, // Sẽ được cập nhật sau
                                    ImagePath = fileName,
                                    DisplayOrder = _images.Count,
                                    IsDefault = _images.Count == 0, // Mặc định nếu là hình đầu tiên
                                    CreatedDate = DateTime.Now
                                };

                                // Thêm vào danh sách
                                _images.Add(image);
                                _tempImagePaths.Add(fileName);

                                // Kích hoạt sự kiện
                                OnImageAdded(image);
                            }
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
                    // Xóa khỏi danh sách
                    _images.Remove(selectedImage);

                    // Nếu là hình tạm thời, xóa khỏi danh sách tạm
                    if (selectedImage.ImageID < 0)
                    {
                        _tempImagePaths.Remove(selectedImage.ImagePath);
                    }

                    // Nếu xóa hình mặc định, đặt hình đầu tiên làm mặc định
                    if (selectedImage.IsDefault && _images.Count > 0)
                    {
                        _images[0].IsDefault = true;
                        OnDefaultImageChanged(_images[0]);
                    }

                    // Kích hoạt sự kiện
                    OnImageDeleted(selectedImage);

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

            // Không thay đổi nếu đã là mặc định
            if (selectedImage.IsDefault) return;

            try
            {
                // Đặt hình ảnh khác không còn là mặc định
                foreach (var image in _images)
                {
                    image.IsDefault = (image == selectedImage);
                }

                // Kích hoạt sự kiện
                OnDefaultImageChanged(selectedImage);

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
            UpdateSelectedImage();
        }

        #endregion

        #region Events

        protected virtual void OnImageAdded(ProductImage image)
        {
            ImageAdded?.Invoke(this, new ProductImageEventArgs(image));
        }

        protected virtual void OnImageDeleted(ProductImage image)
        {
            ImageDeleted?.Invoke(this, new ProductImageEventArgs(image));
        }

        protected virtual void OnDefaultImageChanged(ProductImage image)
        {
            DefaultImageChanged?.Invoke(this, new ProductImageEventArgs(image));
        }

        #endregion

        #region Event Arguments

        public class ProductImageEventArgs : EventArgs
        {
            public ProductImage Image { get; private set; }

            public ProductImageEventArgs(ProductImage image)
            {
                Image = image;
            }
        }

        #endregion
    }
}