using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Models
{
    [Table("Inventory")]
    public class Inventory
    {
        public string? Product_Id { get; set; }
        public string? SKU { get; set; }
        public string? Unit {  get; set; }
        public string? Qty { get; set; }
        public string? Manufacturer_Name { get; set; }
        public string? Shipping { get; set; }
        public string? Shipping_Cost { get; set; }
    }
}
