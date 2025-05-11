// Mã gợi ý cho ICategoryRepository.cs
using System.Collections.Generic;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category GetById(int categoryId);
        List<Category> GetByParent(int? parentCategoryId);
        int Create(Category category, int createdBy);
        bool Update(Category category, int modifiedBy);
        bool Delete(int categoryId, int deletedBy);
    }
}