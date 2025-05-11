using QuanLyCuaHangTienLoi.Db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCuaHangTienLoi
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set up exception handling
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // Create necessary directories
                EnsureDirectoriesExist();

                // Test database connection
                TestDatabaseConnection();

                // Start application
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Lỗi khởi động ứng dụng", ex);
                MessageBox.Show($"Lỗi khởi động ứng dụng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Instance.Error("Thread Exception", e.Exception);
            MessageBox.Show($"Đã xảy ra lỗi: {e.Exception.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Instance.Error("Unhandled Exception", e.ExceptionObject as Exception);
            MessageBox.Show($"Đã xảy ra lỗi nghiêm trọng: {(e.ExceptionObject as Exception)?.Message}",
                "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void EnsureDirectoriesExist()
        {
            // Ensure directories exist
            Directory.CreateDirectory(Constants.Paths.LogPath);
            Directory.CreateDirectory(Constants.Paths.BackupPath);
            Directory.CreateDirectory(Constants.Paths.ImagePath);
            Directory.CreateDirectory(Constants.Paths.FontPath);
        }

        private static void TestDatabaseConnection()
        {
            try
            {
                using (ConnectionManager connectionManager = new ConnectionManager())
                {
                    connectionManager.Open();
                    connectionManager.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Không thể kết nối đến cơ sở dữ liệu", ex);
                throw new Exception("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra lại cấu hình.", ex);
            }
        }
    }
}
