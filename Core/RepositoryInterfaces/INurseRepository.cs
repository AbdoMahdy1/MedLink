using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface INurseRepository : IGenericRepository<Nurse>
    {
        Task<Nurse?> GetWithDetailsAsync(string id);
        Task<IReadOnlyList<Nurse>> GetAllWithDetailsAsync(NurseStatus? status = null);
    }
}
