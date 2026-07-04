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
    }
}
