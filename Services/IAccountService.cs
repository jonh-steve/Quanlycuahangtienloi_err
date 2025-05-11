// Mã gợi ý cho IAccountService.cs
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;

namespace QuanLyCuaHangTienLoi.Services
{
    public interface IAccountService
    {
        AccountDTO Login(string username, string password);
        bool Logout();
        Account GetCurrentAccount();
        bool IsAuthenticated();
        bool HasPermission(string permissionKey);
        List<Account> GetAllAccounts();
        Account GetAccountById(int accountId);
        List<Account> GetAccountsByRole(int roleId);
        bool CreateAccount(Account account, string password);
        bool UpdateAccount(Account account);
        bool ChangePassword(int accountId, string oldPassword, string newPassword);
        bool DeleteAccount(int accountId);
        List<Role> GetAllRoles();
    }
}