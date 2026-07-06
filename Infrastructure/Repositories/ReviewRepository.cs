using Core.Entities;
using Core.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(MedLinkDbContext ctx) : base(ctx) { }
        public async Task<IReadOnlyList<Review>> GetByNurseAsync(string nid) =>
            await _ctx.Reviews.Where(r => r.NurseId == nid).ToListAsync();
        public async Task<Review?> GetByPatientAndNurseAsync(string patientId, string nurseId) =>
    await _ctx.Reviews.FirstOrDefaultAsync(r => r.PatientId == patientId && r.NurseId == nurseId);

        public async Task<IReadOnlyList<Review>> GetByPatientAsync(string patientId) =>
            await _ctx.Reviews.Where(r => r.PatientId == patientId).ToListAsync();
    }
}
