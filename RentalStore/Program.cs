using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentalStore;
using RentalStore.Areas.Identity.Data;
using RentalStore.Data;

public class Program
{
	public static async Task Main(string[] args)
	{
		var host = CreateHostBuilder(args).Build();

		using (var scope = host.Services.CreateScope())
		{
			var services = scope.ServiceProvider;
			var loggerFactory = services.GetRequiredService<ILoggerFactory>();

			try
			{
				var context = services.GetRequiredService<RentalStoreContext>();
				var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
				var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
				await ContextSeed.SeedRolesAsync(userManager, roleManager);
				await ContextSeed.SeedMoviesAsync(context);
				await ContextSeed.SeedAdminAsync(userManager, roleManager);

				// Ensure the database is created and migrated
				context.Database.EnsureCreated();
				context.Database.Migrate();

				// Seed roles and other data

				// Add other seed data methods if needed

				// You can also add additional seed methods for other data
				// e.g., await ContextSeed.SeedMoviesAsync(context);
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex, "An error occurred seeding the DB.");
			}
		}

		host.Run();
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
	.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			});
}
