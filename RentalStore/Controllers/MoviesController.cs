using Microsoft.AspNetCore.Mvc;
using RentalStore.Data;

namespace RentalStore.Controllers
{
    public class MoviesController : Controller
    {
        public IActionResult Index([FromServices] RentalStoreContext context)
        {
            var movies = context.Movies.ToList();
            return View("Index", movies);
        }

        public IActionResult SingleMovie()
        {
            return View();
        }
    }
}
