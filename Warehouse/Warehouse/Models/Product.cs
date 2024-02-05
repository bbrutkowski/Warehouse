using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Warehouse.Helpers;

namespace Warehouse.Models
{
    [Table("Product")]
    public class Product
    {
        public string? Id { get; set; }
        public string? SKU { get; set; }
        public string? Name { get; set; }
        public string? EAN { get; set; }
        public string? Producer_Name { get; set; }
        public string? Category { get; set; }
        public string? Shipping { get; set; }
        [JsonConverter(typeof(BooleanConverter))]
        public bool Is_Wire {  get; set; }
        [JsonConverter(typeof(BooleanConverter))]
        public bool Available { get; set; }
        [JsonConverter(typeof(BooleanConverter))]
        public bool Is_Vendor { get; set; }
        public string? Default_Image { get; set; }
    }
}
