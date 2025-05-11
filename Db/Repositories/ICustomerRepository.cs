using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public interface ICustomerRepository
    {
        // Các phương thức cơ bản
        List<Customer> GetAllCustomers();
        Customer GetCustomerByID(int customerID);
        Customer GetCustomerByPhone(string phoneNumber);
        int CreateCustomer(Customer customer);
        bool UpdateCustomer(Customer customer);
        bool DeleteCustomer(int customerID);

        // Các phương thức tìm kiếm và lọc
        List<Customer> SearchCustomers(string searchTerm);
        List<Customer> GetCustomersByMembershipLevel(string level);

        // Các phương thức liên quan đến điểm tích lũy
        bool UpdateCustomerPoints(int customerID, int pointsChange, string reason, int? orderID = null, int? employeeID = null);
        List<CustomerPoint> GetCustomerPointHistory(int customerID);

        // Các phương thức thống kê
        decimal GetTotalSpent(int customerID);
        int GetOrderCount(int customerID);
        DateTime? GetLastOrderDate(int customerID);
        List<Customer> GetTopCustomers(int topCount = 10, DateTime? startDate = null, DateTime? endDate = null);
    }
}