using Core.Entities;
using Core.RepositoryInterfaces;
using WebAPI.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPI.Services;

namespace WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------------- Services ----------------

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database
            builder.Services.AddDbContext<MedLinkDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpContextAccessor();

            // Unit of Work (بيجمع كل الـ repositories جواه)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IFileService, FileService>();

            // Identity (ده لوحده بيسجّل الـ cookie authentication)
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MedLinkDbContext>()
                .AddDefaultTokenProviders();

            // Identity options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // User settings
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            // 🍪 Cookie authentication settings (بدل JWT)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "MedLink.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;            // مهم للـ Flutter
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;

                // دي API: رجّع status codes بدل redirect لصفحة login
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

            // CORS (لازم AllowCredentials عشان الكوكي تتبعت من تطبيق الـ Flutter)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AppCors", policy =>
                    policy.WithOrigins("http://localhost:5173") // عدّل الـ origin بتاعك
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });

            var app = builder.Build();

            // ---------------- Pipeline ----------------

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Seed roles + admin
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await DbSeeder.SeedAsync(userManager, roleManager);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AppCors");          // قبل Authentication
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}