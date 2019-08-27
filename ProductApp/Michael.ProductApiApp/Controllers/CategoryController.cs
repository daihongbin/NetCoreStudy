using Michael.ProductApiApp.Entities;
using Michael.ProductApiApp.Models;
using Michael.ProductApiApp.Models.ViewModel;
using Michael.ProductApiApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Michael.ProductApiApp.Controllers
{
    
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _category;

        public CategoryController(ICategoryRepository category)
        {
            _category = category;
        }

        // 获取所有分类
        [HttpGet]
        public async Task<Response<List<CategoryViewModel>>> GetAllCategories()
        {
            var response = new Response<List<CategoryViewModel>>();

            var categories = await _category.GetAllCategory();
            if (categories == null || categories.Count <= 0)
            {
                response.Code = "500";
                response.Msg = "未获取到分类";
                return response;
            }

            var categoryVMs = categories.Select(s => new CategoryViewModel
            {
                CategoryId = s.CategoryID,
                ParentId = s.ParentID,
                CategoryName = s.CategoryName,
                ViewOrder = s.ViewOrder,
                Description = s.Description
            }).ToList();

            response.Code = "200";
            response.Msg = "获取分类成功！";
            response.Data = categoryVMs;
            return response;
        }

        // 获取所有分类
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new Response<List<CategoryViewModel>>();

            var categories = await _category.GetAllCategory();
            if (categories == null || categories.Count <= 0)
            {
                response.Code = "500";
                response.Msg = "未获取到分类";
                return NotFound(response.Data);
            }

            var categoryVMs = categories.Select(s => new CategoryViewModel
            {
                CategoryId = s.CategoryID,
                ParentId = s.ParentID,
                CategoryName = s.CategoryName,
                ViewOrder = s.ViewOrder,
                Description = s.Description
            }).ToList();

            response.Code = "200";
            response.Msg = "获取分类成功！";
            response.Data = categoryVMs;
            return Ok(response.Data);
        }

        public async Task<Response<CategoryViewModel>> GetCategoryById(int id)
        {
            var response = new Response<CategoryViewModel>();

            var category = await _category.GetCategoryById(id);
            if (category == null)
            {
                response.Code = "500";
                response.Msg = $"获取编号为{id}的分类失败！";
                return response;
            }

            var categoryVm = new CategoryViewModel
            {
                CategoryId = category.CategoryID,
                ParentId = category.ParentID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ViewOrder = category.ViewOrder
            };
            response.Code = "200";
            response.Msg = $"获取编号为{id}的分类成功！";
            response.Data = categoryVm;
            return response;
        }

        [HttpPost]
        public async Task<Response<string>> AddCategory(CategoryViewModel categoryViewModel)
        {
            var response = new Response<string>();
            if (categoryViewModel == null)
            {
                response.Code = "401";
                response.Msg = "添加分类失败";
                return response;
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

            response.Code = "200";
            response.Msg = "添加分类成功";
            return response;
        }
    }
}
