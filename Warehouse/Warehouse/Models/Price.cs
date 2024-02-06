using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Models
{
    [Table("Price")]
    public class Price
    {
        public string? Id { get; set;}
        public string? SKU { get; set;}
        public decimal PriceNet { get; set;}
        public decimal PriceAfterDiscount { get; set;}
        public int VatRate { get; set;}
        public decimal? PriceAfterLogisticDiscount { get; set;}
    }
}
