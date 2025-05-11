// File: /Models/Interfaces/IOrderDetailRepository.cs
using QuanLyCuaHangTienLoi.Models.Entities;
using System.Collections.Generic;

namespace QuanLyCuaHangTienLoi.Models.Interfaces
{
    public interface IOrderDetailRepository
    {
        List<OrderDetail> GetByOrderID(int orderID);
        bool AddOrderDetail(OrderDetail orderDetail);
        bool UpdateOrderDetail(OrderDetail orderDetail);
        bool DeleteOrderDetail(int orderDetailID);
        List<OrderDetail> GetTopSellingProducts(int top = 10);
    }
}