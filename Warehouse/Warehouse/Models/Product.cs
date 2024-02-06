using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Warehouse.Helpers;

namespace Warehouse.Models
{
    [Table("Product")]
    public class Product
    {
        [JsonConverter(typeof(StringToIntConverter))]
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty(nameof(SKU))]
        public string? SKU { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty(nameof(EAN))]
        public string? EAN { get; set; }

        [JsonProperty("producer_name")]
        public string? ProducerName { get; set; }

        [JsonProperty("category")]
        public string? Category { get; set; }

        [JsonProperty("shipping")]
        public string? Shipping { get; set; }

        [JsonProperty("is_wire")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool IsWire {  get; set; }

        [JsonProperty("available")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool IsAvailable { get; set; }

        [JsonProperty("is_vendor")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool IsVendor { get; set; }

        [JsonProperty("default_image")]
        public string? DefaultImage { get; set; }
    }
}
