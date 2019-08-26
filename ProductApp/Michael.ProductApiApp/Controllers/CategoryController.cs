using Michael.ProductApiApp.Models;
using Michael.ProductApiApp.Models.ViewModel;
using Michael.ProductApiApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Michael.ProductApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _category;

        public CategoryController(ICategoryRepository category)
        {
            _category = category;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryViewModel>>> Get()
        {
            var categories = await _category.GetAllCategory();

            var categoryViewModel = categories.Select(s => new CategoryViewModel
            {
                CategoryId = s.CategoryID,
                ParentId = s.ParentID,
                CategoryName = s.CategoryName,
                ViewOrder = s.ViewOrder,
                Description = s.Description
            });

            return new JsonResult(categoryViewModel);
        }

        [Route("{id}")]
        public async Task<ActionResult<CategoryViewModel>> Get(int id)
        {
            var category = await _category.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = new CategoryViewModel
            {
                CategoryId = category.CategoryID,
                ParentId = category.ParentID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ViewOrder = category.ViewOrder
            };
            return new JsonResult(categoryVM);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CategoryViewModel categoryViewModel)
        {
            if (categoryViewModel == null)
            {
                return BadRequest();
            }

            var category = new Category
            {
                CategoryID = categoryViewModel.CategoryId,
                ParentID = categoryViewModel.ParentId,
                CategoryName = categoryViewModel.CategoryName,
                ViewOrder = categoryViewModel.ViewOrder,
                Description = categoryViewModel.Description
            };
            await _category.AddCategory(category);
            return Ok();
        }
    }
}
