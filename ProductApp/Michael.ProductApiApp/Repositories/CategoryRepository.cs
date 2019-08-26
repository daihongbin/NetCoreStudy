using Michael.ProductApiApp.Data;
using Michael.ProductApiApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Michael.ProductApiApp.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProductDbContext _context;

        public CategoryRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task AddCategory(Category category)
        {
            await _context.Category.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllCategory()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Category.FirstOrDefaultAsync(f => f.CategoryID == id);
        }
    }
}
