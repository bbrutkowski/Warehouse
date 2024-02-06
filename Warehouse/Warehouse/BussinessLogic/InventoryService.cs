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
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IFileService _fileService;

        private readonly string _destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Inventory.csv");
        private readonly string _inventoryFileUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv";

        public InventoryService(IInventoryRepository inventoryRepository, IFileService fileService)
        {
            _inventoryRepository = inventoryRepository;
            _fileService = fileService;
        }

        public async Task<Result<byte[]>> GetInventoryFileAsync(CancellationToken token)
        {
            var byteFile = await _fileService.DownloadFileAsync(_inventoryFileUrl, token);
            if (byteFile.IsFailure) return Result.Failure<byte[]>(byteFile.Error);

            return Result.Success(byteFile.Value);
        }

        public async Task<Result> SaveInventoryFileOnDeviceAsync(byte[] byteFile, CancellationToken token)
        {
            var saveResult = await _fileService.SaveFileOnDeviceAsync(byteFile, _destinationPath, token);
            if (saveResult.IsFailure) return Result.Failure(saveResult.Error);

            return Result.Success();
        }

        public async Task<Result> FilterAndSaveInventoryAsync(CancellationToken token)
        {
            var inventoryFile = await _fileService.GetSavedFileAsync(_destinationPath, token);

            var inventoryList = ConvertCsvToInventoryList(inventoryFile.Value);     
            var filtredInventory = inventoryList.Where(x => x.Shipping == "24h").ToList();

            return await _inventoryRepository.SaveInventoryAsync(filtredInventory);
        }

        private IReadOnlyCollection<Inventory> ConvertCsvToInventoryList(string data)
        {
            var inventoryList = new List<dynamic>();

            try
            {
                using (var reader = new StringReader(data))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    //it's not a good practice to use <dynamic> but I've tried different approaches and this is the only one that works
                    inventoryList = csv.GetRecords<dynamic>().ToList();
                }

                var jsonContent = JsonConvert.SerializeObject(inventoryList, new JsonSerializerSettings());

                return JsonConvert.DeserializeObject<IReadOnlyCollection<Inventory>>(jsonContent) ?? new List<Inventory>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while parsing the file data: {ex.Message}");
                return new List<Inventory>();
            }
        }
    }
}
