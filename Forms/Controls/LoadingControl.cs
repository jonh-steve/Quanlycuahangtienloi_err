// Mã gợi ý cho LoadingControl.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCuaHangTienLoi.Controls
{
    public partial class LoadingControl : UserControl
    {
        private Timer animationTimer;
        private int currentAngle = 0;

        // Màu hồng dễ thương
        private Color mainColor = Color.FromArgb(255, 105, 180); // Hot Pink

        public LoadingControl()
        {
            InitializeComponent();
            SetupAnimation();
        }

        private void SetupAnimation()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            animationTimer = new Timer();
            animationTimer.Interval = 50;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            currentAngle += 10;
            if (currentAngle >= 360)
                currentAngle = 0;

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int size = Math.Min(this.Width, this.Height) - 10;
            Rectangle rect = new Rectangle((this.Width - size) / 2, (this.Height - size) / 2, size, size);

            using (Pen pen = new Pen(mainColor, 4))
            {
                // Vẽ vòng tròn loading
                g.DrawArc(pen, rect, currentAngle, 270);
            }
        }
    }
}