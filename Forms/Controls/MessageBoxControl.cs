// Mã gợi ý cho MessageBoxControl.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCuaHangTienLoi.Controls
{
    public partial class MessageBoxControl : UserControl
    {
        // Màu hồng dễ thương
        private Color headerColor = Color.FromArgb(255, 182, 193); // Light Pink
        private Color bodyColor = Color.FromArgb(255, 240, 245); // Lavender Blush
        private Color buttonColor = Color.FromArgb(255, 105, 180); // Hot Pink

        private Label lblTitle;
        private Label lblMessage;
        private Button btnOK;
        private Button btnCancel;

        public event EventHandler OKClicked;
        public event EventHandler CancelClicked;

        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }

        public MessageBoxControl()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Tạo label tiêu đề
            lblTitle = new Label();
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 40;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.BackColor = headerColor;
            lblTitle.ForeColor = Color.White;

            // Tạo label nội dung
            lblMessage = new Label();
            lblMessage.Dock = DockStyle.Fill;
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            lblMessage.Font = new Font("Segoe UI", 10F);
            lblMessage.BackColor = bodyColor;

            // Tạo panel nút
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 50;
            buttonPanel.BackColor = bodyColor;

            // Tạo nút OK
            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Size = new Size(80, 30);
            btnOK.Location = new Point((buttonPanel.Width / 2) - 90, 10);
            btnOK.BackColor = buttonColor;
            btnOK.ForeColor = Color.White;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += (s, e) => OKClicked?.Invoke(this, EventArgs.Empty);

            // Tạo nút Hủy
            btnCancel = new Button();
            btnCancel.Text = "Hủy";
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point((buttonPanel.Width / 2) + 10, 10);
            btnCancel.BackColor = Color.Gray;
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);

            // Thêm các controls vào panel
            buttonPanel.Controls.Add(btnOK);
            buttonPanel.Controls.Add(btnCancel);

            // Thêm tất cả vào control chính
            this.Controls.Add(lblMessage);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(lblTitle);

            // Set vị trí các nút khi control được resize
            this.Resize += (s, e) => {
                btnOK.Location = new Point((buttonPanel.Width / 2) - 90, 10);
                btnCancel.Location = new Point((buttonPanel.Width / 2) + 10, 10);
            };

            this.Size = new Size(350, 200);
        }

        // Hiển thị chỉ nút OK
        public void ShowOKOnly()
        {
            btnCancel.Visible = false;
            btnOK.Location = new Point((btnOK.Parent.Width - btnOK.Width) / 2, 10);
        }
    }
}