// File: Services/ReportService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Interfaces;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Services
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        // Cache cho báo cáo để tối ưu hiệu suất
        private static Dictionary<string, object> _reportCache = new Dictionary<string, object>();
        private static readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);
        private static Dictionary<string, DateTime> _cacheExpirationTimes = new Dictionary<string, DateTime>();

        public ReportService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository,
            ICategoryRepository categoryRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _categoryRepository = categoryRepository;
        }

        public SalesReportDTO GetSalesReport(DateTime startDate, DateTime endDate)
        {
            string cacheKey = $"SalesReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

            // Kiểm tra cache
            if (_reportCache.ContainsKey(cacheKey) &&
                _cacheExpirationTimes.ContainsKey(cacheKey) &&
                _cacheExpirationTimes[cacheKey] > DateTime.Now)
            {
                return (SalesReportDTO)_reportCache[cacheKey];
            }

            // Nếu không có trong cache hoặc cache đã hết hạn, truy vấn dữ liệu
            var orders = _orderRepository.GetOrdersByDateRange(startDate, endDate, "Paid");

            var salesReport = new SalesReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalOrders = orders.Count,
                TotalSales = orders.Sum(o => o.FinalAmount),
                AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.FinalAmount) : 0
            };

            // Tính toán doanh số theo ngày
            salesReport.DailySales = CalculateDailySales(orders, startDate, endDate);

            // Lấy top sản phẩm bán chạy
            salesReport.TopProducts = GetTopSellingProducts(startDate, endDate, 10);

            // Lấy thống kê phương thức thanh toán
            salesReport.PaymentMethodStats = GetPaymentMethodStats(orders);

            // Lấy chi tiết doanh số
            salesReport.DetailedSales = GetDetailedSales(orders);

            // Lưu vào cache
            _reportCache[cacheKey] = salesReport;
            _cacheExpirationTimes[cacheKey] = DateTime.Now.Add(_cacheExpiration);

            return salesReport;
        }

        public ProfitReportDTO GetProfitReport(DateTime startDate, DateTime endDate)
        {
            string cacheKey = $"ProfitReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

            // Kiểm tra cache
            if (_reportCache.ContainsKey(cacheKey) &&
                _cacheExpirationTimes.ContainsKey(cacheKey) &&
                _cacheExpirationTimes[cacheKey] > DateTime.Now)
            {
                return (ProfitReportDTO)_reportCache[cacheKey];
            }

            // Nếu không có trong cache hoặc cache đã hết hạn, truy vấn dữ liệu
            var orders = _orderRepository.GetOrdersByDateRange(startDate, endDate, "Paid");

            // Tính tổng doanh thu
            decimal totalRevenue = orders.Sum(o => o.FinalAmount);

            // Tính tổng chi phí (giá vốn)
            decimal totalCost = _orderRepository.GetTotalCostForDateRange(startDate, endDate);

            // Tính lợi nhuận
            decimal grossProfit = totalRevenue - totalCost;

            // Tính lợi nhuận theo ngày
            var dailyProfits = CalculateDailyProfits(startDate, endDate);

            // Tính lợi nhuận theo danh mục
            var categoryProfits = CalculateCategoryProfits(startDate, endDate);

            var profitReport = new ProfitReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                GrossProfit = grossProfit,
                GrossProfitMargin = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0,
                DailyProfits = dailyProfits,
                CategoryProfits = categoryProfits
            };

            // Lưu vào cache
            _reportCache[cacheKey] = profitReport;
            _cacheExpirationTimes[cacheKey] = DateTime.Now.Add(_cacheExpiration);

            return profitReport;
        }

        public ProductPerformanceReportDTO GetProductPerformanceReport(DateTime startDate, DateTime endDate, int topCount = 20)
        {
            string cacheKey = $"ProductPerformanceReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}_{topCount}";

            // Kiểm tra cache
            if (_reportCache.ContainsKey(cacheKey) &&
                _cacheExpirationTimes.ContainsKey(cacheKey) &&
                _cacheExpirationTimes[cacheKey] > DateTime.Now)
            {
                return (ProductPerformanceReportDTO)_reportCache[cacheKey];
            }

            // Nếu không có trong cache hoặc cache đã hết hạn, truy vấn dữ liệu
            var topProducts = GetTopSellingProducts(startDate, endDate, topCount);
            var worstProducts = GetWorstSellingProducts(startDate, endDate, topCount);
            var productsProfitability = GetProductsProfitability(startDate, endDate, topCount);

            var productPerformanceReport = new ProductPerformanceReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TopSellingProducts = topProducts,
                WorstSellingProducts = worstProducts,
                MostProfitableProducts = productsProfitability
            };

            // Lưu vào cache
            _reportCache[cacheKey] = productPerformanceReport;
            _cacheExpirationTimes[cacheKey] = DateTime.Now.Add(_cacheExpiration);

            return productPerformanceReport;
        }

        public InventoryReportDTO GetInventoryReport()
        {
            string cacheKey = "InventoryReport";

            // Kiểm tra cache
            if (_reportCache.ContainsKey(cacheKey) &&
                _cacheExpirationTimes.ContainsKey(cacheKey) &&
                _cacheExpirationTimes[cacheKey] > DateTime.Now)
            {
                return (InventoryReportDTO)_reportCache[cacheKey];
            }

            var inventoryItems = _inventoryRepository.GetAllInventory();
            var products = _productRepository.GetAllProducts();

            // Tính tổng giá trị tồn kho
            decimal totalInventoryValue = inventoryItems.Sum(i => i.Quantity * products.FirstOrDefault(p => p.ProductID == i.ProductID)?.CostPrice ?? 0);

            // Tìm các sản phẩm sắp hết hàng
            var lowStockProducts = inventoryItems
                .Where(i => i.Quantity <= products.FirstOrDefault(p => p.ProductID == i.ProductID)?.MinimumStock)
                .Select(i => new LowStockProductDTO
                {
                    ProductID = i.ProductID,
                    ProductName = products.FirstOrDefault(p => p.ProductID == i.ProductID)?.ProductName,
                    CurrentStock = i.Quantity,
                    MinimumStock = products.FirstOrDefault(p => p.ProductID == i.ProductID)?.MinimumStock ?? 0,
                    ReorderQuantity = (products.FirstOrDefault(p => p.ProductID == i.ProductID)?.MinimumStock ?? 0) * 2 - i.Quantity
                })
                .ToList();

            // Phân tích tồn kho theo danh mục
            var inventoryByCategory = GetInventoryByCategory();

            var inventoryReport = new InventoryReportDTO
            {
                TotalProducts = products.Count,
                TotalInventoryValue = totalInventoryValue,
                LowStockProducts = lowStockProducts,
                InventoryByCategory = inventoryByCategory,
                InventoryDetails = inventoryItems.Select(i => new InventoryDetailDTO
                {
                    ProductID = i.ProductID,
                    ProductName = products.FirstOrDefault(p => p.ProductID == i.ProductID)?.ProductName,
                    CategoryName = products.FirstOrDefault(p => p.ProductID == i.ProductID)?.CategoryName,
                    CurrentStock = i.Quantity,
                    UnitCost = products.FirstOrDefault(p => p.ProductID == i.ProductID)?.CostPrice ?? 0,
                    TotalValue = i.Quantity * (products.FirstOrDefault(p => p.ProductID == i.ProductID)?.CostPrice ?? 0),
                    LastUpdated = i.LastUpdated
                }).ToList()
            };

            // Lưu vào cache
            _reportCache[cacheKey] = inventoryReport;
            _cacheExpirationTimes[cacheKey] = DateTime.Now.Add(_cacheExpiration);

            return inventoryReport;
        }

        public DashboardSummaryDTO GetDashboardSummary()
        {
            string cacheKey = "DashboardSummary";

            // Kiểm tra cache
            if (_reportCache.ContainsKey(cacheKey) &&
                _cacheExpirationTimes.ContainsKey(cacheKey) &&
                _cacheExpirationTimes[cacheKey] > DateTime.Now)
            {
                return (DashboardSummaryDTO)_reportCache[cacheKey];
            }

            // Tính toán dữ liệu dashboard
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            var lastWeekStart = today.AddDays(-7);
            var lastMonthStart = today.AddMonths(-1);

            // Doanh số hôm nay
            var todaySales = _orderRepository.GetOrdersByDateRange(today, today, "Paid")
                .Sum(o => o.FinalAmount);

            // Doanh số hôm qua
            var yesterdaySales = _orderRepository.GetOrdersByDateRange(yesterday, yesterday, "Paid")
                .Sum(o => o.FinalAmount);

            // Doanh số 7 ngày qua
            var lastWeekSales = _orderRepository.GetOrdersByDateRange(lastWeekStart, today, "Paid")
                .Sum(o => o.FinalAmount);

            // Doanh số 30 ngày qua
            var lastMonthSales = _orderRepository.GetOrdersByDateRange(lastMonthStart, today, "Paid")
                .Sum(o => o.FinalAmount);

            // Số đơn hàng hôm nay
            var todayOrders = _orderRepository.GetOrdersByDateRange(today, today, "Paid").Count;

            // Sản phẩm sắp hết hàng
            var lowStockCount = _inventoryRepository.GetLowStockProducts().Count;

            var dashboardSummary = new DashboardSummaryDTO
            {
                TodaySales = todaySales,
                YesterdaySales = yesterdaySales,
                LastWeekSales = lastWeekSales,
                LastMonthSales = lastMonthSales,
                TodayOrders = todayOrders,
                LowStockProductCount = lowStockCount,
                // Thêm các số liệu khác tùy vào yêu cầu
            };

            // Lưu vào cache
            _reportCache[cacheKey] = dashboardSummary;
            _cacheExpirationTimes[cacheKey] = DateTime.Now.Add(_cacheExpiration);

            return dashboardSummary;
        }

        public CategorySalesReportDTO GetCategorySalesReport(DateTime startDate, DateTime endDate)
        {
            string cacheKey = $"CategorySalesReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

            // Kiểm tra cache
            if (_reportCache.ContainsKey(cacheKey) &&
                _cacheExpirationTimes.ContainsKey(cacheKey) &&
                _cacheExpirationTimes[cacheKey] > DateTime.Now)
            {
                return (CategorySalesReportDTO)_reportCache[cacheKey];
            }

            // Lấy doanh số theo danh mục
            var categorySales = _orderRepository.GetCategorySales(startDate, endDate);

            var categorySalesReport = new CategorySalesReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                CategorySales = categorySales.Select(cs => new CategorySalesItemDTO
                {
                    CategoryID = cs.CategoryID,
                    CategoryName = cs.CategoryName,
                    TotalSales = cs.TotalSales,
                    Quantity = cs.Quantity,
                    Profit = cs.Profit,
                    ProfitMargin = cs.TotalSales > 0 ? (cs.Profit / cs.TotalSales) * 100 : 0,
                    Percentage = categorySales.Sum(c => c.TotalSales) > 0 ?
                        (cs.TotalSales / categorySales.Sum(c => c.TotalSales)) * 100 : 0
                }).ToList()
            };

            // Lưu vào cache
            _reportCache[cacheKey] = categorySalesReport;
            _cacheExpirationTimes[cacheKey] = DateTime.Now.Add(_cacheExpiration);

            return categorySalesReport;
        }

        // Các phương thức hỗ trợ
        private List<DailySalesDTO> CalculateDailySales(List<OrderDTO> orders, DateTime startDate, DateTime endDate)
        {
            var dailySales = new List<DailySalesDTO>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var ordersOnDate = orders.Where(o => o.OrderDate.Date == date.Date).ToList();

                dailySales.Add(new DailySalesDTO
                {
                    Date = date,
                    OrderCount = ordersOnDate.Count,
                    TotalAmount = ordersOnDate.Sum(o => o.FinalAmount)
                });
            }

            return dailySales;
        }

        private List<ProductSalesDTO> GetTopSellingProducts(DateTime startDate, DateTime endDate, int count)
        {
            return _orderRepository.GetTopSellingProducts(startDate, endDate, count);
        }

        private List<ProductSalesDTO> GetWorstSellingProducts(DateTime startDate, DateTime endDate, int count)
        {
            return _orderRepository.GetWorstSellingProducts(startDate, endDate, count);
        }

        private List<ProductProfitabilityDTO> GetProductsProfitability(DateTime startDate, DateTime endDate, int count)
        {
            return _orderRepository.GetProductsProfitability(startDate, endDate, count);
        }

        private List<PaymentMethodStatsDTO> GetPaymentMethodStats(List<OrderDTO> orders)
        {
            return orders
                .GroupBy(o => new { o.PaymentMethodID, o.PaymentMethod })
                .Select(g => new PaymentMethodStatsDTO
                {
                    MethodID = g.Key.PaymentMethodID,
                    MethodName = g.Key.PaymentMethod,
                    OrderCount = g.Count(),
                    Amount = g.Sum(o => o.FinalAmount),
                    Percentage = orders.Sum(o => o.FinalAmount) > 0 ?
                        (g.Sum(o => o.FinalAmount) / orders.Sum(o => o.FinalAmount)) * 100 : 0
                })
                .OrderByDescending(p => p.Amount)
                .ToList();
        }

        private List<SalesDetailDTO> GetDetailedSales(List<OrderDTO> orders)
        {
            return orders.Select(o => new SalesDetailDTO
            {
                OrderID = o.OrderID,
                OrderCode = o.OrderCode,
                OrderDate = o.OrderDate,
                CustomerName = o.CustomerName,
                EmployeeName = o.EmployeeName,
                TotalAmount = o.FinalAmount,
                PaymentMethod = o.PaymentMethod,
                ItemCount = o.ItemCount
            }).ToList();
        }

        private List<DailyProfitDTO> CalculateDailyProfits(DateTime startDate, DateTime endDate)
        {
            var dailyProfits = new List<DailyProfitDTO>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var ordersOnDate = _orderRepository.GetOrdersByDateRange(date, date, "Paid");
                decimal revenue = ordersOnDate.Sum(o => o.FinalAmount);
                decimal cost = _orderRepository.GetTotalCostForDate(date);

                dailyProfits.Add(new DailyProfitDTO
                {
                    Date = date,
                    Revenue = revenue,
                    Cost = cost,
                    Profit = revenue - cost,
                    ProfitMargin = revenue > 0 ? ((revenue - cost) / revenue) * 100 : 0
                });
            }

            return dailyProfits;
        }

        private List<CategoryProfitDTO> CalculateCategoryProfits(DateTime startDate, DateTime endDate)
        {
            return _orderRepository.GetCategoryProfits(startDate, endDate);
        }

        private List<InventoryCategoryDTO> GetInventoryByCategory()
        {
            var inventoryItems = _inventoryRepository.GetAllInventory();
            var products = _productRepository.GetAllProducts();
            var categories = _categoryRepository.GetAllCategories();

            return categories.Select(c => new InventoryCategoryDTO
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
                ProductCount = products.Count(p => p.CategoryID == c.CategoryID),
                TotalQuantity = inventoryItems
                    .Where(i => products.Any(p => p.ProductID == i.ProductID && p.CategoryID == c.CategoryID))
                    .Sum(i => i.Quantity),
                TotalValue = inventoryItems
                    .Where(i => products.Any(p => p.ProductID == i.ProductID && p.CategoryID == c.CategoryID))
                    .Sum(i => i.Quantity * (products.FirstOrDefault(p => p.ProductID == i.ProductID)?.CostPrice ?? 0))
            }).ToList();
        }

        // Phương thức xóa cache
        public void ClearReportCache()
        {
            _reportCache.Clear();
            _cacheExpirationTimes.Clear();
        }
    }
}