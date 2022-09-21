using DotNet5CRUDMVC.Models;
using DotNet5CRUDMVC.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5CRUDMVC.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            this._context = context;
        }

        // asyncronous programming  ------------ *******************
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }
        public async Task<IActionResult> Create()
        {
            // **********************************************
            var ViewModel = new MovieFormViewModel {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
        };
            return View(ViewModel);
        }
    }
}
