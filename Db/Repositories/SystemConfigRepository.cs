// File: Db/Repositories/SystemConfigRepository.cs
// Mô tả: Lớp thực thi ISystemConfigRepository - quản lý thao tác với bảng SystemConfig
// Tác giả: Steve-Thuong_hai
// Ngày tạo: 12/05/2025

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    /// <summary>
    /// Lớp thực thi ISystemConfigRepository - quản lý thao tác với bảng SystemConfig
    /// </summary>
    public class SystemConfigRepository : BaseRepository, ISystemConfigRepository
    {
        private readonly Logger _logger;

        /// <summary>
        /// Khởi tạo đối tượng SystemConfigRepository
        /// </summary>
        public SystemConfigRepository()
        {
            _logger = new Logger();
        }

        /// <summary>
        /// Lấy tất cả cấu hình hệ thống
        /// </summary>
        /// <returns>Danh sách cấu hình</returns>
        public List<SystemConfig> GetAllConfigs()
        {
            try
            {
                List<SystemConfig> configs = new List<SystemConfig>();

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("app.sp_GetAllSystemConfigs", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                configs.Add(new SystemConfig
                                {
                                    ConfigID = Convert.ToInt32(reader["ConfigID"]),
                                    ConfigKey = reader["ConfigKey"].ToString(),
                                    ConfigValue = reader["ConfigValue"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    DataType = reader["DataType"].ToString(),
                                    IsReadOnly = Convert.ToBoolean(reader["IsReadOnly"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedDate = reader["ModifiedDate"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null
                                });
                            }
                        }
                    }
                }

                return configs;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigRepository.GetAllConfigs", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy cấu hình theo khóa
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <returns>Cấu hình</returns>
        public SystemConfig GetConfigByKey(string configKey)
        {
            try
            {
                SystemConfig config = null;

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("app.sp_GetSystemConfigByKey", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ConfigKey", configKey);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                config = new SystemConfig
                                {
                                    ConfigID = Convert.ToInt32(reader["ConfigID"]),
                                    ConfigKey = reader["ConfigKey"].ToString(),
                                    ConfigValue = reader["ConfigValue"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    DataType = reader["DataType"].ToString(),
                                    IsReadOnly = Convert.ToBoolean(reader["IsReadOnly"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    ModifiedDate = reader["ModifiedDate"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null
                                };
                            }
                        }
                    }
                }

                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigRepository.GetConfigByKey", ex);
                throw;
            }
        }

        /// <summary>
        /// Tạo cấu hình mới
        /// </summary>
        /// <param name="config">Cấu hình cần tạo</param>
        /// <param name="createdBy">ID người tạo</param>
        /// <returns>ID cấu hình mới</returns>
        public int CreateConfig(SystemConfig config, int createdBy)
        {
            try
            {
                int configId = 0;

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("app.sp_CreateSystemConfig", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ConfigKey", config.ConfigKey);
                        command.Parameters.AddWithValue("@ConfigValue", config.ConfigValue);
                        command.Parameters.AddWithValue("@Description", (object)config.Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DataType", (object)config.DataType ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IsReadOnly", config.IsReadOnly);
                        command.Parameters.AddWithValue("@CreatedBy", createdBy);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                configId = Convert.ToInt32(reader["ConfigID"]);
                            }
                        }
                    }
                }

                _logger.LogInfo($"Tạo cấu hình mới: {config.ConfigKey}, ID: {configId}");
                return configId;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigRepository.CreateConfig", ex);
                throw;
            }
        }

        /// <summary>
        /// Cập nhật cấu hình
        /// </summary>
        /// <param name="config">Cấu hình cần cập nhật</param>
        /// <param name="modifiedBy">ID người cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        public bool UpdateConfig(SystemConfig config, int modifiedBy)
        {
            try
            {
                bool result = false;

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("app.sp_UpdateSystemConfig", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ConfigKey", config.ConfigKey);
                        command.Parameters.AddWithValue("@ConfigValue", config.ConfigValue);
                        command.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader["Result"].ToString() == "Configuration updated successfully";
                            }
                        }
                    }
                }

                _logger.LogInfo($"Cập nhật cấu hình: {config.ConfigKey}, Kết quả: {result}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigRepository.UpdateConfig", ex);
                throw;
            }
        }

        /// <summary>
        /// Xóa cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <param name="deletedBy">ID người xóa</param>
        /// <returns>Kết quả xóa</returns>
        public bool DeleteConfig(string configKey, int deletedBy)
        {
            try
            {
                bool result = false;

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("app.sp_DeleteSystemConfig", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ConfigKey", configKey);
                        command.Parameters.AddWithValue("@DeletedBy", deletedBy);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader["Result"].ToString() == "Configuration deleted successfully";
                            }
                        }
                    }
                }

                _logger.LogInfo($"Xóa cấu hình: {configKey}, Kết quả: {result}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigRepository.DeleteConfig", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy lịch sử thay đổi cấu hình
        /// </summary>
        /// <param name="configKey">Khóa cấu hình</param>
        /// <returns>Lịch sử thay đổi</returns>
        public List<ActivityLog> GetConfigHistory(string configKey)
        {
            try
            {
                List<ActivityLog> history = new List<ActivityLog>();
                SystemConfig config = GetConfigByKey(configKey);

                if (config == null)
                    return history;

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("app.sp_GetActivityLogs", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EntityType", "SystemConfig");
                        command.Parameters.AddWithValue("@EntityID", config.ConfigID);
                        command.Parameters.AddWithValue("@MaxRows", 100);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                history.Add(new ActivityLog
                                {
                                    ActivityID = Convert.ToInt32(reader["ActivityID"]),
                                    AccountID = Convert.ToInt32(reader["AccountID"]),
                                    ActivityType = reader["ActivityType"].ToString(),
                                    EntityType = reader["EntityType"].ToString(),
                                    EntityID = Convert.ToInt32(reader["EntityID"]),
                                    Description = reader["Description"].ToString(),
                                    OldValue = reader["OldValue"] != DBNull.Value ? reader["OldValue"].ToString() : null,
                                    NewValue = reader["NewValue"] != DBNull.Value ? reader["NewValue"].ToString() : null,
                                    IPAddress = reader["IPAddress"] != DBNull.Value ? reader["IPAddress"].ToString() : null,
                                    ActivityDate = Convert.ToDateTime(reader["ActivityDate"]),
                                    Username = reader["Username"].ToString(),
                                    EmployeeName = reader["EmployeeName"] != DBNull.Value ? reader["EmployeeName"].ToString() : null
                                });
                            }
                        }
                    }
                }

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigRepository.GetConfigHistory", ex);
                throw;
            }
        }

        /// <summary>
        /// Lấy cấu hình theo loại
        /// </summary>
        /// <param name="dataType">Loại dữ liệu cấu hình</param>
        /// <returns>Danh sách cấu hình</returns>
        public List<SystemConfig> GetConfigsByType(string dataType)
        {
            try
            {
                return GetAllConfigs().Where(c => c.DataType == dataType).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigRepository.GetConfigsByType", ex);
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
                SystemConfig config = GetConfigByKey(configKey);
                return config != null ? config.ConfigValue : defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogError("SystemConfigRepository.GetConfigValue", ex);
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
                _logger.LogError("SystemConfigRepository.GetConfigValueInt", ex);
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
                _logger.LogError("SystemConfigRepository.GetConfigValueBool", ex);
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
                _logger.LogError("SystemConfigRepository.GetConfigValueDecimal", ex);
                return defaultValue;
            }
        }
    }
}