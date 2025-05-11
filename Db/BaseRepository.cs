// Mã gợi ý cho BaseRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace QuanLyCuaHangTienLoi.Db
{
    public abstract class BaseRepository
    {
        protected readonly ConnectionManager _connectionManager;
        protected readonly Logger _logger;

        public BaseRepository()
        {
            _connectionManager = ConnectionManager.Instance;
            _logger = new Logger();
        }

        #region Các phương thức thực thi SQL

        protected int ExecuteNonQuery(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SqlConnection connection = _connectionManager.CreateConnection())
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = storedProcedure;

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi thực thi ExecuteNonQuery: {ex.Message}", ex);
                throw;
            }
        }

        protected T ExecuteScalar<T>(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SqlConnection connection = _connectionManager.CreateConnection())
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = storedProcedure;

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result == DBNull.Value)
                            return default(T);

                        return (T)Convert.ChangeType(result, typeof(T));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi thực thi ExecuteScalar: {ex.Message}", ex);
                throw;
            }
        }

        protected DataTable ExecuteReader(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SqlConnection connection = _connectionManager.CreateConnection())
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = storedProcedure;

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        DataTable dataTable = new DataTable();
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi thực thi ExecuteReader: {ex.Message}", ex);
                throw;
            }
        }

        #endregion

        #region Các phương thức xử lý Transaction

        protected void ExecuteInTransaction(Action<SqlConnection, SqlTransaction> action)
        {
            using (SqlConnection connection = _connectionManager.CreateConnection())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    action(connection, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error($"Lỗi trong transaction: {ex.Message}", ex);
                    throw;
                }
            }
        }

        protected T ExecuteInTransaction<T>(Func<SqlConnection, SqlTransaction, T> func)
        {
            using (SqlConnection connection = _connectionManager.CreateConnection())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    T result = func(connection, transaction);
                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error($"Lỗi trong transaction: {ex.Message}", ex);
                    throw;
                }
            }
        }

        #endregion

        #region Các phương thức truy vấn tùy chỉnh

        protected DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SqlConnection connection = _connectionManager.CreateConnection())
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = sql;

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        DataTable dataTable = new DataTable();
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi thực thi truy vấn SQL: {ex.Message}", ex);
                throw;
            }
        }

        #endregion
    }
}