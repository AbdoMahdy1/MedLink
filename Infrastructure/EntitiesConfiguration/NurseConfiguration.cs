using System.Text.Json;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.EntitiesConfiguration
{
    public class NurseConfiguration : IEntityTypeConfiguration<Nurse>
    {
        public void Configure(EntityTypeBuilder<Nurse> b)
        {
            b.HasKey(n => n.Id);
            b.Property(n => n.Name).IsRequired().HasMaxLength(100);
            b.Property(n => n.Address).HasMaxLength(200);
            b.Property(n => n.Gender).HasMaxLength(10);
            b.Property(n => n.Description).HasMaxLength(500);
            b.Property(n => n.Status).HasConversion<string>().HasMaxLength(20);

            // CvFiles (List<string>) تتخزن كـ JSON في عمود واحد
            var converter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());

            var comparer = new ValueComparer<List<string>>(
                (a, c) => a!.SequenceEqual(c!),
                v => v.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
                v => v.ToList());

            b.Property(n => n.CvFiles)
                .HasConversion(converter)
                .Metadata.SetValueComparer(comparer);

            b.HasOne(n => n.User)
                .WithOne()
                .HasForeignKey<Nurse>(n => n.Id)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(n => n.Orders)
                .WithOne(o => o.Nurse)
                .HasForeignKey(o => o.NurseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(n => n.Reviews)
                .WithOne(r => r.Nurse)
                .HasForeignKey(r => r.NurseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(n => n.NurseServices)
                .WithOne(ns => ns.Nurse)
                .HasForeignKey(ns => ns.NurseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
