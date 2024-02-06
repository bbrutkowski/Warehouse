namespace Warehouse.Models
{
    public record ProductInfoDto
    {
        public string Name { get; set; }
        public string EAN { get; set; }
        public string ProducerName { get; set; }
        public string Category { get; set; }
        public string PictureUrl { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; }
        public decimal NetPrice { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
