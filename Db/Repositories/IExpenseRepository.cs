// Vị trí file: /Db/Repositories/IExpenseRepository.cs

using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    /// <summary>
    /// Interface định nghĩa các phương thức thao tác với dữ liệu chi phí
    /// </summary>
    public interface IExpenseRepository
    {
        /// <summary>
        /// Lấy tất cả chi phí
        /// </summary>
        List<Expense> GetAllExpenses();

        /// <summary>
        /// Lấy chi phí theo ID
        /// </summary>
        Expense GetExpenseByID(int expenseID);

        /// <summary>
        /// Lấy chi phí theo khoảng thời gian
        /// </summary>
        List<Expense> GetExpensesByDateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy chi phí theo loại
        /// </summary>
        List<Expense> GetExpensesByType(int expenseTypeID);

        /// <summary>
        /// Tạo chi phí mới
        /// </summary>
        int CreateExpense(Expense expense);

        /// <summary>
        /// Cập nhật thông tin chi phí
        /// </summary>
        bool UpdateExpense(Expense expense);

        /// <summary>
        /// Xóa chi phí
        /// </summary>
        bool DeleteExpense(int expenseID);

        /// <summary>
        /// Lấy tất cả loại chi phí
        /// </summary>
        List<ExpenseType> GetAllExpenseTypes();

        /// <summary>
        /// Lấy loại chi phí theo ID
        /// </summary>
        ExpenseType GetExpenseTypeByID(int expenseTypeID);

        /// <summary>
        /// Tạo loại chi phí mới
        /// </summary>
        int CreateExpenseType(ExpenseType expenseType);

        /// <summary>
        /// Cập nhật loại chi phí
        /// </summary>
        bool UpdateExpenseType(ExpenseType expenseType);

        /// <summary>
        /// Xóa loại chi phí
        /// </summary>
        bool DeleteExpenseType(int expenseTypeID);

        /// <summary>
        /// Thống kê chi phí theo loại
        /// </summary>
        Dictionary<string, decimal> GetExpenseSummaryByType(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Thống kê chi phí theo thời gian
        /// </summary>
        Dictionary<DateTime, decimal> GetExpenseSummaryByDate(DateTime startDate, DateTime endDate, string groupBy);
    }
}