using CSharpFunctionalExtensions;
using CsvHelper;
using System.Globalization;
using Warehouse.BussinessLogic.Interfaces;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;

namespace Warehouse.BussinessLogic
{
    public class PriceService : IPriceService
    {
        private readonly IPriceRepository _priceRepository;
        private readonly IFileService _fileService;

        private readonly string _priceFileUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv";
        private readonly string _destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Prices.csv");
       
        public PriceService(IPriceRepository priceRepository, IFileService fileService)
        {
            _priceRepository = priceRepository;
            _fileService = fileService;
        }

        public async Task<Result<byte[]>> GetPriceFileAsync(CancellationToken token)
        {
            var byteFile = await _fileService.DownloadFileAsync(_priceFileUrl, token);
            if (byteFile.IsFailure) return Result.Failure<byte[]>(byteFile.Error);

            return Result.Success(byteFile.Value);
        }

        public async Task<Result> SavePriceFileOnDeviceAsync(byte[] byteFile, CancellationToken token)
        {
            var saveResult = await _fileService.SaveFileOnDeviceAsync(byteFile, _destinationPath, token);
            if (saveResult.IsFailure) return Result.Failure(saveResult.Error);

            return Result.Success();
        }

        public async Task<Result> SavePriceAsync(CancellationToken token)
        {
            var priceFile = await _fileService.GetSavedFileAsync(_destinationPath, token);

            var priceList = ConvertCsvToPriceList(priceFile.Value);

            return await _priceRepository.SavePricesAsync(priceList);
        }

        private List<Price> ConvertCsvToPriceList(string data)
        {
            var records = new List<Price>();

            try
            {
                using (var reader = new StringReader(data))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    while (csv.Read())
                    {
                        decimal.TryParse(csv.GetField<string>(2), out decimal priceNet);
                        decimal.TryParse(csv.GetField<string>(3), out decimal priceAfterDiscount);
                        int.TryParse(csv.GetField<string>(4), out int vatRate);
                        decimal.TryParse(csv.GetField<string>(5), out decimal priceAfterLogisticDiscount);
                        
                        var price = new Price
                        {
                            Id = csv.GetField<string>(0),
                            SKU = csv.GetField<string>(1),
                            PriceNet = priceNet,
                            PriceAfterDiscount = priceAfterDiscount,
                            VatRate = vatRate,
                            PriceAfterLogisticDiscount = priceAfterLogisticDiscount
                        };

                        records.Add(price);
                    }
                }
                return records;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while parsing file to price list: {ex.Message}");
                return new List<Price>();
            }
        }
    }
}
