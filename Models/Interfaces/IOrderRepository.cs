// File: /Models/Interfaces/IOrderRepository.cs
using QuanLyCuaHangTienLoi.Models.Entities;
using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.Interfaces
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders(DateTime? startDate = null, DateTime? endDate = null, int? employeeID = null, string paymentStatus = null);
        Order GetOrderByID(int orderID);
        Order GetOrderByCode(string orderCode);
        int CreateOrder(Order order, List<OrderDetail> orderDetails);
        bool UpdateOrder(Order order);
        bool CancelOrder(int orderID, string cancelReason, int employeeID);
        List<OrderDetail> GetOrderDetails(int orderID);
        // Thêm các phương thức báo cáo
        decimal GetTotalSalesByDate(DateTime date);
        int GetOrderCountByDate(DateTime date);
    }
}