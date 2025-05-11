// File: /Services/ImportService.cs
// Mô tả: Lớp triển khai IImportService
// Author: Steve-Thuong_hai

using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyCuaHangTienLoi.Db.Repositories;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Models.DTO;
using QuanLyCuaHangTienLoi.Services.Interfaces;
using Serilog;

namespace QuanLyCuaHangTienLoi.Services
{
    public class ImportService : IImportService
    {
        private readonly IImportRepository _importRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly Logger _logger;

        public ImportService(
            IImportRepository importRepository,
            IInventoryService inventoryService,
            IProductRepository productRepository,
            ISupplierRepository supplierRepository,
            Logger logger)
        {
            _importRepository = importRepository;
            _inventoryService = inventoryService;
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _logger = logger;
        }

        public List<ImportDTO> GetImportList(DateTime? startDate = null, DateTime? endDate = null, int? supplierID = null)
        {
            try
            {
                var importList = new List<ImportDTO>();

                // Nếu không có ngày bắt đầu và kết thúc, lấy danh sách tất cả phiếu nhập
                if (!startDate.HasValue && !endDate.HasValue)
                {
                    var imports = _importRepository.GetAllImports();

                    foreach (var import in imports)
                    {
                        importList.Add(new ImportDTO
                        {
                            ImportID = import.ImportID,
                            SupplierID = import.SupplierID,
                            ImportCode = import.ImportCode,
                            ImportDate = import.ImportDate,
                            TotalAmount = import.TotalAmount,
                            Status = import.Status,
                            Note = import.Note,
                            CreatedBy = import.CreatedBy,
                            CreatedDate = import.CreatedDate,
                            ModifiedDate = import.ModifiedDate,
                            SupplierName = import.SupplierName,
                            EmployeeName = import.EmployeeName,
                            ItemCount = import.ItemCount
                        });
                    }
                }
                else
                {
                    // Sử dụng ngày mặc định nếu không có
                    var start = startDate ?? DateTime.Now.AddMonths(-1);
                    var end = endDate ?? DateTime.Now;

                    var imports = _importRepository.GetImportsByDateRange(start, end, supplierID);

                    foreach (var import in imports)
                    {
                        importList.Add(new ImportDTO
                        {
                            ImportID = import.ImportID,
                            ImportCode = import.ImportCode,
                            ImportDate = import.ImportDate,
                            TotalAmount = import.TotalAmount,
                            Status = import.Status,
                            SupplierID = import.SupplierID,
                            SupplierName = import.SupplierName,
                            EmployeeName = import.EmployeeName,
                            ItemCount = import.ItemCount
                        });
                    }
                }

                return importList;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in GetImportList: {ex.Message}", ex);
                throw;
            }
        }

        public ImportDTO GetImportDetails(int importID)
        {
            try
            {
                // Lấy thông tin phiếu nhập
                var import = _importRepository.GetImportByID(importID);

                if (import == null)
                {
                    return null;
                }

                // Tạo DTO
                var importDTO = new ImportDTO
                {
                    ImportID = import.ImportID,
                    SupplierID = import.SupplierID,
                    ImportCode = import.ImportCode,
                    ImportDate = import.ImportDate,
                    TotalAmount = import.TotalAmount,
                    Status = import.Status,
                    Note = import.Note,
                    CreatedBy = import.CreatedBy,
                    SupplierName = import.SupplierName,
                    EmployeeName = import.EmployeeName
                };

                // Lấy chi tiết phiếu nhập
                if (import.Details != null && import.Details.Count > 0)
                {
                    importDTO.Details = new List<ImportDetailDTO>();

                    foreach (var detail in import.Details)
                    {
                        importDTO.Details.Add(new ImportDetailDTO
                        {
                            ImportDetailID = detail.ImportDetailID,
                            ImportID = detail.ImportID,
                            ProductID = detail.ProductID,
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                            TotalPrice = detail.TotalPrice,
                            ExpiryDate = detail.ExpiryDate,
                            BatchNumber = detail.BatchNumber,
                            ProductName = detail.ProductName,
                            ProductCode = detail.ProductCode,
                            Unit = detail.Unit
                        });
                    }
                }

                return importDTO;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in GetImportDetails: {ex.Message}", ex);
                throw;
            }
        }

        public int CreateNewImport(ImportDTO importDTO, List<ImportDetailDTO> details, int employeeID)
        {
            try
            {
                // Tạo đối tượng Import
                var import = new Import
                {
                    SupplierID = importDTO.SupplierID,
                    Note = importDTO.Note,
                    CreatedBy = employeeID
                };

                // Tạo danh sách ImportDetail
                var importDetails = new List<ImportDetail>();

                foreach (var detail in details)
                {
                    importDetails.Add(new ImportDetail
                    {
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        ExpiryDate = detail.ExpiryDate,
                        BatchNumber = detail.BatchNumber
                    });
                }

                // Gọi repository để tạo phiếu nhập
                return _importRepository.CreateImport(import, importDetails);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in CreateNewImport: {ex.Message}", ex);
                throw;
            }
        }

