// File: /Services/InventoryService.cs
// Mô tả: Lớp triển khai IInventoryService
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;

namespace QuanLyCuaHangTienLoi.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly Logger _logger;

        public InventoryService(IInventoryRepository inventoryRepository, IProductRepository productRepository, Logger logger)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public List<InventoryDTO> GetInventoryList()
        {
            try
            {
                var inventoryList = new List<InventoryDTO>();

                // Lấy tất cả inventory từ repository
                var inventories = _inventoryRepository.GetAllInventory();

                foreach (var inventory in inventories)
                {
                    // Lấy thông tin sản phẩm
                    var product = _productRepository.GetProductByID(inventory.ProductID);

                    if (product != null)
                    {
                        // Tạo DTO với thông tin đầy đủ
                        var inventoryDTO = new InventoryDTO
                        {
                            InventoryID = inventory.InventoryID,
                            ProductID = inventory.ProductID,
                            ProductCode = product.ProductCode,
                            ProductName = product.ProductName,
                            CategoryName = product.CategoryName,
                            Quantity = inventory.Quantity,
                            Unit = product.Unit,
                            LastUpdated = inventory.LastUpdated,
                            CostPrice = product.CostPrice,
                            SellPrice = product.SellPrice,
                            TotalCostValue = inventory.Quantity * product.CostPrice,
                            MinimumStock = product.MinimumStock,
                            IsLowStock = inventory.Quantity <= product.MinimumStock
                        };

                        inventoryList.Add(inventoryDTO);
                    }
                }

                return inventoryList;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in GetInventoryList: {ex.Message}", ex);
                throw;
            }
        }

        public InventoryDTO GetProductInventory(int productID)
        {
            try
            {
                // Lấy thông tin tồn kho
                var inventory = _inventoryRepository.GetInventoryByProductID(productID);

                if (inventory == null)
                {
                    // Nếu chưa có trong kho, trả về thông tin mặc định
                    var product = _productRepository.GetProductByID(productID);

                    if (product != null)
                    {
                        return new InventoryDTO
                        {
                            ProductID = productID,
                            ProductCode = product.ProductCode,
                            ProductName = product.ProductName,
                            CategoryName = product.CategoryName,
                            Quantity = 0,
                            Unit = product.Unit,
                            LastUpdated = DateTime.Now,
                            CostPrice = product.CostPrice,
                            SellPrice = product.SellPrice,
                            TotalCostValue = 0,
                            MinimumStock = product.MinimumStock,
                            IsLowStock = true
                        };
                    }

                    return null;
                }

                // Lấy thông tin sản phẩm
                var productInfo = _productRepository.GetProductByID(productID);

                if (productInfo == null)
                {
                    return null;
                }

                // Tạo DTO với thông tin đầy đủ
                return new InventoryDTO
                {
                    InventoryID = inventory.InventoryID,
                    ProductID = inventory.ProductID,
                    ProductCode = productInfo.ProductCode,
                    ProductName = productInfo.ProductName,
                    CategoryName = productInfo.CategoryName,
                    Quantity = inventory.Quantity,
                    Unit = productInfo.Unit,
                    LastUpdated = inventory.LastUpdated,
                    CostPrice = productInfo.CostPrice,
                    SellPrice = productInfo.SellPrice,
                    TotalCostValue = inventory.Quantity * productInfo.CostPrice,
                    MinimumStock = productInfo.MinimumStock,
                    IsLowStock = inventory.Quantity <= productInfo.MinimumStock
                };
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in GetProductInventory: {ex.Message}", ex);
                throw;
            }
        }

        public bool AdjustInventory(int productID, int newQuantity, string reason, int employeeID)
        {
            try
            {
                return _inventoryRepository.UpdateInventoryQuantity(productID, newQuantity, reason, employeeID);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in AdjustInventory: {ex.Message}", ex);
                throw;
            }
        }

        public List<InventoryDTO> GetLowStockWarnings()
        {
            try
            {
                var lowStockList = new List<InventoryDTO>();

                // Lấy danh sách sản phẩm tồn kho thấp
                var lowStockItems = _inventoryRepository.GetLowStockInventory();

                foreach (var item in lowStockItems)
                {
                    // Lấy thông tin sản phẩm
                    var product = _productRepository.GetProductByID(item.ProductID);

                    if (product != null)
                    {
                        // Tạo DTO với thông tin đầy đủ
                        var inventoryDTO = new InventoryDTO
                        {
                            ProductID = item.ProductID,
                            ProductCode = product.ProductCode,
                            ProductName = product.ProductName,
                            CategoryName = product.CategoryName,
                            Quantity = item.Quantity,
                            Unit = product.Unit,
                            LastUpdated = item.LastUpdated,
                            CostPrice = product.CostPrice,
                            SellPrice = product.SellPrice,
                            TotalCostValue = item.Quantity * product.CostPrice,
                            MinimumStock = product.MinimumStock,
                            IsLowStock = true,
                            ShortageAmount = product.MinimumStock - item.Quantity
                        };

                        lowStockList.Add(inventoryDTO);
                    }
                }

                return lowStockList;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in GetLowStockWarnings: {ex.Message}", ex);
                throw;
            }
        }

        public List<InventoryTransactionDTO> GetInventoryHistory(int productID, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var historyList = new List<InventoryTransactionDTO>();

                // Lấy lịch sử giao dịch từ repository
                var transactions = _inventoryRepository.GetInventoryTransactionsByProduct(productID, startDate, endDate);

                // Lấy thông tin sản phẩm
                var product = _productRepository.GetProductByID(productID);

                if (product == null)
                {
                    return historyList;
                }

                foreach (var transaction in transactions)
                {
                    // Tạo DTO với thông tin đầy đủ
                    var transactionDTO = new InventoryTransactionDTO
                    {
                        TransactionID = transaction.TransactionID,
                        ProductID = transaction.ProductID,
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode,
                        TransactionType = transaction.TransactionType,
                        Quantity = transaction.Quantity,
                        PreviousQuantity = transaction.PreviousQuantity,
                        CurrentQuantity = transaction.CurrentQuantity,
                        UnitPrice = transaction.UnitPrice,
                        TotalAmount = transaction.TotalAmount,
                        ReferenceID = transaction.ReferenceID,
                        ReferenceType = transaction.ReferenceType,
                        Note = transaction.Note,
                        TransactionDate = transaction.TransactionDate,
                        CreatedBy = transaction.CreatedBy
                    };

                    historyList.Add(transactionDTO);
                }

                return historyList;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in GetInventoryHistory: {ex.Message}", ex);
                throw;
            }
        }

        public bool ProcessStockForSale(int productID, int quantity, int orderID)
        {
            try
            {
                // Kiểm tra tồn kho
                if (!_inventoryRepository.CheckInventoryAvailability(productID, quantity))
                {
                    return false;
                }

                // Lấy thông tin sản phẩm
                var product = _productRepository.GetProductByID(productID);

                if (product == null)
                {
                    return false;
                }

                // Lấy thông tin tồn kho hiện tại
                var inventory = _inventoryRepository.GetInventoryByProductID(productID);

                if (inventory == null)
                {
                    return false;
                }

                // Tạo giao dịch tồn kho
                var transaction = new InventoryTransaction
                {
                    ProductID = productID,
                    TransactionType = "Sale",
                    Quantity = quantity,
                    PreviousQuantity = inventory.Quantity,
                    CurrentQuantity = inventory.Quantity - quantity,
                    UnitPrice = product.SellPrice,
                    TotalAmount = product.SellPrice * quantity,
                    ReferenceID = orderID,
                    ReferenceType = "Order",
                    TransactionDate = DateTime.Now
                };

                // Thêm giao dịch tồn kho
                _inventoryRepository.AddInventoryTransaction(transaction);

                // Cập nhật số lượng tồn kho
                return _inventoryRepository.UpdateInventoryQuantity(productID, inventory.Quantity - quantity, "Sale from Order #" + orderID, 0);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in ProcessStockForSale: {ex.Message}", ex);
                return false;
            }
        }

        public bool ProcessStockForImport(int productID, int quantity, int importID, decimal costPrice)
        {
            try
            {
                // Lấy thông tin tồn kho hiện tại
                var inventory = _inventoryRepository.GetInventoryByProductID(productID);

                int currentQuantity = 0;

                if (inventory != null)
                {
                    currentQuantity = inventory.Quantity;
                }

                // Tạo giao dịch tồn kho
                var transaction = new InventoryTransaction
                {
                    ProductID = productID,
                    TransactionType = "Import",
                    Quantity = quantity,
                    PreviousQuantity = currentQuantity,
                    CurrentQuantity = currentQuantity + quantity,
                    UnitPrice = costPrice,
                    TotalAmount = costPrice * quantity,
                    ReferenceID = importID,
                    ReferenceType = "Import",
                    TransactionDate = DateTime.Now
                };

                // Thêm giao dịch tồn kho
                _inventoryRepository.AddInventoryTransaction(transaction);

                // Cập nhật số lượng tồn kho
                return _inventoryRepository.UpdateInventoryQuantity(productID, currentQuantity + quantity, "Import from ImportID #" + importID, 0);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in ProcessStockForImport: {ex.Message}", ex);
                return false;
            }
        }

        public bool ProcessStockForOrderCancellation(int orderID)
        {
            try
            {
                // TODO: Lấy chi tiết đơn hàng từ OrderRepository
                // Xử lý hoàn lại tồn kho cho từng sản phẩm

                // Mã giả:
                
                var orderDetails = _orderRepository.GetOrderDetailsByOrderID(orderID);
                
                foreach (var detail in orderDetails)
                {
                    // Lấy thông tin tồn kho hiện tại
                    var inventory = _inventoryRepository.GetInventoryByProductID(detail.ProductID);
                    
                    if (inventory != null)
                    {
                        // Tạo giao dịch tồn kho
                        var transaction = new InventoryTransaction
                        {
                            ProductID = detail.ProductID,
                            TransactionType = "Return",
                            Quantity = detail.Quantity,
                            PreviousQuantity = inventory.Quantity,
                            CurrentQuantity = inventory.Quantity + detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                            TotalAmount = detail.UnitPrice * detail.Quantity,
                            ReferenceID = orderID,
                            ReferenceType = "OrderCancel",
                            Note = "Order cancelled",
                            TransactionDate = DateTime.Now
                        };
                        
                        // Thêm giao dịch tồn kho
                        _inventoryRepository.AddInventoryTransaction(transaction);
                        
                        // Cập nhật số lượng tồn kho
                        _inventoryRepository.UpdateInventoryQuantity(detail.ProductID, inventory.Quantity + detail.Quantity, "Return from cancelled Order #" + orderID, 0);
                    }
                }
                

                return true;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in ProcessStockForOrderCancellation: {ex.Message}", ex);
                return false;
            }
        }

        public byte[] ExportInventoryReport(DateTime asOfDate)
        {
            // TODO: Implement export inventory report
            // Có thể sử dụng thư viện như EPPlus để xuất ra Excel

            return null;
        }
    }
}