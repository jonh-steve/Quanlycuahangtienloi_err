// Steve-Thuong_hai
using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Utils;

namespace QuanLyCuaHangTienLoi.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _customerRepository;
        private readonly Logger _logger;

        public CustomerService(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _logger = new Logger();
        }

        public List<CustomerDTO> GetAllCustomers()
        {
            try
            {
                return _customerRepository.GetAllCustomers();
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerService.GetAllCustomers: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public CustomerDTO GetCustomerByID(int customerID)
        {
            try
            {
                return _customerRepository.GetCustomerByID(customerID);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerService.GetCustomerByID: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public int CreateCustomer(Customer customer, int createdBy)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (string.IsNullOrWhiteSpace(customer.CustomerName))
                {
                    throw new ArgumentException("Tên khách hàng không được để trống");
                }

                if (!string.IsNullOrWhiteSpace(customer.PhoneNumber))
                {
                    if (!Validators.IsValidPhoneNumber(customer.PhoneNumber))
                    {
                        throw new ArgumentException("Số điện thoại không hợp lệ");
                    }
                }

                if (!string.IsNullOrWhiteSpace(customer.Email))
                {
                    if (!Validators.IsValidEmail(customer.Email))
                    {
                        throw new ArgumentException("Email không hợp lệ");
                    }
                }

                // Kiểm tra cấp độ thành viên
                if (string.IsNullOrWhiteSpace(customer.MembershipLevel))
                {
                    customer.MembershipLevel = "Regular";
                }

                return _customerRepository.CreateCustomer(customer, createdBy);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerService.CreateCustomer: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public bool UpdateCustomer(Customer customer, int modifiedBy)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (customer.CustomerID <= 0)
                {
                    throw new ArgumentException("ID khách hàng không hợp lệ");
                }

                if (string.IsNullOrWhiteSpace(customer.CustomerName))
                {
                    throw new ArgumentException("Tên khách hàng không được để trống");
                }

                if (!string.IsNullOrWhiteSpace(customer.PhoneNumber))
                {
                    if (!Validators.IsValidPhoneNumber(customer.PhoneNumber))
                    {
                        throw new ArgumentException("Số điện thoại không hợp lệ");
                    }
                }

                if (!string.IsNullOrWhiteSpace(customer.Email))
                {
                    if (!Validators.IsValidEmail(customer.Email))
                    {
                        throw new ArgumentException("Email không hợp lệ");
                    }
                }

                return _customerRepository.UpdateCustomer(customer, modifiedBy);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerService.UpdateCustomer: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public List<CustomerDTO> SearchCustomers(string searchTerm)
        {
            try
            {
                // Đảm bảo searchTerm không null
                searchTerm = searchTerm ?? string.Empty;

                return _customerRepository.SearchCustomers(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerService.SearchCustomers: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public bool UpdateCustomerPoints(int customerID, int pointsChange, string reason, int modifiedBy)
        {
            try
            {
                if (customerID <= 0)
                {
                    throw new ArgumentException("ID khách hàng không hợp lệ");
                }

                if (string.IsNullOrWhiteSpace(reason))
                {
                    reason = pointsChange >= 0 ? "Cộng điểm" : "Trừ điểm";
                }

                return _customerRepository.UpdateCustomerPoints(customerID, pointsChange, reason, modifiedBy);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerService.UpdateCustomerPoints: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public List<CustomerPointHistoryDTO> GetCustomerPointHistory(int customerID)
        {
            try
            {
                if (customerID <= 0)
                {
                    throw new ArgumentException("ID khách hàng không hợp lệ");
                }

                return _customerRepository.GetCustomerPointHistory(customerID);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerService.GetCustomerPointHistory: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public string DetermineMembershipLevel(int points)
        {
            if (points >= 5000)
                return "Platinum";
            else if (points >= 3000)
                return "Gold";
            else if (points >= 1000)
                return "Silver";
            else
                return "Regular";
        }

        public decimal GetMembershipDiscount(string membershipLevel)
        {
            switch (membershipLevel)
            {
                case "Platinum":
                    return 0.10m; // 10%
                case "Gold":
                    return 0.05m; // 5%
                case "Silver":
                    return 0.03m; // 3%
                default:
                    return 0.00m; // 0%
            }
        }

        public int CalculatePointsFromOrder(decimal orderAmount)
        {
            // Quy đổi: cứ 10,000 VNĐ = 1 điểm
            return (int)(orderAmount / 10000);
        }
    }
}