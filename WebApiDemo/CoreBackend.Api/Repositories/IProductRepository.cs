using System.Collections.Generic;
using CoreBackend.Api.Entities;

namespace CoreBackend.Api.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();

        Product GetProduct(int productId,bool includeMaterials);

        IEnumerable<Material> GetMaterialsForProduct(int productId);

        Material GetMaterialForProduct(int productId,int materialId);

        bool ProductExist(int productId);

        void AddProduct(Product product);

        bool Save();

        void DeleteProduct(Product product);
    }
}
