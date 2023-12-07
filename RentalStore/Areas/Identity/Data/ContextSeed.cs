using Microsoft.AspNetCore.Identity;
using RentalStore.Data;
using RentalStore.Models;

namespace RentalStore.Areas.Identity.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Basic.ToString()));
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FirstName = "erik",
                LastName = "wiklund",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "!Aphora11");
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Basic.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Moderator.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Admin.ToString());
                }

            }
        }

        public static async Task SeedMoviesAsync(RentalStoreContext context)
        {
            // Seed Movies
            if (!context.Movies.Any())
            {
                int movieImageId = 1; // Assuming 1 is the image identifier
                byte[] movieImageData = GetImageData(movieImageId);

                var movies = new List<Movie>
        {
            new Movie
            {
                Title = "Movie 1",
                Description = "Description 1",
                MovieImage = movieImageData,
                Price = 9.99m,
                Genre = "Western",
                ReleaseDate = new DateTime(2012, 12, 12),
                DurationMinutes = 120,
                Rating = "5",
            },
            new Movie
            {
                Title = "Movie 2",
                Description = "Description 2",
                MovieImage = movieImageData,
                Price = 12.99m,
                Genre = "Western",
                ReleaseDate = new DateTime(2012, 12, 12),
                DurationMinutes = 120,
                Rating = "5",
            },
            // Add more movies as needed
        };

                context.Movies.AddRange(movies);
                await context.SaveChangesAsync();
            }
        }

        // Example GetImageData method (replace it with your actual implementation)
        private static byte[] GetImageData(int imageId)
        {
            // Logic to retrieve and convert image data to byte array
            // Replace this with your actual implementation
            return new byte[] { /* your image data here */ };
        }


    }
}
