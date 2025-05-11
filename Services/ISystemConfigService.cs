// File: Services/ISystemConfigService.cs
// Mô tả: Interface định nghĩa các dịch vụ quản lý cấu hình hệ thống
// Tác giả: Steve-Thuong_hai
// Ngày tạo: 12/05/2025

using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Services
{
    /// <summary>
    /// Interface định nghĩa các dịch vụ quản lý cấu hình hệ thống
    /// </summary>
    public interface ISystemConfigService
    {
        /// <summary>
        /// Lấy tất cả cấu hình hệ thống
        /// </summary>
        /// <returns>Danh sách DTO cấu hình</returns>
        List<SystemConfigDTO> GetAllConfigs();

        /// <summary>
        /// Lấy cấu hình theo khóa
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <returns>DTO cấu hình</returns>
        SystemConfigDTO GetConfigByKey(string configKey);

        /// <summary>
        /// Tạo cấu hình mới
        /// </summary>
        /// <param name="configDTO">DTO cấu hình cần tạo</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>ID cấu hình mới</returns>
        int CreateConfig(SystemConfigDTO configDTO, int currentUserId);

        /// <summary>
        /// Cập nhật cấu hình
        /// </summary>
        /// <param name="configDTO">DTO cấu hình cần cập nhật</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả cập nhật</returns>
        bool UpdateConfig(SystemConfigDTO configDTO, int currentUserId);

        /// <summary>
        /// Xóa cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả xóa</returns>
        bool DeleteConfig(string configKey, int currentUserId);

        /// <summary>
        /// Lấy lịch sử thay đổi cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <returns>Lịch sử thay đổi</returns>
        List<ActivityLogDTO> GetConfigHistory(string configKey);

        /// <summary>
        /// Lấy cấu hình theo loại
        /// </summary>
        /// <param name="dataType">Loại dữ liệu cấu hình</param>
        /// <returns>Danh sách DTO cấu hình</returns>
        List<SystemConfigDTO> GetConfigsByType(string dataType);

        /// <summary>
        /// Lấy giá trị cấu hình theo khóa
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị cấu hình</returns>
        string GetConfigValue(string configKey, string defaultValue = null);

        /// <summary>
        /// Lấy giá trị cấu hình kiểu int
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị cấu hình kiểu int</returns>
        int GetConfigValueInt(string configKey, int defaultValue = 0);

        /// <summary>
        /// Lấy giá trị cấu hình kiểu bool
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị cấu hình kiểu bool</returns>
        bool GetConfigValueBool(string configKey, bool defaultValue = false);

        /// <summary>
        /// Lấy giá trị cấu hình kiểu decimal
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị cấu hình kiểu decimal</returns>
        decimal GetConfigValueDecimal(string configKey, decimal defaultValue = 0);

        /// <summary>
        /// Kiểm tra quyền truy cập cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Có quyền hay không</returns>
        bool HasConfigAccess(string configKey, int currentUserId);

        /// <summary>
        /// Tạo bản sao lưu CSDL
        /// </summary>
        /// <param name="backupPath">Đường dẫn sao lưu</param>
        /// <param name="backupName">Tên bản sao lưu</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả sao lưu</returns>
        string CreateDatabaseBackup(string backupPath, string backupName, int currentUserId);

        /// <summary>
        /// Phục hồi CSDL từ bản sao lưu
        /// </summary>
        /// <param name="backupPath">Đường dẫn bản sao lưu</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả phục hồi</returns>
        string RestoreDatabase(string backupPath, int currentUserId);

        /// <summary>
        /// Lấy danh sách bản sao lưu
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Danh sách bản sao lưu</returns>
        List<BackupDTO> GetBackupHistory(DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Xóa bản sao lưu cũ
        /// </summary>
        /// <param name="daysToKeep">Số ngày giữ lại</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả xóa</returns>
        string DeleteOldBackups(int daysToKeep, int currentUserId);

        /// <summary>
        /// Lấy log hệ thống
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="logLevel">Mức độ log</param>
        /// <param name="maxRows">Số dòng tối đa</param>
        /// <returns>Danh sách log hệ thống</returns>
        List<SystemLogDTO> GetSystemLogs(DateTime? startDate = null, DateTime? endDate = null,
            string logLevel = null, int maxRows = 1000);

        /// <summary>
        /// Lấy log hoạt động
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="activityType">Loại hoạt động</param>
        /// <param name="entityType">Loại đối tượng</param>
        /// <param name="accountId">ID tài khoản</param>
        /// <param name="maxRows">Số dòng tối đa</param>
        /// <returns>Danh sách log hoạt động</returns>
        List<ActivityLogDTO> GetActivityLogs(DateTime? startDate = null, DateTime? endDate = null,
            string activityType = null, string entityType = null, int? accountId = null, int maxRows = 1000);

        /// <summary>
        /// Lập lịch sao lưu tự động
        /// </summary>
        /// <param name="backupPath">Đường dẫn sao lưu</param>
        /// <param name="frequency">Tần suất sao lưu (Daily, Weekly, Monthly)</param>
        /// <param name="time">Thời gian sao lưu</param>
        /// <param name="daysToKeep">Số ngày giữ lại bản sao lưu</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả lập lịch</returns>
        bool ScheduleBackup(string backupPath, string frequency, TimeSpan time, int daysToKeep, int currentUserId);

        /// <summary>
        /// Hủy lịch sao lưu tự động
        /// </summary>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả hủy lịch</returns>
        bool CancelBackupSchedule(int currentUserId);

        /// <summary>
        /// Tối ưu hóa CSDL
        /// </summary>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả tối ưu hóa</returns>
        string OptimizeDatabase(int currentUserId);

        /// <summary>
        /// Xuất cấu hình hệ thống
        /// </summary>
        /// <param name="exportPath">Đường dẫn xuất</param>
        /// <returns>Đường dẫn file xuất</returns>
        string ExportConfigurations(string exportPath);

        /// <summary>
        /// Nhập cấu hình hệ thống
        /// </summary>
        /// <param name="importPath">Đường dẫn nhập</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả nhập</returns>
        string ImportConfigurations(string importPath, int currentUserId);
    }
}