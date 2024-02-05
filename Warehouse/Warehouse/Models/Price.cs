using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Models
{
    [Table("Price")]
    public class Price
    {
        public string? Id { get; set;}
        public string? SKU { get; set;}
        public string? PriceNet { get; set;}
        public string? PriceAfterDiscount { get; set;}
        public string? VatRate { get; set;}
        public string? PriceAfterLogisticDiscount { get; set;}
    }
}
