// File: /Services/Interfaces/IOrderService.cs
using QuanLyCuaHangTienLoi.Models.Entities;
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Services.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetAllOrders(DateTime? startDate = null, DateTime? endDate = null, int? employeeID = null, string paymentStatus = null);
        Order GetOrderByID(int orderID);
        Order GetOrderByCode(string orderCode);
        int CreateOrder(Order order, List<OrderDetail> orderDetails);
        bool UpdateOrder(Order order);
        bool CancelOrder(int orderID, string cancelReason, int employeeID);

        // Phương thức nghiệp vụ bán hàng
        decimal CalculateOrderTotal(List<OrderDetail> orderDetails);
        decimal CalculateTax(decimal subtotal);
        bool ProcessPayment(int orderID, string paymentMethod, decimal amountPaid, out decimal change);
        List<OrderDetail> GetOrderDetails(int orderID);

        // Báo cáo bán hàng
        decimal GetTotalSalesByDate(DateTime date);
        int GetOrderCountByDate(DateTime date);
        List<OrderDetail> GetTopSellingProducts(int top = 10);
    }
}