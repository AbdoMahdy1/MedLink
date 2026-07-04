using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPatientRepository Patients { get; }
        INurseRepository Nurses { get; }
        IOrderRepository Orders { get; }
        IReviewRepository Reviews { get; }
        ISystemServiceRepository SystemServices { get; }
        INurseServiceRepository NurseServices { get; }
        Task<int> CompleteAsync();
    }
}
