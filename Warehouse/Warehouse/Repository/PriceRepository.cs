using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Warehouse.Context;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;
using Dapper;

namespace Warehouse.Repository
{
    public class PriceRepository : IPriceRepository
    {
        private readonly DataContext _context;
        private readonly IDownloadRepository _downloadRepository;

        private const string ProductTableName = "Price";

        public PriceRepository(DataContext context, IDownloadRepository downloadRepository)
        {
            _context = context;
            _downloadRepository = downloadRepository;
        }

        public async Task<IReadOnlyCollection<Price>> GetPricesAsync(string url)
        {
            var destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var destinationPath = Path.Combine(destinationFolder, "Prices.csv");

            var byteFile = await _downloadRepository.DownloadFileAsync(url);

            await _downloadRepository.SaveFileOnDevice(byteFile, destinationPath);

            return ConvertFileToPriceList(byteFile);
        }

        private static IReadOnlyCollection<Price> ConvertFileToPriceList(byte[] data)
        {
            try
            {
                using var memoryStream = new MemoryStream(data);
                using var reader = new StreamReader(memoryStream);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ","
                });

                csv.Read();
                csv.ReadHeader();

                var records = new List<Price>();

                while (csv.Read())
                {
                    var price = new Price
                    {
                        Id = csv.GetField<string>(0),
                        SKU = csv.GetField<string>(1),
                        PriceNet = csv.GetField<string>(2),
                        PriceAfterDiscount = csv.GetField<string>(3),
                        VatRate = csv.GetField<string>(4),
                        PriceAfterLogisticDiscount = csv.GetField<string>(5)
                    };

                    records.Add(price);
                }

                return records;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Conversion to Price list error: {ex.Message}");
                return new List<Price>();
            }
        }

        public async Task<bool> SavePricesAsync(IReadOnlyCollection<Price> prices)
        {
            if (!prices.Any()) return false;

            var tableExistsQuery = @"SELECT COUNT(*)
                                     FROM INFORMATION_SCHEMA.TABLES
                                     WHERE TABLE_NAME = @TableName";

            var createProductTableQuery = @"CREATE TABLE Price (Id NVARCHAR(255) PRIMARY KEY,
                                                                SKU NVARCHAR(255),
                                                                PriceNet NVARCHAR(255),
                                                                PriceAfterDiscount NVARCHAR(255),
                                                                VatRate NVARCHAR(255),
                                                                PriceAfterLogisticDiscount NVARCHAR(255))";
                                                                                                                                                                   
            var insertQuery = @"INSERT INTO Price (Id, SKU, PriceNet, PriceAfterDiscount, VatRate, PriceAfterLogisticDiscount)
                                           VALUES (@Id, @SKU, @PriceNet, @PriceAfterDiscount, @VatRate, @PriceAfterLogisticDiscount)";


            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var tableExists = await connection.ExecuteScalarAsync<int>(tableExistsQuery, new { TableName = ProductTableName }, transaction: transaction);
                if (tableExists == 0) await connection.ExecuteAsync(createProductTableQuery, transaction: transaction);

                await connection.ExecuteAsync(insertQuery, prices, transaction: transaction);

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
    }
}
