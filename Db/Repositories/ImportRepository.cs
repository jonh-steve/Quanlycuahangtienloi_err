// File: /Db/Repositories/ImportRepository.cs
// Mô tả: Lớp triển khai IImportRepository
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public class ImportRepository : BaseRepository, IImportRepository
    {
        public ImportRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public List<Import> GetAllImports()
        {
            var imports = new List<Import>();

            try
            {
                using (var command = CreateCommand(@"
                    SELECT i.*, s.SupplierName, e.FullName AS EmployeeName,
                    (SELECT COUNT(*) FROM app.ImportDetail WHERE ImportID = i.ImportID) AS ItemCount
                    FROM app.Import i
                    INNER JOIN app.Supplier s ON i.SupplierID = s.SupplierID
                    INNER JOIN app.Employee e ON i.CreatedBy = e.EmployeeID
                    ORDER BY i.ImportDate DESC"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            imports.Add(new Import
                            {
                                ImportID = Convert.ToInt32(reader["ImportID"]),
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                ImportCode = reader["ImportCode"].ToString(),
                                ImportDate = Convert.ToDateTime(reader["ImportDate"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                Status = reader["Status"].ToString(),
                                Note = reader["Note"].ToString(),
                                CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null,
                                // Thông tin bổ sung
                                SupplierName = reader["SupplierName"].ToString(),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                ItemCount = Convert.ToInt32(reader["ItemCount"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetAllImports: {ex.Message}", ex);
                throw;
            }

            return imports;
        }

        public Import GetImportByID(int importID)
        {
            Import import = null;

            try
            {
                using (var command = CreateCommand("app.sp_GetImportByID"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ImportID", importID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            import = new Import
                            {
                                ImportID = Convert.ToInt32(reader["ImportID"]),
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                ImportCode = reader["ImportCode"].ToString(),
                                ImportDate = Convert.ToDateTime(reader["ImportDate"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                Status = reader["Status"].ToString(),
                                Note = reader["Note"].ToString(),
                                CreatedBy = Convert.ToInt32(reader["EmployeeID"]),
                                // Thông tin bổ sung
                                SupplierName = reader["SupplierName"].ToString(),
                                EmployeeName = reader["EmployeeName"].ToString()
                            };
                        }

                        if (import != null && reader.NextResult())
                        {
                            import.Details = new List<ImportDetail>();

                            while (reader.Read())
                            {
                                import.Details.Add(new ImportDetail
                                {
                                    ImportDetailID = Convert.ToInt32(reader["ImportDetailID"]),
                                    ImportID = importID,
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                                    ExpiryDate = reader["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpiryDate"]) : (DateTime?)null,
                                    BatchNumber = reader["BatchNumber"].ToString(),
                                    // Thông tin bổ sung
                                    ProductName = reader["ProductName"].ToString(),
                                    ProductCode = reader["ProductCode"].ToString(),
                                    Unit = reader["Unit"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetImportByID: {ex.Message}", ex);
                throw;
            }

            return import;
        }

        public List<Import> GetImportsByDateRange(DateTime startDate, DateTime endDate, int? supplierID = null, string status = null)
        {
            var imports = new List<Import>();

            try
            {
                using (var command = CreateCommand("app.sp_GetImportsByDateRange"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    if (supplierID.HasValue)
                        command.Parameters.AddWithValue("@SupplierID", supplierID.Value);
                    else
                        command.Parameters.AddWithValue("@SupplierID", DBNull.Value);

                    if (!string.IsNullOrEmpty(status))
                        command.Parameters.AddWithValue("@Status", status);
                    else
                        command.Parameters.AddWithValue("@Status", DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            imports.Add(new Import
                            {
                                ImportID = Convert.ToInt32(reader["ImportID"]),
                                ImportCode = reader["ImportCode"].ToString(),
                                ImportDate = Convert.ToDateTime(reader["ImportDate"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                Status = reader["Status"].ToString(),
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                // Thông tin bổ sung
                                SupplierName = reader["SupplierName"].ToString(),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                ItemCount = Convert.ToInt32(reader["ItemCount"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetImportsByDateRange: {ex.Message}", ex);
                throw;
            }

            return imports;
        }

        public int CreateImport(Import import, List<ImportDetail> importDetails)
        {
            // Sử dụng DataTable cho ImportItems
            var importItems = new DataTable();
            importItems.Columns.Add("ProductID", typeof(int));
            importItems.Columns.Add("Quantity", typeof(int));
            importItems.Columns.Add("UnitPrice", typeof(decimal));
            importItems.Columns.Add("ExpiryDate", typeof(DateTime));
            importItems.Columns.Add("BatchNumber", typeof(string));

            foreach (var detail in importDetails)
            {
                var row = importItems.NewRow();
                row["ProductID"] = detail.ProductID;
                row["Quantity"] = detail.Quantity;
                row["UnitPrice"] = detail.UnitPrice;

                if (detail.ExpiryDate.HasValue)
                    row["ExpiryDate"] = detail.ExpiryDate.Value;
                else
                    row["ExpiryDate"] = DBNull.Value;

                row["BatchNumber"] = detail.BatchNumber ?? (object)DBNull.Value;
                importItems.Rows.Add(row);
            }

            try
            {
                using (var command = CreateCommand("app.sp_CreateImport"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SupplierID", import.SupplierID);
                    command.Parameters.AddWithValue("@Note", import.Note ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmployeeID", import.CreatedBy);

                    // Tạo parameter cho table-valued parameter
                    var importItemsParam = command.Parameters.AddWithValue("@ImportItems", importItems);
                    importItemsParam.SqlDbType = SqlDbType.Structured;
                    importItemsParam.TypeName = "app.ImportItemType";

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["ImportID"]);
                        }
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in CreateImport: {ex.Message}", ex);
                throw;
            }
        }

        public bool UpdateImport(Import import)
        {
            try
            {
                using (var command = CreateCommand(@"
                    UPDATE app.Import SET
                        Note = @Note,
                        ModifiedDate = GETDATE()
                    WHERE ImportID = @ImportID AND Status = 'Pending'"))
                {
                    command.Parameters.AddWithValue("@ImportID", import.ImportID);
                    command.Parameters.AddWithValue("@Note", import.Note ?? (object)DBNull.Value);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in UpdateImport: {ex.Message}", ex);
                return false;
            }
        }

        public bool CancelImport(int importID, string cancelReason, int employeeID)
        {
            try
            {
                using (var command = CreateCommand(@"
                    UPDATE app.Import SET
                        Status = 'Cancelled',
                        Note = ISNULL(Note + ' | ', '') + 'Cancelled: ' + @CancelReason,
                        ModifiedDate = GETDATE()
                    WHERE ImportID = @ImportID AND Status <> 'Completed'"))
                {
                    command.Parameters.AddWithValue("@ImportID", importID);
                    command.Parameters.AddWithValue("@CancelReason", cancelReason);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Log the activity
                        using (var logCommand = CreateCommand(@"
                            INSERT INTO app.ActivityLog (
                                AccountID, ActivityType, EntityType, EntityID, Description
                            ) VALUES (
                                @AccountID, 'Cancel', 'Import', @ImportID, 
                                'Cancelled import: ' + (SELECT ImportCode FROM app.Import WHERE ImportID = @ImportID) + 
                                ' - Reason: ' + @CancelReason
                            )"))
                        {
                            logCommand.Parameters.AddWithValue("@AccountID", employeeID);
                            logCommand.Parameters.AddWithValue("@ImportID", importID);
                            logCommand.Parameters.AddWithValue("@CancelReason", cancelReason);

                            logCommand.ExecuteNonQuery();
                        }

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in CancelImport: {ex.Message}", ex);
                return false;
            }
        }

        public List<ImportDetail> GetImportDetailsByImportID(int importID)
        {
            var details = new List<ImportDetail>();

            try
            {
                using (var command = CreateCommand(@"
                    SELECT id.*, p.ProductName, p.ProductCode, p.Unit
                    FROM app.ImportDetail id
                    INNER JOIN app.Product p ON id.ProductID = p.ProductID
                    WHERE id.ImportID = @ImportID"))
                {
                    command.Parameters.AddWithValue("@ImportID", importID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            details.Add(new ImportDetail
                            {
                                ImportDetailID = Convert.ToInt32(reader["ImportDetailID"]),
                                ImportID = importID,
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                                ExpiryDate = reader["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpiryDate"]) : (DateTime?)null,
                                BatchNumber = reader["BatchNumber"] != DBNull.Value ? reader["BatchNumber"].ToString() : null,
                                // Thông tin bổ sung
                                ProductName = reader["ProductName"].ToString(),
                                ProductCode = reader["ProductCode"].ToString(),
                                Unit = reader["Unit"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in GetImportDetailsByImportID: {ex.Message}", ex);
                throw;
            }

            return details;
        }

        public bool AddImportDetail(ImportDetail detail)
        {
            try
            {
                using (var command = CreateCommand(@"
                    INSERT INTO app.ImportDetail (
                        ImportID, ProductID, Quantity, UnitPrice, TotalPrice, ExpiryDate, BatchNumber
                    ) VALUES (
                        @ImportID, @ProductID, @Quantity, @UnitPrice, @Quantity * @UnitPrice, @ExpiryDate, @BatchNumber
                    )"))
                {
                    command.Parameters.AddWithValue("@ImportID", detail.ImportID);
                    command.Parameters.AddWithValue("@ProductID", detail.ProductID);
                    command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    command.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);

                    if (detail.ExpiryDate.HasValue)
                        command.Parameters.AddWithValue("@ExpiryDate", detail.ExpiryDate.Value);
                    else
                        command.Parameters.AddWithValue("@ExpiryDate", DBNull.Value);

                    command.Parameters.AddWithValue("@BatchNumber", detail.BatchNumber ?? (object)DBNull.Value);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Update import total amount
                        using (var updateCommand = CreateCommand(@"
                            UPDATE app.Import SET
                                TotalAmount = (SELECT SUM(TotalPrice) FROM app.ImportDetail WHERE ImportID = @ImportID),
                                ModifiedDate = GETDATE()
                            WHERE ImportID = @ImportID"))
                        {
                            updateCommand.Parameters.AddWithValue("@ImportID", detail.ImportID);
                            updateCommand.ExecuteNonQuery();
                        }

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in AddImportDetail: {ex.Message}", ex);
                return false;
            }
        }

        public bool DeleteImportDetail(int importDetailID)
        {
            try
            {
                // Get importID first for updating TotalAmount later
                int importID = 0;
                using (var getCommand = CreateCommand("SELECT ImportID FROM app.ImportDetail WHERE ImportDetailID = @ImportDetailID"))
                {
                    getCommand.Parameters.AddWithValue("@ImportDetailID", importDetailID);
                    var result = getCommand.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        importID = Convert.ToInt32(result);
                    }
                    else
                    {
                        // Detail not found
                        return false;
                    }
                }

                // Delete the detail
                using (var command = CreateCommand("DELETE FROM app.ImportDetail WHERE ImportDetailID = @ImportDetailID"))
                {
                    command.Parameters.AddWithValue("@ImportDetailID", importDetailID);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0 && importID > 0)
                    {
                        // Update import total amount
                        using (var updateCommand = CreateCommand(@"
                            UPDATE app.Import SET
                                TotalAmount = ISNULL((SELECT SUM(TotalPrice) FROM app.ImportDetail WHERE ImportID = @ImportID), 0),
                                ModifiedDate = GETDATE()
                            WHERE ImportID = @ImportID"))
                        {
                            updateCommand.Parameters.AddWithValue("@ImportID", importID);
                            updateCommand.ExecuteNonQuery();
                        }

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in DeleteImportDetail: {ex.Message}", ex);
                return false;
            }
        }

        public bool UpdateImportDetail(ImportDetail detail)
        {
            try
            {
                using (var command = CreateCommand(@"
                    UPDATE app.ImportDetail SET
                        Quantity = @Quantity,
                        UnitPrice = @UnitPrice,
                        TotalPrice = @Quantity * @UnitPrice,
                        ExpiryDate = @ExpiryDate,
                        BatchNumber = @BatchNumber
                    WHERE ImportDetailID = @ImportDetailID"))
                {
                    command.Parameters.AddWithValue("@ImportDetailID", detail.ImportDetailID);
                    command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    command.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);

                    if (detail.ExpiryDate.HasValue)
                        command.Parameters.AddWithValue("@ExpiryDate", detail.ExpiryDate.Value);
                    else
                        command.Parameters.AddWithValue("@ExpiryDate", DBNull.Value);

                    command.Parameters.AddWithValue("@BatchNumber", detail.BatchNumber ?? (object)DBNull.Value);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Update import total amount
                        using (var updateCommand = CreateCommand(@"
                            UPDATE app.Import SET
                                TotalAmount = (SELECT SUM(TotalPrice) FROM app.ImportDetail WHERE ImportID = @ImportID),
                                ModifiedDate = GETDATE()
                            WHERE ImportID = @ImportID"))
                        {
                            updateCommand.Parameters.AddWithValue("@ImportID", detail.ImportID);
                            updateCommand.ExecuteNonQuery();
                        }

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error in UpdateImportDetail: {ex.Message}", ex);
                return false;
            }
        }
    }
}