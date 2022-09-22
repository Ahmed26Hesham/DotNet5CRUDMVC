using DotNet5CRUDMVC.Models;
using DotNet5CRUDMVC.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
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

        /*
        الanti forgery بتعمل التالي
        الاول بيكون موجود مع الفورم بتاعتك token السيرفر بعته للبراوزر
        ولما اليوزر بيعمل post
        ValidateAnti…
        وظيفتها انها تتأكد ان الtoken ده موجود ومظبوط
        وده نوع من انواع الحماية علشان تمنع ان حد يبعتلك ريكوستات من برا الابليكشن
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            //*********************
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View(model);

            }

            var Files = Request.Form.Files;

            if(!Files.Any())
            {

             model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
             ModelState.AddModelError("Poster", "please Select Movie Poster");
             return View(model);

            }

            var Poster = Files.FirstOrDefault();
            var AllowedExtension = new List<string> { ".jpg", ".png" };
            
            if(!AllowedExtension.Contains(Path.GetExtension(Poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "only allowed .jpg , .png");
                return View(model);
            }

            if(Poster.Length > 1048576)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster can not be more than 1 MB");
                return View(model);

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

             return RedirectToAction(nameof(Index));
        }

    }
}
