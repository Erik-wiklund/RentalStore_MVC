using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalStore.Areas.Identity.Data;
using RentalStore.Data;
using RentalStore.Models;

namespace RentalStore.Controllers
{
	[Authorize(Roles = "Admin, Moderator")]
	public class AdminController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly RentalStoreContext _MovieManager;

		public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, RentalStoreContext movieManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_MovieManager = movieManager;
		}

		public IActionResult Index()
		{
			return View("~/Views/Admin/Index.cshtml");
		}

		public async Task<IActionResult> RoleManager(List<ManageUserRolesViewModel> model)
		{
			var roles = await _roleManager.Roles.ToListAsync();
			return View("~/Views/Admin/RoleManager/Index.cshtml", roles);
		}
		[HttpPost]
		public async Task<IActionResult> AddRole(string roleName)
		{
			if (roleName != null)
			{
				await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
			}
			return RedirectToAction("RoleManager");
		}

		public async Task<IActionResult> EditRole(string roleId)
		{
			var role = await _roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
				return View("NotFound");
			}

			var model = new ManageUserRolesViewModel
			{
				RoleId = role.Id,
				RoleName = role.Name
			};

			return View("~/Views/Admin/RoleManager/EditRole.cshtml", role);
		}

		[HttpPost]
		public async Task<IActionResult> EditRole(string roleId, string roleName)
		{
			var role = await _roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
				return View("NotFound");
			}

			// Update the role's properties
			role.Name = roleName;

			// Use UpdateAsync with the modified role
			var result = await _roleManager.UpdateAsync(role);

			if (result.Succeeded)
			{
				// Redirect to the list of roles or another appropriate action
				return RedirectToAction("RoleManager");
			}

			// If the update fails, add errors to ModelState and return to the edit view
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			// Pass the model back to the view
			return View("~/Views/Admin/RoleManager/EditRole.cshtml", role);
		}

		public async Task<IActionResult> DeleteRole(string roleId)
		{
			if (string.IsNullOrEmpty(roleId))
			{
				// Handle the case where roleId is null or empty
				return BadRequest("Role Id is required.");
			}

			var role = await _roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				// Handle the case where the role is not found
				return NotFound($"Role with Id = {roleId} not found.");
			}

			var result = await _roleManager.DeleteAsync(role);

			if (result.Succeeded)
			{
				// Redirect to the list of roles or another appropriate action
				return RedirectToAction("RoleManager");
			}
			else
			{
				// If the deletion fails, add errors to ModelState and return to the view
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

				// You might want to handle the case where deletion fails differently
				return View("Error");
			}
		}

		public async Task<IActionResult> UserRoles()
		{
			var users = await _userManager.Users.ToListAsync();
			var userRolesViewModel = new List<UserRolesViewModel>();
			foreach (ApplicationUser user in users)
			{
				var thisViewModel = new UserRolesViewModel();
				thisViewModel.Id = user.Id;
				thisViewModel.Email = user.Email;
				thisViewModel.FirstName = user.FirstName;
				thisViewModel.LastName = user.LastName;
				thisViewModel.UserName = user.UserName;
				thisViewModel.Roles = await GetUserRoles(user);
				userRolesViewModel.Add(thisViewModel);
			}

			return View("~/Views/Admin/UserRoles/Index.cshtml", userRolesViewModel);
		}
		private async Task<List<string>> GetUserRoles(ApplicationUser user)
		{
			return new List<string>(await _userManager.GetRolesAsync(user));
		}

		public async Task<IActionResult> Manage(string userId)
		{
			ViewBag.userId = userId;
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
				return View("NotFound");
			}

			ViewBag.UserName = user.UserName;
			var model = new List<ManageUserRolesViewModel>();
			foreach (var role in _roleManager.Roles.ToList())
			{
				var userRolesViewModel = new ManageUserRolesViewModel
				{
					RoleId = role.Id,
					RoleName = role.Name
				};
				if (await _userManager.IsInRoleAsync(user, role.Name))
				{
					userRolesViewModel.Selected = true;
				}
				else
				{
					userRolesViewModel.Selected = false;
				}
				model.Add(userRolesViewModel);
			}

			return View("~/Views/Admin/UserRoles/Manage.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return View();
			}

			var roles = await _userManager.GetRolesAsync(user);
			var result = await _userManager.RemoveFromRolesAsync(user, roles);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot remove user existing roles");
				return View(model);
			}

			result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot add selected roles to user");
				return View(model);
			}

			return RedirectToAction("UserRoles");
		}

		public async Task<IActionResult> MovieManager([FromServices] RentalStoreContext context)
		{
			var movies = await context.Movies.ToListAsync();
			var moviesViewModel = new List<MovieViewModel>();

			foreach (Movie movie in movies)
			{
				var thisViewModel = new MovieViewModel
				{
					Id = movie.Id,
					Title = movie.Title,
					Description = movie.Description,
					MovieImage = movie.MovieImage,
					Price = movie.Price,
					ReleaseDate = movie.ReleaseDate,
					DurationMinutes = movie.DurationMinutes,
					Genre = movie.Genre,
					InStock = movie.InStock,
					Rating = movie.Rating,
					// Add other properties as needed
				};

				moviesViewModel.Add(thisViewModel);
			}

			return View("~/Views/Admin/MovieManager/Index.cshtml", moviesViewModel);
		}

		public async Task<IActionResult> EditMovie(int movieId, [FromServices] RentalStoreContext context)
		{
			var movie = await context.Movies.FindAsync(movieId);
			if (movie == null)
			{
				ViewBag.ErrorMessage = $"Movie with Id = {movieId} cannot be found";
				return View("NotFound");
			}

			var model = new MovieViewModel
			{
				Id = movie.Id,
				Title = movie.Title,
				Description = movie.Description,
				MovieImage = movie.MovieImage,
				ReleaseDate = movie.ReleaseDate,
				DurationMinutes = movie.DurationMinutes,
				Genre = movie.Genre,
				InStock = movie.InStock,
				Rating = movie.Rating,
				Price = movie.Price,
				// Add other properties as needed
			};

			return View("~/Views/Admin/MovieManager/EditMovie.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> EditMovie(MovieViewModel model, [FromServices] RentalStoreContext context)
		{
			var movie = await context.Movies.FindAsync(model.Id);
			if (movie == null)
			{
				ViewBag.ErrorMessage = $"Movie with Id = {model.Id} cannot be found";
				return View("NotFound");
			}

			// Update the movie's properties
			movie.Title = model.Title;
			movie.Description = model.Description;
			movie.ReleaseDate = model.ReleaseDate;
			movie.DurationMinutes = model.DurationMinutes;
			movie.Genre = model.Genre;
			movie.InStock = model.InStock;
			movie.Rating = model.Rating;
			movie.Price = model.Price;

			if (Request.Form.Files.Count > 0)
			{
				IFormFile file = Request.Form.Files.FirstOrDefault();
				using (var dataStream = new MemoryStream())
				{
					await file.CopyToAsync(dataStream);
					movie.MovieImage = dataStream.ToArray();
				}
			}

			// Save changes to the database
			await context.SaveChangesAsync();

			// Redirect to the list of movies or another appropriate action
			return RedirectToAction("MovieManager");
		}

		public IActionResult CreateMovie()
		{
			return View("~/Views/Admin/MovieManager/CreateMovie.cshtml");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateMovie(MovieViewModel model, [FromServices] RentalStoreContext context)
		{
			if (ModelState.IsValid)
			{
				// Create a Movie instance from the MovieViewModel
				var movie = new Movie
				{
					Title = model.Title,
					Description = model.Description,
					ReleaseDate = model.ReleaseDate,
					Price = model.Price,
					Genre = model.Genre,
					InStock = model.InStock,
					Rating = model.Rating,
					// Map other properties accordingly
				};

				// Add the new movie to the context
				context.Movies.Add(movie);

				if (Request.Form.Files.Count > 0)
				{
					IFormFile file = Request.Form.Files.FirstOrDefault();
					using (var dataStream = new MemoryStream())
					{
						await file.CopyToAsync(dataStream);
						movie.MovieImage = dataStream.ToArray();
					}
				}

				// Save changes to the database
				await context.SaveChangesAsync();

				// Redirect to the MovieManager action
				return RedirectToAction("MovieManager");
			}

			// If ModelState is not valid, return to the create view with the model
			return View("Index", model);
		}

		public async Task<IActionResult> DeleteMovie(int movieId, [FromServices] RentalStoreContext context)
		{
			var movie = await context.Movies.FindAsync(movieId);

			if (movie == null)
			{
				// Handle the case where the movie is not found
				return NotFound($"Movie with Id = {movieId} not found.");
			}

			context.Movies.Remove(movie);
			await context.SaveChangesAsync();

			// Redirect to the action that displays the list of movies
			return RedirectToAction("Index", "Admin");
		}

		[HttpPost]
		public string RentalManager(string searchString, bool notUsed)
		{
			return "From [HttpPost]Index: filter on " + searchString;
		}


		public async Task<IActionResult> RentalManager([FromServices] RentalStoreContext context, string searchType, string searchString)
		{
			var rentals = context.Rentals
				.Include(r => r.Movie)
				.Include(r => r.User)
				.ToList();

			switch (searchType)
			{
				case "All":
					// No specific filter, use the original list
					break;
				case "CustomerName":
					rentals = rentals.Where(r => r.User.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
					break;
				case "DaysRented":
					rentals = rentals.Where(r => r.NumberOfDaysRented.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
					break;
				case "MovieName":
					rentals = rentals.Where(r => r.Movie.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
					break;
				case "DateReturned":
					DateTime searchDateReturned;
					if (DateTime.TryParse(searchString, out searchDateReturned))
					{
						// Assuming you want to search for rentals on the specified date
						rentals = rentals.Where(r => r.DateReturned.HasValue && r.DateReturned.Value.Date == searchDateReturned.Date).ToList();
					}
					else
					{
						// Handle the case where searchString is not a valid DateTime
						ViewBag.ErrorMessage = "Invalid date format for search.";
						return View("Index");
					}
					break;

				case "DateRented":
					DateTime searchDate;
					if (DateTime.TryParse(searchString, out searchDate))
					{
						// Assuming you want to search for rentals on the specified date
						rentals = rentals.Where(r => r.DateRented.Date == searchDate.Date).ToList();
					}
					else
					{
						// Handle the case where searchString is not a valid DateTime
						ViewBag.ErrorMessage = "Invalid date format for search.";
						return View("Index");
					}
					break;
				case "NotReturned":
					rentals = rentals.Where(r => !r.IsReturned).ToList();
					break;
			}

			var rentalViewModels = rentals.Select(rental => new RentalViewModel
			{
				Id = rental.Id,
				DateRented = rental.DateRented,
				NumberOfDaysRented = rental.NumberOfDaysRented,
				IsReturned = rental.IsReturned,
				DateReturned = rental.DateReturned,
				Movie = rental.Movie != null
					? new Movie
					{
						Title = rental.Movie.Title,
						MovieImage = rental.Movie.MovieImage
						// Add other properties as needed
					}
					: null,
				Customer = rental.User
			}).ToList();

			if (rentalViewModels == null || rentalViewModels.Count == 0)
			{
				ViewBag.ErrorMessage = "No rentals found.";
				return View("Index");
			}

			return View("RentalManager", rentalViewModels);
		}




	}
}
