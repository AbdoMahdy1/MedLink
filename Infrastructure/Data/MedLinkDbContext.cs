using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class MedLinkDbContext : IdentityDbContext<AppUser>
    {
        public MedLinkDbContext(DbContextOptions<MedLinkDbContext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Nurse> Nurses => Set<Nurse>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<SystemService> SystemServices => Set<SystemService>();
        public DbSet<NurseService> NurseServices => Set<NurseService>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // مهم جداً لـ Identity
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
