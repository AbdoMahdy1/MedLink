using Core.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly MedLinkDbContext _ctx;
        protected readonly DbSet<T> _set;
        public GenericRepository(MedLinkDbContext ctx) { _ctx = ctx; _set = ctx.Set<T>(); }

        public async Task<IReadOnlyList<T>> GetAllAsync() => await _set.ToListAsync();
        public async Task<T?> GetByIdAsync(string id) => await _set.FindAsync(id);
        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> p) => await _set.Where(p).ToListAsync();
        public async Task AddAsync(T e) => await _set.AddAsync(e);
        public async Task AddRangeAsync(IEnumerable<T> e) => await _set.AddRangeAsync(e);
        public void Update(T e) => _set.Update(e);
        public void Delete(T e) => _set.Remove(e);
    }
}
