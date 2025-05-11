// Vị trí file: /Services/ExpenseService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace QuanLyCuaHangTienLoi.Services
{
    /// <summary>
    /// Lớp thực thi interface IExpenseService để xử lý nghiệp vụ liên quan đến chi phí
    /// </summary>
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly Logger _logger;

        public ExpenseService(IExpenseRepository expenseRepository, Logger logger)
        {
            _expenseRepository = expenseRepository;
            _logger = logger;
        }

        public List<ExpenseDTO> GetAllExpenses()
        {
            try
            {
                var expenses = _expenseRepository.GetAllExpenses();
                return expenses.Select(MapToExpenseDTO).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.GetAllExpenses", ex);
                throw;
            }
        }

        public ExpenseDTO GetExpenseByID(int expenseID)
        {
            try
            {
                var expense = _expenseRepository.GetExpenseByID(expenseID);
                return expense != null ? MapToExpenseDTO(expense) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.GetExpenseByID", ex);
                throw;
            }
        }

        public List<ExpenseDTO> GetExpensesByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var expenses = _expenseRepository.GetExpensesByDateRange(startDate, endDate);
                return expenses.Select(MapToExpenseDTO).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.GetExpensesByDateRange", ex);
                throw;
            }
        }

        public List<ExpenseDTO> GetExpensesByType(int expenseTypeID)
        {
            try
            {
                var expenses = _expenseRepository.GetExpensesByType(expenseTypeID);
                return expenses.Select(MapToExpenseDTO).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.GetExpensesByType", ex);
                throw;
            }
        }

        public int CreateExpense(ExpenseDTO expenseDTO)
        {
            try
            {
                // Validate dữ liệu
                if (expenseDTO.Amount <= 0)
                {
                    throw new ArgumentException("Số tiền chi phí phải lớn hơn 0");
                }

                if (expenseDTO.ExpenseTypeID <= 0)
                {
                    throw new ArgumentException("Loại chi phí không hợp lệ");
                }

                if (expenseDTO.ExpenseDate > DateTime.Now)
                {
                    throw new ArgumentException("Ngày chi phí không thể là ngày trong tương lai");
                }

                var expense = MapToExpense(expenseDTO);
                return _expenseRepository.CreateExpense(expense);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.CreateExpense", ex);
                throw;
            }
        }

        public bool UpdateExpense(ExpenseDTO expenseDTO)
        {
            try
            {
                // Validate dữ liệu
                if (expenseDTO.ExpenseID <= 0)
                {
                    throw new ArgumentException("ID chi phí không hợp lệ");
                }

                if (expenseDTO.Amount <= 0)
                {
                    throw new ArgumentException("Số tiền chi phí phải lớn hơn 0");
                }

                if (expenseDTO.ExpenseTypeID <= 0)
                {
                    throw new ArgumentException("Loại chi phí không hợp lệ");
                }

                if (expenseDTO.ExpenseDate > DateTime.Now)
                {
                    throw new ArgumentException("Ngày chi phí không thể là ngày trong tương lai");
                }

                var expense = MapToExpense(expenseDTO);
                return _expenseRepository.UpdateExpense(expense);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.UpdateExpense", ex);
                throw;
            }
        }

        public bool DeleteExpense(int expenseID)
        {
            try
            {
                if (expenseID <= 0)
                {
                    throw new ArgumentException("ID chi phí không hợp lệ");
                }

                return _expenseRepository.DeleteExpense(expenseID);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.DeleteExpense", ex);
                throw;
            }
        }

        public List<ExpenseTypeDTO> GetAllExpenseTypes()
        {
            try
            {
                var expenseTypes = _expenseRepository.GetAllExpenseTypes();
                return expenseTypes.Select(MapToExpenseTypeDTO).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.GetAllExpenseTypes", ex);
                throw;
            }
        }

        public ExpenseTypeDTO GetExpenseTypeByID(int expenseTypeID)
        {
            try
            {
                var expenseType = _expenseRepository.GetExpenseTypeByID(expenseTypeID);
                return expenseType != null ? MapToExpenseTypeDTO(expenseType) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.GetExpenseTypeByID", ex);
                throw;
            }
        }

        public int CreateExpenseType(ExpenseTypeDTO expenseTypeDTO)
        {
            try
            {
                // Validate dữ liệu
                if (string.IsNullOrWhiteSpace(expenseTypeDTO.TypeName))
                {
                    throw new ArgumentException("Tên loại chi phí không được để trống");
                }

                var expenseType = MapToExpenseType(expenseTypeDTO);
                return _expenseRepository.CreateExpenseType(expenseType);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.CreateExpenseType", ex);
                throw;
            }
        }

        public bool UpdateExpenseType(ExpenseTypeDTO expenseTypeDTO)
        {
            try
            {
                // Validate dữ liệu
                if (expenseTypeDTO.ExpenseTypeID <= 0)
                {
                    throw new ArgumentException("ID loại chi phí không hợp lệ");
                }

                if (string.IsNullOrWhiteSpace(expenseTypeDTO.TypeName))
                {
                    throw new ArgumentException("Tên loại chi phí không được để trống");
                }

                var expenseType = MapToExpenseType(expenseTypeDTO);
                return _expenseRepository.UpdateExpenseType(expenseType);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.UpdateExpenseType", ex);
                throw;
            }
        }

        public bool DeleteExpenseType(int expenseTypeID)
        {
            try
            {
                if (expenseTypeID <= 0)
                {
                    throw new ArgumentException("ID loại chi phí không hợp lệ");
                }

// Tiếp tục ở phần còn lại của ExpenseService.cs

                // Kiểm tra xem có chi phí nào sử dụng loại chi phí này không
                var expenses = _expenseRepository.GetExpensesByType(expenseTypeID);
                if (expenses.Count > 0)
                {
                    throw new InvalidOperationException("Không thể xóa loại chi phí này vì đã có chi phí sử dụng");
                }

                return _expenseRepository.DeleteExpenseType(expenseTypeID);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.DeleteExpenseType", ex);
                throw;
            }
        }

        public Dictionary<string, decimal> GetExpenseSummaryByType(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _expenseRepository.GetExpenseSummaryByType(startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.GetExpenseSummaryByType", ex);
                throw;
            }
        }

        public Dictionary<DateTime, decimal> GetExpenseSummaryByDate(DateTime startDate, DateTime endDate, string groupBy)
        {
            try
            {
                // Validate dữ liệu
                if (groupBy != "Day" && groupBy != "Week" && groupBy != "Month")
                {
                    throw new ArgumentException("Tham số groupBy không hợp lệ. Giá trị hợp lệ là 'Day', 'Week', 'Month'");
                }

                return _expenseRepository.GetExpenseSummaryByDate(startDate, endDate, groupBy);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.GetExpenseSummaryByDate", ex);
                throw;
            }
        }

        public string ExportExpensesToExcel(List<ExpenseDTO> expenses, string filePath)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Chi phí");

                    // Thiết lập tiêu đề
                    worksheet.Cells[1, 1].Value = "BÁO CÁO CHI PHÍ";
                    worksheet.Cells[1, 1, 1, 7].Merge = true;
                    worksheet.Cells[1, 1].Style.Font.Size = 16;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 192, 203)); // Màu hồng dễ thương

                    // Thiết lập tiêu đề cột
                    string[] headers = new string[] { "STT", "Loại chi phí", "Số tiền", "Ngày chi", "Mô tả", "Người tạo", "Ngày tạo" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[3, i + 1].Value = headers[i];
                        worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 204, 229)); // Màu hồng nhạt
                        worksheet.Cells[3, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Nhập dữ liệu
                    for (int i = 0; i < expenses.Count; i++)
                    {
                        worksheet.Cells[i + 4, 1].Value = i + 1; // STT
                        worksheet.Cells[i + 4, 2].Value = expenses[i].ExpenseTypeName;
                        worksheet.Cells[i + 4, 3].Value = expenses[i].Amount;
                        worksheet.Cells[i + 4, 3].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[i + 4, 4].Value = expenses[i].ExpenseDate;
                        worksheet.Cells[i + 4, 4].Style.Numberformat.Format = "dd/MM/yyyy";
                        worksheet.Cells[i + 4, 5].Value = expenses[i].Description;
                        worksheet.Cells[i + 4, 6].Value = expenses[i].EmployeeName;
                        worksheet.Cells[i + 4, 7].Value = expenses[i].CreatedDate;
                        worksheet.Cells[i + 4, 7].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";

                        // Thiết lập border
                        for (int j = 1; j <= 7; j++)
                        {
                            worksheet.Cells[i + 4, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }

                    // Tổng cộng
                    int lastRow = expenses.Count + 4;
                    worksheet.Cells[lastRow, 2].Value = "TỔNG CỘNG";
                    worksheet.Cells[lastRow, 2].Style.Font.Bold = true;
                    worksheet.Cells[lastRow, 3].Formula = $"SUM(C4:C{lastRow - 1})";
                    worksheet.Cells[lastRow, 3].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[lastRow, 3].Style.Font.Bold = true;
                    worksheet.Cells[lastRow, 2, lastRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[lastRow, 2, lastRow, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 204, 229)); // Màu hồng nhạt

                    // Chữ ký
                    worksheet.Cells[lastRow + 2, 5].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                    worksheet.Cells[lastRow + 2, 5, lastRow + 2, 7].Merge = true;
                    worksheet.Cells[lastRow + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[lastRow + 3, 5].Value = "Người lập báo cáo";
                    worksheet.Cells[lastRow + 3, 5, lastRow + 3, 7].Merge = true;
                    worksheet.Cells[lastRow + 3, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[lastRow + 3, 5].Style.Font.Bold = true;

                    worksheet.Cells[lastRow + 7, 5].Value = "Steve-Thuong_hai";
                    worksheet.Cells[lastRow + 7, 5, lastRow + 7, 7].Merge = true;
                    worksheet.Cells[lastRow + 7, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[lastRow + 7, 5].Style.Font.Bold = true;

                    // Auto-fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Lưu file
                    package.SaveAs(new FileInfo(filePath));
                }

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.ExportExpensesToExcel", ex);
                throw;
            }
        }

        public string ExportExpensesToPDF(List<ExpenseDTO> expenses, string filePath)
        {
            try
            {
                // Tạo document mới
                using (Document document = new Document(PageSize.A4, 36, 36, 36, 36))
                {
                    // Tạo writer
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);

                        // Mở document
                        document.Open();

                        // Thiết lập font và màu sắc
                        BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        Font titleFont = new Font(baseFont, 16, Font.BOLD, new BaseColor(219, 112, 147)); // Màu hồng đậm
                        Font headerFont = new Font(baseFont, 11, Font.BOLD, new BaseColor(219, 112, 147)); // Màu hồng đậm
                        Font normalFont = new Font(baseFont, 10, Font.NORMAL);
                        Font totalFont = new Font(baseFont, 11, Font.BOLD);
                        Font signatureFont = new Font(baseFont, 11, Font.BOLD);

                        // Tiêu đề
                        Paragraph title = new Paragraph("BÁO CÁO CHI PHÍ", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        title.SpacingAfter = 20;
                        document.Add(title);

                        // Tạo bảng
                        PdfPTable table = new PdfPTable(6);
                        table.WidthPercentage = 100;
                        table.SetWidths(new float[] { 1f, 3f, 2f, 2f, 4f, 2f });

                        // Thêm header
                        PdfPCell cell1 = new PdfPCell(new Phrase("STT", headerFont));
                        cell1.BackgroundColor = new BaseColor(255, 204, 229); // Màu hồng nhạt
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.Padding = 5;

                        PdfPCell cell2 = new PdfPCell(new Phrase("Loại chi phí", headerFont));
                        cell2.BackgroundColor = new BaseColor(255, 204, 229);
                        cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell2.Padding = 5;

                        PdfPCell cell3 = new PdfPCell(new Phrase("Số tiền", headerFont));
                        cell3.BackgroundColor = new BaseColor(255, 204, 229);
                        cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell3.Padding = 5;

                        PdfPCell cell4 = new PdfPCell(new Phrase("Ngày chi", headerFont));
                        cell4.BackgroundColor = new BaseColor(255, 204, 229);
                        cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell4.Padding = 5;

                        PdfPCell cell5 = new PdfPCell(new Phrase("Mô tả", headerFont));
                        cell5.BackgroundColor = new BaseColor(255, 204, 229);
                        cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell5.Padding = 5;

                        PdfPCell cell6 = new PdfPCell(new Phrase("Người tạo", headerFont));
                        cell6.BackgroundColor = new BaseColor(255, 204, 229);
                        cell6.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell6.Padding = 5;

                        table.AddCell(cell1);
                        table.AddCell(cell2);
                        table.AddCell(cell3);
                        table.AddCell(cell4);
                        table.AddCell(cell5);
                        table.AddCell(cell6);

                        // Thêm dữ liệu
                        decimal totalAmount = 0;
                        for (int i = 0; i < expenses.Count; i++)
                        {
                            PdfPCell sttCell = new PdfPCell(new Phrase((i + 1).ToString(), normalFont));
                            sttCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            sttCell.Padding = 5;
                            table.AddCell(sttCell);

                            PdfPCell typeCell = new PdfPCell(new Phrase(expenses[i].ExpenseTypeName, normalFont));
                            typeCell.Padding = 5;
                            table.AddCell(typeCell);

                            PdfPCell amountCell = new PdfPCell(new Phrase(expenses[i].Amount.ToString("#,##0"), normalFont));
                            amountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            amountCell.Padding = 5;
                            table.AddCell(amountCell);

                            PdfPCell dateCell = new PdfPCell(new Phrase(expenses[i].ExpenseDate.ToString("dd/MM/yyyy"), normalFont));
                            dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            dateCell.Padding = 5;
                            table.AddCell(dateCell);

                            PdfPCell descCell = new PdfPCell(new Phrase(expenses[i].Description, normalFont));
                            descCell.Padding = 5;
                            table.AddCell(descCell);

                            PdfPCell empCell = new PdfPCell(new Phrase(expenses[i].EmployeeName, normalFont));
                            empCell.Padding = 5;
                            table.AddCell(empCell);

                            totalAmount += expenses[i].Amount;
                        }

                        // Thêm tổng cộng
                        PdfPCell totalLabelCell = new PdfPCell(new Phrase("TỔNG CỘNG", totalFont));
                        totalLabelCell.Colspan = 2;
                        totalLabelCell.BackgroundColor = new BaseColor(255, 204, 229);
                        totalLabelCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        totalLabelCell.Padding = 5;
                        table.AddCell(totalLabelCell);

                        PdfPCell totalValueCell = new PdfPCell(new Phrase(totalAmount.ToString("#,##0"), totalFont));
                        totalValueCell.BackgroundColor = new BaseColor(255, 204, 229);
                        totalValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        totalValueCell.Padding = 5;
                        table.AddCell(totalValueCell);

                        PdfPCell emptyCell = new PdfPCell(new Phrase(""));
                        emptyCell.Colspan = 3;
                        emptyCell.Border = PdfPCell.NO_BORDER;
                        table.AddCell(emptyCell);

                        document.Add(table);

                        // Thêm chữ ký
                        Paragraph signatureDate = new Paragraph(
                            $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}",
                            normalFont);
                        signatureDate.Alignment = Element.ALIGN_RIGHT;
                        signatureDate.SpacingBefore = 30;
                        document.Add(signatureDate);

                        Paragraph signatureTitle = new Paragraph("Người lập báo cáo", signatureFont);
                        signatureTitle.Alignment = Element.ALIGN_RIGHT;
                        signatureTitle.SpacingBefore = 10;
                        document.Add(signatureTitle);

                        Paragraph signature = new Paragraph("Steve-Thuong_hai", signatureFont);
                        signature.Alignment = Element.ALIGN_RIGHT;
                        signature.SpacingBefore = 50;
                        document.Add(signature);

                        // Đóng document
                        document.Close();
                    }
                }

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError("ExpenseService.ExportExpensesToPDF", ex);
                throw;
            }
        }

        #region Helper Methods

        private ExpenseDTO MapToExpenseDTO(Expense expense)
        {
            return new ExpenseDTO
            {
                ExpenseID = expense.ExpenseID,
                ExpenseTypeID = expense.ExpenseTypeID,
                ExpenseTypeName = expense.ExpenseTypeName,
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate,
                Description = expense.Description,
                EmployeeID = expense.EmployeeID,
                EmployeeName = expense.EmployeeName,
                CreatedDate = expense.CreatedDate,
                ModifiedDate = expense.ModifiedDate
            };
        }

        private Expense MapToExpense(ExpenseDTO expenseDTO)
        {
            return new Expense
            {
                ExpenseID = expenseDTO.ExpenseID,
                ExpenseTypeID = expenseDTO.ExpenseTypeID,
                Amount = expenseDTO.Amount,
                ExpenseDate = expenseDTO.ExpenseDate,
                Description = expenseDTO.Description,
                EmployeeID = expenseDTO.EmployeeID,
                CreatedDate = expenseDTO.CreatedDate,
                ModifiedDate = expenseDTO.ModifiedDate
            };
        }

        private ExpenseTypeDTO MapToExpenseTypeDTO(ExpenseType expenseType)
        {
            return new ExpenseTypeDTO
            {
                ExpenseTypeID = expenseType.ExpenseTypeID,
                TypeName = expenseType.TypeName,
                Description = expenseType.Description,
                IsActive = expenseType.IsActive
            };
        }

        private ExpenseType MapToExpenseType(ExpenseTypeDTO expenseTypeDTO)
        {
            return new ExpenseType
            {
                ExpenseTypeID = expenseTypeDTO.ExpenseTypeID,
                TypeName = expenseTypeDTO.TypeName,
                Description = expenseTypeDTO.Description,
                IsActive = expenseTypeDTO.IsActive
            };
        }

        #endregion
    }
}