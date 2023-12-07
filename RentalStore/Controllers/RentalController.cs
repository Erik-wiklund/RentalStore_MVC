using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalStore.Areas.Identity.Data;
using RentalStore.Data;
using RentalStore.Models;

namespace RentalStore.Controllers
{
    public class RentalController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RentalController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index([FromServices] RentalStoreContext context)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var rentals = await context.Rentals
                .Include(r => r.Movie)
                .Where(r => r.User.Id == user.Id)
                .ToListAsync();

            var rentalViewModels = new List<RentalViewModel>();

            foreach (var rental in rentals)
            {
                var thisViewModel = new RentalViewModel
                {
                    Id = rental.Id,
                    DateRented = rental.DateRented,
                    NumberOfDaysRented = rental.NumberOfDaysRented,
                    IsReturned = rental.IsReturned,
                    DateReturned = rental.DateReturned
                };

                // Assume RentalMovieModel is a different model and you want to map its properties to Movie
                if (rental.Movie != null)
                {
                    thisViewModel.Movie = new Movie
                    {
                        Title = rental.Movie.Title,
                        MovieImage = rental.Movie.MovieImage
                        // Add other properties as needed
                    };
                }

                rentalViewModels.Add(thisViewModel);
            }

            if (rentalViewModels == null || rentalViewModels.Count == 0)
            {
                ViewBag.ErrorMessage = $"No rentals found for user with ID = {user.Id}";
                return View("Index");
            }

