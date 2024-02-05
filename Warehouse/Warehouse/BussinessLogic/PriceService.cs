using Warehouse.BussinessLogic.Interfaces;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;

namespace Warehouse.BussinessLogic
{
    public class PriceService : IPriceService
    {
        private readonly IPriceRepository _priceRepository;
        private const string PriceFileUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv";

        public PriceService(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public async Task<IReadOnlyCollection<Price>> GetPricesAsync() => await _priceRepository.GetPricesAsync(PriceFileUrl);

        public async Task<bool> SavePricesAsync(IReadOnlyCollection<Price> prices) => await _priceRepository.SavePricesAsync(prices);
    }
}
