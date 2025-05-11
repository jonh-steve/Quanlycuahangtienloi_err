// Steve-Thuong_hai
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Db;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public class CustomerRepository : BaseRepository
    {
        public CustomerRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public List<CustomerDTO> GetAllCustomers()
        {
            var customers = new List<CustomerDTO>();

            try
            {
                using (var command = CreateCommand("app.sp_GetAllCustomers"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new CustomerDTO
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Email = reader["Email"].ToString(),
                                Address = reader["Address"].ToString(),
                                MembershipLevel = reader["MembershipLevel"].ToString(),
                                Points = Convert.ToInt32(reader["Points"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null,
                                OrderCount = reader["OrderCount"] != DBNull.Value ? Convert.ToInt32(reader["OrderCount"]) : 0,
                                TotalSpent = reader["TotalSpent"] != DBNull.Value ? Convert.ToDecimal(reader["TotalSpent"]) : 0,
                                LastOrderDate = reader["LastOrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastOrderDate"]) : (DateTime?)null
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Lỗi khi lấy danh sách khách hàng: {ex.Message}", LogLevel.Error);
                throw;
            }

            return customers;
        }

        public CustomerDTO GetCustomerByID(int customerID)
        {
            CustomerDTO customer = null;

            try
            {
                using (var command = CreateCommand("app.sp_GetCustomerByID"))
                {
                    command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer = new CustomerDTO
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Email = reader["Email"].ToString(),
                                Address = reader["Address"].ToString(),
                                MembershipLevel = reader["MembershipLevel"].ToString(),
                                Points = Convert.ToInt32(reader["Points"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Lỗi khi lấy thông tin khách hàng ID {customerID}: {ex.Message}", LogLevel.Error);
                throw;
            }

            return customer;
        }

        public int CreateCustomer(Customer customer, int createdBy)
        {
            int customerID = 0;

            try
            {
                using (var command = CreateCommand("app.sp_CreateCustomer"))
                {
                    command.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 100).Value = customer.CustomerName;
                    command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = (object)customer.PhoneNumber ?? DBNull.Value;
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object)customer.Email ?? DBNull.Value;
                    command.Parameters.Add("@Address", SqlDbType.NVarChar, 200).Value = (object)customer.Address ?? DBNull.Value;
                    command.Parameters.Add("@MembershipLevel", SqlDbType.NVarChar, 20).Value = customer.MembershipLevel;
                    command.Parameters.Add("@Points", SqlDbType.Int).Value = customer.Points;
                    command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = createdBy;

                    var outputParam = command.Parameters.Add("@CustomerID", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    customerID = (int)outputParam.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Lỗi khi tạo khách hàng mới: {ex.Message}", LogLevel.Error);
                throw;
            }

            return customerID;
        }

        public bool UpdateCustomer(Customer customer, int modifiedBy)
        {
            try
            {
                using (var command = CreateCommand("app.sp_UpdateCustomer"))
                {
                    command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customer.CustomerID;
                    command.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 100).Value = customer.CustomerName;
                    command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = (object)customer.PhoneNumber ?? DBNull.Value;
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object)customer.Email ?? DBNull.Value;
                    command.Parameters.Add("@Address", SqlDbType.NVarChar, 200).Value = (object)customer.Address ?? DBNull.Value;
                    command.Parameters.Add("@MembershipLevel", SqlDbType.NVarChar, 20).Value = customer.MembershipLevel;
                    command.Parameters.Add("@Points", SqlDbType.Int).Value = customer.Points;
                    command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = modifiedBy;

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Lỗi khi cập nhật khách hàng ID {customer.CustomerID}: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public List<CustomerDTO> SearchCustomers(string searchTerm)
        {
            var customers = new List<CustomerDTO>();

            try
            {
                using (var command = CreateCommand("app.sp_SearchCustomers"))
                {
                    command.Parameters.Add("@SearchTerm", SqlDbType.NVarChar, 100).Value = searchTerm;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new CustomerDTO
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Email = reader["Email"].ToString(),
                                Address = reader["Address"].ToString(),
                                MembershipLevel = reader["MembershipLevel"].ToString(),
                                Points = Convert.ToInt32(reader["Points"]),
                                OrderCount = reader["OrderCount"] != DBNull.Value ? Convert.ToInt32(reader["OrderCount"]) : 0,
                                TotalSpent = reader["TotalSpent"] != DBNull.Value ? Convert.ToDecimal(reader["TotalSpent"]) : 0,
                                LastOrderDate = reader["LastOrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastOrderDate"]) : (DateTime?)null
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Lỗi khi tìm kiếm khách hàng với từ khóa '{searchTerm}': {ex.Message}", LogLevel.Error);
                throw;
            }

            return customers;
        }

        public bool UpdateCustomerPoints(int customerID, int pointsChange, string reason, int modifiedBy)
        {
            try
            {
                using (var command = CreateCommand("app.sp_UpdateCustomerPoints"))
                {
                    command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                    command.Parameters.Add("@PointsChange", SqlDbType.Int).Value = pointsChange;
                    command.Parameters.Add("@Reason", SqlDbType.NVarChar, 200).Value = reason;
                    command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = modifiedBy;

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Lỗi khi cập nhật điểm khách hàng ID {customerID}: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public List<CustomerPointHistoryDTO> GetCustomerPointHistory(int customerID)
        {
            var history = new List<CustomerPointHistoryDTO>();

            try
            {
                using (var command = CreateCommand("app.sp_GetCustomerPointHistory"))
                {
                    command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new CustomerPointHistoryDTO
                            {
                                PointID = Convert.ToInt32(reader["PointID"]),
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                Points = Convert.ToInt32(reader["Points"]),
                                PointType = reader["PointType"].ToString(),
                                Description = reader["Description"].ToString(),
                                ReferenceID = reader["ReferenceID"] != DBNull.Value ? Convert.ToInt32(reader["ReferenceID"]) : (int?)null,
                                ReferenceType = reader["ReferenceType"].ToString(),
                                ReferenceDescription = reader["ReferenceDescription"].ToString(),
                                TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                                CreatedByName = reader["CreatedByName"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Lỗi khi lấy lịch sử điểm khách hàng ID {customerID}: {ex.Message}", LogLevel.Error);
                throw;
            }

            return history;
        }
    }
}