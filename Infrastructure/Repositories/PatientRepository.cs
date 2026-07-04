using Core.Entities;
using Core.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(MedLinkDbContext ctx) : base(ctx) { }
        public async Task<Patient?> GetWithDetailsAsync(string id) =>
            await _ctx.Patients.Include(p => p.Orders).Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}
