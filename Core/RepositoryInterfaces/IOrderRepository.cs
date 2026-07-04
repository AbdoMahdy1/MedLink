using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetWithDetailsAsync(string id);
        Task<IReadOnlyList<Order>> GetByPatientAsync(string patientId);
        Task<IReadOnlyList<Order>> GetByNurseAsync(string nurseId);
        Task<int> CountByNurseOnDateAsync(string nurseId, DateTime date);
        Task<bool> NurseHasCareOrderOnDateAsync(string nurseId, DateTime date);
    }
}
