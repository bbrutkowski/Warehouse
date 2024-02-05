using Warehouse.Models;

namespace Warehouse.BussinessLogic.Interfaces
{
    public interface IProductService
    {
        Task<IReadOnlyCollection<Product>> GetProductsAsync();
        Task<bool> SaveProductsAsync(IReadOnlyCollection<Product> products);
        Task<ProductInfoDto> GetProductBySkuAsync(string sku);
    }
}
