using System.Linq;
using CoreDemo.Models;
using CoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreDemo.Controllers
{
    public class HomeController:Controller
    {
        private readonly ICinemaService _cinemaService;

        private readonly IMovieService _movieService;

        private readonly IDataProvider _dataProvider;

        public HomeController(ICinemaService cinemaService,IMovieService movieService,IDataProvider dataProvider)
        {
            _cinemaService = cinemaService;
            _movieService = movieService;

            _dataProvider = dataProvider;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "电影院列表" + _dataProvider.Get();

            return View(await _cinemaService.GetllAllAsync());
        }

        public IActionResult Add()
        {
            ViewBag.Title = "添加电影院";

            return View(new Cinema());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Cinema model)
        {
            if (ModelState.IsValid)
            {
                await _cinemaService.AddAsync(model);
            }

            return RedirectToAction("Index");
        }
        
        public IActionResult Edit(int cinemaId)
        {
            var cinema = _cinemaService.GetByIdAsync(cinemaId).Result;
            if (cinema == null)
            {
                return BadRequest();
            }
            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cinema updateModel)
        {
            if (updateModel == null)
            {
                return BadRequest();
            }

            var cinema = await _cinemaService.GetByIdAsync(updateModel.Id);
            if (cinema == null)
            {
                return NotFound();
            }

            cinema.Id = updateModel.Id;
            cinema.Name = updateModel.Name;
            cinema.Location = updateModel.Location;
            cinema.Capacity = updateModel.Capacity;

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int cinemaId)
        {
            var cinema = await _cinemaService.GetByIdAsync(cinemaId);
            if (cinema == null)
            {
                return BadRequest();
            }

            await _cinemaService.DeleteAsync(cinemaId);

            //同时需要删除电影院下面的电影
            var movies = await _movieService.GetByCinemaAsync(cinemaId);
            if (movies != null && movies.Count() >= 0)
            {
                await _movieService.DeleteMovieByCinemaAsync(cinemaId);
            }

            return RedirectToAction("Index");
        }
    }
}
