using Warehouse.BussinessLogic.Interfaces;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;

namespace Warehouse.BussinessLogic
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository; 

        private const string ProductFileUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv";

        public ProductService(IProductRepository productRepository) => _productRepository = productRepository;

        public async Task<IReadOnlyCollection<Product>> GetProductsAsync()
        {
            var productList = await _productRepository.GetProductsAsync(ProductFileUrl);  
            
            return productList.Where(x => !x.Is_Wire && x.Shipping != null && x.Shipping == "24h")
                              .ToList();
        }

        public async Task<bool> SaveProductsAsync(IReadOnlyCollection<Product> products) => await _productRepository.SaveProductsAsync(products);

        public async Task<ProductInfoDto> GetProductBySkuAsync(string sku) => await _productRepository.GetProductBySkuAsync(sku);
    }
}
