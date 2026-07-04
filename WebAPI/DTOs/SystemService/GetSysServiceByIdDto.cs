namespace WebAPI.DTOs.SystemService
{
    public class GetSysServiceByIdDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PricingType { get; set; }
        public bool IsDefault { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}
