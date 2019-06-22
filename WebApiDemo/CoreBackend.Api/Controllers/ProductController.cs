using CoreBackend.Api.Dtos;
using CoreBackend.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using CoreBackend.Api.Repositories;
using System.Collections.Generic;
using AutoMapper;
using CoreBackend.Api.Entities;

namespace CoreBackend.Api.Controllers
{
    //[Route("api/product")]
    [Route("api/[controller]")]
    public class ProductController:Controller
    {
        private readonly ILogger<ProductController> _logger;

        private readonly IMailService _mailService;

        private readonly IProductRepository _productRepository;

        public ProductController(ILogger<ProductController> logger, IMailService mailService,IProductRepository productRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _productRepository = productRepository;
        }

        //[HttpGet("all")]
        [HttpGet]
        public IActionResult GetProducts()
        {
            #region 使用内存数据的版本
            /*
            //请求获取注入，推荐使用构造函数注入
            //HttpContext.RequestServices.GetService(typeof(ILogger<ProductController>));

            //return new JsonResult(ProductService.Current.Products);

            //var temp = new JsonResult(ProductService.Current.Products)
            //{
            //    StatusCode = 200
            //};
            //return temp;

            return Ok(ProductService.Current.Products);
            */
            #endregion

            var products = _productRepository.GetProducts();
            //var results = new List<ProductWithoutMaterialDto>();
            //foreach (var product in products)
            //{
            //    results.Add(new ProductWithoutMaterialDto
            //    {
            //        Id = product.Id,
            //        Name = product.Name,
            //        Price = product.Price,
            //        Description = product.Description
            //    });
            //}
            var results = Mapper.Map<IEnumerable<ProductWithoutMaterialDto>>(products);

            return Ok(results);
        }

        [Route("{id}",Name = "GetProduct")]//起个名字，给CreatedAtRoute这个方法用
        public IActionResult GetProduct(int id,bool includeMaterial = false)
        {
            //return new JsonResult(ProductService.Current.Products.SingleOrDefault(x => x.Id == id));
            #region 使用内存数据的版本
            /*
            try
            {
                //throw new Exception("来个异常！！！");

                var product = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
                if (product == null)
                {
                    _logger.LogInformation($"Id为{id}的产品没有被找到..");
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"查找Id为{id}的产品出现了错误！！", ex);
                return StatusCode(500, "处理请求的时候发生了错误！");
            } 
            */
            #endregion

            var product = _productRepository.GetProduct(id, includeMaterial);
            if (product == null)
            {
                return NotFound();
            }

            if (includeMaterial)
            {
                //var productWithMaterialResult = new ProductDto
                //{
                //    Id = product.Id,
                //    Name = product.Name,
                //    Price = product.Price,
                //    Description = product.Description
                //};

                //foreach (var material in product.Materials)
                //{
                //    productWithMaterialResult.Materials.Add(new MaterialDto
                //    {
                //        Id = material.Id,
                //        Name = material.Name
                //    });
                //}
                var productWithMaterialResult = Mapper.Map<ProductDto>(product);

                return Ok(productWithMaterialResult);
            }

            //var onlyProductResult = new ProductDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Price = product.Price,
            //    Description = product.Description
            //};
            var onlyProductResult = Mapper.Map<ProductWithoutMaterialDto>(product);

            return Ok(onlyProductResult);
        }

        [HttpPost]
        public IActionResult Post([FromBody]ProductCreation product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var maxId = ProductService.Current.Products.Max(x => x.Id);
            //var newProduct = new ProductDto
            //{
            //    Id = ++maxId,
            //    Name = product.Name,
            //    Price = product.Price,
            //    Description = product.Description
            //};
            //ProductService.Current.Products.Add(newProduct);

            //return CreatedAtRoute("GetProduct",new { id = newProduct.Id},newProduct);

            var newProduct = Mapper.Map<Product>(product);
            _productRepository.AddProduct(newProduct);
            if (!_productRepository.Save())
            {
                return StatusCode(500,"保存产品的时候出错");
            }

            var dto = Mapper.Map<ProductWithoutMaterialDto>(newProduct);
            return CreatedAtRoute("GetProduct", new { id = dto.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id,[FromBody] ProductModification product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            var model = _productRepository.GetProduct(id,false);
            if (model == null)
            {
                return NotFound();
            }

            //model.Name = product.Name;
            //model.Price = product.Price;
            //model.Description = product.Description;

            Mapper.Map(product,model);
            if (!_productRepository.Save())
            {
                return StatusCode(500,"保存产品的时候出错");
            }

            //return Ok(model);
            return NoContent();
        }

        /*
         Patch 请求格式
         [
            {
            "op":"replace",
            "path":"/name",
            "value":"New Name"
            },
            {
            "op":"replace",
            "path":"/description",
            "value":"New Description"
            }
         ]
         */
        [HttpPatch("{id}")]
        public IActionResult Patch(int id,[FromBody] JsonPatchDocument<ProductModification> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var productEntity = _productRepository.GetProduct(id,false);
            if (productEntity == null)
            {
                return NotFound();
            }
            
            var toPatch = Mapper.Map<ProductModification>(productEntity);
            patchDoc.ApplyTo(toPatch,ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (toPatch.Name == "产品")
            {
                ModelState.AddModelError("Name","产品的名称不可以是'产品'二字");
            }
            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(toPatch,productEntity);
            if (!_productRepository.Save())
            {
                return StatusCode(500,"更新的时候出错");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = _productRepository.GetProduct(id,false);
            if (model == null)
            {
                return NotFound();
            }

            _productRepository.DeleteProduct(model);
            if (!_productRepository.Save())
            {
                return StatusCode(500,"删除的时候出错");
            }
            _mailService.Send("Product Deleted",$"Id为{id}的产品被删除了");
            return NoContent();
        }
    }
}
