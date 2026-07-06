using Core.Common;
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
        public async Task<PagedResult<Nurse>> SearchAsync(NurseFilterParams p)
        {
            var query = _ctx.Nurses
                .Include(n => n.User)
                .Include(n => n.NurseServices).ThenInclude(ns => ns.SystemService)
                .Include(n => n.Reviews)
                .Where(n => n.Status == NurseStatus.Approved)   // المقبولين بس
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(p.Search))
                query = query.Where(n => n.Name.Contains(p.Search));

            if (!string.IsNullOrWhiteSpace(p.Gender))
                query = query.Where(n => n.Gender == p.Gender);

            if (p.MinExperience.HasValue)
                query = query.Where(n => n.ExperienceYears >= p.MinExperience.Value);

            // فلترة الخدمة + السعر مع بعض
            if (!string.IsNullOrWhiteSpace(p.ServiceId))
            {
                query = query.Where(n => n.NurseServices.Any(ns =>
                    ns.SystemServiceId == p.ServiceId
                    && (!p.MinPrice.HasValue || ns.Price >= p.MinPrice)
                    && (!p.MaxPrice.HasValue || ns.Price <= p.MaxPrice)));
            }
            else
            {
                if (p.MinPrice.HasValue)
                    query = query.Where(n => n.NurseServices.Any(ns => ns.Price >= p.MinPrice));
                if (p.MaxPrice.HasValue)
                    query = query.Where(n => n.NurseServices.Any(ns => ns.Price <= p.MaxPrice));
            }

            // الترتيب
            query = (p.SortBy?.ToLower()) switch
            {
                "experience" => p.Desc ? query.OrderByDescending(n => n.ExperienceYears)
                                        : query.OrderBy(n => n.ExperienceYears),
                "name" => p.Desc ? query.OrderByDescending(n => n.Name)
                                 : query.OrderBy(n => n.Name),
                _ => query.OrderBy(n => n.Name)
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((p.Page - 1) * p.PageSize)
                .Take(p.PageSize)
                .ToListAsync();

            return new PagedResult<Nurse>
            { Items = items, Page = p.Page, PageSize = p.PageSize, TotalCount = total };
        }
    }
}
