// Vị trí file: /Services/IExpenseService.cs

using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;

namespace QuanLyCuaHangTienLoi.Services
{
    /// <summary>
    /// Interface định nghĩa các phương thức nghiệp vụ liên quan đến chi phí
    /// </summary>
    public interface IExpenseService
    {
        /// <summary>
        /// Lấy tất cả chi phí
        /// </summary>
        List<ExpenseDTO> GetAllExpenses();

        /// <summary>
        /// Lấy chi phí theo ID
        /// </summary>
        ExpenseDTO GetExpenseByID(int expenseID);

        /// <summary>
        /// Lấy chi phí theo khoảng thời gian
        /// </summary>
        List<ExpenseDTO> GetExpensesByDateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy chi phí theo loại
        /// </summary>
        List<ExpenseDTO> GetExpensesByType(int expenseTypeID);

        /// <summary>
        /// Tạo chi phí mới
        /// </summary>
        int CreateExpense(ExpenseDTO expenseDTO);

        /// <summary>
        /// Cập nhật thông tin chi phí
        /// </summary>
        bool UpdateExpense(ExpenseDTO expenseDTO);

        /// <summary>
        /// Xóa chi phí
        /// </summary>
        bool DeleteExpense(int expenseID);

        /// <summary>
        /// Lấy tất cả loại chi phí
        /// </summary>
        List<ExpenseTypeDTO> GetAllExpenseTypes();

        /// <summary>
        /// Lấy loại chi phí theo ID
        /// </summary>
        ExpenseTypeDTO GetExpenseTypeByID(int expenseTypeID);

        /// <summary>
        /// Tạo loại chi phí mới
        /// </summary>
        int CreateExpenseType(ExpenseTypeDTO expenseTypeDTO);

        /// <summary>
        /// Cập nhật loại chi phí
        /// </summary>
        bool UpdateExpenseType(ExpenseTypeDTO expenseTypeDTO);

        /// <summary>
        /// Xóa loại chi phí
        /// </summary>
        bool DeleteExpenseType(int expenseTypeID);

        /// <summary>
        /// Thống kê chi phí theo loại
        /// </summary>
        Dictionary<string, decimal> GetExpenseSummaryByType(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Thống kê chi phí theo thời gian (ngày, tuần, tháng)
        /// </summary>
        Dictionary<DateTime, decimal> GetExpenseSummaryByDate(DateTime startDate, DateTime endDate, string groupBy);

        /// <summary>
        /// Xuất báo cáo chi phí ra Excel
        /// </summary>
        string ExportExpensesToExcel(List<ExpenseDTO> expenses, string filePath);

        /// <summary>
        /// Xuất báo cáo chi phí ra PDF
        /// </summary>
        string ExportExpensesToPDF(List<ExpenseDTO> expenses, string filePath);
    }
}