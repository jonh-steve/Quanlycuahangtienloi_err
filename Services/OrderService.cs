// File: /Services/OrderService.cs
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.Interfaces;
using QuanLyCuaHangTienLoi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyCuaHangTienLoi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISystemConfigRepository _configRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository,
            ICustomerRepository customerRepository,
            ISystemConfigRepository configRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _customerRepository = customerRepository;
            _configRepository = configRepository;
        }

        public List<Order> GetAllOrders(DateTime? startDate = null, DateTime? endDate = null, int? employeeID = null, string paymentStatus = null)
        {
            return _orderRepository.GetAllOrders(startDate, endDate, employeeID, paymentStatus);
        }

        public Order GetOrderByID(int orderID)
        {
            return _orderRepository.GetOrderByID(orderID);
        }

        public Order GetOrderByCode(string orderCode)
        {
            return _orderRepository.GetOrderByCode(orderCode);
        }

        public int CreateOrder(Order order, List<OrderDetail> orderDetails)
        {
            // Kiểm tra tồn kho trước khi tạo đơn
            foreach (var item in orderDetails)
            {
                var product = _productRepository.GetProductByID(item.ProductID);
                var inventory = _inventoryRepository.GetInventoryByProductID(item.ProductID);

                if (product == null)
                {
                    throw new Exception($"Sản phẩm có ID {item.ProductID} không tồn tại");
                }

                if (inventory == null || inventory.Quantity < item.Quantity)
                {
                    throw new Exception($"Sản phẩm {product.ProductName} không đủ số lượng trong kho");
                }

                // Cập nhật thông tin sản phẩm cho order detail
                item.ProductName = product.ProductName;
                item.ProductCode = product.ProductCode;
                item.UnitPrice = product.SellPrice;
                item.Unit = product.Unit;
                item.TotalPrice = item.Quantity * item.UnitPrice - item.Discount;
            }

            // Cập nhật thông tin tổng đơn hàng
            order.TotalAmount = orderDetails.Sum(d => d.Quantity * d.UnitPrice);
            order.Tax = CalculateTax(order.TotalAmount);
            order.FinalAmount = order.TotalAmount + order.Tax - order.Discount;

            // Tạo đơn hàng
            int orderID = _orderRepository.CreateOrder(order, orderDetails);

            // Cập nhật điểm cho khách hàng nếu có
            if (order.CustomerID.HasValue)
            {
                decimal pointsPerAmount = 0.01m; // 1 điểm cho mỗi 100,000 VND
                int pointsEarned = (int)(order.FinalAmount * pointsPerAmount);

                if (pointsEarned > 0)
                {
                    _customerRepository.UpdateCustomerPoints(
                        order.CustomerID.Value,
                        pointsEarned,
                        $"Tích điểm từ đơn hàng {order.OrderCode}",
                        order.EmployeeID);
                }
            }

            return orderID;
        }

        public bool UpdateOrder(Order order)
        {
            return _orderRepository.UpdateOrder(order);
        }

        public bool CancelOrder(int orderID, string cancelReason, int employeeID)
        {
            return _orderRepository.CancelOrder(orderID, cancelReason, employeeID);
        }

        public decimal CalculateOrderTotal(List<OrderDetail> orderDetails)
        {
            return orderDetails.Sum(d => d.Quantity * d.UnitPrice - d.Discount);
        }

        public decimal CalculateTax(decimal subtotal)
        {
            // Lấy thuế suất từ cấu hình hệ thống
            decimal taxRate = 10; // Mặc định 10%
            var config = _configRepository.GetConfigByKey("TaxRate");
            if (config != null)
            {
                decimal.TryParse(config.ConfigValue, out taxRate);
            }

            return subtotal * (taxRate / 100);
        }

        public bool ProcessPayment(int orderID, string paymentMethod, decimal amountPaid, out decimal change)
        {
            var order = _orderRepository.GetOrderByID(orderID);
            if (order == null)
            {
                change = 0;
                return false;
            }

            // Kiểm tra số tiền khách đưa
            if (amountPaid < order.FinalAmount)
            {
                change = 0;
                return false;
            }

            change = amountPaid - order.FinalAmount;

            // Cập nhật trạng thái đơn hàng thành "Paid"
            order.PaymentStatus = "Paid";
            order.PaymentMethod = paymentMethod;

            return _orderRepository.UpdateOrder(order);
        }

        public List<OrderDetail> GetOrderDetails(int orderID)
        {
            return _orderRepository.GetOrderDetails(orderID);
        }

        public decimal GetTotalSalesByDate(DateTime date)
        {
            return _orderRepository.GetTotalSalesByDate(date);
        }

        public int GetOrderCountByDate(DateTime date)
        {
            return _orderRepository.GetOrderCountByDate(date);
        }

        public List<OrderDetail> GetTopSellingProducts(int top = 10)
        {
            return _orderDetailRepository.GetTopSellingProducts(top);
        }
    }
}