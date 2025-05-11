// Mã gợi ý cho IAccountRepository.cs
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public interface IAccountRepository
    {
        AccountDTO Authenticate(string username, string password);
        Account GetById(int accountId);
        List<Account> GetAll();
        List<Account> GetByRole(int roleId);
        int Create(Account account, string password);
        bool Update(Account account);
        bool ChangePassword(int accountId, string oldPassword, string newPassword);
        bool Delete(int accountId);
        List<Role> GetAllRoles();
    }
}