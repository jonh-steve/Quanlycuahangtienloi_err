// Tạo file trong thư mục /Models/DTO
using System;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class RoleDTO
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Thuộc tính bổ sung có thể thêm trong tương lai
        public int UserCount { get; set; }  // Số lượng người dùng có vai trò này
        public string DisplayName { get { return $"{RoleName} ({Description})"; } }
    }
}