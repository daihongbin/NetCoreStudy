using System;
using System.Collections.Generic;
using System.Linq;
using Michael.ProductApiApp.Models;

namespace Michael.ProductApiApp.Data
{
    public static class DBInitializer
    {
        public static void Initialize(ProductDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            if (dbContext.Category.Any())
            {
                return;
            }
            
            var categoryList = new List<Category>
            {
                new Category
                {
                    CategoryID = 1,
                    ParentID = 0,
                    CategoryName = "裤装",
                    ViewOrder = 1,
                    Description = "裤子"
                },
                new Category
                {
                    CategoryID = 11,
                    ParentID = 1,
                    CategoryName = "牛仔裤",
                    ViewOrder = 1,
                    Description = "牛仔裤"
                },
                new Category
                {
                    CategoryID = 12,
                    ParentID = 1,
                    CategoryName = "休闲裤",
                    ViewOrder = 1,
                    Description = "休闲裤"
                }
            };

            dbContext.Category.AddRange(categoryList);
            dbContext.SaveChanges();

            // 👴
            var goodsList = new List<Goods>
            {
                new Goods
                {
                    GoodsSN = "0000001",
                    GoodsName = "黑色牛仔裤子",
                    CategoryID = 11,
                    MarketPrice = 120.22m,
                    SalePrice = 100.22m,
                    Img = "/Upload/0000001.jpg",
                    OnSale = true,
                    InputTime = DateTime.Now,
                    ModifyTime = DateTime.Now,
                    IsReal = true,
                    BarCode = "0000001001",
                    Quantity = 12,
                    Description = "一条普通的牛仔裤"
                },
                new Goods
                {
                    GoodsSN = "0000002",
                    GoodsName = "黑色休闲裤",
                    CategoryID = 12,
                    MarketPrice = 120.22m,
                    SalePrice = 100.22m,
                    Img = "/Upload/0000001.jpg",
                    OnSale = true,
                    InputTime = DateTime.Now,
                    ModifyTime = DateTime.Now,
                    IsReal = true,
                    BarCode = "0000002001",
                    Quantity = 12,
                    Description = "一条普通的牛仔裤"
                }
            };
            dbContext.Goods.AddRange(goodsList);
            dbContext.SaveChanges();
        }
    }
}