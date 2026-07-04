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
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> b)
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).IsRequired().HasMaxLength(100);
            b.Property(p => p.Address).HasMaxLength(200);
            b.Property(p => p.Gender).HasMaxLength(10);

            // one-to-one مع AppUser بنفس الـ Id
            b.HasOne(p => p.User).WithOne()
             .HasForeignKey<Patient>(p => p.Id)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(p => p.Orders).WithOne(o => o.Patient)
             .HasForeignKey(o => o.PatientId).OnDelete(DeleteBehavior.Restrict);

            b.HasMany(p => p.Reviews).WithOne(r => r.Patient)
             .HasForeignKey(r => r.PatientId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
