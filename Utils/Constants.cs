using System.Drawing;
using System.IO;
using System.Windows.Forms;

public static class Constants
{
    // Màu sắc chủ đạo
    public static class Colors
    {
        public static Color PrimaryPink = Color.FromArgb(255, 105, 180);
        public static Color LightPink = Color.FromArgb(255, 182, 193);
        public static Color DarkPink = Color.FromArgb(219, 112, 147);
        public static Color White = Color.White;
        public static Color LightGray = Color.FromArgb(245, 245, 245);
        public static Color DarkGray = Color.FromArgb(169, 169, 169);
    }

    // Các đường dẫn
    public static class Paths
    {
        public static string ImagePath = Path.Combine(Application.StartupPath, "Resources", "Images");
        public static string FontPath = Path.Combine(Application.StartupPath, "Resources", "Fonts");
        public static string LogPath = Path.Combine(Application.StartupPath, AppSettings.Instance.LogPath);
        public static string BackupPath = Path.Combine(Application.StartupPath, AppSettings.Instance.BackupPath);
    }

    // Thông báo hệ thống
    public static class Messages
    {
        public static string ConnectionError = "Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra lại cấu hình!";
        public static string LoginSuccess = "Đăng nhập thành công!";
        public static string LoginFailed = "Tên đăng nhập hoặc mật khẩu không đúng!";
        public static string SaveSuccess = "Lưu dữ liệu thành công!";
        public static string SaveFailed = "Lưu dữ liệu thất bại!";
        public static string DeleteConfirm = "Bạn có chắc chắn muốn xóa dữ liệu này?";
        public static string DeleteSuccess = "Xóa dữ liệu thành công!";
        public static string DeleteFailed = "Xóa dữ liệu thất bại!";
    }
}