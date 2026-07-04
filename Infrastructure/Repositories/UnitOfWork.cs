using Core.RepositoryInterfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MedLinkDbContext _ctx;
        public IPatientRepository Patients { get; }
        public INurseRepository Nurses { get; }
        public IOrderRepository Orders { get; }
        public IReviewRepository Reviews { get; }
        public ISystemServiceRepository SystemServices { get; }
        public INurseServiceRepository NurseServices { get; }

        public UnitOfWork(MedLinkDbContext ctx)
        {
            _ctx = ctx;
            Patients = new PatientRepository(ctx);
            Nurses = new NurseRepository(ctx);
            Orders = new OrderRepository(ctx);
            Reviews = new ReviewRepository(ctx);
            SystemServices = new SystemServiceRepository(ctx);
            NurseServices = new NurseServiceRepository(ctx);
        }

        public async Task<int> CompleteAsync() => await _ctx.SaveChangesAsync();
        public void Dispose() => _ctx.Dispose();
    }
}
