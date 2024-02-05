using Microsoft.AspNetCore.Mvc;
using Warehouse.BussinessLogic.Interfaces;

namespace Warehouse.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;
        private readonly IPriceService _priceService;

        public ProductController(IProductService productService, IInventoryService inventory, IPriceService priceService)
        {
            _productService = productService;
            _inventoryService = inventory;
            _priceService = priceService;
        }

        [HttpGet]
        [Route(nameof(GetProducts))]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            await _productService.SaveProductsAsync(products);

            var inventory = await _inventoryService.GetInventoryAsync();
            await _inventoryService.SaveInventoryAsync(inventory);

            var prices = await _priceService.GetPricesAsync();
            await _priceService.SavePricesAsync(prices);

            return Ok();
        }

        [HttpGet]
        [Route(nameof(GetProductInformationBySku))]
        public async Task<IActionResult> GetProductInformationBySku(string sku)
        {
            var productInfo = await _productService.GetProductBySkuAsync(sku);

            await Console.Out.WriteLineAsync($"Name: {productInfo.Name}, EAN: {productInfo.EAN}, Producer name: {productInfo.ProducerName}, " +
                $"Category {productInfo.Category}, URL: {productInfo.PictureUrl}, Qty {productInfo.Qty}, unit: {productInfo.Unit}, price net: {productInfo.NetPrice}, " +
                $"shipping cost: {productInfo.ShippingPrice}");

            return Ok();
        }

    }
}
