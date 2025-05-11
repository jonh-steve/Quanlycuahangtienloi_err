using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCuaHangTienLoi.Controls
{
    public partial class DateRangeSelectorControl : UserControl
    {
        public event EventHandler DateRangeChanged;

        // Thuộc tính ngày bắt đầu
        private DateTime _startDate = DateTime.Today.AddDays(-30);
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                dtpStartDate.Value = _startDate;
                UpdateCustomRadioButton();
            }
        }

        // Thuộc tính ngày kết thúc
        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                dtpEndDate.Value = _endDate;
                UpdateCustomRadioButton();
            }
        }

        public DateRangeSelectorControl()
        {
            InitializeComponent();

            // Thiết lập màu hồng dễ thương
            this.BackColor = Color.FromArgb(255, 230, 240);
            btnApply.BackColor = Color.FromArgb(252, 157, 192);

            // Thiết lập sự kiện
            this.Load += DateRangeSelectorControl_Load;
            radToday.CheckedChanged += RadioButton_CheckedChanged;
            radYesterday.CheckedChanged += RadioButton_CheckedChanged;
            radLast7Days.CheckedChanged += RadioButton_CheckedChanged;
            radLast30Days.CheckedChanged += RadioButton_CheckedChanged;
            radThisMonth.CheckedChanged += RadioButton_CheckedChanged;
            radLastMonth.CheckedChanged += RadioButton_CheckedChanged;
            radCustom.CheckedChanged += RadioButton_CheckedChanged;
            btnApply.Click += BtnApply_Click;

            // Thiết lập kích thước mặc định
            this.Size = new Size(400, 140);
        }

        private void DateRangeSelectorControl_Load(object sender, EventArgs e)
        {
            // Thiết lập giá trị mặc định
            dtpStartDate.Value = _startDate;
            dtpEndDate.Value = _endDate;

            // Chọn "30 ngày qua" làm mặc định
            radLast30Days.Checked = true;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton && ((RadioButton)sender).Checked)
            {
                UpdateDateRange((RadioButton)sender);
            }
        }

        private void UpdateDateRange(RadioButton radioButton)
        {
            // Vô hiệu hóa sự kiện DateRangeChanged tạm thời
            DateRangeChanged = null;

            switch (radioButton.Name)
            {
                case "radToday":
                    _startDate = DateTime.Today;
                    _endDate = DateTime.Today;
                    break;

                case "radYesterday":
                    _startDate = DateTime.Today.AddDays(-1);
                    _endDate = DateTime.Today.AddDays(-1);
                    break;

                case "radLast7Days":
                    _startDate = DateTime.Today.AddDays(-6);
                    _endDate = DateTime.Today;
                    break;

                case "radLast30Days":
                    _startDate = DateTime.Today.AddDays(-29);
                    _endDate = DateTime.Today;
                    break;

                case "radThisMonth":
                    _startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    _endDate = DateTime.Today;
                    break;

                case "radLastMonth":
                    var firstDayOfThisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    _startDate = firstDayOfThisMonth.AddMonths(-1);
                    _endDate = firstDayOfThisMonth.AddDays(-1);
                    break;

                case "radCustom":
                    // Lấy giá trị từ DateTimePicker
                    _startDate = dtpStartDate.Value;
                    _endDate = dtpEndDate.Value;
                    break;
            }

            // Cập nhật giá trị DateTimePicker
            dtpStartDate.Value = _startDate;
            dtpEndDate.Value = _endDate;

            // Bật/tắt DateTimePicker dựa vào việc có chọn tùy chỉnh hay không
            dtpStartDate.Enabled = radioButton.Name == "radCustom";
            dtpEndDate.Enabled = radioButton.Name == "radCustom";

            // Phục hồi sự kiện DateRangeChanged và kích hoạt nó
            DateRangeChanged = OnDateRangeChanged;
            OnDateRangeChanged(this, EventArgs.Empty);
        }

        private void UpdateCustomRadioButton()
        {
            // Vô hiệu hóa sự kiện DateRangeChanged tạm thời
            var tempHandler = DateRangeChanged;
            DateRangeChanged = null;

            // Chọn nút "Tùy chỉnh" khi giá trị ngày thay đổi trực tiếp
            radCustom.Checked = true;

            // Bật DateTimePicker
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;

            // Phục hồi sự kiện DateRangeChanged
            DateRangeChanged = tempHandler;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (radCustom.Checked)
            {
                _startDate = dtpStartDate.Value;
                _endDate = dtpEndDate.Value;

                if (_startDate > _endDate)
                {
                    MessageBox.Show("Ngày bắt đầu không thể sau ngày kết thúc", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            OnDateRangeChanged(this, EventArgs.Empty);
        }

        protected virtual void OnDateRangeChanged(object sender, EventArgs e)
        {
            DateRangeChanged?.Invoke(this, e);
        }

        private void InitializeComponent()
        {
            // Code tạo các control cho user control
            this.grpDateRange = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();

            this.radToday = new System.Windows.Forms.RadioButton();
            this.radYesterday = new System.Windows.Forms.RadioButton();
            this.radLast7Days = new System.Windows.Forms.RadioButton();
            this.radLast30Days = new System.Windows.Forms.RadioButton();
            this.radThisMonth = new System.Windows.Forms.RadioButton();
            this.radLastMonth = new System.Windows.Forms.RadioButton();
            this.radCustom = new System.Windows.Forms.RadioButton();

            this.panelCustom = new System.Windows.Forms.Panel();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.btnApply = new System.Windows.Forms.Button();

            // Thiết lập các thuộc tính cho control
            this.grpDateRange.Text = "Khoảng thời gian";
            this.grpDateRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);

            this.radToday.Text = "Hôm nay";
            this.radYesterday.Text = "Hôm qua";
            this.radLast7Days.Text = "7 ngày qua";
            this.radLast30Days.Text = "30 ngày qua";
            this.radThisMonth.Text = "Tháng này";
            this.radLastMonth.Text = "Tháng trước";
            this.radCustom.Text = "Tùy chỉnh";

            this.btnApply.Text = "Áp dụng";
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);

            // Thiết lập bố cục
            // ...
        }
    }
}