using DotNet5CRUDMVC.Models;
using DotNet5CRUDMVC.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5CRUDMVC.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private List<string> _AllowedExtension = new List<string> { ".jpg", ".png" };
        private long _MaxAllowedPosterSize = 1048576;


        public MoviesController(ApplicationDbContext context , IToastNotification toastNotification )
        {
            this._context = context;
            this._toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(m=>m.Rate).ToListAsync();
            return View(movies);
        }
        public async Task<IActionResult> Create()
        {
            var ViewModel = new MovieFormViewModel {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
        };
            return View("MovieForm",ViewModel);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);

            }

            var Files = Request.Form.Files;

            if(!Files.Any())
            {

             model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
             ModelState.AddModelError("Poster", "please Select Movie Poster");
             return View("MovieForm", model);

            }

            var Poster = Files.FirstOrDefault();
            
            if(!_AllowedExtension.Contains(Path.GetExtension(Poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "only allowed .jpg , .png");
                return View("MovieForm", model);
            }

            if(Poster.Length > _MaxAllowedPosterSize)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster can not be more than 1 MB");
                return View("MovieForm", model);

            }

            using var DataStream = new MemoryStream();
            await Poster.CopyToAsync(DataStream);


            var movie = new Movie()
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Rate = model.Rate,
                StoryLine = model.StoryLine,
                Poster = DataStream.ToArray()
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Movie Created Successfully");

             return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? Id)
        {

            if (Id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(Id);

            if (movie == null)
                return NotFound();
            
            var ViewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                GenreId=movie.GenreId,
                Rate = movie.Rate,
                StoryLine=movie.StoryLine,
                Poster=movie.Poster, 
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("MovieForm", ViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);

            }
            var movie = await _context.Movies.FindAsync(model.Id);

            if (movie == null)
                return NotFound();

            var Files = Request.Form.Files;

            if (Files.Any())
            {

                var Poster = Files.FirstOrDefault();

                using var DataStream = new MemoryStream();
                await Poster.CopyToAsync(DataStream);

                model.Poster = DataStream.ToArray();

                if (!_AllowedExtension.Contains(Path.GetExtension(Poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "only allowed .jpg , .png");
                    return View("MovieForm", model);
                }

                if (Poster.Length > _MaxAllowedPosterSize)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster can not be more than 1 MB");
                    return View("MovieForm", model);

                }


                movie.Poster = model.Poster;

            }


            movie.Title = model.Title;
            movie.GenreId = model.GenreId;
            movie.Year = model.Year;
            movie.Rate = model.Rate;
            movie.StoryLine = model.StoryLine;

            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Movie Updated Successfully");


            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.Include(m=>m.Genre).SingleOrDefaultAsync(m=>m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok();
        }

    }
}
