using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface INurseServiceRepository : IGenericRepository<NurseService>
    {
        Task<NurseService?> GetWithDetailsAsync(string id);
        Task<IReadOnlyList<NurseService>> GetByNurseAsync(string nurseId);
        Task<List<string>> GetNurseIdsMissingServiceAsync(string systemServiceId);
    }
}
