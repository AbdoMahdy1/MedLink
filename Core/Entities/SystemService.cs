using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SystemService
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCareService { get; set; }
        public bool IsFixedPrice { get; set; }     // سعر ثابت ولا رينج؟
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal FixedPrice { get; set; }
        public string? ImageUrl { get; set; }

        public List<NurseService> NurseServices { get; set; } = new();
    }
}
