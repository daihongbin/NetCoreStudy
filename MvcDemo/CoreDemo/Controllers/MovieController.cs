using CoreDemo.Models;
using CoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreDemo.Controllers
{
    public class MovieController:Controller
    {
        private readonly IMovieService _movieService;
        private readonly ICinemaService _cinemaService;

        public MovieController(IMovieService movieService,ICinemaService cinemaService)
        {
            _movieService = movieService;
            _cinemaService = cinemaService;
        }

        public async Task<IActionResult> Index(int cinemaId)
        {
            var cinema = await _cinemaService.GetByIdAsync(cinemaId);

            ViewBag.Title = $"{cinema.Name}这个电影院上映的电影有：";
            ViewBag.CinemaId = cinemaId;
            
            return View(await _movieService.GetByCinemaAsync(cinemaId));
        }

        public IActionResult Add(int cinemaId)
        {
            ViewBag.Title = "添加电影";

            return View(new Movie { CinemaId = cinemaId});
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(Movie model)
        {
            if (ModelState.IsValid)
            {
                await _movieService.AddAsync(model);
            }

            return RedirectToAction("Index",new {cinemaId = model.CinemaId});
        }

        public IActionResult Edit(int movieId)
        {
            var movie = _movieService.GetMovieByIdAsync(movieId).Result;
            if (movie == null)
            {
                return BadRequest();
            }
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie updateMovie)
        {
            if (updateMovie == null)
            {
                return BadRequest();
            }

            var movie = await _movieService.GetMovieByIdAsync(updateMovie.Id);
            if (movie == null)
            {
                return BadRequest();
            }

            movie.Id = updateMovie.Id;
            movie.CinemaId = updateMovie.CinemaId;
            movie.Name = updateMovie.Name;
            movie.ReleaseDate = updateMovie.ReleaseDate;
            movie.Starring = updateMovie.Starring;

            return RedirectToAction("Index", new {cinemaId = movie.CinemaId});
        }

        public async Task<IActionResult> Delete(int movieId)
        {
            var movie = await _movieService.GetMovieByIdAsync(movieId);
            if (movie == null)
            {
                return BadRequest();
            }

            await _movieService.DeleteByIdAsync(movieId);

            return RedirectToAction("Index", new {cinemaId = movie.CinemaId});
        }
    }
}
