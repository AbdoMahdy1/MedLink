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
    public class NurseRepository : GenericRepository<Nurse>, INurseRepository
    {
        public NurseRepository(MedLinkDbContext ctx) : base(ctx) { }

        public async Task<Nurse?> GetWithDetailsAsync(string id) =>
            await _ctx.Nurses
                .Include(n => n.User)
                .Include(n => n.NurseServices).ThenInclude(ns => ns.SystemService)
                .Include(n => n.Reviews)
                .FirstOrDefaultAsync(n => n.Id == id);

        public async Task<IReadOnlyList<Nurse>> GetAllWithDetailsAsync(NurseStatus? status = null)
        {
            var q = _ctx.Nurses
                .Include(n => n.User)
                .Include(n => n.NurseServices).ThenInclude(ns => ns.SystemService)
                .Include(n => n.Reviews)
                .AsQueryable();
            if (status.HasValue) q = q.Where(n => n.Status == status.Value);
            return await q.ToListAsync();
        }
    }
}
