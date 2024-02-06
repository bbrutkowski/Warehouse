using CSharpFunctionalExtensions;
using Dapper;
using Warehouse.Context;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;

namespace Warehouse.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DataContext _context;
        private const string InventoryTableName = "Inventory";

        public InventoryRepository(DataContext context) => _context = context;

        public async Task<Result> SaveInventoryAsync(IReadOnlyCollection<Inventory> inventories)
        {
            if (!inventories.Any()) return Result.Failure("Inventory list is empty");

            var tableExistsQuery = @"SELECT COUNT(*)
                                     FROM INFORMATION_SCHEMA.TABLES
                                     WHERE TABLE_NAME = @TableName";

            var createInventoryTableQuery = @"CREATE TABLE Inventory (product_id INT PRIMARY KEY,
                                                                      sku NVARCHAR(255),
                                                                      unit NVARCHAR(255),
                                                                      qty DECIMAL(18, 2) NOT NULL,
                                                                      manufacturer NVARCHAR(255),
                                                                      shipping NVARCHAR(255),
                                                                      shipping_cost DECIMAL(18, 2))";                                                                      
                                                                                                                                                                                                                                                                                  
            var insertQuery = @"INSERT INTO Inventory (product_id, sku, unit, qty, manufacturer, shipping, shipping_cost)
                                               VALUES (@ProductId, @SKU, @Unit, @Qty, @ManufacturerName, @Shipping, @ShippingCost)";

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {             
                var tableExists = await connection.ExecuteScalarAsync<int>(tableExistsQuery, new { TableName = InventoryTableName }, transaction: transaction);
                if (tableExists == 0) await connection.ExecuteAsync(createInventoryTableQuery, transaction: transaction);

                await connection.ExecuteAsync(insertQuery, inventories, transaction: transaction);
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
