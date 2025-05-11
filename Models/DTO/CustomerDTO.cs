using System;

namespace QuanLyCuaHangTienLoi.Models.DTO
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string MembershipLevel { get; set; }
        public int Points { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal TotalSpent { get; set; }
        public int OrderCount { get; set; }
        public DateTime? LastOrderDate { get; set; }

        // Trường hiển thị
        public string FormattedTotalSpent => TotalSpent.ToString("N0") + " VNĐ";
        public string FormattedPoints => Points.ToString("N0");
        public string FormattedLastOrderDate => LastOrderDate.HasValue ? LastOrderDate.Value.ToString("dd/MM/yyyy") : "Chưa có đơn hàng";
        public string MembershipColor
        {
            get
            {
                switch (MembershipLevel)
                {
                    case "Gold": return "#FFD700";
                    case "Silver": return "#C0C0C0";
                    case "Platinum": return "#E5E4E2";
                    default: return "#FFFFFF";
                }
            }
        }
    }
}