// File: /Models/Repositories/OrderRepository.cs
using QuanLyCuaHangTienLoi.Db;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace QuanLyCuaHangTienLoi.Models.Repositories
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public List<Order> GetAllOrders(DateTime? startDate = null, DateTime? endDate = null, int? employeeID = null, string paymentStatus = null)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("app.sp_GetOrdersByDateRange", Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate ?? SqlDateTime.MinValue.Value);
                    cmd.Parameters.AddWithValue("@EndDate", endDate ?? SqlDateTime.MaxValue.Value);

                    if (employeeID.HasValue)
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID.Value);
                    else
                        cmd.Parameters.AddWithValue("@EmployeeID", DBNull.Value);

                    if (!string.IsNullOrEmpty(paymentStatus))
                        cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                    else
                        cmd.Parameters.AddWithValue("@PaymentStatus", DBNull.Value);

                    List<Order> orders = new List<Order>();
                    Connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order order = new Order
                            {
                                OrderID = Convert.ToInt32(reader["OrderID"]),
                                OrderCode = reader["OrderCode"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                Tax = Convert.ToDecimal(reader["Tax"]),
                                FinalAmount = Convert.ToDecimal(reader["FinalAmount"]),
                                PaymentStatus = reader["PaymentStatus"].ToString(),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                CustomerName = reader["CustomerName"].ToString(),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                ItemCount = Convert.ToInt32(reader["ItemCount"])
                            };
                            orders.Add(order);
                        }
                    }
                    return orders;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }

        public Order GetOrderByID(int orderID)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("app.sp_GetOrderByID", Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderID);

                    Order order = null;
                    List<OrderDetail> orderDetails = new List<OrderDetail>();

                    Connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = new Order
                            {
                                OrderID = Convert.ToInt32(reader["OrderID"]),
                                OrderCode = reader["OrderCode"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                Discount = Convert.ToDecimal(reader["Discount"]),
                                Tax = Convert.ToDecimal(reader["Tax"]),
                                FinalAmount = Convert.ToDecimal(reader["FinalAmount"]),
                                PaymentStatus = reader["PaymentStatus"].ToString(),
                                Note = reader["Note"].ToString(),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                CustomerID = reader["CustomerID"] != DBNull.Value ? Convert.ToInt32(reader["CustomerID"]) : (int?)null,
                                CustomerName = reader["CustomerName"].ToString(),
                                CustomerPhone = reader["CustomerPhone"].ToString(),
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                EmployeeName = reader["EmployeeName"].ToString()
                            };
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            OrderDetail detail = new OrderDetail
                            {
                                OrderDetailID = Convert.ToInt32(reader["OrderDetailID"]),
                                OrderID = Convert.ToInt32(reader["OrderID"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["ProductName"].ToString(),
                                ProductCode = reader["ProductCode"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                Discount = Convert.ToDecimal(reader["Discount"]),
                                TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                                Unit = reader["Unit"].ToString()
                            };
                            orderDetails.Add(detail);
                        }
                    }

                    if (order != null)
                    {
                        order.OrderDetails = orderDetails;
                    }

                    return order;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }

        public Order GetOrderByCode(string orderCode)
        {
            // Triển khai phương thức này tương tự như GetOrderByID
            throw new NotImplementedException();
        }

        public int CreateOrder(Order order, List<OrderDetail> orderDetails)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("app.sp_CreateOrder", Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (order.CustomerID.HasValue)
                        cmd.Parameters.AddWithValue("@CustomerID", order.CustomerID.Value);
                    else
                        cmd.Parameters.AddWithValue("@CustomerID", DBNull.Value);

                    cmd.Parameters.AddWithValue("@EmployeeID", order.EmployeeID);
                    cmd.Parameters.AddWithValue("@PaymentMethodID", order.PaymentMethodID);

                    if (!string.IsNullOrEmpty(order.Note))
                        cmd.Parameters.AddWithValue("@Note", order.Note);
                    else
                        cmd.Parameters.AddWithValue("@Note", DBNull.Value);

                    // Tạo bảng tạm để lưu chi tiết đơn hàng
                    DataTable orderItemsTable = new DataTable();
                    orderItemsTable.Columns.Add("ProductID", typeof(int));
                    orderItemsTable.Columns.Add("Quantity", typeof(int));
                    orderItemsTable.Columns.Add("UnitPrice", typeof(decimal));
                    orderItemsTable.Columns.Add("Discount", typeof(decimal));

                    foreach (var item in orderDetails)
                    {
                        orderItemsTable.Rows.Add(item.ProductID, item.Quantity, item.UnitPrice, item.Discount);
                    }

                    SqlParameter orderItemsParam = cmd.Parameters.AddWithValue("@OrderItems", orderItemsTable);
                    orderItemsParam.SqlDbType = SqlDbType.Structured;
                    orderItemsParam.TypeName = "app.OrderItemType";

                    SqlParameter orderIDParam = cmd.Parameters.Add("@OrderID", SqlDbType.Int);
                    orderIDParam.Direction = ParameterDirection.Output;

                    SqlParameter orderCodeParam = cmd.Parameters.Add("@OrderCode", SqlDbType.NVarChar, 20);
                    orderCodeParam.Direction = ParameterDirection.Output;

                    SqlParameter finalAmountParam = cmd.Parameters.Add("@FinalAmount", SqlDbType.Decimal);
                    finalAmountParam.Precision = 18;
                    finalAmountParam.Scale = 2;
                    finalAmountParam.Direction = ParameterDirection.Output;

                    Connection.Open();
                    cmd.ExecuteNonQuery();

                    int orderID = (int)orderIDParam.Value;
                    string orderCode = orderCodeParam.Value.ToString();
                    decimal finalAmount = (decimal)finalAmountParam.Value;

                    // Cập nhật lại thông tin cho đơn hàng
                    order.OrderID = orderID;
                    order.OrderCode = orderCode;
                    order.FinalAmount = finalAmount;

                    return orderID;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }

        public bool UpdateOrder(Order order)
        {
            // Triển khai phương thức này
            throw new NotImplementedException();
        }

        public bool CancelOrder(int orderID, string cancelReason, int employeeID)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("app.sp_CancelOrder", Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderID);
                    cmd.Parameters.AddWithValue("@CancelReason", cancelReason);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                    Connection.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }

        public List<OrderDetail> GetOrderDetails(int orderID)
        {
            // Triển khai phương thức này
            throw new NotImplementedException();
        }

        public decimal GetTotalSalesByDate(DateTime date)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SUM(FinalAmount) FROM app.Order WHERE CAST(OrderDate AS DATE) = @Date AND PaymentStatus = 'Paid'", Connection))
                {
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    Connection.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return 0;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }

        public int GetOrderCountByDate(DateTime date)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM app.Order WHERE CAST(OrderDate AS DATE) = @Date AND PaymentStatus = 'Paid'", Connection))
                {
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    Connection.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return 0;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
        }
    }
}