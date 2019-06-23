using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDemo.Models;

namespace CoreDemo.Services
{
    public interface IMovieService
    {
        Task AddAsync(Movie model);

        Task<IEnumerable<Movie>> GetByCinemaAsync(int cinemaId);

        Task DeleteMovieByCinemaAsync(int cinemaId);

        Task<Movie> GetMovieByIdAsync(int movieId);

        Task DeleteByIdAsync(int movieId);
    }
}
