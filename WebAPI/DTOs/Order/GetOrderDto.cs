namespace WebAPI.DTOs.Order
{
    public class GetOrderDto
    {
        public string Id { get; set; }
        public string Patient { get; set; }
        public string Nurse { get; set; }
        public string Service { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
