using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface ISystemServiceRepository : IGenericRepository<SystemService>
    {
        Task<IReadOnlyList<SystemService>> GetDefaultsAsync();
    }
}
