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
    public class NurseServiceRepository : GenericRepository<NurseService>, INurseServiceRepository
    {
        public NurseServiceRepository(MedLinkDbContext ctx) : base(ctx) { }

        public async Task<NurseService?> GetWithDetailsAsync(string id) =>
            await _ctx.NurseServices.Include(ns => ns.SystemService)
                .FirstOrDefaultAsync(ns => ns.Id == id);

        public async Task<IReadOnlyList<NurseService>> GetByNurseAsync(string nurseId) =>
            await _ctx.NurseServices.Include(ns => ns.SystemService)
                .Where(ns => ns.NurseId == nurseId).ToListAsync();

        // الممرضين اللي لسه ملهمش الخدمة دي (للاستخدام لما الأدمن يضيف خدمة جديدة)
        public async Task<List<string>> GetNurseIdsMissingServiceAsync(string systemServiceId)
        {
            var nurseIds = await _ctx.Nurses.Select(n => n.Id).ToListAsync();
            var have = await _ctx.NurseServices
                .Where(ns => ns.SystemServiceId == systemServiceId)
                .Select(ns => ns.NurseId).ToListAsync();
            return nurseIds.Except(have).ToList();
        }
    }
}
