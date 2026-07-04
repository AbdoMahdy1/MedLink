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
    public class SystemServiceConfiguration : IEntityTypeConfiguration<SystemService>
    {
        public void Configure(EntityTypeBuilder<SystemService> b)
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Name).IsRequired().HasMaxLength(100);
            b.Property(s => s.Description).HasMaxLength(500);
            b.Property(s => s.MinPrice).HasColumnType("decimal(18,2)");
            b.Property(s => s.MaxPrice).HasColumnType("decimal(18,2)");
            b.Property(s => s.FixedPrice).HasColumnType("decimal(18,2)");
        }
    }
}
