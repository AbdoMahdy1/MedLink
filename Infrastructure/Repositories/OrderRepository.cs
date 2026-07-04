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
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(MedLinkDbContext ctx) : base(ctx) { }
        public async Task<Order?> GetWithDetailsAsync(string id) =>
            await _ctx.Orders.Include(o => o.NurseService).ThenInclude(ns => ns.SystemService)
                .Include(o => o.Nurse).Include(o => o.Patient)
                .FirstOrDefaultAsync(o => o.Id == id);
        public async Task<IReadOnlyList<Order>> GetByPatientAsync(string pid) =>
            await _ctx.Orders.Include(o => o.NurseService).ThenInclude(ns => ns.SystemService)
                .Where(o => o.PatientId == pid).ToListAsync();
        public async Task<IReadOnlyList<Order>> GetByNurseAsync(string nid) =>
            await _ctx.Orders.Where(o => o.NurseId == nid).ToListAsync();
        public async Task<int> CountByNurseOnDateAsync(string nurseId, DateTime date)
        {
            var day = date.Date;
            return await _ctx.Orders
                .Where(o => o.NurseId == nurseId
                         && o.Date.Date == day
                         && o.Status != OrderStatus.Cancelled
                         && o.Status != OrderStatus.Rejected)
                .CountAsync();
        }

        public async Task<bool> NurseHasCareOrderOnDateAsync(string nurseId, DateTime date)
        {
            var day = date.Date;
            return await _ctx.Orders
                .Include(o => o.NurseService).ThenInclude(ns => ns.SystemService)
                .AnyAsync(o => o.NurseId == nurseId
                            && o.Date.Date == day
                            && o.Status != OrderStatus.Cancelled
                            && o.Status != OrderStatus.Rejected
                            && o.NurseService.SystemService.IsCareService);
        }
    }
}
