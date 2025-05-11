// File: Db/Repositories/ISystemConfigRepository.cs
// Mô tả: Interface định nghĩa các thao tác với bảng SystemConfig trong CSDL
// Tác giả: Steve-Thuong_hai
// Ngày tạo: 12/05/2025

using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    /// <summary>
    /// Interface định nghĩa các thao tác với bảng SystemConfig
    /// </summary>
    public interface ISystemConfigRepository
    {
        /// <summary>
        /// Lấy tất cả cấu hình hệ thống
        /// </summary>
        /// <returns>Danh sách cấu hình</returns>
        List<SystemConfig> GetAllConfigs();

        /// <summary>
        /// Lấy cấu hình theo khóa
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <returns>Cấu hình</returns>
        SystemConfig GetConfigByKey(string configKey);

        /// <summary>
        /// Tạo cấu hình mới
        /// </summary>
        /// <param name="config">Cấu hình cần tạo</param>
        /// <param name="createdBy">ID người tạo</param>
        /// <returns>ID cấu hình mới</returns>
        int CreateConfig(SystemConfig config, int createdBy);

        /// <summary>
        /// Cập nhật cấu hình
        /// </summary>
        /// <param name="config">Cấu hình cần cập nhật</param>
        /// <param name="modifiedBy">ID người cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        bool UpdateConfig(SystemConfig config, int modifiedBy);

        /// <summary>
        /// Xóa cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="deletedBy">ID người xóa</param>
        /// <returns>Kết quả xóa</returns>
        bool DeleteConfig(string configKey, int deletedBy);

        /// <summary>
        /// Lấy lịch sử thay đổi cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <returns>Lịch sử thay đổi</returns>
        List<ActivityLog> GetConfigHistory(string configKey);

        /// <summary>
        /// Lấy cấu hình theo loại
        /// </summary>
        /// <param name="dataType">Loại dữ liệu cấu hình</param>
        /// <returns>Danh sách cấu hình</returns>
        List<SystemConfig> GetConfigsByType(string dataType);

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
    }
}