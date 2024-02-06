using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System.Globalization;
using Warehouse.BussinessLogic.Interfaces;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;

namespace Warehouse.BussinessLogic
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        private readonly string _destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Products.csv");
        private readonly string _productFileUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv";

        public ProductService(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task<ProductInfoDto> GetProductBySkuAsync(string sku) => await _productRepository.GetProductBySkuAsync(sku);

        public async Task<Result<byte[]>> GetProductsFileAsync(CancellationToken token)
        {
            var byteFile = await _fileService.DownloadFileAsync(_productFileUrl, token);
            if (byteFile.IsFailure) return Result.Failure<byte[]>(byteFile.Error);
        
            return Result.Success(byteFile.Value);
        }

        public async Task<Result> SaveProductsFileOnDeviceAsync(byte[] byteFile, CancellationToken token)
        {
            var saveResult = await _fileService.SaveFileOnDeviceAsync(byteFile, _destinationPath, token);
            if (saveResult.IsFailure) return Result.Failure(saveResult.Error);
         
            return Result.Success();        
        }

        public async Task<Result> SaveFilteredProductsAsync(CancellationToken token)
        {
            var productFile = await _fileService.GetSavedFileAsync(_destinationPath, token);

            var productsList = ConvertCsvToProductList(productFile.Value);

            var filtredProducts = productsList.Where(x => !x.IsWire && x.Shipping != null && x.Shipping == "24h")
                                              .ToList();

            return await _productRepository.SaveProductsAsync(filtredProducts);
        }

        private IReadOnlyCollection<Product> ConvertCsvToProductList(string data)
        {
            var productList = new List<dynamic>();

            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";"
                };

                using (var reader = new StringReader(data))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    //it's not a good practice to use <dynamic> but I've tried different approaches and this is the only one that works
                    productList = csv.GetRecords<dynamic>().ToList();
                }

                var jsonContent = JsonConvert.SerializeObject(productList, new JsonSerializerSettings());

                return JsonConvert.DeserializeObject<IReadOnlyCollection<Product>>(jsonContent) ?? new List<Product>(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while parsing the CSV data: {ex.Message}");
                return new List<Product>();
            }
        }
    }
}
