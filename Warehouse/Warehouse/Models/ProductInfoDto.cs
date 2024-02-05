namespace Warehouse.Models
{
    public record ProductInfoDto
    {
        public string Name { get; set; }
        public string EAN { get; set; }
        public string ProducerName { get; set; }
        public string Category { get; set; }
        public string PictureUrl { get; set; }
        public string Qty { get; set; }
        public string Unit { get; set; }
        public string NetPrice { get; set; }
        public string ShippingPrice { get; set; }
    }
}
