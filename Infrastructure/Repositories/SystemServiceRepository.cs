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
    public class SystemServiceRepository : GenericRepository<SystemService>, ISystemServiceRepository
    {
        public SystemServiceRepository(MedLinkDbContext ctx) : base(ctx) { }
        public async Task<IReadOnlyList<SystemService>> GetDefaultsAsync() =>
            await _ctx.SystemServices.Where(s => s.IsDefault).ToListAsync();
    }
}
