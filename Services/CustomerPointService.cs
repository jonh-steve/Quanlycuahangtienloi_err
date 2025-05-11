// Steve-Thuong_hai
using System;
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Db.Repositories;

namespace QuanLyCuaHangTienLoi.Services
{
    public class CustomerPointService
    {
        private readonly CustomerRepository _customerRepository;
        private readonly CustomerService _customerService;
        private readonly Logger _logger;

        public CustomerPointService(CustomerRepository customerRepository, CustomerService customerService)
        {
            _customerRepository = customerRepository;
            _customerService = customerService;
            _logger = new Logger();
        }

        public bool AddPointsFromOrder(int customerID, int orderID, decimal orderAmount, int employeeID)
        {
            try
            {
                // Tính điểm từ giá trị đơn hàng
                int pointsToAdd = _customerService.CalculatePointsFromOrder(orderAmount);

                // Không có điểm để cộng
                if (pointsToAdd <= 0)
                    return true;

                // Lý do cộng điểm
                string reason = $"Đơn hàng #{orderID} - {orderAmount:N0} VNĐ";

                // Cộng điểm cho khách hàng
                return _customerRepository.UpdateCustomerPoints(customerID, pointsToAdd, reason, employeeID);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerPointService.AddPointsFromOrder: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public bool RedeemPoints(int customerID, int pointsToRedeem, string reason, int employeeID)
        {
            try
            {
                // Lấy thông tin khách hàng
                var customer = _customerService.GetCustomerByID(customerID);

                // Kiểm tra khách hàng tồn tại
                if (customer == null)
                {
                    throw new ArgumentException("Khách hàng không tồn tại");
                }

                // Kiểm tra đủ điểm để đổi
                if (customer.Points < pointsToRedeem)
                {
                    throw new ArgumentException("Không đủ điểm để đổi");
                }

                // Nếu không có lý do, tạo lý do mặc định
                if (string.IsNullOrWhiteSpace(reason))
                {
                    reason = $"Đổi điểm - {pointsToRedeem} điểm";
                }

                // Trừ điểm (số âm)
                return _customerRepository.UpdateCustomerPoints(customerID, -pointsToRedeem, reason, employeeID);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerPointService.RedeemPoints: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public bool AdjustMembershipLevel(int customerID, int employeeID)
        {
            try
            {
                // Lấy thông tin khách hàng
                var customer = _customerService.GetCustomerByID(customerID);

                // Kiểm tra khách hàng tồn tại
                if (customer == null)
                {
                    throw new ArgumentException("Khách hàng không tồn tại");
                }

                // Xác định cấp độ thành viên dựa trên điểm
                string newLevel = _customerService.DetermineMembershipLevel(customer.Points);

                // Nếu cấp độ không thay đổi, không cần cập nhật
                if (newLevel == customer.MembershipLevel)
                {
                    return true;
                }

                // Tạo đối tượng Customer để cập nhật
                var updatedCustomer = new Customer
                {
                    CustomerID = customer.CustomerID,
                    CustomerName = customer.CustomerName,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    Address = customer.Address,
                    MembershipLevel = newLevel,
                    Points = customer.Points
                };

                // Cập nhật khách hàng
                return _customerService.UpdateCustomer(updatedCustomer, employeeID);
            }
            catch (Exception ex)
            {
                _logger.Log($"Lỗi trong CustomerPointService.AdjustMembershipLevel: {ex.Message}", LogLevel.Error);
                throw;
            }
        }
    }
}