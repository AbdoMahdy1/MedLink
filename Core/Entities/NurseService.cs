using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class NurseService
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public decimal Price { get; set; }

        public string NurseId { get; set; }
        public Nurse Nurse { get; set; }

        public string SystemServiceId { get; set; }
        public SystemService SystemService { get; set; }
    }
}
