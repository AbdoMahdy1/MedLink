using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IReadOnlyList<Review>> GetByNurseAsync(string nurseId);
    }
}
