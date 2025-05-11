// Mã gợi ý cho AccountRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountDTO Authenticate(string username, string password)
        {
            try
            {
                // Mã hóa mật khẩu trước khi kiểm tra
                string passwordHash = Security.HashPassword(password);

                var parameters = new Dictionary<string, object>
                {
                    { "@Username", username },
                    { "@PasswordHash", passwordHash }
                };

                var data = ExecuteReader("app.sp_AuthenticateUser", parameters);

                if (data.Rows.Count == 0)
                    return null;

                var row = data.Rows[0];

                AccountDTO accountDTO = new AccountDTO
                {
                    AccountID = Convert.ToInt32(row["AccountID"]),
                    Username = row["Username"].ToString(),
                    Email = row["Email"].ToString(),
                    RoleID = Convert.ToInt32(row["RoleID"]),
                    RoleName = row["RoleName"].ToString()
                };

                // Kiểm tra nếu có thông tin nhân viên
                if (row["EmployeeID"] != DBNull.Value)
                {
                    accountDTO.EmployeeID = Convert.ToInt32(row["EmployeeID"]);
                    accountDTO.EmployeeName = row["EmployeeName"].ToString();
                }

                return accountDTO;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi xác thực tài khoản: {ex.Message}", ex);
                return null;
            }
        }

        public Account GetById(int accountId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@AccountID", accountId }
                };

                var data = ExecuteReader("SELECT * FROM app.Account WHERE AccountID = @AccountID", parameters);

                if (data.Rows.Count == 0)
                    return null;

                var row = data.Rows[0];

                Account account = new Account
                {
                    AccountID = Convert.ToInt32(row["AccountID"]),
                    Username = row["Username"].ToString(),
                    Email = row["Email"].ToString(),
                    RoleID = Convert.ToInt32(row["RoleID"]),
                    LastLogin = row["LastLogin"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["LastLogin"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedDate = row["ModifiedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ModifiedDate"]),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                };

                return account;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi lấy thông tin tài khoản: {ex.Message}", ex);
                return null;
            }
        }

        public List<Account> GetAll()
        {
            try
            {
                var data = ExecuteReader("app.sp_GetAllAccounts");
                List<Account> accounts = new List<Account>();

                foreach (DataRow row in data.Rows)
                {
                    Account account = new Account
                    {
                        AccountID = Convert.ToInt32(row["AccountID"]),
                        Username = row["Username"].ToString(),
                        Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString(),
                        RoleID = Convert.ToInt32(row["RoleID"]),
                        LastLogin = row["LastLogin"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["LastLogin"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        ModifiedDate = row["ModifiedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ModifiedDate"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };

                    accounts.Add(account);
                }

                return accounts;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi lấy danh sách tài khoản: {ex.Message}", ex);
                return new List<Account>();
            }
        }

        public List<Account> GetByRole(int roleId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@RoleID", roleId }
                };

                var data = ExecuteReader("SELECT * FROM app.Account WHERE RoleID = @RoleID", parameters);
                List<Account> accounts = new List<Account>();

                foreach (DataRow row in data.Rows)
                {
                    Account account = new Account
                    {
                        AccountID = Convert.ToInt32(row["AccountID"]),
                        Username = row["Username"].ToString(),
                        Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString(),
                        RoleID = Convert.ToInt32(row["RoleID"]),
                        LastLogin = row["LastLogin"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["LastLogin"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        ModifiedDate = row["ModifiedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ModifiedDate"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };

                    accounts.Add(account);
                }

                return accounts;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi lấy danh sách tài khoản theo vai trò: {ex.Message}", ex);
                return new List<Account>();
            }
        }

        public int Create(Account account, string password)
        {
            try
            {
                // Mã hóa mật khẩu
                string passwordHash = Security.HashPassword(password);
                string passwordSalt = Security.GenerateSalt();

                var parameters = new Dictionary<string, object>
                {
                    { "@Username", account.Username },
                    { "@PasswordHash", passwordHash },
                    { "@PasswordSalt", passwordSalt },
                    { "@Email", account.Email },
                    { "@RoleID", account.RoleID },
                    { "@CreatedBy", 1 } // Thay thế bằng ID người dùng hiện tại
                };

                var data = ExecuteReader("app.sp_CreateAccount", parameters);

                if (data.Rows.Count > 0)
                    return Convert.ToInt32(data.Rows[0]["AccountID"]);

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi tạo tài khoản: {ex.Message}", ex);
                return 0;
            }
        }

        public bool Update(Account account)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@AccountID", account.AccountID },
                    { "@Email", account.Email },
                    { "@RoleID", account.RoleID },
                    { "@IsActive", account.IsActive },
                    { "@ModifiedBy", 1 } // Thay thế bằng ID người dùng hiện tại
                };

                var result = ExecuteNonQuery("app.sp_UpdateAccount", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi cập nhật tài khoản: {ex.Message}", ex);
                return false;
            }
        }

        public bool ChangePassword(int accountId, string oldPassword, string newPassword)
        {
            try
            {
                // Mã hóa mật khẩu
                string oldPasswordHash = Security.HashPassword(oldPassword);
                string newPasswordHash = Security.HashPassword(newPassword);
                string newPasswordSalt = Security.GenerateSalt();

                var parameters = new Dictionary<string, object>
                {
                    { "@AccountID", accountId },
                    { "@OldPasswordHash", oldPasswordHash },
                    { "@NewPasswordHash", newPasswordHash },
                    { "@NewPasswordSalt", newPasswordSalt }
                };

                var result = ExecuteNonQuery("app.sp_ChangePassword", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi đổi mật khẩu: {ex.Message}", ex);
                return false;
            }
        }

        public bool Delete(int accountId)
        {
            try
            {
                // Thông thường không nên xóa tài khoản mà chỉ vô hiệu hóa
                var parameters = new Dictionary<string, object>
                {
                    { "@AccountID", accountId },
                    { "@IsActive", false },
                    { "@ModifiedBy", 1 } // Thay thế bằng ID người dùng hiện tại
                };

                var result = ExecuteNonQuery("UPDATE app.Account SET IsActive = @IsActive, ModifiedDate = GETDATE() WHERE AccountID = @AccountID", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi xóa tài khoản: {ex.Message}", ex);
                return false;
            }
        }

        public List<Role> GetAllRoles()
        {
            try
            {
                var data = ExecuteReader("SELECT * FROM app.Role");
                List<Role> roles = new List<Role>();

                foreach (DataRow row in data.Rows)
                {
                    Role role = new Role
                    {
                        RoleID = Convert.ToInt32(row["RoleID"]),
                        RoleName = row["RoleName"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };

                    roles.Add(role);
                }

                return roles;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi lấy danh sách vai trò: {ex.Message}", ex);
                return new List<Role>();
            }
        }
    }
}