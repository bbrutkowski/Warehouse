using Warehouse.Context;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;
using Dapper;
using CSharpFunctionalExtensions;

namespace Warehouse.Repository
{
    public class PriceRepository : IPriceRepository
    {
        private readonly DataContext _context;
        private const string ProductTableName = "Price";

        public PriceRepository(DataContext context) => _context = context;

        public async Task<Result> SavePricesAsync(IReadOnlyCollection<Price> prices)
        {
            if (!prices.Any()) return Result.Failure("Error trying to save. Price list is empty");

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
                return Result.Success();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Result.Failure(e.Message);
            }
        }
    }
}
