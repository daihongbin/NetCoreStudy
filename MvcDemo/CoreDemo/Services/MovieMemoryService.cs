using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDemo.Models;

namespace CoreDemo.Services
{
    public class MovieMemoryService : IMovieService
    {
        public readonly List<Movie> _movies = new List<Movie>();

        public MovieMemoryService()
        {
            _movies.Add(new Movie
            {
                CinemaId = 1,
                Id = 1,
                Name = "Superman",
                ReleaseDate = new DateTime(2018,10,1),
                Starring = "Nick"
            });

            _movies.Add(new Movie
            {
                CinemaId = 1,
                Id = 2,
                Name = "Ghost",
                ReleaseDate = new DateTime(1997, 5, 4),
                Starring = "Michael Jackson"
            });

            _movies.Add(new Movie
            {
                CinemaId = 2,
                Id = 3,
                Name = "Fight",
                ReleaseDate = new DateTime(2018, 12, 3),
                Starring = "Tommy"
            });
        }

        Task IMovieService.AddAsync(Movie model)
        {
            var maxId = _movies.Max(x => x.Id);
            model.Id = maxId + 1;
            _movies.Add(model);
            return Task.CompletedTask;
        }

        Task<IEnumerable<Movie>> IMovieService.GetByCinemaAsync(int cinemaId)
        {
            return Task.Run(() => _movies.Where(w => w.CinemaId == cinemaId));
        }
    }
}
