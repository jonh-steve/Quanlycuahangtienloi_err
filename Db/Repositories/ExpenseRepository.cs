// Vị trí file: /Db/Repositories/ExpenseRepository.cs

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Db;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    /// <summary>
    /// Lớp thực thi interface IExpenseRepository để thao tác với dữ liệu chi phí
    /// </summary>
    public class ExpenseRepository : BaseRepository, IExpenseRepository
    {
        public ExpenseRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public List<Expense> GetAllExpenses()
        {
            try
            {
                var expenses = new List<Expense>();

                using (var command = CreateCommand("app.sp_GetAllExpenses"))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var expense = new Expense
                            {
                                ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                                ExpenseTypeName = reader["ExpenseType"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                Description = reader["Description"].ToString(),
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };

                            expenses.Add(expense);
                        }
                    }
                }

                return expenses;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetAllExpenses", ex);
                throw;
            }
        }

        public Expense GetExpenseByID(int expenseID)
        {
            try
            {
                Expense expense = null;

                using (var command = CreateCommand("app.sp_GetExpenseByID"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpenseID", expenseID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            expense = new Expense
                            {
                                ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                                ExpenseTypeName = reader["ExpenseType"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                Description = reader["Description"].ToString(),
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null
                            };
                        }
                    }
                }

                return expense;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetExpenseByID", ex);
                throw;
            }
        }

        public List<Expense> GetExpensesByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var expenses = new List<Expense>();

                using (var command = CreateCommand("app.sp_GetAllExpenses"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var expense = new Expense
                            {
                                ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                                ExpenseTypeName = reader["ExpenseType"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                Description = reader["Description"].ToString(),
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };

                            expenses.Add(expense);
                        }
                    }
                }

                return expenses;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetExpensesByDateRange", ex);
                throw;
            }
        }

        public List<Expense> GetExpensesByType(int expenseTypeID)
        {
            try
            {
                var expenses = new List<Expense>();

                using (var command = CreateCommand("app.sp_GetAllExpenses"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpenseTypeID", expenseTypeID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var expense = new Expense
                            {
                                ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                                ExpenseTypeName = reader["ExpenseType"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                Description = reader["Description"].ToString(),
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };

                            expenses.Add(expense);
                        }
                    }
                }

                return expenses;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetExpensesByType", ex);
                throw;
            }
        }

        public int CreateExpense(Expense expense)
        {
            try
            {
                int expenseID = 0;

                using (var command = CreateCommand("app.sp_CreateExpense"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpenseTypeID", expense.ExpenseTypeID);
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@ExpenseDate", expense.ExpenseDate);
                    command.Parameters.AddWithValue("@Description", expense.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmployeeID", expense.EmployeeID);

                    var outParam = new SqlParameter("@ExpenseID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outParam);

                    command.ExecuteNonQuery();

                    expenseID = (int)command.Parameters["@ExpenseID"].Value;
                }

                return expenseID;
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateExpense", ex);
                throw;
            }
        }

        public bool UpdateExpense(Expense expense)
        {
            try
            {
                using (var command = CreateCommand("app.sp_UpdateExpense"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpenseID", expense.ExpenseID);
                    command.Parameters.AddWithValue("@ExpenseTypeID", expense.ExpenseTypeID);
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@ExpenseDate", expense.ExpenseDate);
                    command.Parameters.AddWithValue("@Description", expense.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ModifiedBy", expense.EmployeeID);

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("UpdateExpense", ex);
                return false;
            }
        }

        public bool DeleteExpense(int expenseID)
        {
            try
            {
                using (var command = CreateCommand("app.sp_DeleteExpense"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpenseID", expenseID);
                    command.Parameters.AddWithValue("@DeletedBy", 1); // Tạm thời hardcode, cần cập nhật từ người đăng nhập

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("DeleteExpense", ex);
                return false;
            }
        }

        public List<ExpenseType> GetAllExpenseTypes()
        {
            try
            {
                var expenseTypes = new List<ExpenseType>();

                using (var command = CreateCommand("app.sp_GetAllExpenseTypes"))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var expenseType = new ExpenseType
                            {
                                ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                                TypeName = reader["TypeName"].ToString(),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };

                            expenseTypes.Add(expenseType);
                        }
                    }
                }

                return expenseTypes;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetAllExpenseTypes", ex);
                throw;
            }
        }

        public ExpenseType GetExpenseTypeByID(int expenseTypeID)
        {
            try
            {
                ExpenseType expenseType = null;

                using (var command = CreateCommand("SELECT * FROM app.ExpenseType WHERE ExpenseTypeID = @ExpenseTypeID"))
                {
                    command.Parameters.AddWithValue("@ExpenseTypeID", expenseTypeID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            expenseType = new ExpenseType
                            {
                                ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                                TypeName = reader["TypeName"].ToString(),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }

                return expenseType;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetExpenseTypeByID", ex);
                throw;
            }
        }

        public int CreateExpenseType(ExpenseType expenseType)
        {
            try
            {
                int expenseTypeID = 0;

                using (var command = CreateCommand("app.sp_CreateExpenseType"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TypeName", expenseType.TypeName);
                    command.Parameters.AddWithValue("@Description", expenseType.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedBy", 1); // Tạm thời hardcode, cần cập nhật từ người đăng nhập

                    var outParam = new SqlParameter("@ExpenseTypeID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outParam);

                    command.ExecuteNonQuery();

                    expenseTypeID = (int)command.Parameters["@ExpenseTypeID"].Value;
                }

                return expenseTypeID;
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateExpenseType", ex);
                throw;
            }
        }

        public bool UpdateExpenseType(ExpenseType expenseType)
        {
            try
            {
                using (var command = CreateCommand("app.sp_UpdateExpenseType"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpenseTypeID", expenseType.ExpenseTypeID);
                    command.Parameters.AddWithValue("@TypeName", expenseType.TypeName);
                    command.Parameters.AddWithValue("@Description", expenseType.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", expenseType.IsActive);
                    command.Parameters.AddWithValue("@ModifiedBy", 1); // Tạm thời hardcode, cần cập nhật từ người đăng nhập

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("UpdateExpenseType", ex);
                return false;
            }
        }

        public bool DeleteExpenseType(int expenseTypeID)
        {
            try
            {
                using (var command = CreateCommand("app.sp_DeleteExpenseType"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpenseTypeID", expenseTypeID);
                    command.Parameters.AddWithValue("@DeletedBy", 1); // Tạm thời hardcode, cần cập nhật từ người đăng nhập

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("DeleteExpenseType", ex);
                return false;
            }
        }

        public Dictionary<string, decimal> GetExpenseSummaryByType(DateTime startDate, DateTime endDate)
        {
            try
            {
                var summary = new Dictionary<string, decimal>();

                using (var command = CreateCommand("app.sp_GetExpenseSummaryByType"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string typeName = reader["TypeName"].ToString();
                            decimal totalAmount = Convert.ToDecimal(reader["TotalAmount"]);
                            summary.Add(typeName, totalAmount);
                        }
                    }
                }

                return summary;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetExpenseSummaryByType", ex);
                throw;
            }
        }

        public Dictionary<DateTime, decimal> GetExpenseSummaryByDate(DateTime startDate, DateTime endDate, string groupBy)
        {
            try
            {
                var summary = new Dictionary<DateTime, decimal>();

                using (var command = CreateCommand("app.sp_GetExpenseSummaryByDate"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@GroupBy", groupBy);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date;
                            if (groupBy == "Day")
                            {
                                date = Convert.ToDateTime(reader["ExpenseDate"]);
                            }
                            else if (groupBy == "Week")
                            {
                                date = Convert.ToDateTime(reader["WeekStart"]);
                            }
                            else // Month
                            {
                                int year = Convert.ToInt32(reader["Year"]);
                                int month = Convert.ToInt32(reader["Month"]);
                                date = new DateTime(year, month, 1);
                            }

                            decimal totalAmount = Convert.ToDecimal(reader["TotalAmount"]);
                            summary.Add(date, totalAmount);
                        }
                    }
                }

                return summary;
            }
            catch (Exception ex)
            {
                Logger.LogError("GetExpenseSummaryByDate", ex);
                throw;
            }
        }
    }
}