using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntitiesConfiguration
{
    public class NurseServiceConfiguration : IEntityTypeConfiguration<NurseService>
    {
        public void Configure(EntityTypeBuilder<NurseService> b)
        {
            b.HasKey(ns => ns.Id);
            b.Property(ns => ns.Price).HasColumnType("decimal(18,2)");

            b.HasOne(ns => ns.SystemService).WithMany(ss => ss.NurseServices)
             .HasForeignKey(ns => ns.SystemServiceId).OnDelete(DeleteBehavior.Restrict);

            // يمنع تكرار نفس الخدمة لنفس الممرض
            b.HasIndex(ns => new { ns.NurseId, ns.SystemServiceId }).IsUnique();
        }
    }
}
