using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDemo.Models;

namespace CoreDemo.Services
{
    public class CinemaMemoryService : ICinemaService
    {
        private readonly List<Cinema> _cinemas = new List<Cinema>();

        public CinemaMemoryService()
        {
            _cinemas.Add(new Cinema
            {
                Id = 1,
                Name = "City Cinema",
                Location = "Road ABC, No.123",
                Capacity = 1000
            });

            _cinemas.Add(new Cinema
            {
                Id = 2,
                Name = "Fly Cinema",
                Location = "Road Hello, No.1024",
                Capacity = 500
            });
        }

        Task ICinemaService.AddAsync(Cinema model)
        {
            var maxId = _cinemas.Max(x => x.Id);
            model.Id = maxId + 1;
            _cinemas.Add(model);
            return Task.CompletedTask;
        }

        Task<Cinema> ICinemaService.GetByIdAsync(int id)
        {
            return Task.Run(() => _cinemas.FirstOrDefault(x => x.Id == id));
        }

        Task<IEnumerable<Cinema>> ICinemaService.GetllAllAsync()
        {
            return Task.Run(() => _cinemas.AsEnumerable());
        }

        Task ICinemaService.DeleteAsync(int cinemaId)
        {
            var cinema = _cinemas.FirstOrDefault(f => f.Id == cinemaId);
            _cinemas.Remove(cinema);
            return Task.CompletedTask;
        }
    }
}
