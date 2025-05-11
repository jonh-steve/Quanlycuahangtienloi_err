// Steve-Thuong_hai
// Bổ sung cho ReportService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Models.Repositories;

namespace QuanLyCuaHangTienLoi.Services
{
    // Thêm các DTO cho báo cáo doanh số
    public class DailySalesReportDTO
    {
        public DateTime SalesDate { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalTax { get; set; }
        public decimal NetSales { get; set; }
    }

    public class ProductSalesReportDTO
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
        public decimal Profit { get; set; }
    }

    public class CategorySalesReportDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
        public decimal Profit { get; set; }
        public decimal Percentage { get; set; }
    }

    public partial class ReportService
    {
        // Bổ sung các properties
        private readonly OrderRepository _orderRepository;
        private readonly ProductRepository _productRepository;

        // Bổ sung constructor
        public ReportService(CustomerRepository customerRepository,
            OrderRepository orderRepository, ProductRepository productRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = new Logger();
        }

        // Phương thức lấy báo cáo doanh số theo ngày
        public List<DailySalesReportDTO> GetSalesReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Gọi stored procedure để lấy dữ liệu
                var report = _orderRepository.GetDailySalesReport(startDate, endDate);

                if (report == null || !report.Any())
                {
                    return new List<DailySalesReportDTO>();
                }

                // Chuyển đổi dữ liệu từ dynamic sang DTO
                var result = new List<DailySalesReportDTO>();

                foreach (var item in report)
                {
                    result.Add(new DailySalesReportDTO
                    {
                        SalesDate = item.SalesDate,
                        OrderCount = item.OrderCount,
                        TotalSales = item.GrossSales,
                        TotalTax = item.TotalTax,
                        NetSales = item.NetSales
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi khi lấy báo cáo doanh số: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        // Phương thức lấy báo cáo doanh số theo sản phẩm
        public List<ProductSalesReportDTO> GetProductSalesReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Gọi stored procedure để lấy dữ liệu
                var report = _productRepository.GetProductSalesReport(startDate, endDate);

                if (report == null || !report.Any())
                {
                    return new List<ProductSalesReportDTO>();
                }

                // Chuyển đổi dữ liệu từ dynamic sang DTO
                var result = new List<ProductSalesReportDTO>();

                foreach (var item in report)
                {
                    result.Add(new ProductSalesReportDTO
                    {
                        ProductID = item.ProductID,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        CategoryName = item.CategoryName,
                        QuantitySold = item.TotalQuantitySold,
                        TotalSales = item.TotalSales,
                        Profit = item.EstimatedProfit
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi khi lấy báo cáo doanh số theo sản phẩm: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        // Phương thức lấy báo cáo doanh số theo danh mục
        public List<CategorySalesReportDTO> GetCategorySalesReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Gọi stored procedure để lấy dữ liệu
                var report = _productRepository.GetCategorySalesReport(startDate, endDate);

                if (report == null || !report.Any())
                {
                    return new List<CategorySalesReportDTO>();
                }

                // Chuyển đổi dữ liệu từ dynamic sang DTO
                var result = new List<CategorySalesReportDTO>();

                // Tính tổng doanh thu để tính tỷ lệ phần trăm
                decimal totalSales = report.Sum(r => r.TotalSales);

                foreach (var item in report)
                {
                    result.Add(new CategorySalesReportDTO
                    {
                        CategoryID = item.CategoryID,
                        CategoryName = item.CategoryName,
                        ProductCount = item.ProductCount,
                        QuantitySold = item.TotalQuantitySold,
                        TotalSales = item.TotalSales,
                        Profit = item.TotalProfit,
                        Percentage = totalSales > 0 ? item.TotalSales / totalSales : 0
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi khi lấy báo cáo doanh số theo danh mục: {ex.Message}", LogLevel.Error);
                throw;
            }
        }
        // Phương thức xuất báo cáo doanh số ra Excel
        public bool ExportSalesReportToExcel(string filePath, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Lấy dữ liệu báo cáo
                var salesReport = GetSalesReport(startDate, endDate);
                var productSalesReport = GetProductSalesReport(startDate, endDate);
                var categorySalesReport = GetCategorySalesReport(startDate, endDate);

                _logger.Log($"Xuất báo cáo doanh số ra Excel: {filePath}", LogLevel.Info);

                // Tạo thư mục chứa file nếu chưa tồn tại
                string directory = System.IO.Path.GetDirectoryName(filePath);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                // Trong thực tế, sẽ sử dụng EPPlus để xuất Excel
                // Đây là mô phỏng xuất Excel
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    // Tạo sheet Tổng quan
                    var summarySheet = package.Workbook.Worksheets.Add("Tổng quan");
                    summarySheet.Cells[1, 1].Value = $"BÁO CÁO DOANH SỐ BÁN HÀNG ({startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy})";
                    summarySheet.Cells[1, 1, 1, 6].Merge = true;

                    // Thêm thông tin tổng quan
                    summarySheet.Cells[3, 1].Value = "Tổng số đơn hàng:";
                    summarySheet.Cells[3, 2].Value = salesReport.Sum(s => s.OrderCount);

                    summarySheet.Cells[4, 1].Value = "Tổng doanh thu:";
                    summarySheet.Cells[4, 2].Value = salesReport.Sum(s => s.TotalSales);

                    summarySheet.Cells[5, 1].Value = "Tổng thuế:";
                    summarySheet.Cells[5, 2].Value = salesReport.Sum(s => s.TotalTax);

                    summarySheet.Cells[6, 1].Value = "Doanh thu thuần:";
                    summarySheet.Cells[6, 2].Value = salesReport.Sum(s => s.NetSales);

                    // Tạo sheet Doanh số theo ngày
                    var dailySheet = package.Workbook.Worksheets.Add("Doanh số theo ngày");
                    dailySheet.Cells[1, 1].Value = "Ngày";
                    dailySheet.Cells[1, 2].Value = "Số đơn hàng";
                    dailySheet.Cells[1, 3].Value = "Doanh thu";
                    dailySheet.Cells[1, 4].Value = "Thuế";
                    dailySheet.Cells[1, 5].Value = "Doanh thu thuần";

                    // Thêm dữ liệu doanh số theo ngày
                    int row = 2;
                    foreach (var item in salesReport)
                    {
                        dailySheet.Cells[row, 1].Value = item.SalesDate.ToString("dd/MM/yyyy");
                        dailySheet.Cells[row, 2].Value = item.OrderCount;
                        dailySheet.Cells[row, 3].Value = item.TotalSales;
                        dailySheet.Cells[row, 4].Value = item.TotalTax;
                        dailySheet.Cells[row, 5].Value = item.NetSales;
                        row++;
                    }

                    // Tạo sheet Doanh số theo sản phẩm
                    var productSheet = package.Workbook.Worksheets.Add("Doanh số theo sản phẩm");
                    productSheet.Cells[1, 1].Value = "STT";
                    productSheet.Cells[1, 2].Value = "Mã SP";
                    productSheet.Cells[1, 3].Value = "Tên sản phẩm";
                    productSheet.Cells[1, 4].Value = "Danh mục";
                    productSheet.Cells[1, 5].Value = "Số lượng bán";
                    productSheet.Cells[1, 6].Value = "Doanh thu";
                    productSheet.Cells[1, 7].Value = "Lợi nhuận";

                    // Thêm dữ liệu doanh số theo sản phẩm
                    row = 2;
                    foreach (var item in productSalesReport)
                    {
                        productSheet.Cells[row, 1].Value = row - 1;
                        productSheet.Cells[row, 2].Value = item.ProductCode;
                        productSheet.Cells[row, 3].Value = item.ProductName;
                        productSheet.Cells[row, 4].Value = item.CategoryName;
                        productSheet.Cells[row, 5].Value = item.QuantitySold;
                        productSheet.Cells[row, 6].Value = item.TotalSales;
                        productSheet.Cells[row, 7].Value = item.Profit;
                        row++;
                    }

                    // Lưu file Excel
                    var fileInfo = new System.IO.FileInfo(filePath);
                    package.SaveAs(fileInfo);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi khi xuất báo cáo doanh số ra Excel: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        // Phương thức xuất báo cáo doanh số ra PDF
        public bool ExportSalesReportToPdf(string filePath, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Lấy dữ liệu báo cáo
                var salesReport = GetSalesReport(startDate, endDate);
                var productSalesReport = GetProductSalesReport(startDate, endDate);
                var categorySalesReport = GetCategorySalesReport(startDate, endDate);

                _logger.Log($"Xuất báo cáo doanh số ra PDF: {filePath}", LogLevel.Info);

                // Tạo thư mục chứa file nếu chưa tồn tại
                string directory = System.IO.Path.GetDirectoryName(filePath);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                // Trong thực tế, sẽ sử dụng iTextSharp để xuất PDF
                // Đây là mô phỏng xuất PDF
                using (System.IO.FileStream fs = System.IO.File.Create(filePath))
                {
                    // Trong thực tế, sẽ sử dụng iTextSharp để tạo nội dung PDF
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi khi xuất báo cáo doanh số ra PDF: {ex.Message}", LogLevel.Error);
                throw;
            }
        }
    }
}