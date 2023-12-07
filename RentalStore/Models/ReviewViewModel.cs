using System.ComponentModel.DataAnnotations;

namespace RentalStore.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int rentalId { get; set; }
        [Required]
        public Rental Rental { get; set; }
        [Required(ErrorMessage = "Review text is required.")]
        public string ReviewText { get; set; }

        [Required(ErrorMessage = "Score is required.")]
        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5.")]
        public int Score { get; set; }
    }
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int rentalId { get; set; }
        [Required]
        public Rental Rental { get; set; }

        [Required(ErrorMessage = "Review text is required.")]
        public string ReviewText { get; set; }

        [Required(ErrorMessage = "Score is required.")]
        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5.")]
        public int Score { get; set; }
    }
}
