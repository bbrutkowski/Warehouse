using CSharpFunctionalExtensions;
using Warehouse.Models;

namespace Warehouse.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<Result> SaveProductsAsync(List<Product> products);
        Task<ProductInfoDto> GetProductBySkuAsync(string sku);
    }
}
