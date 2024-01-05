using System.ComponentModel.DataAnnotations;

namespace RentalStore.Models
{
    public class RentalPlan
    {
        public int Id { get; set; }
        [Display(Name = "Rental Plan")]
        [Required]
        public string RentalPlanName { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        public float Price { get; set; }
    }

    public class RentalPlanViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Rental Plan")]
        [Required]
        public string RentalPlanName { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        public float Price { get; set; }
    }

}
