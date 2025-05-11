// File: Services/SystemConfigService.cs
// Mô tả: Lớp dịch vụ quản lý cấu hình hệ thống
// Tác giả: Steve-Thuong_hai
// Ngày tạo: 12/05/2025

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Services
{
    /// <summary>
    /// Lớp dịch vụ quản lý cấu hình hệ thống
    /// </summary>
    public class SystemConfigService : ISystemConfigService
    {
        private readonly ISystemConfigRepository _systemConfigRepository;
        private readonly Logger _logger;
        private readonly IAccountRepository _accountRepository;
        private static Dictionary<string, object> _configCache = new Dictionary<string, object>();
        private static DateTime _lastCacheRefresh = DateTime.MinValue;
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        private static readonly object _cacheLock = new object();

        /// <summary>
        /// Khởi tạo dịch vụ cấu hình hệ thống
        /// </summary>
        public SystemConfigService()
        {
            _systemConfigRepository = new SystemConfigRepository();
            _accountRepository = new AccountRepository();
            _logger = new Logger();
        }

        /// <summary>
        /// Khởi tạo dịch vụ cấu hình hệ thống với repository được tiêm vào
        /// </summary>
        /// <param name="systemConfigRepository">Repository cấu hình hệ thống</param>
        /// <param name="accountRepository">Repository tài khoản</param>
        public SystemConfigService(ISystemConfigRepository systemConfigRepository, IAccountRepository accountRepository)
        {
            _systemConfigRepository = systemConfigRepository;
            _accountRepository = accountRepository;
            _logger = new Logger();
        }

        /// <summary>
        /// Làm mới cache nếu cần
        /// </summary>
        private void RefreshCacheIfNeeded()
        {
            lock (_cacheLock)
            {
                if (DateTime.Now - _lastCacheRefresh > _cacheDuration)
                {
                    _configCache.Clear();
                    _lastCacheRefresh = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Lấy tất cả cấu hình hệ thống
        /// </summary>
        /// <returns>Danh sách DTO cấu hình</returns>
        public List<SystemConfigDTO> GetAllConfigs()
        {
            try
            {
                RefreshCacheIfNeeded();

                if (!_configCache.ContainsKey("AllConfigs"))
                {
                    lock (_cacheLock)
                    {
                        if (!_configCache.ContainsKey("AllConfigs"))
                        {
                            List<SystemConfig> configs = _systemConfigRepository.GetAllConfigs();
                            List<SystemConfigDTO> configDTOs = configs.Select(c => new SystemConfigDTO
                            {
                                ConfigID = c.ConfigID,
                                ConfigKey = c.ConfigKey,
                                ConfigValue = c.ConfigValue,
                                Description = c.Description,
                                DataType = c.DataType,
                                IsReadOnly = c.IsReadOnly,
                                CreatedDate = c.CreatedDate,
                                ModifiedDate = c.ModifiedDate
                            }).ToList();

                            _configCache["AllConfigs"] = configDTOs;
                        }
                    }
                }

                return (List<SystemConfigDTO>)_configCache["AllConfigs"];
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetAllConfigs", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy cấu hình theo khóa
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <returns>DTO cấu hình</returns>
        public SystemConfigDTO GetConfigByKey(string configKey)
        {
            try
            {
                if (string.IsNullOrEmpty(configKey))
                    return null;

                RefreshCacheIfNeeded();
                string cacheKey = $"Config_{configKey}";

                if (!_configCache.ContainsKey(cacheKey))
                {
                    lock (_cacheLock)
                    {
                        if (!_configCache.ContainsKey(cacheKey))
                        {
                            SystemConfig config = _systemConfigRepository.GetConfigByKey(configKey);
                            if (config == null)
                                return null;

                            SystemConfigDTO configDTO = new SystemConfigDTO
                            {
                                ConfigID = config.ConfigID,
                                ConfigKey = config.ConfigKey,
                                ConfigValue = config.ConfigValue,
                                Description = config.Description,
                                DataType = config.DataType,
                                IsReadOnly = config.IsReadOnly,
                                CreatedDate = config.CreatedDate,
                                ModifiedDate = config.ModifiedDate
                            };

                            _configCache[cacheKey] = configDTO;
                        }
                    }
                }

                return (SystemConfigDTO)_configCache[cacheKey];
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetConfigByKey", ex);
                throw;
            }
        }

        /// <summary>
        /// Tạo cấu hình mới
        /// </summary>
        /// <param name="configDTO">DTO cấu hình cần tạo</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>ID cấu hình mới</returns>
        public int CreateConfig(SystemConfigDTO configDTO, int currentUserId)
        {
            try
            {
                if (!HasConfigAccess(configDTO.ConfigKey, currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền tạo cấu hình");

                SystemConfig config = new SystemConfig
                {
                    ConfigKey = configDTO.ConfigKey,
                    ConfigValue = configDTO.ConfigValue,
                    Description = configDTO.Description,
                    DataType = configDTO.DataType,
                    IsReadOnly = configDTO.IsReadOnly
                };

                int configId = _systemConfigRepository.CreateConfig(config, currentUserId);

                // Xóa cache liên quan
                lock (_cacheLock)
                {
                    _configCache.Remove("AllConfigs");
                    string cacheKey = $"Config_{configDTO.ConfigKey}";
                    if (_configCache.ContainsKey(cacheKey))
                        _configCache.Remove(cacheKey);
                }

                return configId;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.CreateConfig", ex);
                throw;
            }
        }

        /// <summary>
        /// Cập nhật cấu hình
        /// </summary>
        /// <param name="configDTO">DTO cấu hình cần cập nhật</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả cập nhật</returns>
        public bool UpdateConfig(SystemConfigDTO configDTO, int currentUserId)
        {
            try
            {
                if (!HasConfigAccess(configDTO.ConfigKey, currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền cập nhật cấu hình");

                SystemConfig config = new SystemConfig
                {
                    ConfigKey = configDTO.ConfigKey,
                    ConfigValue = configDTO.ConfigValue
                };

                bool result = _systemConfigRepository.UpdateConfig(config, currentUserId);

                // Xóa cache liên quan
                lock (_cacheLock)
                {
                    _configCache.Remove("AllConfigs");
                    string cacheKey = $"Config_{configDTO.ConfigKey}";
                    if (_configCache.ContainsKey(cacheKey))
                        _configCache.Remove(cacheKey);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.UpdateConfig", ex);
                throw;
            }
        }

        /// <summary>
        /// Xóa cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả xóa</returns>
        public bool DeleteConfig(string configKey, int currentUserId)
        {
            try
            {
                if (!HasConfigAccess(configKey, currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền xóa cấu hình");

                bool result = _systemConfigRepository.DeleteConfig(configKey, currentUserId);

                // Xóa cache liên quan
                lock (_cacheLock)
                {
                    _configCache.Remove("AllConfigs");
                    string cacheKey = $"Config_{configKey}";
                    if (_configCache.ContainsKey(cacheKey))
                        _configCache.Remove(cacheKey);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.DeleteConfig", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy lịch sử thay đổi cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <returns>Lịch sử thay đổi</returns>
        public List<ActivityLogDTO> GetConfigHistory(string configKey)
        {
            try
            {
                List<ActivityLog> logs = _systemConfigRepository.GetConfigHistory(configKey);

                return logs.Select(l => new ActivityLogDTO
                {
                    ActivityID = l.ActivityID,
                    AccountID = l.AccountID,
                    ActivityType = l.ActivityType,
                    EntityType = l.EntityType,
                    EntityID = l.EntityID,
                    Description = l.Description,
                    OldValue = l.OldValue,
                    NewValue = l.NewValue,
                    IPAddress = l.IPAddress,
                    ActivityDate = l.ActivityDate,
                    Username = l.Username,
                    EmployeeName = l.EmployeeName
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetConfigHistory", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy cấu hình theo loại
        /// </summary>
        /// <param name="dataType">Loại dữ liệu cấu hình</param>
        /// <returns>Danh sách DTO cấu hình</returns>
        public List<SystemConfigDTO> GetConfigsByType(string dataType)
        {
            try
            {
                return GetAllConfigs().Where(c => c.DataType == dataType).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetConfigsByType", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy giá trị cấu hình theo khóa
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị cấu hình</returns>
        public string GetConfigValue(string configKey, string defaultValue = null)
        {
            try
            {
                SystemConfigDTO config = GetConfigByKey(configKey);
                return config != null ? config.ConfigValue : defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetConfigValue", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Lấy giá trị cấu hình kiểu int
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị cấu hình kiểu int</returns>
        public int GetConfigValueInt(string configKey, int defaultValue = 0)
        {
            try
            {
                string value = GetConfigValue(configKey);
                return !string.IsNullOrEmpty(value) && int.TryParse(value, out int result) ? result : defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetConfigValueInt", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Lấy giá trị cấu hình kiểu bool
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị cấu hình kiểu bool</returns>
        public bool GetConfigValueBool(string configKey, bool defaultValue = false)
        {
            try
            {
                string value = GetConfigValue(configKey);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;

                if (bool.TryParse(value, out bool result))
                    return result;

                return value.ToLower() == "1" || value.ToLower() == "yes" || value.ToLower() == "true";
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetConfigValueBool", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Lấy giá trị cấu hình kiểu decimal
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị cấu hình kiểu decimal</returns>
        public decimal GetConfigValueDecimal(string configKey, decimal defaultValue = 0)
        {
            try
            {
                string value = GetConfigValue(configKey);
                return !string.IsNullOrEmpty(value) && decimal.TryParse(value, out decimal result) ? result : defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetConfigValueDecimal", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Kiểm tra quyền truy cập cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Có quyền hay không</returns>
        public bool HasConfigAccess(string configKey, int currentUserId)
        {
            try
            {
                // Kiểm tra quyền Admin
                var account = _accountRepository.GetAccountById(currentUserId);
                if (account == null)
                    return false;

                // Chỉ admin mới có quyền quản lý cấu hình
                return account.RoleID == 1; // Giả sử RoleID 1 là Admin
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.HasConfigAccess", ex);
                return false;
            }
        }

        /// <summary>
        /// Tạo bản sao lưu CSDL
        /// </summary>
        /// <param name="backupPath">Đường dẫn sao lưu</param>
        /// <param name="backupName">Tên bản sao lưu</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả sao lưu</returns>
        public string CreateDatabaseBackup(string backupPath, string backupName, int currentUserId)
        {
            try
            {
                if (!HasConfigAccess("BackupDatabase", currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền sao lưu CSDL");

                if (string.IsNullOrEmpty(backupPath))
                    backupPath = GetConfigValue("DefaultBackupPath", @"C:\Backups");

                if (string.IsNullOrEmpty(backupName))
                    backupName = $"QuanLyCuaHangTienLoi_Full_{DateTime.Now:yyyyMMdd_HHmmss}";

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(backupPath))
                    Directory.CreateDirectory(backupPath);

                // Gọi stored procedure sao lưu
                using (var connection = new SystemConfigRepository().GetConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "app.sp_CreateFullBackup";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BackupPath", backupPath);
                        command.Parameters.AddWithValue("@BackupName", backupName);
                        command.Parameters.AddWithValue("@AccountID", currentUserId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return reader["Result"].ToString();
                        }
                    }
                }

                return "Sao lưu CSDL thất bại";
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.CreateDatabaseBackup", ex);
                throw;
            }
        }

        /// <summary>
        /// Phục hồi CSDL từ bản sao lưu
        /// </summary>
        /// <param name="backupPath">Đường dẫn bản sao lưu</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả phục hồi</returns>
        public string RestoreDatabase(string backupPath, int currentUserId)
        {
            try
            {
                if (!HasConfigAccess("RestoreDatabase", currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền phục hồi CSDL");

                if (string.IsNullOrEmpty(backupPath) || !File.Exists(backupPath))
                    throw new FileNotFoundException("Không tìm thấy file sao lưu", backupPath);

                // Gọi stored procedure phục hồi
                using (var connection = new SystemConfigRepository().GetConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "app.sp_RestoreDatabase";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BackupPath", backupPath);
                        command.Parameters.AddWithValue("@AccountID", currentUserId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return reader["Result"].ToString();
                        }
                    }
                }

                return "Phục hồi CSDL thất bại";
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.RestoreDatabase", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách bản sao lưu
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Danh sách bản sao lưu</returns>
        public List<BackupDTO> GetBackupHistory(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Gọi stored procedure lấy lịch sử sao lưu
                List<BackupDTO> backups = new List<BackupDTO>();

                using (var connection = new SystemConfigRepository().GetConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "app.sp_GetBackupHistory";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        if (startDate.HasValue)
                            command.Parameters.AddWithValue("@StartDate", startDate.Value);

                        if (endDate.HasValue)
                            command.Parameters.AddWithValue("@EndDate", endDate.Value);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                backups.Add(new BackupDTO
                                {
                                    BackupID = Convert.ToInt32(reader["BackupID"]),
                                    BackupName = reader["BackupName"].ToString(),
                                    BackupPath = reader["BackupPath"].ToString(),
                                    BackupSize = reader["BackupSize"] != DBNull.Value ?
                                        Convert.ToInt64(reader["BackupSize"]) : 0,
                                    BackupDate = Convert.ToDateTime(reader["BackupDate"]),
                                    Status = reader["Status"].ToString(),
                                    Note = reader["Note"] != DBNull.Value ?
                                        reader["Note"].ToString() : null,
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    EmployeeName = reader["EmployeeName"] != DBNull.Value ?
                                        reader["EmployeeName"].ToString() : null
                                });
                            }
                        }
                    }
                }

                return backups;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetBackupHistory", ex);
                throw;
            }
        }

        /// <summary>
        /// Xóa bản sao lưu cũ
        /// </summary>
        /// <param name="daysToKeep">Số ngày giữ lại</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả xóa</returns>
        public string DeleteOldBackups(int daysToKeep, int currentUserId)
        {
            try
            {
                if (!HasConfigAccess("DeleteOldBackups", currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền xóa bản sao lưu cũ");

                if (daysToKeep < 1)
                    daysToKeep = 30; // Mặc định giữ 30 ngày

                // Gọi stored procedure xóa bản sao lưu cũ
                using (var connection = new SystemConfigRepository().GetConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "app.sp_DeleteOldBackups";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DaysToKeep", daysToKeep);
                        command.Parameters.AddWithValue("@AccountID", currentUserId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return reader["Result"].ToString();
                        }
                    }
                }

                return "Xóa bản sao lưu cũ thất bại";
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.DeleteOldBackups", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy log hệ thống
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="logLevel">Mức độ log</param>
        /// <param name="maxRows">Số dòng tối đa</param>
        /// <returns>Danh sách log hệ thống</returns>
        public List<SystemLogDTO> GetSystemLogs(DateTime? startDate = null, DateTime? endDate = null,
            string logLevel = null, int maxRows = 1000)
        {
            try
            {
                List<SystemLogDTO> logs = new List<SystemLogDTO>();

                using (var connection = new SystemConfigRepository().GetConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "app.sp_GetSystemLogs";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        if (startDate.HasValue)
                            command.Parameters.AddWithValue("@StartDate", startDate.Value);

                        if (endDate.HasValue)
                            command.Parameters.AddWithValue("@EndDate", endDate.Value);

                        if (!string.IsNullOrEmpty(logLevel))
                            command.Parameters.AddWithValue("@LogLevel", logLevel);

                        command.Parameters.AddWithValue("@MaxRows", maxRows);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new SystemLogDTO
                                {
                                    LogID = Convert.ToInt32(reader["LogID"]),
                                    LogLevel = reader["LogLevel"].ToString(),
                                    Message = reader["Message"].ToString(),
                                    Source = reader["Source"] != DBNull.Value ?
                                        reader["Source"].ToString() : null,
                                    Exception = reader["Exception"] != DBNull.Value ?
                                        reader["Exception"].ToString() : null,
                                    StackTrace = reader["StackTrace"] != DBNull.Value ?
                                        reader["StackTrace"].ToString() : null,
                                    AccountID = reader["AccountID"] != DBNull.Value ?
                                        Convert.ToInt32(reader["AccountID"]) : (int?)null,
                                    Username = reader["Username"] != DBNull.Value ?
                                        reader["Username"].ToString() : null,
                                    IPAddress = reader["IPAddress"] != DBNull.Value ?
                                        reader["IPAddress"].ToString() : null,
                                    LogDate = Convert.ToDateTime(reader["LogDate"])
                                });
                            }
                        }
                    }
                }

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetSystemLogs", ex);
                throw;
            }
        }

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
        public List<ActivityLogDTO> GetActivityLogs(DateTime? startDate = null, DateTime? endDate = null,
            string activityType = null, string entityType = null, int? accountId = null, int maxRows = 1000)
        {
            try
            {
                List<ActivityLogDTO> logs = new List<ActivityLogDTO>();

                using (var connection = new SystemConfigRepository().GetConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "app.sp_GetActivityLogs";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        if (startDate.HasValue)
                            command.Parameters.AddWithValue("@StartDate", startDate.Value);

                        if (endDate.HasValue)
                            command.Parameters.AddWithValue("@EndDate", endDate.Value);

                        if (!string.IsNullOrEmpty(activityType))
                            command.Parameters.AddWithValue("@ActivityType", activityType);

                        if (!string.IsNullOrEmpty(entityType))
                            command.Parameters.AddWithValue("@EntityType", entityType);

                        if (accountId.HasValue)
                            command.Parameters.AddWithValue("@AccountID", accountId.Value);

                        command.Parameters.AddWithValue("@MaxRows", maxRows);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(new ActivityLogDTO
                                {
                                    ActivityID = Convert.ToInt32(reader["ActivityID"]),
                                    ActivityType = reader["ActivityType"].ToString(),
                                    EntityType = reader["EntityType"].ToString(),
                                    EntityID = Convert.ToInt32(reader["EntityID"]),
                                    Description = reader["Description"].ToString(),
                                    OldValue = reader["OldValue"] != DBNull.Value ?
                                        reader["OldValue"].ToString() : null,
                                    NewValue = reader["NewValue"] != DBNull.Value ?
                                        reader["NewValue"].ToString() : null,
                                    IPAddress = reader["IPAddress"] != DBNull.Value ?
                                        reader["IPAddress"].ToString() : null,
                                    ActivityDate = Convert.ToDateTime(reader["ActivityDate"]),
                                    AccountID = Convert.ToInt32(reader["AccountID"]),
                                    Username = reader["Username"].ToString(),
                                    EmployeeName = reader["EmployeeName"] != DBNull.Value ?
                                        reader["EmployeeName"].ToString() : null
                                });
                            }
                        }
                    }
                }

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.GetActivityLogs", ex);
                throw;
            }
        }

        /// <summary>
        /// Lập lịch sao lưu tự động
        /// </summary>
        /// <param name="backupPath">Đường dẫn sao lưu</param>
        /// <param name="frequency">Tần suất sao lưu (Daily, Weekly, Monthly)</param>
        /// <param name="time">Thời gian sao lưu</param>
        /// <param name="daysToKeep">Số ngày giữ lại bản sao lưu</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả lập lịch</returns>
        public bool ScheduleBackup(string backupPath, string frequency, TimeSpan time, int daysToKeep, int currentUserId)
        {
            try
            {
                if (!HasConfigAccess("ScheduleBackup", currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền lập lịch sao lưu tự động");

                if (string.IsNullOrEmpty(backupPath))
                    backupPath = GetConfigValue("DefaultBackupPath", @"C:\Backups");

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(backupPath))
                    Directory.CreateDirectory(backupPath);

                // Lưu cấu hình lập lịch
                UpdateConfig(new SystemConfigDTO
                {
                    ConfigKey = "BackupSchedule_Enabled",
                    ConfigValue = "true"
                }, currentUserId);

                UpdateConfig(new SystemConfigDTO
                {
                    ConfigKey = "BackupSchedule_Path",
                    ConfigValue = backupPath
                }, currentUserId);

                UpdateConfig(new SystemConfigDTO
                {
                    ConfigKey = "BackupSchedule_Frequency",
                    ConfigValue = frequency
                }, currentUserId);

                UpdateConfig(new SystemConfigDTO
                {
                    ConfigKey = "BackupSchedule_Time",
                    ConfigValue = time.ToString()
                }, currentUserId);

                UpdateConfig(new SystemConfigDTO
                {
                    ConfigKey = "BackupSchedule_DaysToKeep",
                    ConfigValue = daysToKeep.ToString()
                }, currentUserId);

                // Ghi log hoạt động
                _logger.LogInfo($"Đã lập lịch sao lưu tự động: Frequency={frequency}, Time={time}, Path={backupPath}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.ScheduleBackup", ex);
                throw;
            }
        }

        /// <summary>
        /// Hủy lịch sao lưu tự động
        /// </summary>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả hủy lịch</returns>
        public bool CancelBackupSchedule(int currentUserId)
        {
            try
            {
                if (!HasConfigAccess("ScheduleBackup", currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền hủy lịch sao lưu tự động");

                // Lưu cấu hình lập lịch
                UpdateConfig(new SystemConfigDTO
                {
                    ConfigKey = "BackupSchedule_Enabled",
                    ConfigValue = "false"
                }, currentUserId);

                // Ghi log hoạt động
                _logger.LogInfo("Đã hủy lịch sao lưu tự động");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.CancelBackupSchedule", ex);
                throw;
            }
        }

        /// <summary>
        /// Tối ưu hóa CSDL
        /// </summary>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả tối ưu hóa</returns>
        public string OptimizeDatabase(int currentUserId)
        {
            try
            {
                if (!HasConfigAccess("OptimizeDatabase", currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền tối ưu hóa CSDL");

                StringBuilder result = new StringBuilder();

                using (var connection = new SystemConfigRepository().GetConnection())
                {
                    connection.Open();

                    // Tối ưu hóa indexes
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            DECLARE @tableName NVARCHAR(128)
                            DECLARE @indexName NVARCHAR(128)
                            DECLARE @sql NVARCHAR(500)
                            
                            DECLARE index_cursor CURSOR FOR
                            SELECT t.name AS TableName, i.name AS IndexName
                            FROM sys.indexes i
                            INNER JOIN sys.tables t ON i.object_id = t.object_id
                            WHERE i.type_desc = 'NONCLUSTERED' AND i.is_disabled = 0
                            AND t.schema_id = SCHEMA_ID('app')
                            
                            OPEN index_cursor
                            FETCH NEXT FROM index_cursor INTO @tableName, @indexName
                            
                            WHILE @@FETCH_STATUS = 0
                            BEGIN
                                SET @sql = 'ALTER INDEX ' + @indexName + ' ON app.' + @tableName + ' REORGANIZE'
                                EXEC sp_executesql @sql
                                
                                FETCH NEXT FROM index_cursor INTO @tableName, @indexName
                            END
                            
                            CLOSE index_cursor
                            DEALLOCATE index_cursor
                        ";
                        command.ExecuteNonQuery();
                    }

                    result.AppendLine("Đã tối ưu hóa indexes.");

                    // Cập nhật thống kê
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            DECLARE @tableName NVARCHAR(128)
                            DECLARE @sql NVARCHAR(500)
                            
                            DECLARE table_cursor CURSOR FOR
                            SELECT name FROM sys.tables
                            WHERE schema_id = SCHEMA_ID('app')
                            
                            OPEN table_cursor
                            FETCH NEXT FROM table_cursor INTO @tableName
                            
                            WHILE @@FETCH_STATUS = 0
                            BEGIN
                                SET @sql = 'UPDATE STATISTICS app.' + @tableName
                                EXEC sp_executesql @sql
                                
                                FETCH NEXT FROM table_cursor INTO @tableName
                            END
                            
                            CLOSE table_cursor
                            DEALLOCATE table_cursor
                        ";
                        command.ExecuteNonQuery();
                    }

                    result.AppendLine("Đã cập nhật thống kê.");

                    // Xóa dữ liệu tạm thời
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            -- Xóa log hệ thống cũ (giữ lại log 30 ngày gần đây)
                            DELETE FROM app.SystemLog
                            WHERE LogDate < DATEADD(DAY, -30, GETDATE())
                            
                            -- Xóa log hoạt động cũ (giữ lại log 90 ngày gần đây)
                            DELETE FROM app.ActivityLog
                            WHERE ActivityDate < DATEADD(DAY, -90, GETDATE())
                        ";
                        int rowsAffected = command.ExecuteNonQuery();
                        result.AppendLine($"Đã xóa {rowsAffected} dòng dữ liệu log cũ.");
                    }
                }

                // Ghi log hoạt động
                _logger.LogInfo("Đã tối ưu hóa CSDL");

                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.OptimizeDatabase", ex);
                throw;
            }
        }

        /// <summary>
        /// Xuất cấu hình hệ thống
        /// </summary>
        /// <param name="exportPath">Đường dẫn xuất</param>
        /// <returns>Đường dẫn file xuất</returns>
        public string ExportConfigurations(string exportPath)
        {
            try
            {
                if (string.IsNullOrEmpty(exportPath))
                {
                    string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    exportPath = Path.Combine(defaultPath, $"SystemConfig_Export_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                }

                List<SystemConfigDTO> configs = GetAllConfigs();

                // Loại bỏ các cấu hình nhạy cảm
                configs = configs.Where(c =>
                    !c.ConfigKey.Contains("Password") &&
                    !c.ConfigKey.Contains("Secret") &&
                    !c.ConfigKey.Contains("Key")
                ).ToList();

                string json = JsonConvert.SerializeObject(configs, Formatting.Indented);
                File.WriteAllText(exportPath, json, Encoding.UTF8);

                return exportPath;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.ExportConfigurations", ex);
                throw;
            }
        }

        /// <summary>
        /// Nhập cấu hình hệ thống
        /// </summary>
        /// <param name="importPath">Đường dẫn nhập</param>
        /// <param name="currentUserId">ID người dùng hiện tại</param>
        /// <returns>Kết quả nhập</returns>
        public string ImportConfigurations(string importPath, int currentUserId)
        {
            try
            {
                if (!HasConfigAccess("ImportConfigurations", currentUserId))
                    throw new UnauthorizedAccessException("Không có quyền nhập cấu hình");

                if (string.IsNullOrEmpty(importPath) || !File.Exists(importPath))
                    throw new FileNotFoundException("Không tìm thấy file cấu hình", importPath);

                string json = File.ReadAllText(importPath, Encoding.UTF8);
                List<SystemConfigDTO> configs = JsonConvert.DeserializeObject<List<SystemConfigDTO>>(json);

                if (configs == null || configs.Count == 0)
                    return "Không có cấu hình nào để nhập";

                int importedCount = 0;
                StringBuilder result = new StringBuilder();

                foreach (var config in configs)
                {
                    try
                    {
                        // Kiểm tra cấu hình có tồn tại chưa
                        var existingConfig = GetConfigByKey(config.ConfigKey);

                        if (existingConfig != null)
                        {
                            // Cập nhật cấu hình
                            UpdateConfig(new SystemConfigDTO
                            {
                                ConfigKey = config.ConfigKey,
                                ConfigValue = config.ConfigValue
                            }, currentUserId);
                        }
                        else
                        {
                            // Tạo cấu hình mới
                            CreateConfig(new SystemConfigDTO
                            {
                                ConfigKey = config.ConfigKey,
                                ConfigValue = config.ConfigValue,
                                Description = config.Description,
                                DataType = config.DataType,
                                IsReadOnly = config.IsReadOnly
                            }, currentUserId);
                        }

                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        result.AppendLine($"Lỗi khi nhập cấu hình '{config.ConfigKey}': {ex.Message}");
                    }
                }

                result.Insert(0, $"Đã nhập {importedCount}/{configs.Count} cấu hình.");

                // Xóa cache
                lock (_cacheLock)
                {
                    _configCache.Clear();
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigService.ImportConfigurations", ex);
                throw;
            }
        }
    }
}