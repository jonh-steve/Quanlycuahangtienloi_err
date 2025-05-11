using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class AccountSessionDTO
    {
        public int AccountID { get; set; }
        public string Username { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string SessionToken { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
