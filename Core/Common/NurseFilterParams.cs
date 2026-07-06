using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public class NurseFilterParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int Page { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
        }

        public string? Search { get; set; }        // بحث بالاسم
        public string? Gender { get; set; }
        public string? ServiceId { get; set; }      // SystemServiceId
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinExperience { get; set; }
        public string? SortBy { get; set; }         // "name" | "experience"
        public bool Desc { get; set; }
    }
}