        public bool UpdateImport(ImportDTO importDTO, List<ImportDetailDTO> details)
        {
            try
            {
                // Kiểm tra xem phiếu nhập có thể cập nhật không
                if (!CanEditImport(importDTO.ImportID))
                {
                    return false;
                }

                // Cập nhật thông tin phiếu nhập
                var import = new Import
                {
                    ImportID = importDTO.ImportID,
                    Note = importDTO.Note
                };

                bool result = _importRepository.UpdateImport(import);

                if (!result)
                {
                    return false;
                }

                // Lấy chi tiết hiện tại
                var currentDetails = _importRepository.GetImportDetailsByImportID(importDTO.ImportID);

                // Danh sách chi tiết cần thêm, cập nhật, xóa
                var detailsToAdd = new List<ImportDetailDTO>();
                var detailsToUpdate = new List<ImportDetailDTO>();
                var detailsToDelete = new List<int>();

                // Xác định hành động cho từng chi tiết
                foreach (var detail in details)
                {
                    if (detail.ImportDetailID <= 0)
                    {
                        // Chi tiết mới
                        detailsToAdd.Add(detail);
                    }
                    else
                    {
                        // Chi tiết cần cập nhật
                        detailsToUpdate.Add(detail);
                    }
                }

                // Xác định chi tiết cần xóa
                foreach (var currentDetail in currentDetails)
                {
                    if (!details.Any(d => d.ImportDetailID == currentDetail.ImportDetailID))
                    {
                        detailsToDelete.Add(currentDetail.ImportDetailID);
                    }
                }

                // Thực hiện thêm chi tiết mới
                foreach (var detail in detailsToAdd)
                {
                    var importDetail = new ImportDetail
                    {
                        ImportID = importDTO.ImportID,
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        ExpiryDate = detail.ExpiryDate,
                        BatchNumber = detail.BatchNumber
                    };

                    _importRepository.AddImportDetail(importDetail);
                }

                // Thực hiện cập nhật chi tiết
                foreach (var detail in detailsToUpdate)
                {
                    var importDetail = new ImportDetail
                    {
                        ImportDetailID = detail.ImportDetailID,
                        ImportID = importDTO.ImportID,
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        ExpiryDate = detail.ExpiryDate,
                        BatchNumber = detail.BatchNumber
                    };

                    _importRepository.UpdateImportDetail(importDetail);
                }

                // Thực hiện xóa chi tiết
                foreach (var detailID in detailsToDelete)
                {
                    _importRepository.DeleteImportDetail(detailID);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in UpdateImport: {ex.Message}", ex);
                return false;
            }
        }

        public bool CompleteImport(int importID, int employeeID)
        {
            try
            {
                // Kiểm tra xem phiếu nhập có thể hoàn thành không
                if (!CanEditImport(importID))
                {
                    return false;
                }

                // Lấy chi tiết phiếu nhập
                var import = _importRepository.GetImportByID(importID);

                if (import == null || import.Details == null || import.Details.Count == 0)
                {
                    return false;
                }

                // Cập nhật tồn kho cho từng sản phẩm
                foreach (var detail in import.Details)
                {
                    _inventoryService.ProcessStockForImport(detail.ProductID, detail.Quantity, importID, detail.UnitPrice);
                }

                // Cập nhật trạng thái phiếu nhập
                var updateImport = new Import
                {
                    ImportID = importID,
                    Status = "Completed",
                    Note = (import.Note ?? "") + " | Completed by EmployeeID: " + employeeID
                };

                return _importRepository.UpdateImport(updateImport);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in CompleteImport: {ex.Message}", ex);
                return false;
            }
        }

        public bool CancelImport(int importID, string reason, int employeeID)
        {
            try
            {
                return _importRepository.CancelImport(importID, reason, employeeID);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in CancelImport: {ex.Message}", ex);
                return false;
            }
        }

        public bool CanEditImport(int importID)
        {
            try
            {
                // Lấy thông tin phiếu nhập
                var import = _importRepository.GetImportByID(importID);

                if (import == null)
                {
                    return false;
                }

                // Chỉ có thể cập nhật phiếu nhập có trạng thái Pending
                return import.Status == "Pending";
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in CanEditImport: {ex.Message}", ex);
                return false;
            }
        }

        public List<ProductDTO> GetProductsBySupplier(int supplierID)
        {
            try
            {
                var products = new List<ProductDTO>();

                // TODO: Implement lấy sản phẩm từ nhà cung cấp
                // Có thể lấy từ lịch sử nhập hàng hoặc lấy tất cả sản phẩm

                // Lấy tất cả sản phẩm
                var allProducts = _productRepository.GetAllProducts();

                foreach (var product in allProducts)
                {
                    products.Add(new ProductDTO
                    {
                        ProductID = product.ProductID,
                        ProductCode = product.ProductCode,
                        ProductName = product.ProductName,
                        CategoryID = product.CategoryID,
                        CategoryName = product.CategoryName,
                        CostPrice = product.CostPrice,
                        SellPrice = product.SellPrice,
                        Unit = product.Unit
                    });
                }

                return products;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in GetProductsBySupplier: {ex.Message}", ex);
                throw;
            }
        }

        public byte[] ExportImportToPdf(int importID)
        {
            // TODO: Implement export import to PDF
            // Có thể sử dụng thư viện như iTextSharp hoặc PdfSharp

            return null;
        }

        public string GenerateImportCode()
        {
            // Tạo mã phiếu nhập theo định dạng: IMP + YYYYMMDD + XXXX (X là số thứ tự)
            var today = DateTime.Now;
            var datePart = today.ToString("yyyyMMdd");

            // TODO: Truy vấn số thứ tự tiếp theo
            // Mã giả:
            /*
            var lastImportCode = _importRepository.GetLastImportCodeOfDay(today);
            int sequence = 1;
            
            if (!string.IsNullOrEmpty(lastImportCode) && lastImportCode.Length >= 15)
            {
                int.TryParse(lastImportCode.Substring(11), out sequence);
                sequence++;
            }
            */

            int sequence = 1; // Tạm thời mặc định là 1

            return "IMP" + datePart + sequence.ToString("D4");
        }
    }
}