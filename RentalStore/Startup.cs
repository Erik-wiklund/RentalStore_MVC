using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using RentalStore.Areas.Identity.Data;
using RentalStore.Data;

namespace RentalStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<RentalStoreContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 22)),
        mysqlOptions => mysqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore)),
    ServiceLifetime.Scoped);


            services.AddIdentity<ApplicationUser, IdentityRole>()
                        .AddEntityFrameworkStores<RentalStoreContext>()
                        .AddDefaultUI()
                        .AddDefaultTokenProviders();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/home");
                    return Task.CompletedTask;
                });

                endpoints.MapControllerRoute(
            name: "home",
            pattern: "home",
            defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                     name: "admin",
                     pattern: "{controller=Admin}/{action=Index}/{id?}");


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

        }
    }
}
