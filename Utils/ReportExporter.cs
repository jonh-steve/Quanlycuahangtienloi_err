using System;
using System.Data;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QuanLyCuaHangTienLoi.Models.DTO;
using System.ComponentModel;

namespace QuanLyCuaHangTienLoi.Utils
{
    public class ReportExporter
    {
        private readonly Logger _logger;

        public ReportExporter(Logger logger)
        {
            _logger = logger;

            // Thiết lập giấy phép không thương mại cho EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public bool ExportSalesReportToExcel<T>(string filePath, string reportTitle, string[] headers, T[] data, object summary = null) where T : class
        {
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    // Tạo worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Báo cáo doanh số");

                    // Thiết lập tiêu đề báo cáo
                    worksheet.Cells[1, 1].Value = reportTitle;
                    worksheet.Cells[1, 1, 1, headers.Length].Merge = true;
                    worksheet.Cells[1, 1, 1, headers.Length].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, 1, headers.Length].Style.Font.Size = 14;
                    worksheet.Cells[1, 1, 1, headers.Length].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Thiết lập tiêu đề các cột
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[3, i + 1].Value = headers[i];
                        worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[3, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 157, 192));
                        worksheet.Cells[3, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Thiết lập độ rộng cột
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Column(i + 1).AutoFit();
                        worksheet.Column(i + 1).Width = worksheet.Column(i + 1).Width + 5;
                    }

                    // Thêm dữ liệu vào worksheet
                    var properties = typeof(T).GetProperties();

