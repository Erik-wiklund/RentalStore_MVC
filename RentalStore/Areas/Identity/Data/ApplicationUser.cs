using Microsoft.AspNetCore.Identity;

namespace RentalStore.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int UsernameChangeLimit { get; set; } = 10;
    public byte[] ProfilePicture { get; set; } = new byte[] { };
}

