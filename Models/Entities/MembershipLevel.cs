// Steve-Thuong_hai
namespace QuanLyCuaHangTienLoi.Models.Entities
{
    public class MembershipLevel
    {
        public int LevelID { get; set; }

        public string LevelName { get; set; }

        public string Description { get; set; }

        public int MinimumPoints { get; set; }

        public decimal DiscountPercentage { get; set; }

        public bool IsActive { get; set; }

        // Constructor
        public MembershipLevel()
        {
            IsActive = true;
        }

        // Các cấp độ membership mặc định
        public static MembershipLevel[] DefaultLevels = new MembershipLevel[]
        {
            new MembershipLevel { LevelID = 1, LevelName = "Regular", MinimumPoints = 0, DiscountPercentage = 0 },
            new MembershipLevel { LevelID = 2, LevelName = "Silver", MinimumPoints = 1000, DiscountPercentage = 3 },
            new MembershipLevel { LevelID = 3, LevelName = "Gold", MinimumPoints = 3000, DiscountPercentage = 5 },
            new MembershipLevel { LevelID = 4, LevelName = "Platinum", MinimumPoints = 5000, DiscountPercentage = 10 }
        };
    }
}