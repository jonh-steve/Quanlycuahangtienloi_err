// Steve-Thuong_hai
using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class Customer
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        public string CustomerName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Address { get; set; }

        public string MembershipLevel { get; set; } // Regular, Silver, Gold, etc.

        public int Points { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // Constructor
        public Customer()
        {
            MembershipLevel = "Regular";
            Points = 0;
            CreatedDate = DateTime.Now;
        }
    }
}