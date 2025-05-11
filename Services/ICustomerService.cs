using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Services
{
    public interface ICustomerService
    {
        // Các phương thức quản lý khách hàng
        List<CustomerDTO> GetAllCustomers();
        CustomerDTO GetCustomerByID(int customerID);
        CustomerDTO GetCustomerByPhone(string phoneNumber);
        int CreateCustomer(CustomerDTO customerDTO);
        bool UpdateCustomer(CustomerDTO customerDTO);
        bool DeleteCustomer(int customerID);

        // Các phương thức tìm kiếm
        List<CustomerDTO> SearchCustomers(string searchTerm);

        // Các phương thức quản lý điểm
        bool AddPoints(int customerID, int points, string reason, int? orderID = null, int? employeeID = null);
        bool UsePoints(int customerID, int points, string reason, int? orderID = null, int? employeeID = null);
        List<CustomerPoint> GetPointHistory(int customerID);

        // Các phương thức phân tích khách hàng
        List<CustomerDTO> GetTopCustomers(int topCount = 10, DateTime? startDate = null, DateTime? endDate = null);

        // Các phương thức quản lý cấp thành viên
        List<MembershipLevel> GetMembershipLevels();
        bool UpdateMembershipLevel(int customerID, string newLevel);
        void CalculateCustomerMembership(int customerID);
        decimal GetMembershipDiscount(string membershipLevel);
    }
}