using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangTienLoi.Db
{
    public class ConnectionManager : IDisposable
    {
        private static readonly string _connectionString = AppSettings.Instance.ConnectionString;
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public ConnectionManager()
        {
            _connection = new SqlConnection(_connectionString);
        }

        public SqlConnection Connection => _connection;

        public void Open()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Không thể mở kết nối đến database", ex);
                throw;
            }
        }

        public void Close()
        {
            try
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Không thể đóng kết nối database", ex);
            }
        }

        public void BeginTransaction()
        {
            try
            {
                Open();
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Không thể bắt đầu transaction", ex);
                throw;
            }
        }

        public void Commit()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Không thể commit transaction", ex);
                throw;
            }
        }

        public void Rollback()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Không thể rollback transaction", ex);
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            Close();
            _connection.Dispose();
        }
    }
}
