namespace WebAPI.DTOs.Order
{
    public class AddOrderDto
    {
        public string NurseServiceId { get; set; }  // خدمة الممرض (مش SystemService)
        public DateTime OrderTime { get; set; }
        public string ServiceType { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public int PatientAge { get; set; }
    }
}
