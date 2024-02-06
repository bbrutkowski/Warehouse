using Dapper;
using Warehouse.Context;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;
using CSharpFunctionalExtensions;

namespace Warehouse.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        private const string ProductTableName = "Product";

        public ProductRepository(DataContext context) => _context = context;

        public async Task<Result> SaveProductsAsync(List<Product> products)
        {
            if (!products.Any()) return Result.Failure("Error trying to save. Product list is empty");
    
            var tableExistsQuery = @"SELECT COUNT(*)
                                     FROM INFORMATION_SCHEMA.TABLES
                                     WHERE TABLE_NAME = @TableName";

            var createProductTableQuery = @"CREATE TABLE Product (ID INT PRIMARY KEY,
                                                                  SKU NVARCHAR(255),
                                                                  name NVARCHAR(255),
                                                                  EAN NVARCHAR(255),
                                                                  producer_name NVARCHAR(255),
                                                                  category NVARCHAR(255),
                                                                  is_wire BIT,
                                                                  available BIT,
                                                                  is_vendor BIT,
                                                                  default_image NVARCHAR(MAX))"; 
                                                                                            
            var insertQuery = @"INSERT INTO Product (ID, SKU, name, EAN, producer_name, category, is_wire, available, is_vendor, default_image)
                                             VALUES (@Id, @SKU, @Name, @EAN, @ProducerName, @Category, @IsWire, @IsAvailable, @IsVendor, @DefaultImage)";

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {               
                var tableExists = await connection.ExecuteScalarAsync<int>(tableExistsQuery, new { TableName = ProductTableName }, transaction: transaction);
                if (tableExists == 0) await connection.ExecuteAsync(createProductTableQuery, transaction: transaction);

                await connection.ExecuteAsync(insertQuery, products, transaction: transaction);

                transaction.Commit();
                return Result.Success();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Result.Failure(e.Message);
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
