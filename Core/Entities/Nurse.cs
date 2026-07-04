using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Nurse
    {
        public string Id { get; set; }          // PK + FK لـ AppUser
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string? Description { get; set; }
        public int ExperienceYears { get; set; }
        public List<string> CvFiles { get; set; } = new();   // هتتخزن JSON (شرح تحت)
        public string? ImageUrl { get; set; }

        public NurseStatus Status { get; set; } = NurseStatus.Pending;  // للموافقة

        public AppUser User { get; set; }
        public List<Order> Orders { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<NurseService> NurseServices { get; set; } = new();
    }
}
