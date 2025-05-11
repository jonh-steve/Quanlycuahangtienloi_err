// Mã gợi ý cho AccountService.cs
using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Db.Repositories;

namespace QuanLyCuaHangTienLoi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private AccountDTO _currentAccount;
        private static AccountService _instance;

        // Singleton pattern
        public static AccountService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AccountService();
                return _instance;
            }
        }

        private AccountService()
        {
            _accountRepository = new AccountRepository();
        }

        public AccountDTO Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            _currentAccount = _accountRepository.Authenticate(username, password);
            return _currentAccount;
        }

        public bool Logout()
        {
            _currentAccount = null;
            return true;
        }

        public Account GetCurrentAccount()
        {
            if (_currentAccount == null)
                return null;

            return _accountRepository.GetById(_currentAccount.AccountID);
        }

        public bool IsAuthenticated()
        {
            return _currentAccount != null;
        }

        public bool HasPermission(string permissionKey)
        {
            if (_currentAccount == null)
                return false;

            // Cài đặt đơn giản theo vai trò, có thể mở rộng thành hệ thống phân quyền chi tiết sau
            switch (permissionKey.ToLower())
            {
                case "admin":
                    return _currentAccount.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                case "manager":
                    return _currentAccount.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                           _currentAccount.RoleName.Equals("Manager", StringComparison.OrdinalIgnoreCase);
                case "user":
                    return true; // Mọi người dùng đều có quyền cơ bản
                default:
                    return false;
            }
        }

        public List<Account> GetAllAccounts()
        {
            return _accountRepository.GetAll();
        }

        public Account GetAccountById(int accountId)
        {
            return _accountRepository.GetById(accountId);
        }

        public List<Account> GetAccountsByRole(int roleId)
        {
            return _accountRepository.GetByRole(roleId);
        }

        public bool CreateAccount(Account account, string password)
        {
            if (string.IsNullOrEmpty(account.Username) || string.IsNullOrEmpty(password))
                return false;

            int result = _accountRepository.Create(account, password);
            return result > 0;
        }

        public bool UpdateAccount(Account account)
        {
            if (account == null || account.AccountID <= 0)
                return false;

            return _accountRepository.Update(account);
        }

        public bool ChangePassword(int accountId, string oldPassword, string newPassword)
        {
            if (accountId <= 0 || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                return false;

            return _accountRepository.ChangePassword(accountId, oldPassword, newPassword);
        }

        public bool DeleteAccount(int accountId)
        {
            if (accountId <= 0)
                return false;

            return _accountRepository.Delete(accountId);
        }

        public List<Role> GetAllRoles()
        {
            return _accountRepository.GetAllRoles();
        }
    }
}