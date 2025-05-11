// Mã gợi ý cho CategoryService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyCuaHangTienLoi.Models.Entities;
using QuanLyCuaHangTienLoi.Db.Repositories;

namespace QuanLyCuaHangTienLoi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountService _accountService;
        private static CategoryService _instance;

        // Singleton pattern
        public static CategoryService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CategoryService();
                return _instance;
            }
        }

        private CategoryService()
        {
            _categoryRepository = new CategoryRepository();
            _accountService = AccountService.Instance;
        }

        public List<Category> GetAllCategories()
        {
            return _categoryRepository.GetAll();
        }

        public Category GetCategoryById(int categoryId)
        {
            return _categoryRepository.GetById(categoryId);
        }

        public List<Category> GetRootCategories()
        {
            return _categoryRepository.GetByParent(null);
        }

        public List<Category> GetChildCategories(int parentCategoryId)
        {
            return _categoryRepository.GetByParent(parentCategoryId);
        }

        public bool CreateCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.CategoryName))
                return false;

            // Kiểm tra quyền người dùng
            if (!_accountService.IsAuthenticated() || !_accountService.HasPermission("manager"))
                return false;

            int currentUserId = _accountService.GetCurrentAccount().AccountID;
            int result = _categoryRepository.Create(category, currentUserId);
            return result > 0;
        }

        public bool UpdateCategory(Category category)
        {
            if (category.CategoryID <= 0 || string.IsNullOrEmpty(category.CategoryName))
                return false;

            // Kiểm tra quyền người dùng
            if (!_accountService.IsAuthenticated() || !_accountService.HasPermission("manager"))
                return false;

            int currentUserId = _accountService.GetCurrentAccount().AccountID;
            return _categoryRepository.Update(category, currentUserId);
        }

        public bool DeleteCategory(int categoryId)
        {
            if (categoryId <= 0)
                return false;

            // Kiểm tra quyền người dùng
            if (!_accountService.IsAuthenticated() || !_accountService.HasPermission("manager"))
                return false;

            // Kiểm tra danh mục có sản phẩm hoặc danh mục con không
            Category category = _categoryRepository.GetById(categoryId);
            if (category == null)
                return false;

            if (category.ProductCount > 0)
                throw new Exception("Không thể xóa danh mục đang chứa sản phẩm!");

            if (category.ChildCategories != null && category.ChildCategories.Count > 0)
                throw new Exception("Không thể xóa danh mục đang chứa danh mục con!");

            int currentUserId = _accountService.GetCurrentAccount().AccountID;
            return _categoryRepository.Delete(categoryId, currentUserId);
        }
    }
}