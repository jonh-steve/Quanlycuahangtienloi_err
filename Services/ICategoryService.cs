// Mã gợi ý cho ICategoryService.cs
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Services
{
    public interface ICategoryService
    {
        List<Category> GetAllCategories();
        Category GetCategoryById(int categoryId);
        List<Category> GetRootCategories();
        List<Category> GetChildCategories(int parentCategoryId);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(int categoryId);
    }
}