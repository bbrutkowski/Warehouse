using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Models
{
    [Table("Inventory")]
    public class Inventory
    {
        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        [JsonProperty("sku")]
        public string? SKU { get; set; }

        [JsonProperty("unit")]
        public string? Unit {  get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("manufacturer_name")]
        public string? ManufacturerName { get; set; }

        [JsonProperty("shipping")]
        public string? Shipping { get; set; }

        [JsonProperty("shipping_cost")]
        public decimal? ShippingCost { get; set; } 
    }
}