                    for (int i = 0; i < data.Length; i++)
                    {
                        var item = data[i];

                        for (int j = 0; j < properties.Length; j++)
                        {
                            var property = properties[j];
                            var value = property.GetValue(item);

                            if (value != null)
                            {
                                worksheet.Cells[i + 4, j + 1].Value = value;

                                // Định dạng cột kiểu số
                                if (value is decimal || value is double || value is float)
                                {
                                    worksheet.Cells[i + 4, j + 1].Style.Numberformat.Format = "#,##0";
                                    worksheet.Cells[i + 4, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                }
                                // Định dạng cột kiểu ngày tháng
                                else if (value is DateTime)
                                {
                                    worksheet.Cells[i + 4, j + 1].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 4, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }
                            }

                            // Thêm viền cho ô dữ liệu
                            worksheet.Cells[i + 4, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }

                    // Thêm thông tin tổng kết (nếu có)
                    if (summary != null)
                    {
                        var summaryRow = data.Length + 5;
                        worksheet.Cells[summaryRow, 1].Value = "TỔNG CỘNG:";
                        worksheet.Cells[summaryRow, 1].Style.Font.Bold = true;

                        var summaryProperties = summary.GetType().GetProperties();
                        for (int j = 0; j < summaryProperties.Length; j++)
                        {
                            var property = summaryProperties[j];
                            var value = property.GetValue(summary);

                            if (value != null)
                            {
                                // Tìm cột tương ứng
                                for (int col = 0; col < properties.Length; col++)
                                {
                                    if (properties[col].Name == property.Name)
                                    {
                                        worksheet.Cells[summaryRow, col + 1].Value = value;

                                        // Định dạng cột kiểu số
                                        if (value is decimal || value is double || value is float)
                                        {
                                            worksheet.Cells[summaryRow, col + 1].Style.Numberformat.Format = "#,##0";
                                            worksheet.Cells[summaryRow, col + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        }

                                        worksheet.Cells[summaryRow, col + 1].Style.Font.Bold = true;
                                        worksheet.Cells[summaryRow, col + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // Thêm thông tin thời gian xuất báo cáo
                    var timeRow = data.Length + 7;
                    worksheet.Cells[timeRow, 1].Value = "Thời gian xuất báo cáo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    worksheet.Cells[timeRow, 1, timeRow, headers.Length].Merge = true;
                    worksheet.Cells[timeRow, 1, timeRow, headers.Length].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[timeRow, 1, timeRow, headers.Length].Style.Font.Italic = true;

                    // Lưu file Excel
                    package.SaveAs(new FileInfo(filePath));
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in ExportSalesReportToExcel: {ex.Message}");
                return false;
            }
        }

        public bool ExportSalesReportToPdf<T>(string filePath, string reportTitle, string[] headers, T[] data, object summary = null) where T : class
        {
            try
            {
                // Tạo tài liệu PDF
                using (Document document = new Document(PageSize.A4.Rotate(), 20, 20, 30, 30))
                {
                    // Tạo writer
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                    // Mở tài liệu
                    document.Open();

                    // Tạo font
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font titleFont = new Font(baseFont, 14, Font.BOLD);
                    Font headerFont = new Font(baseFont, 10, Font.BOLD);
                    Font normalFont = new Font(baseFont, 10, Font.NORMAL);
                    Font boldFont = new Font(baseFont, 10, Font.BOLD);
                    Font italicFont = new Font(baseFont, 8, Font.ITALIC);

                    // Thêm tiêu đề báo cáo
                    Paragraph title = new Paragraph(reportTitle, titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    document.Add(new Paragraph(" ")); // Khoảng trống

                    // Tạo bảng
                    PdfPTable table = new PdfPTable(headers.Length);
                    table.WidthPercentage = 100;

                    // Thiết lập tiêu đề cột
                    foreach (var header in headers)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                        cell.BackgroundColor = new BaseColor(252, 157, 192);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }

                    // Thêm dữ liệu vào bảng
                    var properties = typeof(T).GetProperties();

                    foreach (var item in data)
                    {
                        foreach (var property in properties)
                        {
                            var value = property.GetValue(item);
                            string cellValue = value?.ToString() ?? "";

                            PdfPCell cell = new PdfPCell(new Phrase(cellValue, normalFont));
                            cell.Padding = 5;

                            // Căn lề cho các cột số
                            if (value is decimal || value is double || value is float || value is int)
                            {
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            }
                            // Căn lề cho các cột ngày tháng
                            else if (value is DateTime)
                            {
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            }

                            table.AddCell(cell);
                        }
                    }

                    // Thêm hàng tổng kết (nếu có)
                    if (summary != null)
                    {
                        // Thêm hàng trống
                        for (int i = 0; i < headers.Length; i++)
                        {
                            PdfPCell emptyCell = new PdfPCell(new Phrase(" ", normalFont));
                            emptyCell.Border = 0;
                            table.AddCell(emptyCell);
                        }

                        // Thêm hàng tổng kết
                        PdfPCell totalLabelCell = new PdfPCell(new Phrase("TỔNG CỘNG:", boldFont));
                        totalLabelCell.Padding = 5;
                        totalLabelCell.Colspan = 1;
                        table.AddCell(totalLabelCell);

                        var summaryProperties = summary.GetType().GetProperties();

                        // Thêm các ô còn lại cho hàng tổng kết
                        for (int i = 1; i < headers.Length; i++)
                        {
                            bool cellFilled = false;

                            foreach (var property in summaryProperties)
                            {
                                // Tìm thuộc tính tương ứng của đối tượng tổng kết
                                if (properties[i].Name == property.Name)
                                {
                                    var value = property.GetValue(summary);
                                    string cellValue = value?.ToString() ?? "";

                                    PdfPCell cell = new PdfPCell(new Phrase(cellValue, boldFont));
                                    cell.Padding = 5;

                                    // Căn lề cho các cột số
                                    if (value is decimal || value is double || value is float || value is int)
                                    {
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    }

                                    table.AddCell(cell);
                                    cellFilled = true;
                                    break;
                                }
                            }

                            if (!cellFilled)
                            {
                                PdfPCell emptyCell = new PdfPCell(new Phrase(" ", normalFont));
                                table.AddCell(emptyCell);
                            }
                        }
                    }

                    // Thêm bảng vào tài liệu
                    document.Add(table);

                    // Thêm thông tin thời gian xuất báo cáo
                    document.Add(new Paragraph(" ")); // Khoảng trống
                    Paragraph timeInfo = new Paragraph("Thời gian xuất báo cáo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), italicFont);
                    timeInfo.Alignment = Element.ALIGN_RIGHT;
                    document.Add(timeInfo);

                    // Đóng tài liệu
                    document.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in ExportSalesReportToPdf: {ex.Message}");
                return false;
            }
        }

        public bool ExportDataTableToExcel(string filePath, string reportTitle, DataTable dataTable, bool hasFooter = false)
        {
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    // Tạo worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Report");

                    // Thiết lập tiêu đề báo cáo
                    worksheet.Cells[1, 1].Value = reportTitle;
                    worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Merge = true;
                    worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Style.Font.Size = 14;
                    worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Thiết lập tiêu đề các cột
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[3, i + 1].Value = dataTable.Columns[i].ColumnName;
                        worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[3, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 157, 192));
                        worksheet.Cells[3, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Thiết lập độ rộng cột
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Column(i + 1).AutoFit();
                        worksheet.Column(i + 1).Width = worksheet.Column(i + 1).Width + 5;
                    }

                    // Thêm dữ liệu vào worksheet
                    int rowCount = hasFooter ? dataTable.Rows.Count - 1 : dataTable.Rows.Count;

                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 4, j + 1].Value = dataTable.Rows[i][j];

                            // Định dạng cột kiểu số
                            if (dataTable.Columns[j].DataType == typeof(decimal) ||
                                dataTable.Columns[j].DataType == typeof(double) ||
                                dataTable.Columns[j].DataType == typeof(float))
                            {
                                worksheet.Cells[i + 4, j + 1].Style.Numberformat.Format = "#,##0";
                                worksheet.Cells[i + 4, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            }
                            // Định dạng cột kiểu ngày tháng
                            else if (dataTable.Columns[j].DataType == typeof(DateTime))
                            {
                                worksheet.Cells[i + 4, j + 1].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            // Thêm viền cho ô dữ liệu
                            worksheet.Cells[i + 4, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }

                    // Thêm hàng tổng kết (nếu có)
                    if (hasFooter)
                    {
                        int footerRow = rowCount + 4;
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            worksheet.Cells[footerRow, j + 1].Value = dataTable.Rows[rowCount][j];
                            worksheet.Cells[footerRow, j + 1].Style.Font.Bold = true;
                            worksheet.Cells[footerRow, j + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            // Định dạng cột kiểu số
                            if (dataTable.Columns[j].DataType == typeof(decimal) ||
                                dataTable.Columns[j].DataType == typeof(double) ||
                                dataTable.Columns[j].DataType == typeof(float))
                            {
                                worksheet.Cells[footerRow, j + 1].Style.Numberformat.Format = "#,##0";
                                worksheet.Cells[footerRow, j + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            }
                        }
                    }

                    // Thêm thông tin thời gian xuất báo cáo
                    int timeRow = (hasFooter ? rowCount + 6 : rowCount + 5);
                    worksheet.Cells[timeRow, 1].Value = "Thời gian xuất báo cáo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    worksheet.Cells[timeRow, 1, timeRow, dataTable.Columns.Count].Merge = true;
                    worksheet.Cells[timeRow, 1, timeRow, dataTable.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[timeRow, 1, timeRow, dataTable.Columns.Count].Style.Font.Italic = true;

                    // Lưu file Excel
                    package.SaveAs(new FileInfo(filePath));
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in ExportDataTableToExcel: {ex.Message}");
                return false;
            }
        }

        // Thêm các phương thức xuất báo cáo khác tại đây...
    }
}