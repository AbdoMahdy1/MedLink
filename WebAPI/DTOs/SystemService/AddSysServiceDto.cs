namespace WebAPI.DTOs.SystemService
{
    public class AddSystemServiceDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCareService { get; set; }
        public bool IsFixedPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal FixedPrice { get; set; }
        public IFormFile? Image { get; set; }
    }
}
