using CsvHelper.Configuration;
using CsvHelper;
using Dapper;
using Newtonsoft.Json;
using System.Globalization;
using Warehouse.Context;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;
using System.Text;

namespace Warehouse.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        private readonly IDownloadRepository _downloadRepository;

        private const string ProductTableName = "Product";

        public ProductRepository(DataContext context, IDownloadRepository downloadRepository)
        {
            _context = context;
            _downloadRepository = downloadRepository;
        }

        public async Task<IReadOnlyCollection<Product>> GetProductsAsync(string url)
        {
            var destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var destinationPath = Path.Combine(destinationFolder, "Products.csv");

            var byteFile = await _downloadRepository.DownloadFileAsync(url);

            await _downloadRepository.SaveFileOnDevice(byteFile, destinationPath);

            var jsonContent = ConvertCsvToProductListJson(byteFile);
            return JsonConvert.DeserializeObject<IReadOnlyCollection<Product>>(jsonContent) ?? new List<Product>();
        }

        public async Task<bool> SaveProductsAsync(IReadOnlyCollection<Product> products)
        {
            if (!products.Any()) return false;
    
            var tableExistsQuery = @"SELECT COUNT(*)
                                     FROM INFORMATION_SCHEMA.TABLES
                                     WHERE TABLE_NAME = @TableName";

            var createProductTableQuery = @"CREATE TABLE Product (ID NVARCHAR(255) PRIMARY KEY,
                                                                  SKU NVARCHAR(255),
                                                                  name NVARCHAR(255),
                                                                  EAN NVARCHAR(255),
                                                                  producer_name NVARCHAR(255),
                                                                  category NVARCHAR(255),
                                                                  is_wire BIT,
                                                                  available BIT,
                                                                  is_vendor BIT,
                                                                  default_image NVARCHAR(MAX)
                                                                  CONSTRAINT UC_ID UNIQUE (ID))"; 
                                                                                             
            var insertQuery = @"INSERT INTO Product (ID, SKU, name, EAN, producer_name, category, is_wire, available, is_vendor, default_image)
                                             VALUES (@Id, @SKU, @Name, @EAN, @Producer_Name, @Category, @Is_Wire, @Available, @Is_Vendor, @Default_Image)";

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {               
                var tableExists = await connection.ExecuteScalarAsync<int>(tableExistsQuery, new { TableName = ProductTableName }, transaction: transaction);
                if (tableExists == 0) await connection.ExecuteAsync(createProductTableQuery, transaction: transaction);

                await connection.ExecuteAsync(insertQuery, products, transaction: transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                await Console.Out.WriteLineAsync(e.Message);
                return false;
            }
        }

        private static string ConvertCsvToProductListJson(byte[] data)
        {
            try
            {
                string csvContent = Encoding.UTF8.GetString(data);

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";"
                };

                using var reader = new StringReader(csvContent);
                using var csv = new CsvReader(reader, csvConfig);
                var productList = csv.GetRecords<dynamic>().ToList();

                return JsonConvert.SerializeObject(productList, new JsonSerializerSettings());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Conversion CSV to JSON error: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<ProductInfoDto> GetProductBySkuAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku)) return new ProductInfoDto();

            var getProductQuery = @"SELECT  P.name AS Name,
                                            P.EAN AS EAN,
                                            P.producer_name AS ProducerName,
                                            P.category AS Category,
                                            P.default_image AS PictureUrl,
                                            I.qty AS Qty,
                                            I.unit AS Unit,
                                            PR.PriceNet AS NetPrice,
                                            I.shipping_cost AS ShippingPrice
                                     FROM   Product P
                                     JOIN   Inventory I ON P.SKU = I.sku
                                     JOIN   Price PR ON P.SKU = PR.SKU
                                    WHERE   P.SKU = @SKU;";
                                                                                    
            using var connection = _context.CreateConnection();
            connection.Open();

            try
            {
                var productInfo = await connection.QueryFirstOrDefaultAsync<ProductInfoDto>(getProductQuery, new { SKU = sku }) ?? new ProductInfoDto();
                return productInfo;
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                return new ProductInfoDto();
            }
        }
    }
}
