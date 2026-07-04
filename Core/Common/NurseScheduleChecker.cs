using Core.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public static class NurseScheduleChecker
    {
        // بترجع: هل متاح؟ ولو لأ، ليه؟
        public static async Task<(bool Ok, string? Reason)> CheckAsync(
            IUnitOfWork uow, string nurseId, DateTime date, bool isCareService)
        {
            // 1) لو عنده خدمة رعاية في اليوم ده → مش متاح خالص
            if (await uow.Orders.NurseHasCareOrderOnDateAsync(nurseId, date))
                return (false, "الممرض مشغول برعاية اليوم ده، مش متاح لأي خدمة تانية");

            var count = await uow.Orders.CountByNurseOnDateAsync(nurseId, date);

            // 2) وصل للحد الأقصى (15)
            if (count >= ScheduleSettings.MaxServicesPerDay)
                return (false, $"الممرض وصل الحد الأقصى ({ScheduleSettings.MaxServicesPerDay} خدمات في اليوم)");

            // 3) لو الطلب ده رعاية → لازم يكون يومه فاضي (الرعاية بتشغل اليوم كله)
            if (isCareService && count > 0)
                return (false, "الممرض عنده طلبات اليوم ده، مينفعش ياخد خدمة رعاية");

            return (true, null);
        }
    }
}
