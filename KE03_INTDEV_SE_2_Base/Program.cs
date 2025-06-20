using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Logging;

namespace KE03_INTDEV_SE_2_Base
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Logging configureren
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddEventSourceLogger();

            // Services toevoegen aan de container
            builder.Services.AddDbContext<MatrixIncDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Authenticatie
            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            // Cookie authenticatie toevoegen
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Login";
                });

            // Repositories registreren in DI container
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IPartRepository, PartRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            var app = builder.Build();

            // HTTP request pipeline configureren
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Database initialiseren en migreren
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<MatrixIncDbContext>();
                context.Database.EnsureCreated(); // Create database and tables without migrations
                MatrixIncDbInitializer.Initialize(context);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Authenticatie en autorisatie middleware toevoegen
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
