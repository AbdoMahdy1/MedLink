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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.Address).HasMaxLength(200);
            b.Property(o => o.Description).HasMaxLength(500);
            b.Property(o => o.ServiceType).HasMaxLength(50);
            b.Property(o => o.Status).HasMaxLength(20);
            b.Property(o => o.Price).HasColumnType("decimal(18,2)");

            b.HasOne(o => o.NurseService).WithMany()
             .HasForeignKey(o => o.NurseServiceId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
