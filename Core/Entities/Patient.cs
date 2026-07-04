using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Patient
    {
        public string Id { get; set; }          // PK + FK لـ AppUser
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string? ImageUrl { get; set; }

        public AppUser User { get; set; }
        public List<Order> Orders { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
