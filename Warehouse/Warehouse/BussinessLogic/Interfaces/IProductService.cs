using CSharpFunctionalExtensions;
using Warehouse.Models;

namespace Warehouse.BussinessLogic.Interfaces
{
    public interface IProductService
    {
        Task<Result<byte[]>> GetProductsFileAsync(CancellationToken token);
        Task<Result> SaveFilteredProductsAsync(CancellationToken token);
        Task<ProductInfoDto> GetProductBySkuAsync(string sku);
        Task<Result> SaveProductsFileOnDeviceAsync(byte[] byteFile, CancellationToken token);
    }
}
