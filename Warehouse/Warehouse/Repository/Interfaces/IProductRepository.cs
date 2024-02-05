using Warehouse.Models;

namespace Warehouse.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> SaveProductsAsync(IReadOnlyCollection<Product> products);
        Task<IReadOnlyCollection<Product>> GetProductsAsync(string url);
        Task<ProductInfoDto> GetProductBySkuAsync(string sku);
    }
}
