namespace WebAPI.DTOs.Order
{
    public class OrderDto
    {
        public int? Id { get; set; }
        public string? Patient { get; set; }
        public string? PatientId { get; set; }
        public string? Nurse { get; set; }
        public string? NurseId { get; set; }
        public string? Service { get; set; }
        public string? ServiceId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
