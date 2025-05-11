// Mã gợi ý cho CategoryRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using QuanLyCuaHangTienLoi.Models.Entities;

namespace QuanLyCuaHangTienLoi.Db.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public List<Category> GetAll()
        {
            try
            {
                var data = ExecuteReader("app.sp_GetAllCategories");
                List<Category> categories = new List<Category>();

                foreach (DataRow row in data.Rows)
                {
                    Category category = new Category
                    {
                        CategoryID = Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                        ParentCategoryID = row["ParentCategoryID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ParentCategoryID"]),
                        ParentCategoryName = row["ParentCategoryName"] == DBNull.Value ? null : row["ParentCategoryName"].ToString(),
                        DisplayOrder = Convert.ToInt32(row["DisplayOrder"]),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        Level = Convert.ToInt32(row["Level"]),
                        Hierarchy = row["Hierarchy"].ToString(),
                        ProductCount = Convert.ToInt32(row["ProductCount"])
                    };

                    categories.Add(category);
                }

                return categories;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi lấy danh sách danh mục: {ex.Message}", ex);
                return new List<Category>();
            }
        }

        public Category GetById(int categoryId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@CategoryID", categoryId }
                };

                var data = ExecuteReader("app.sp_GetCategoryByID", parameters);

                if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
                    return null;

                var row = data.Tables[0].Rows[0];

                Category category = new Category
                {
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    CategoryName = row["CategoryName"].ToString(),
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    ParentCategoryID = row["ParentCategoryID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ParentCategoryID"]),
                    ParentCategoryName = row["ParentCategoryName"] == DBNull.Value ? null : row["ParentCategoryName"].ToString(),
                    DisplayOrder = Convert.ToInt32(row["DisplayOrder"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedDate = row["ModifiedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ModifiedDate"])
                };

                // Lấy danh sách danh mục con nếu có
                if (data.Tables.Count > 1 && data.Tables[1].Rows.Count > 0)
                {
                    category.ChildCategories = new List<Category>();

                    foreach (DataRow childRow in data.Tables[1].Rows)
                    {
                        Category childCategory = new Category
                        {
                            CategoryID = Convert.ToInt32(childRow["CategoryID"]),
                            CategoryName = childRow["CategoryName"].ToString(),
                            Description = childRow["Description"] == DBNull.Value ? null : childRow["Description"].ToString(),
                            DisplayOrder = Convert.ToInt32(childRow["DisplayOrder"]),
                            IsActive = Convert.ToBoolean(childRow["IsActive"]),
                            ParentCategoryID = categoryId
                        };

                        category.ChildCategories.Add(childCategory);
                    }
                }

                return category;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi lấy thông tin danh mục: {ex.Message}", ex);
                return null;
            }
        }

        public List<Category> GetByParent(int? parentCategoryId)
        {
            try
            {
                string sql = parentCategoryId.HasValue ?
                             "SELECT * FROM app.Category WHERE ParentCategoryID = @ParentCategoryID AND IsActive = 1 ORDER BY DisplayOrder, CategoryName" :
                             "SELECT * FROM app.Category WHERE ParentCategoryID IS NULL AND IsActive = 1 ORDER BY DisplayOrder, CategoryName";

                var parameters = new Dictionary<string, object>();
                if (parentCategoryId.HasValue)
                {
                    parameters.Add("@ParentCategoryID", parentCategoryId);
                }

                var data = ExecuteQuery(sql, parameters);
                List<Category> categories = new List<Category>();

                foreach (DataRow row in data.Rows)
                {
                    Category category = new Category
                    {
                        CategoryID = Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                        ParentCategoryID = row["ParentCategoryID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ParentCategoryID"]),
                        DisplayOrder = Convert.ToInt32(row["DisplayOrder"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };

                    categories.Add(category);
                }

                return categories;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi lấy danh sách danh mục theo danh mục cha: {ex.Message}", ex);
                return new List<Category>();
            }
        }

        public int Create(Category category, int createdBy)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@CategoryName", category.CategoryName },
                    { "@Description", category.Description },
                    { "@ParentCategoryID", category.ParentCategoryID },
                    { "@DisplayOrder", category.DisplayOrder },
                    { "@CreatedBy", createdBy }
                };

                var data = ExecuteReader("app.sp_CreateCategory", parameters);

                if (data.Rows.Count > 0)
                    return Convert.ToInt32(data.Rows[0]["CategoryID"]);

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi tạo danh mục: {ex.Message}", ex);
                return 0;
            }
        }

        public bool Update(Category category, int modifiedBy)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@CategoryID", category.CategoryID },
                    { "@CategoryName", category.CategoryName },
                    { "@Description", category.Description },
                    { "@ParentCategoryID", category.ParentCategoryID },
                    { "@DisplayOrder", category.DisplayOrder },
                    { "@IsActive", category.IsActive },
                    { "@ModifiedBy", modifiedBy }
                };

                var result = ExecuteNonQuery("app.sp_UpdateCategory", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi cập nhật danh mục: {ex.Message}", ex);
                return false;
            }
        }

        public bool Delete(int categoryId, int deletedBy)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@CategoryID", categoryId },
                    { "@DeletedBy", deletedBy }
                };

                var result = ExecuteNonQuery("app.sp_DeleteCategory", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Lỗi xóa danh mục: {ex.Message}", ex);
                return false;
            }
        }
    }
}