using Michael.ProductApiApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Michael.ProductApiApp.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategory();

        Task AddCategory(Category category);

        Task<Category> GetCategoryById(int id);
    }
}