            return View(rentalViewModels);
        }







        public async Task<IActionResult> CreateRental(int movieId, [FromServices] RentalStoreContext context)
        {
            var movie = context.Movies.FirstOrDefault(m => m.Id == movieId);

            if (movie == null)
            {
                ViewBag.ErrorMessage = $"Movie with Id = {movieId} cannot be found";
                return View("NotFound");
            }

            // Retrieve the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var movieViewModel = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                MovieImage = movie.MovieImage, // Assuming MovieImage is a byte array
                                               // Add other properties as needed
            };

            // Pass the user details to the view
            ViewBag.UserEmail = user.Email;

            ViewData["Title"] = $"Rent {movie.Title}";
            return View(movieViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRental(int movieId, int numberOfDaysRented, [FromServices] RentalStoreContext context)
        {
            if (ModelState.IsValid)
            {
                var movie = await context.Movies.FindAsync(movieId);

                if (movie == null)
                {
                    ViewBag.ErrorMessage = $"Movie with Id = {movieId} cannot be found";
                    return View("NotFound");
                }

                // Retrieve the currently logged-in user
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    // Handle the case where the user is not found
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                // Create a new Rental object and associate it with the logged-in user
                var rental = new Rental
                {
                    User = user, // Set the existing user directly
                    Movie = movie,
                    NumberOfDaysRented = numberOfDaysRented,
                    DateRented = DateTime.Now,
                    DateReturned = null,
                    IsReturned = false
                };

                // Update movie stock and save changes
                movie.InStock--;
                context.Update(movie);

                // Save the rental object to the database
                context.Rentals.Add(rental);
                await context.SaveChangesAsync();

                // Redirect to a confirmation or success page
                return RedirectToAction("RentConfirmation", new { id = rental.Id });
            }

            return View("~/Views/Admin/MovieManager/Index.cshtml");
        }

        public async Task<IActionResult> RentConfirmation(int id, [FromServices] RentalStoreContext model)
        {
            var rental = await model.Rentals.FindAsync(id);

            if (rental == null)
            {
                // Handle the case where the rental is not found
                return NotFound($"Unable to load rental with ID '{id}'.");
            }

            var rentalModel = new RentalViewModel
            {
                Id = rental.Id,
            };

            return View("RentConfirmation", rentalModel);
        }

        public async Task<IActionResult> ReturnMovie(int rentalId, [FromServices] RentalStoreContext context)
        {
            var rental = await context.Rentals
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

            if (rental == null)
            {
                ViewBag.ErrorMessage = $"Rental with Id = {rentalId} cannot be found";
                return View("NotFound");
            }

            var rentalViewModel = new RentalViewModel
            {
                Id = rental.Id,
                DateRented = rental.DateRented,
                NumberOfDaysRented = rental.NumberOfDaysRented,
                IsReturned = rental.IsReturned,
                DateReturned = rental.DateReturned
            };

            // Assume RentalMovieModel is a different model and you want to map its properties to Movie
            if (rental.Movie != null)
            {
                rentalViewModel.Movie = new Movie
                {
                    Title = rental.Movie.Title,
                    MovieImage = rental.Movie.MovieImage
                    // Add other properties as needed
                };
            }

            return View("~/Views/Rental/Index.cshtml", new List<RentalViewModel> { rentalViewModel });
        }



        [HttpPost]
        public async Task<IActionResult> ReturnMovie(int rentalId, int movieId, [FromServices] RentalStoreContext model)
        {
            if (ModelState.IsValid)
            {
                var rental = await model.Rentals
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

                if (rental == null)
                {
                    // Handle the case where the rental is not found
                    return NotFound($"Unable to load rental with ID '{rentalId}'.");
                }

                if (rental.IsReturned)
                {
                    // Handle the case where the movie has already been returned
                    ViewBag.ErrorMessage = $"Movie with ID {rentalId} has already been returned.";
                    return View("Error");
                }

                // Update rental details
                rental.DateReturned = DateTime.Now;
                rental.IsReturned = true;

                // Update movie stock
                rental.Movie.InStock++;

                // Save changes to the database
                await model.SaveChangesAsync();

                // Redirect to a confirmation or success page
                return RedirectToAction("Index", rental);
            }

            // If ModelState is not valid, return to the Index view
            return View();
        }
        public IActionResult CreateReview(int rentalId, [FromServices] RentalStoreContext context)
        {
            var rental = context.Rentals
        .Include(r => r.Movie) // Make sure to include the Movie data
        .FirstOrDefault(m => m.Id == rentalId);

            if (rental == null)
            {
                ViewBag.ErrorMessage = $"Rental with Id = {rentalId} cannot be found";
                return View("NotFound");
            }

            var RentalViewModel = new RentalViewModel
            {
                Id = rental.Id,
                Movie = rental.Movie,

            };
            return View("Review", RentalViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(int rentalId, string ReviewText, int Score, [FromServices] RentalStoreContext context)
        {
            var existingReview = await context.Reviews
                .Include(r => r.Rental) // Make sure to include the associated Rental data
                .FirstOrDefaultAsync(r => r.rentalId == rentalId);

            if (existingReview != null && existingReview.Rental != null)
            {
                // A review already exists for this RentalId, handle accordingly
                ViewBag.ErrorMessage = $"A review for the selected rental already exists. You cannot submit multiple reviews for the same rental.";

                var rental = await context.Rentals
                    .Include(r => r.Movie) // Make sure to include the Movie data
                    .FirstOrDefaultAsync(m => m.Id == rentalId);

                var RentalViewModel = new RentalViewModel
                {
                    Id = rental.Id,
                    Movie = rental.Movie,
                };

                // Pass the model to the view even in error cases
                return View("Review", RentalViewModel);
            }

            if (ModelState.IsValid)
            {
                var rental = await context.Rentals.FindAsync(rentalId);

                if (rental == null)
                {
                    ViewBag.ErrorMessage = $"Rental with Id = {rentalId} cannot be found";
                    return View("Review");
                }

                var review = new Review
                {
                    rentalId = rental.Id,
                    Rental = rental,
                    ReviewText = ReviewText,
                    Score = Score,
                };

                context.Reviews.Add(review);
                await context.SaveChangesAsync();

                return RedirectToAction("ReviewConfirmation", new { id = review.Id });
            }

            return View("ReviewConfirmation");
        }





        public async Task<IActionResult> ReviewConfirmation(int id, int rentalId, [FromServices] RentalStoreContext model)
        {
            var review = await model.Reviews.FindAsync(id);

            if (review == null)
            {
                // Handle the case where the review is not found
                return NotFound($"Unable to load review with ID '{id}'.");
            }

            var reviewModel = new ReviewViewModel
            {
                Id = review.Id,
                // Populate other properties as needed
            };

            return View("ReviewConfirmation", reviewModel);
        }

    }
}
