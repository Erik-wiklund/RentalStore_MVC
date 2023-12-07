using RentalStore.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace RentalStore.Models
{

    public class Rental
    {
        public int Id { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public Movie Movie { get; set; }
        public int NumberOfDaysRented { get; set; }

        public DateTime DateRented { get; set; }

        public DateTime? DateReturned { get; set; }
        public bool IsReturned { get; set; }

    }
    public class RentalViewModel
    {
        public int Id { get; set; }

        [Required]
        public UserRolesViewModel Customer { get; set; }

        [Required]
        public Movie Movie { get; set; }
        public int NumberOfDaysRented { get; set; }

        public DateTime DateRented { get; set; }

        public DateTime? DateReturned { get; set; }
        public bool IsReturned { get; set; }

    }
}
