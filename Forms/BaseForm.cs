using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCuaHangTienLoi.Forms
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Set custom style
            this.BackColor = Constants.Colors.White;

            // Add border
            this.Paint += BaseForm_Paint;

            // Add controls for title bar
            InitializeTitleBar();
        }

        private void BaseForm_Paint(object sender, PaintEventArgs e)
        {
            // Draw border
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                Constants.Colors.LightPink, 1, ButtonBorderStyle.Solid,
                Constants.Colors.LightPink, 1, ButtonBorderStyle.Solid,
                Constants.Colors.LightPink, 1, ButtonBorderStyle.Solid,
                Constants.Colors.LightPink, 1, ButtonBorderStyle.Solid);
        }

        private void InitializeTitleBar()
        {
            // Title bar panel
            Panel titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Constants.Colors.PrimaryPink
            };

            // Close button
            Button btnClose = new Button
            {
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                Text = "×",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Dock = DockStyle.Right,
                ForeColor = Constants.Colors.White,
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            // Minimize button
            Button btnMinimize = new Button
            {
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                Text = "_",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Dock = DockStyle.Right,
                ForeColor = Constants.Colors.White,
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            // Form title
            Label lblTitle = new Label
            {
                Text = this.Text,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                ForeColor = Constants.Colors.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Add controls to title bar
            titleBar.Controls.Add(btnClose);
            titleBar.Controls.Add(btnMinimize);
            titleBar.Controls.Add(lblTitle);

            // Enable form dragging
            titleBar.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            this.Controls.Add(titleBar);
        }

        // For form dragging
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        protected void ShowLoading()
        {
            // TODO: Implement loading control
        }

        protected void HideLoading()
        {
            // TODO: Implement loading control
        }

        protected void ShowMessage(string message, string title = "Thông báo", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }
    }
}
