// File: /Db/Repositories/InventoryRepository.cs
// Mô tả: Lớp triển khai IInventoryRepository
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public class InventoryRepository : BaseRepository, IInventoryRepository
    {
        public InventoryRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public List<Inventory> GetAllInventory()
        {
            var inventories = new List<Inventory>();

            try
            {
                using (var command = CreateCommand("app.sp_GetAllInventory"))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inventories.Add(new Inventory
                            {
                                InventoryID = Convert.ToInt32(reader["InventoryID"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetAllInventory: {ex.Message}", ex);
                throw;
            }

            return inventories;
        }

        public Inventory GetInventoryByProductID(int productID)
        {
            Inventory inventory = null;

            try
            {
                using (var command = CreateCommand("SELECT * FROM app.Inventory WHERE ProductID = @ProductID"))
                {
                    command.Parameters.AddWithValue("@ProductID", productID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inventory = new Inventory
                            {
                                InventoryID = Convert.ToInt32(reader["InventoryID"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                LastUpdated = Convert.ToDateTime(reader["LastUpdated"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetInventoryByProductID: {ex.Message}", ex);
                throw;
            }

            return inventory;
        }

        public List<Inventory> GetLowStockInventory()
        {
            var inventories = new List<Inventory>();

            try
            {
                using (var command = CreateCommand("app.sp_GetLowStockProducts"))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inventories.Add(new Inventory
                            {
                                InventoryID = 0, // Không có trong kết quả của stored procedure
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Quantity = Convert.ToInt32(reader["CurrentStock"]),
                                LastUpdated = DateTime.Now // Không có trong kết quả của stored procedure
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetLowStockInventory: {ex.Message}", ex);
                throw;
            }

            return inventories;
        }

        public bool UpdateInventoryQuantity(int productID, int newQuantity, string note, int employeeID)
        {
            try
            {
                using (var command = CreateCommand("app.sp_AdjustInventory"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductID", productID);
                    command.Parameters.AddWithValue("@NewQuantity", newQuantity);
                    command.Parameters.AddWithValue("@Reason", note);
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in UpdateInventoryQuantity: {ex.Message}", ex);
                return false;
            }
        }

        public List<InventoryTransaction> GetInventoryTransactionsByProduct(int productID, DateTime? startDate, DateTime? endDate)
        {
            var transactions = new List<InventoryTransaction>();

            try
            {
                using (var command = CreateCommand("app.sp_GetInventoryTransactionsByProduct"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductID", productID);

                    if (startDate.HasValue)
                        command.Parameters.AddWithValue("@StartDate", startDate.Value);
                    else
                        command.Parameters.AddWithValue("@StartDate", DBNull.Value);

                    if (endDate.HasValue)
                        command.Parameters.AddWithValue("@EndDate", endDate.Value);
                    else
                        command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new InventoryTransaction
                            {
                                TransactionID = Convert.ToInt32(reader["TransactionID"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                TransactionType = reader["TransactionType"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                PreviousQuantity = Convert.ToInt32(reader["PreviousQuantity"]),
                                CurrentQuantity = Convert.ToInt32(reader["CurrentQuantity"]),
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(reader["UnitPrice"]) : 0,
                                TotalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : 0,
                                ReferenceID = reader["ReferenceID"] != DBNull.Value ? Convert.ToInt32(reader["ReferenceID"]) : (int?)null,
                                ReferenceType = reader["ReferenceType"].ToString(),
                                Note = reader["Note"].ToString(),
                                TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : (int?)null
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetInventoryTransactionsByProduct: {ex.Message}", ex);
                throw;
            }

            return transactions;
        }

        public List<InventoryTransaction> GetInventoryTransactions(DateTime startDate, DateTime endDate, string transactionType = null)
        {
            var transactions = new List<InventoryTransaction>();

            try
            {
                string sql = @"
                    SELECT t.*, p.ProductName 
                    FROM app.InventoryTransaction t
                    INNER JOIN app.Product p ON t.ProductID = p.ProductID
                    WHERE t.TransactionDate BETWEEN @StartDate AND @EndDate";

                if (!string.IsNullOrEmpty(transactionType))
                    sql += " AND t.TransactionType = @TransactionType";

                sql += " ORDER BY t.TransactionDate DESC";

                using (var command = CreateCommand(sql))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    if (!string.IsNullOrEmpty(transactionType))
                        command.Parameters.AddWithValue("@TransactionType", transactionType);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new InventoryTransaction
                            {
                                TransactionID = Convert.ToInt32(reader["TransactionID"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                TransactionType = reader["TransactionType"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                PreviousQuantity = Convert.ToInt32(reader["PreviousQuantity"]),
                                CurrentQuantity = Convert.ToInt32(reader["CurrentQuantity"]),
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(reader["UnitPrice"]) : 0,
                                TotalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : 0,
                                ReferenceID = reader["ReferenceID"] != DBNull.Value ? Convert.ToInt32(reader["ReferenceID"]) : (int?)null,
                                ReferenceType = reader["ReferenceType"].ToString(),
                                Note = reader["Note"].ToString(),
                                TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : (int?)null,
                                ProductName = reader["ProductName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetInventoryTransactions: {ex.Message}", ex);
                throw;
            }

            return transactions;
        }

        public int AddInventoryTransaction(InventoryTransaction transaction)
        {
            try
            {
                using (var command = CreateCommand(@"
                    INSERT INTO app.InventoryTransaction (
                        ProductID, TransactionType, Quantity, PreviousQuantity, 
                        CurrentQuantity, UnitPrice, TotalAmount, ReferenceID, 
                        ReferenceType, Note, TransactionDate, CreatedBy)
                    VALUES (
                        @ProductID, @TransactionType, @Quantity, @PreviousQuantity, 
                        @CurrentQuantity, @UnitPrice, @TotalAmount, @ReferenceID, 
                        @ReferenceType, @Note, @TransactionDate, @CreatedBy);
                    SELECT SCOPE_IDENTITY();"))
                {
                    command.Parameters.AddWithValue("@ProductID", transaction.ProductID);
                    command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                    command.Parameters.AddWithValue("@Quantity", transaction.Quantity);
                    command.Parameters.AddWithValue("@PreviousQuantity", transaction.PreviousQuantity);
                    command.Parameters.AddWithValue("@CurrentQuantity", transaction.CurrentQuantity);

                    if (transaction.UnitPrice.HasValue)
                        command.Parameters.AddWithValue("@UnitPrice", transaction.UnitPrice.Value);
                    else
                        command.Parameters.AddWithValue("@UnitPrice", DBNull.Value);

                    if (transaction.TotalAmount.HasValue)
                        command.Parameters.AddWithValue("@TotalAmount", transaction.TotalAmount.Value);
                    else
                        command.Parameters.AddWithValue("@TotalAmount", DBNull.Value);

                    if (transaction.ReferenceID.HasValue)
                        command.Parameters.AddWithValue("@ReferenceID", transaction.ReferenceID.Value);
                    else
                        command.Parameters.AddWithValue("@ReferenceID", DBNull.Value);

                    command.Parameters.AddWithValue("@ReferenceType", transaction.ReferenceType ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Note", transaction.Note ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);

                    if (transaction.CreatedBy.HasValue)
                        command.Parameters.AddWithValue("@CreatedBy", transaction.CreatedBy.Value);
                    else
                        command.Parameters.AddWithValue("@CreatedBy", DBNull.Value);

                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in AddInventoryTransaction: {ex.Message}", ex);
                throw;
            }
        }

        public bool CheckInventoryAvailability(int productID, int requiredQuantity)
        {
            try
            {
                using (var command = CreateCommand("SELECT Quantity FROM app.Inventory WHERE ProductID = @ProductID"))
                {
                    command.Parameters.AddWithValue("@ProductID", productID);

                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int availableQuantity = Convert.ToInt32(result);
                        return availableQuantity >= requiredQuantity;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in CheckInventoryAvailability: {ex.Message}", ex);
                return false;
            }
        }
    }
}