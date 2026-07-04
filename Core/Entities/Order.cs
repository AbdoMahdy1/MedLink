using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Address { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public int PatientAge { get; set; }
        public decimal Price { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; } = "Pending";

        // FKs
        public string NurseId { get; set; }
        public Nurse Nurse { get; set; }

        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public string NurseServiceId { get; set; }
        public NurseService NurseService { get; set; }
    }
}
