namespace RentalStore.Models
{

    public class User
    {

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public ICollection<string> Roles { get; set; } = new List<string>();
    }

    public class UserRolesViewModel
    {

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}
