// File: Utils/ReportExporter.cs
using System;
using System.IO;
using System.Drawing;
using QuanLyCuaHangTienLoi.Models.DTO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing.Chart;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;
using Font = iTextSharp.text.Font;

namespace QuanLyCuaHangTienLoi.Utils
{
    public class ReportExporter
    {
        private readonly string _exportPath;

        public ReportExporter()
        {
            // Tạo thư mục lưu báo cáo nếu không tồn tại
            _exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "QuanLyCuaHangTienLoi", "Reports");

            if (!Directory.Exists(_exportPath))
            {
                Directory.CreateDirectory(_exportPath);
            }

            // Đăng ký EPPlus license
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        #region Export Sales Report

        public string ExportSalesReportToExcel(SalesReportDTO salesReport, DateTime startDate, DateTime endDate)
        {
            string fileName = $"BaoCaoDoanhSo_{startDate:yyyyMMdd}_den_{endDate:yyyyMMdd}.xlsx";
            string filePath = Path.Combine(_exportPath, fileName);

            using (var package = new ExcelPackage())
            {
                // Tạo worksheet tổng quan
                var overviewSheet = package.Workbook.Worksheets.Add("Tổng quan");

                // Tiêu đề báo cáo
                overviewSheet.Cells[1, 1].Value = "BÁO CÁO DOANH SỐ BÁN HÀNG";
                overviewSheet.Cells[1, 1, 1, 6].Merge = true;
                overviewSheet.Cells[1, 1].Style.Font.Size = 16;
                overviewSheet.Cells[1, 1].Style.Font.Bold = true;
                overviewSheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Thông tin thời gian
                overviewSheet.Cells[2, 1].Value = $"Từ ngày: {startDate:dd/MM/yyyy} đến ngày: {endDate:dd/MM/yyyy}";
                overviewSheet.Cells[2, 1, 2, 6].Merge = true;
                overviewSheet.Cells[2, 1].Style.Font.Size = 12;
                overviewSheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Thông tin tổng quan
                overviewSheet.Cells[4, 1].Value = "Tổng số đơn hàng:";
                overviewSheet.Cells[4, 2].Value = salesReport.TotalOrders;
                overviewSheet.Cells[4, 2].Style.Numberformat.Format = "#,##0";

                overviewSheet.Cells[5, 1].Value = "Tổng doanh số:";
                overviewSheet.Cells[5, 2].Value = salesReport.TotalSales;
                overviewSheet.Cells[5, 2].Style.Numberformat.Format = "#,##0";

                overviewSheet.Cells[6, 1].Value = "Giá trị đơn hàng trung bình:";
                overviewSheet.Cells[6, 2].Value = salesReport.AverageOrderValue;
                overviewSheet.Cells[6, 2].Style.Numberformat.Format = "#,##0";

                // Doanh số theo ngày
                overviewSheet.Cells[8, 1].Value = "DOANH SỐ THEO NGÀY";
                overviewSheet.Cells[8, 1, 8, 6].Merge = true;
                overviewSheet.Cells[8, 1].Style.Font.Bold = true;
                overviewSheet.Cells[8, 1].Style.Font.Size = 12;

                overviewSheet.Cells[9, 1].Value = "Ngày";
                overviewSheet.Cells[9, 2].Value = "Số đơn hàng";
                overviewSheet.Cells[9, 3].Value = "Doanh số";

                overviewSheet.Cells[9, 1, 9, 3].Style.Font.Bold = true;
                overviewSheet.Cells[9, 1, 9, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                overviewSheet.Cells[9, 1, 9, 3].Style.Fill.BackgroundColor.SetColor(Color.LightPink);

                int row = 10;
                foreach (var daily in salesReport.DailySales)
                {
                    overviewSheet.Cells[row, 1].Value = daily.Date.ToString("dd/MM/yyyy");
                    overviewSheet.Cells[row, 2].Value = daily.OrderCount;
                    overviewSheet.Cells[row, 3].Value = daily.TotalAmount;
                    overviewSheet.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                    row++;
                }

                // Tạo biểu đồ doanh số theo ngày
                var dailyChart = overviewSheet.Drawings.AddChart("DailySalesChart", eChartType.Line);
                dailyChart.SetPosition(8, 0, 4, 0);
                dailyChart.SetSize(600, 400);
                dailyChart.Title.Text = "Doanh số theo ngày";

                var dailySeries = dailyChart.Series.Add(
                    overviewSheet.Cells[10, 3, row - 1, 3],
                    overviewSheet.Cells[10, 1, row - 1, 1]);
                dailySeries.Header = "Doanh số";

                // Tạo worksheet chi tiết đơn hàng
                var detailSheet = package.Workbook.Worksheets.Add("Chi tiết đơn hàng");

                // Tiêu đề báo cáo chi tiết
                detailSheet.Cells[1, 1].Value = "CHI TIẾT ĐƠN HÀNG";
                detailSheet.Cells[1, 1, 1, 8].Merge = true;
                detailSheet.Cells[1, 1].Style.Font.Size = 16;
                detailSheet.Cells[1, 1].Style.Font.Bold = true;
                detailSheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Thông tin thời gian
                detailSheet.Cells[2, 1].Value = $"Từ ngày: {startDate:dd/MM/yyyy} đến ngày: {endDate:dd/MM/yyyy}";
                detailSheet.Cells[2, 1, 2, 8].Merge = true;
                detailSheet.Cells[2, 1].Style.Font.Size = 12;
                detailSheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Tiêu đề cột
                detailSheet.Cells[4, 1].Value = "Mã đơn hàng";
                detailSheet.Cells[4, 2].Value = "Ngày";
                detailSheet.Cells[4, 3].Value = "Khách hàng";
                detailSheet.Cells[4, 4].Value = "Nhân viên";
                detailSheet.Cells[4, 5].Value = "Số lượng SP";
                detailSheet.Cells[4, 6].Value = "Tổng tiền";
                detailSheet.Cells[4, 7].Value = "Phương thức thanh toán";

                detailSheet.Cells[4, 1, 4, 7].Style.Font.Bold = true;
                detailSheet.Cells[4, 1, 4, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                detailSheet.Cells[4, 1, 4, 7].Style.Fill.BackgroundColor.SetColor(Color.LightPink);

                // Dữ liệu chi tiết
                row = 5;
                foreach (var detail in salesReport.DetailedSales)
                {
                    detailSheet.Cells[row, 1].Value = detail.OrderCode;
                    detailSheet.Cells[row, 2].Value = detail.OrderDate.ToString("dd/MM/yyyy HH:mm");
                    detailSheet.Cells[row, 3].Value = detail.CustomerName;
                    detailSheet.Cells[row, 4].Value = detail.EmployeeName;
                    detailSheet.Cells[row, 5].Value = detail.ItemCount;
                    detailSheet.Cells[row, 6].Value = detail.TotalAmount;
                    detailSheet.Cells[row, 7].Value = detail.PaymentMethod;

                    detailSheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";
                    row++;
                }

                // Định dạng tất cả các cột
                overviewSheet.Cells.AutoFitColumns();
                detailSheet.Cells.AutoFitColumns();

                // Tạo worksheet Top sản phẩm
                var topProductsSheet = package.Workbook.Worksheets.Add("Top sản phẩm");

                // Tiêu đề
                topProductsSheet.Cells[1, 1].Value = "TOP SẢN PHẨM BÁN CHẠY";
                topProductsSheet.Cells[1, 1, 1, 6].Merge = true;
                topProductsSheet.Cells[1, 1].Style.Font.Size = 16;
                topProductsSheet.Cells[1, 1].Style.Font.Bold = true;
                topProductsSheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Tiêu đề cột
                topProductsSheet.Cells[3, 1].Value = "STT";
                topProductsSheet.Cells[3, 2].Value = "Mã sản phẩm";
                topProductsSheet.Cells[3, 3].Value = "Tên sản phẩm";
                topProductsSheet.Cells[3, 4].Value = "Số lượng đã bán";
                topProductsSheet.Cells[3, 5].Value = "Doanh số";

                topProductsSheet.Cells[3, 1, 3, 5].Style.Font.Bold = true;
                topProductsSheet.Cells[3, 1, 3, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                topProductsSheet.Cells[3, 1, 3, 5].Style.Fill.BackgroundColor.SetColor(Color.LightPink);

                // Dữ liệu sản phẩm
                row = 4;
                int stt = 1;
                foreach (var product in salesReport.TopProducts)
                {
                    topProductsSheet.Cells[row, 1].Value = stt++;
                    topProductsSheet.Cells[row, 2].Value = product.ProductCode;
                    topProductsSheet.Cells[row, 3].Value = product.ProductName;
                    topProductsSheet.Cells[row, 4].Value = product.Quantity;
                    topProductsSheet.Cells[row, 5].Value = product.TotalAmount;

                    topProductsSheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
                    row++;
                }

                // Tạo biểu đồ top sản phẩm
                var topProductsChart = topProductsSheet.Drawings.AddChart("TopProductsChart", eChartType.Pie);
                topProductsChart.SetPosition(3, 0, 6, 0);
                topProductsChart.SetSize(500, 400);
                topProductsChart.Title.Text = "Top 5 sản phẩm bán chạy";

                var topSeries = topProductsChart.Series.Add(
                    topProductsSheet.Cells[4, 5, row - 1, 5],
                    topProductsSheet.Cells[4, 3, row - 1, 3]);

                topProductsSheet.Cells.AutoFitColumns();

                // Lưu file
                package.SaveAs(new FileInfo(filePath));
            }

            return filePath;
        }

        public string ExportSalesReportToPdf(SalesReportDTO salesReport, DateTime startDate, DateTime endDate)
        {
            string fileName = $"BaoCaoDoanhSo_{startDate:yyyyMMdd}_den_{endDate:yyyyMMdd}.pdf";
            string filePath = Path.Combine(_exportPath, fileName);

            Document document = new Document(PageSize.A4, 30, 30, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();

            // Font chữ
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font titleFont = new Font(baseFont, 16, Font.BOLD);
            Font subtitleFont = new Font(baseFont, 12, Font.ITALIC);
            Font headerFont = new Font(baseFont, 12, Font.BOLD);
            Font normalFont = new Font(baseFont, 10, Font.NORMAL);
            Font smallFont = new Font(baseFont, 8, Font.NORMAL);

            // Tiêu đề báo cáo
            Paragraph title = new Paragraph("BÁO CÁO DOANH SỐ BÁN HÀNG", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            // Thông tin thời gian
            Paragraph date = new Paragraph($"Từ ngày: {startDate:dd/MM/yyyy} đến ngày: {endDate:dd/MM/yyyy}", subtitleFont);
            date.Alignment = Element.ALIGN_CENTER;
            date.SpacingAfter = 20;
            document.Add(date);

            // Thông tin tổng quan
            PdfPTable overviewTable = new PdfPTable(2);
            overviewTable.WidthPercentage = 60;
            overviewTable.HorizontalAlignment = Element.ALIGN_CENTER;
            overviewTable.SpacingAfter = 20;

            overviewTable.AddCell(CreateCell("Tổng số đơn hàng:", headerFont, Element.ALIGN_LEFT));
            overviewTable.AddCell(CreateCell(salesReport.TotalOrders.ToString("#,##0"), normalFont, Element.ALIGN_RIGHT));

            overviewTable.AddCell(CreateCell("Tổng doanh số:", headerFont, Element.ALIGN_LEFT));
            overviewTable.AddCell(CreateCell(salesReport.TotalSales.ToString("#,##0") + " VNĐ", normalFont, Element.ALIGN_RIGHT));

            overviewTable.AddCell(CreateCell("Giá trị đơn hàng trung bình:", headerFont, Element.ALIGN_LEFT));
            overviewTable.AddCell(CreateCell(salesReport.AverageOrderValue.ToString("#,##0") + " VNĐ", normalFont, Element.ALIGN_RIGHT));

            document.Add(overviewTable);

            // Tiêu đề doanh số theo ngày
            Paragraph dailyTitle = new Paragraph("DOANH SỐ THEO NGÀY", headerFont);
            dailyTitle.Alignment = Element.ALIGN_CENTER;
            dailyTitle.SpacingAfter = 10;
            document.Add(dailyTitle);

            // Bảng doanh số theo ngày
            PdfPTable dailyTable = new PdfPTable(3);
            dailyTable.WidthPercentage = 100;
            dailyTable.SpacingAfter = 20;

            // Tiêu đề cột
            dailyTable.AddCell(CreateCell("Ngày", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            dailyTable.AddCell(CreateCell("Số đơn hàng", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            dailyTable.AddCell(CreateCell("Doanh số", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));

            // Dữ liệu theo ngày
            foreach (var daily in salesReport.DailySales)
            {
                dailyTable.AddCell(CreateCell(daily.Date.ToString("dd/MM/yyyy"), normalFont, Element.ALIGN_CENTER));
                dailyTable.AddCell(CreateCell(daily.OrderCount.ToString(), normalFont, Element.ALIGN_CENTER));
                dailyTable.AddCell(CreateCell(daily.TotalAmount.ToString("#,##0") + " VNĐ", normalFont, Element.ALIGN_RIGHT));
            }

            document.Add(dailyTable);

            // Tiêu đề top sản phẩm
            Paragraph topProductsTitle = new Paragraph("TOP 10 SẢN PHẨM BÁN CHẠY", headerFont);
            topProductsTitle.Alignment = Element.ALIGN_CENTER;
            topProductsTitle.SpacingAfter = 10;
            document.Add(topProductsTitle);

            // Bảng top sản phẩm
            PdfPTable productsTable = new PdfPTable(5);
            productsTable.WidthPercentage = 100;

            // Tiêu đề cột
            productsTable.AddCell(CreateCell("STT", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            productsTable.AddCell(CreateCell("Mã SP", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            productsTable.AddCell(CreateCell("Tên sản phẩm", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            productsTable.AddCell(CreateCell("Số lượng", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            productsTable.AddCell(CreateCell("Doanh số", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));

            // Dữ liệu top sản phẩm
            int stt = 1;
            foreach (var product in salesReport.TopProducts.Take(10))
            {
                productsTable.AddCell(CreateCell(stt++.ToString(), normalFont, Element.ALIGN_CENTER));
                productsTable.AddCell(CreateCell(product.ProductCode, normalFont, Element.ALIGN_CENTER));
                productsTable.AddCell(CreateCell(product.ProductName, normalFont, Element.ALIGN_LEFT));
                productsTable.AddCell(CreateCell(product.Quantity.ToString(), normalFont, Element.ALIGN_CENTER));
                productsTable.AddCell(CreateCell(product.TotalAmount.ToString("#,##0") + " VNĐ", normalFont, Element.ALIGN_RIGHT));
            }

            document.Add(productsTable);

            // Chân trang
            Paragraph footer = new Paragraph("Steve-Thuong_hai", smallFont);
            footer.Alignment = Element.ALIGN_RIGHT;
            document.Add(footer);

            document.Close();

            return filePath;
        }

        #endregion

        #region Export Inventory Report

        public string ExportInventoryReportToExcel(InventoryReportDTO inventoryReport)
        {
            string fileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd}.xlsx";
            string filePath = Path.Combine(_exportPath, fileName);

            using (var package = new ExcelPackage())
            {
                // Tạo worksheet tổng quan
                var overviewSheet = package.Workbook.Worksheets.Add("Tổng quan");

                // Tiêu đề báo cáo
                overviewSheet.Cells[1, 1].Value = "BÁO CÁO TỒN KHO";
                overviewSheet.Cells[1, 1, 1, 6].Merge = true;
                overviewSheet.Cells[1, 1].Style.Font.Size = 16;
                overviewSheet.Cells[1, 1].Style.Font.Bold = true;
                overviewSheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Thông tin thời gian
                overviewSheet.Cells[2, 1].Value = $"Ngày báo cáo: {DateTime.Now:dd/MM/yyyy}";
                overviewSheet.Cells[2, 1, 2, 6].Merge = true;
                overviewSheet.Cells[2, 1].Style.Font.Size = 12;
                overviewSheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Thông tin tổng quan
                overviewSheet.Cells[4, 1].Value = "Tổng số sản phẩm:";
                overviewSheet.Cells[4, 2].Value = inventoryReport.TotalProducts;
                overviewSheet.Cells[4, 2].Style.Numberformat.Format = "#,##0";

                overviewSheet.Cells[5, 1].Value = "Tổng giá trị tồn kho:";
                overviewSheet.Cells[5, 2].Value = inventoryReport.TotalInventoryValue;
                overviewSheet.Cells[5, 2].Style.Numberformat.Format = "#,##0";

                overviewSheet.Cells[6, 1].Value = "Số sản phẩm sắp hết hàng:";
                overviewSheet.Cells[6, 2].Value = inventoryReport.LowStockProducts.Count;
                overviewSheet.Cells[6, 2].Style.Numberformat.Format = "#,##0";

                // Tồn kho theo danh mục
                overviewSheet.Cells[8, 1].Value = "TỒN KHO THEO DANH MỤC";
                overviewSheet.Cells[8, 1, 8, 6].Merge = true;
                overviewSheet.Cells[8, 1].Style.Font.Bold = true;
                overviewSheet.Cells[8, 1].Style.Font.Size = 12;

                overviewSheet.Cells[9, 1].Value = "Danh mục";
                overviewSheet.Cells[9, 2].Value = "Số sản phẩm";
                overviewSheet.Cells[9, 3].Value = "Tồn kho";
                overviewSheet.Cells[9, 4].Value = "Giá trị";

                overviewSheet.Cells[9, 1, 9, 4].Style.Font.Bold = true;
                overviewSheet.Cells[9, 1, 9, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                overviewSheet.Cells[9, 1, 9, 4].Style.Fill.BackgroundColor.SetColor(Color.LightPink);

                int row = 10;
                foreach (var category in inventoryReport.InventoryByCategory)
                {
                    overviewSheet.Cells[row, 1].Value = category.CategoryName;
                    overviewSheet.Cells[row, 2].Value = category.ProductCount;
                    overviewSheet.Cells[row, 3].Value = category.TotalQuantity;
                    overviewSheet.Cells[row, 4].Value = category.TotalValue;
                    overviewSheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                    row++;
                }

                // Tạo biểu đồ tồn kho theo danh mục
                var categoryChart = overviewSheet.Drawings.AddChart("CategoryInventoryChart", eChartType.Pie);
                categoryChart.SetPosition(8, 0, 5, 0);
                categoryChart.SetSize(500, 400);
                categoryChart.Title.Text = "Giá trị tồn kho theo danh mục";

                var categorySeries = categoryChart.Series.Add(
                    overviewSheet.Cells[10, 4, row - 1, 4],
                    overviewSheet.Cells[10, 1, row - 1, 1]);

                // Tạo worksheet sản phẩm sắp hết hàng
                var lowStockSheet = package.Workbook.Worksheets.Add("Sản phẩm sắp hết hàng");

                // Tiêu đề
                lowStockSheet.Cells[1, 1].Value = "DANH SÁCH SẢN PHẨM SẮP HẾT HÀNG";
                lowStockSheet.Cells[1, 1, 1, 6].Merge = true;
                lowStockSheet.Cells[1, 1].Style.Font.Size = 16;
                lowStockSheet.Cells[1, 1].Style.Font.Bold = true;
                lowStockSheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Tiêu đề cột
                lowStockSheet.Cells[3, 1].Value = "Mã sản phẩm";
                lowStockSheet.Cells[3, 2].Value = "Tên sản phẩm";
                lowStockSheet.Cells[3, 3].Value = "Tồn kho hiện tại";
                lowStockSheet.Cells[3, 4].Value = "Tồn kho tối thiểu";
                lowStockSheet.Cells[3, 5].Value = "Cần nhập thêm";

                lowStockSheet.Cells[3, 1, 3, 5].Style.Font.Bold = true;
                lowStockSheet.Cells[3, 1, 3, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                lowStockSheet.Cells[3, 1, 3, 5].Style.Fill.BackgroundColor.SetColor(Color.LightPink);

                // Dữ liệu sản phẩm sắp hết hàng
                row = 4;
                foreach (var product in inventoryReport.LowStockProducts)
                {
                    lowStockSheet.Cells[row, 1].Value = product.ProductID;
                    lowStockSheet.Cells[row, 2].Value = product.ProductName;
                    lowStockSheet.Cells[row, 3].Value = product.CurrentStock;
                    lowStockSheet.Cells[row, 4].Value = product.MinimumStock;
                    lowStockSheet.Cells[row, 5].Value = product.ReorderQuantity;

                    // Định dạng màu sắc dựa trên mức độ
                    if (product.CurrentStock == 0)
                    {
                        lowStockSheet.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        lowStockSheet.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 150, 150)); // Đỏ nhạt
                    }
                    else if (product.CurrentStock < product.MinimumStock / 2)
                    {
                        lowStockSheet.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        lowStockSheet.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 200, 200)); // Hồng nhạt
                    }

                    row++;
                }

                // Tạo worksheet chi tiết tồn kho
                var detailSheet = package.Workbook.Worksheets.Add("Chi tiết tồn kho");

                // Tiêu đề
                detailSheet.Cells[1, 1].Value = "CHI TIẾT TỒN KHO";
                detailSheet.Cells[1, 1, 1, 7].Merge = true;
                detailSheet.Cells[1, 1].Style.Font.Size = 16;
                detailSheet.Cells[1, 1].Style.Font.Bold = true;
                detailSheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Tiêu đề cột
                detailSheet.Cells[3, 1].Value = "Mã sản phẩm";
                detailSheet.Cells[3, 2].Value = "Tên sản phẩm";
                detailSheet.Cells[3, 3].Value = "Danh mục";
                detailSheet.Cells[3, 4].Value = "Tồn kho";
                detailSheet.Cells[3, 5].Value = "Đơn giá";
                detailSheet.Cells[3, 6].Value = "Giá trị";
                detailSheet.Cells[3, 7].Value = "Cập nhật cuối";

                detailSheet.Cells[3, 1, 3, 7].Style.Font.Bold = true;
                detailSheet.Cells[3, 1, 3, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                detailSheet.Cells[3, 1, 3, 7].Style.Fill.BackgroundColor.SetColor(Color.LightPink);

                // Dữ liệu chi tiết tồn kho
                row = 4;
                foreach (var item in inventoryReport.InventoryDetails)
                {
                    detailSheet.Cells[row, 1].Value = item.ProductID;
                    detailSheet.Cells[row, 2].Value = item.ProductName;
                    detailSheet.Cells[row, 3].Value = item.CategoryName;
                    detailSheet.Cells[row, 4].Value = item.CurrentStock;
                    detailSheet.Cells[row, 5].Value = item.UnitCost;
                    detailSheet.Cells[row, 6].Value = item.TotalValue;
                    detailSheet.Cells[row, 7].Value = item.LastUpdated.ToString("dd/MM/yyyy HH:mm");

                    detailSheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
                    detailSheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";

                    // Định dạng màu sắc cho sản phẩm sắp hết hàng
                    if (inventoryReport.LowStockProducts.Any(p => p.ProductID == item.ProductID))
                    {
                        detailSheet.Cells[row, 1, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        detailSheet.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 200, 200)); // Hồng nhạt
                    }

                    row++;
                }

                // Định dạng tất cả các cột
                overviewSheet.Cells.AutoFitColumns();
                lowStockSheet.Cells.AutoFitColumns();
                detailSheet.Cells.AutoFitColumns();

                // Lưu file
                package.SaveAs(new FileInfo(filePath));
            }

            return filePath;
        }

        public string ExportInventoryReportToPdf(InventoryReportDTO inventoryReport)
        {
            string fileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd}.pdf";
            string filePath = Path.Combine(_exportPath, fileName);

            Document document = new Document(PageSize.A4.Rotate(), 30, 30, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();

            // Font chữ
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font titleFont = new Font(baseFont, 16, Font.BOLD);
            Font subtitleFont = new Font(baseFont, 12, Font.ITALIC);
            Font headerFont = new Font(baseFont, 12, Font.BOLD);
            Font normalFont = new Font(baseFont, 10, Font.NORMAL);
            Font smallFont = new Font(baseFont, 8, Font.NORMAL);

            // Tiêu đề báo cáo
            Paragraph title = new Paragraph("BÁO CÁO TỒN KHO", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            // Thông tin thời gian
            Paragraph date = new Paragraph($"Ngày báo cáo: {DateTime.Now:dd/MM/yyyy}", subtitleFont);
            date.Alignment = Element.ALIGN_CENTER;
            date.SpacingAfter = 20;
            document.Add(date);

            // Thông tin tổng quan
            PdfPTable overviewTable = new PdfPTable(2);
            overviewTable.WidthPercentage = 60;
            overviewTable.HorizontalAlignment = Element.ALIGN_CENTER;
            overviewTable.SpacingAfter = 20;

            overviewTable.AddCell(CreateCell("Tổng số sản phẩm:", headerFont, Element.ALIGN_LEFT));
            overviewTable.AddCell(CreateCell(inventoryReport.TotalProducts.ToString("#,##0"), normalFont, Element.ALIGN_RIGHT));

            overviewTable.AddCell(CreateCell("Tổng giá trị tồn kho:", headerFont, Element.ALIGN_LEFT));
            overviewTable.AddCell(CreateCell(inventoryReport.TotalInventoryValue.ToString("#,##0") + " VNĐ", normalFont, Element.ALIGN_RIGHT));

            overviewTable.AddCell(CreateCell("Số sản phẩm sắp hết hàng:", headerFont, Element.ALIGN_LEFT));
            overviewTable.AddCell(CreateCell(inventoryReport.LowStockProducts.Count.ToString("#,##0"), normalFont, Element.ALIGN_RIGHT));

            document.Add(overviewTable);

            // Tiêu đề sản phẩm sắp hết hàng
            if (inventoryReport.LowStockProducts.Count > 0)
            {
                Paragraph lowStockTitle = new Paragraph("DANH SÁCH SẢN PHẨM SẮP HẾT HÀNG", headerFont);
                lowStockTitle.Alignment = Element.ALIGN_CENTER;
                lowStockTitle.SpacingAfter = 10;
                document.Add(lowStockTitle);

                // Bảng sản phẩm sắp hết hàng
                PdfPTable lowStockTable = new PdfPTable(5);
                lowStockTable.WidthPercentage = 100;
                lowStockTable.SpacingAfter = 20;

                // Tiêu đề cột
                lowStockTable.AddCell(CreateCell("Mã SP", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
                lowStockTable.AddCell(CreateCell("Tên sản phẩm", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
                lowStockTable.AddCell(CreateCell("Tồn kho hiện tại", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
                lowStockTable.AddCell(CreateCell("Tồn kho tối thiểu", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
                lowStockTable.AddCell(CreateCell("Cần nhập thêm", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));

                // Dữ liệu sản phẩm sắp hết hàng
                foreach (var product in inventoryReport.LowStockProducts)
                {
                    lowStockTable.AddCell(CreateCell(product.ProductID.ToString(), normalFont, Element.ALIGN_CENTER));
                    lowStockTable.AddCell(CreateCell(product.ProductName, normalFont, Element.ALIGN_LEFT));
                    lowStockTable.AddCell(CreateCell(product.CurrentStock.ToString(), normalFont, Element.ALIGN_CENTER));
                    lowStockTable.AddCell(CreateCell(product.MinimumStock.ToString(), normalFont, Element.ALIGN_CENTER));
                    lowStockTable.AddCell(CreateCell(product.ReorderQuantity.ToString(), normalFont, Element.ALIGN_CENTER));
                }

                document.Add(lowStockTable);
            }

            // Tiêu đề tồn kho theo danh mục
            Paragraph categoryTitle = new Paragraph("TỒN KHO THEO DANH MỤC", headerFont);
            categoryTitle.Alignment = Element.ALIGN_CENTER;
            categoryTitle.SpacingAfter = 10;
            document.Add(categoryTitle);

            // Bảng tồn kho theo danh mục
            PdfPTable categoryTable = new PdfPTable(4);
            categoryTable.WidthPercentage = 100;
            categoryTable.SpacingAfter = 20;

            // Tiêu đề cột
            categoryTable.AddCell(CreateCell("Danh mục", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            categoryTable.AddCell(CreateCell("Số sản phẩm", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            categoryTable.AddCell(CreateCell("Số lượng tồn", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            categoryTable.AddCell(CreateCell("Giá trị", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));

            // Dữ liệu tồn kho theo danh mục
            foreach (var category in inventoryReport.InventoryByCategory)
            {
                categoryTable.AddCell(CreateCell(category.CategoryName, normalFont, Element.ALIGN_LEFT));
                categoryTable.AddCell(CreateCell(category.ProductCount.ToString(), normalFont, Element.ALIGN_CENTER));
                categoryTable.AddCell(CreateCell(category.TotalQuantity.ToString(), normalFont, Element.ALIGN_CENTER));
                categoryTable.AddCell(CreateCell(category.TotalValue.ToString("#,##0") + " VNĐ", normalFont, Element.ALIGN_RIGHT));
            }

            document.Add(categoryTable);

            // Chân trang
            Paragraph footer = new Paragraph("Steve-Thuong_hai", smallFont);
            footer.Alignment = Element.ALIGN_RIGHT;
            document.Add(footer);

            document.Close();

            return filePath;
        }

        public string ExportLowStockReportToPdf(List<LowStockProductDTO> lowStockProducts)
        {
            string fileName = $"BaoCaoSanPhamSapHetHang_{DateTime.Now:yyyyMMdd}.pdf";
            string filePath = Path.Combine(_exportPath, fileName);

            Document document = new Document(PageSize.A4, 30, 30, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();

            // Font chữ
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font titleFont = new Font(baseFont, 16, Font.BOLD);
            Font subtitleFont = new Font(baseFont, 12, Font.ITALIC);
            Font headerFont = new Font(baseFont, 12, Font.BOLD);
            Font normalFont = new Font(baseFont, 10, Font.NORMAL);
            Font smallFont = new Font(baseFont, 8, Font.NORMAL);

            // Tiêu đề báo cáo
            Paragraph title = new Paragraph("BÁO CÁO SẢN PHẨM SẮP HẾT HÀNG", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            // Thông tin thời gian
            Paragraph date = new Paragraph($"Ngày báo cáo: {DateTime.Now:dd/MM/yyyy}", subtitleFont);
            date.Alignment = Element.ALIGN_CENTER;
            date.SpacingAfter = 20;
            document.Add(date);

            // Bảng sản phẩm sắp hết hàng
            PdfPTable lowStockTable = new PdfPTable(5);
            lowStockTable.WidthPercentage = 100;
            lowStockTable.SpacingAfter = 20;

            // Tiêu đề cột
            lowStockTable.AddCell(CreateCell("Mã SP", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            lowStockTable.AddCell(CreateCell("Tên sản phẩm", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            lowStockTable.AddCell(CreateCell("Tồn kho hiện tại", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            lowStockTable.AddCell(CreateCell("Tồn kho tối thiểu", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));
            lowStockTable.AddCell(CreateCell("Cần nhập thêm", headerFont, Element.ALIGN_CENTER, new BaseColor(255, 182, 193)));

            // Dữ liệu sản phẩm sắp hết hàng
            foreach (var product in lowStockProducts)
            {
                PdfPCell cell = CreateCell(product.ProductID.ToString(), normalFont, Element.ALIGN_CENTER);
                if (product.CurrentStock == 0)
                {
                    cell.BackgroundColor = new BaseColor(255, 150, 150); // Đỏ nhạt
                }
                else if (product.CurrentStock < product.MinimumStock / 2)
                {
                    cell.BackgroundColor = new BaseColor(255, 200, 200); // Hồng nhạt
                }
                lowStockTable.AddCell(cell);

                cell = CreateCell(product.ProductName, normalFont, Element.ALIGN_LEFT);
                if (product.CurrentStock == 0)
                {
                    cell.BackgroundColor = new BaseColor(255, 150, 150);
                }
                else if (product.CurrentStock < product.MinimumStock / 2)
                {
                    cell.BackgroundColor = new BaseColor(255, 200, 200);
                }
                lowStockTable.AddCell(cell);

                cell = CreateCell(product.CurrentStock.ToString(), normalFont, Element.ALIGN_CENTER);
                if (product.CurrentStock == 0)
                {
                    cell.BackgroundColor = new BaseColor(255, 150, 150);
                }
                else if (product.CurrentStock < product.MinimumStock / 2)
                {
                    cell.BackgroundColor = new BaseColor(255, 200, 200);
                }
                lowStockTable.AddCell(cell);

                cell = CreateCell(product.MinimumStock.ToString(), normalFont, Element.ALIGN_CENTER);
                if (product.CurrentStock == 0)
                {
                    cell.BackgroundColor = new BaseColor(255, 150, 150);
                }
                else if (product.CurrentStock < product.MinimumStock / 2)
                {
                    cell.BackgroundColor = new BaseColor(255, 200, 200);
                }
                lowStockTable.AddCell(cell);

                cell = CreateCell(product.ReorderQuantity.ToString(), normalFont, Element.ALIGN_CENTER);
                if (product.CurrentStock == 0)
                {
                    cell.BackgroundColor = new BaseColor(255, 150, 150);
                }
                else if (product.CurrentStock < product.MinimumStock / 2)
                {
                    cell.BackgroundColor = new BaseColor(255, 200, 200);
                }
                lowStockTable.AddCell(cell);
            }

            document.Add(lowStockTable);

            // Chú thích
            Paragraph note = new Paragraph("Chú thích:", headerFont);
            note.SpacingAfter = 5;
            document.Add(note);

            PdfPTable noteTable = new PdfPTable(2);
            noteTable.WidthPercentage = 60;
            noteTable.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell redCell = new PdfPCell();
            redCell.BackgroundColor = new BaseColor(255, 150, 150);
            redCell.FixedHeight = 15;
            noteTable.AddCell(redCell);
            noteTable.AddCell(CreateCell("Sản phẩm đã hết hàng (cần nhập gấp)", normalFont, Element.ALIGN_LEFT));

            PdfPCell pinkCell = new PdfPCell();
            pinkCell.BackgroundColor = new BaseColor(255, 200, 200);
            pinkCell.FixedHeight = 15;
            noteTable.AddCell(pinkCell);
            noteTable.AddCell(CreateCell("Sản phẩm sắp hết hàng (dưới 50% mức tối thiểu)", normalFont, Element.ALIGN_LEFT));

            document.Add(noteTable);

            // Chân trang
            Paragraph footer = new Paragraph("Steve-Thuong_hai", smallFont);
            footer.Alignment = Element.ALIGN_RIGHT;
            document.Add(footer);

            document.Close();

            return filePath;
        }

        #endregion

        #region Helper Methods

        private PdfPCell CreateCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = alignment;
            cell.Padding = 5;
            return cell;
        }

        private PdfPCell CreateCell(string text, Font font, int alignment, BaseColor backgroundColor)
        {
            PdfPCell cell = CreateCell(text, font, alignment);
            cell.BackgroundColor = backgroundColor;
            return cell;
        }

        #endregion
    }
}