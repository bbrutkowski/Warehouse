using Microsoft.AspNetCore.Mvc;
using Warehouse.BussinessLogic.Interfaces;

namespace Warehouse.Controllers
{
    //name should describe the resource that supports the controller
    //but there was no such information in the task, so it is the "main" controller

    [Route("api/main")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;
        private readonly IPriceService _priceService;

        public MainController(IProductService productService, IInventoryService inventory, IPriceService priceService)
        {
            _productService = productService;
            _inventoryService = inventory;
            _priceService = priceService;
        }

        [HttpGet]
        [Route(nameof(SaveFilteredDataFromFiles))]
        public async Task<IActionResult> SaveFilteredDataFromFiles(CancellationToken token)
        {
            var productFile = await _productService.GetProductsFileAsync(token);
            await _productService.SaveProductsFileOnDeviceAsync(productFile.Value, token);

            var saveProductResult = await _productService.SaveFilteredProductsAsync(token);

            var inventoryFile = await _inventoryService.GetInventoryFileAsync(token);
            await _inventoryService.SaveInventoryFileOnDeviceAsync(inventoryFile.Value, token);

            var saveInventoryResult = await _inventoryService.FilterAndSaveInventoryAsync(token);

            var priceFile = await _priceService.GetPriceFileAsync(token);
            await _priceService.SavePriceFileOnDeviceAsync(priceFile.Value, token);

            var savePriceResult = await _priceService.SavePriceAsync(token);

            if (saveProductResult.IsSuccess && saveInventoryResult.IsSuccess && savePriceResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet]
        [Route(nameof(GetProductInformationBySku))]
        public async Task<IActionResult> GetProductInformationBySku(string sku)
        {
            return Ok(await _productService.GetProductBySkuAsync(sku));
        }
    }
}
