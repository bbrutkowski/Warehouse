using CsvHelper.Configuration;
using CsvHelper;
using Dapper;
using Newtonsoft.Json;
using System.Globalization;
using Warehouse.Context;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;

namespace Warehouse.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DataContext _context;
        private readonly IDownloadRepository _downloadRepository;
        private const string InventoryTableName = "Inventory";

        public InventoryRepository(DataContext context, IDownloadRepository downloadRepository)
        {
            _context = context;
            _downloadRepository = downloadRepository;
        }

        public async Task<List<Inventory>> GetInventoryAsync(string url)
        {
            var destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var destinationPath = Path.Combine(destinationFolder, "Inventory.csv");

            var byteFile = await _downloadRepository.DownloadFileAsync(url);

            await _downloadRepository.SaveFileOnDevice(byteFile, destinationPath);

            string jsonContent = ConvertToInventoryJson(byteFile);
            return JsonConvert.DeserializeObject<List<Inventory>>(jsonContent) ?? new List<Inventory>();
        }

        public async Task<bool> SaveInventory(IReadOnlyCollection<Inventory> inventories)
        {
            if (!inventories.Any()) return false;

            var tableExistsQuery = @"SELECT COUNT(*)
                                     FROM INFORMATION_SCHEMA.TABLES
                                     WHERE TABLE_NAME = @TableName";

            var createInventoryTableQuery = @"CREATE TABLE Inventory (product_id NVARCHAR(255) PRIMARY KEY,
                                                                      sku NVARCHAR(255),
                                                                      unit NVARCHAR(255),
                                                                      qty NVARCHAR(255),
                                                                      manufacturer NVARCHAR(255),
                                                                      shipping NVARCHAR(255),
                                                                      shipping_cost NVARCHAR(255),
                                                                      CONSTRAINT UC_product_id UNIQUE (product_id))";
                                                                                                                                                                                                                                                                                  
            var insertQuery = @"INSERT INTO Inventory (product_id, sku, unit, qty, manufacturer, shipping, shipping_cost)
                                               VALUES (@product_id, @SKU, @Unit, @Qty, @Manufacturer_Name, @Shipping, @Shipping_Cost)";

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {             
                var tableExists = await connection.ExecuteScalarAsync<int>(tableExistsQuery, new { TableName = InventoryTableName }, transaction: transaction);
                if (tableExists == 0) await connection.ExecuteAsync(createInventoryTableQuery, transaction: transaction);

                await connection.ExecuteAsync(insertQuery, inventories, transaction: transaction);
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

        private static string ConvertToInventoryJson(byte[] data)
        {
            try
            {
                using var memoryStream = new MemoryStream(data);
                using var reader = new StreamReader(memoryStream);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
             
                var records = csv.GetRecords<dynamic>();
                return JsonConvert.SerializeObject(records, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Conversion CSV to JSON error: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
