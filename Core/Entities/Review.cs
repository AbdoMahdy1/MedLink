using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Review
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Rate { get; set; }                  // 1..5
        public string? Description { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public string NurseId { get; set; }
        public Nurse Nurse { get; set; }
    }
}
