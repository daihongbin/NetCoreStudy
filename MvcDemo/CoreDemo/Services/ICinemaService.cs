using System.Threading.Tasks;
using System.Collections;
using CoreDemo.Models;
using System.Collections.Generic;

namespace CoreDemo.Services
{
    public interface ICinemaService
    {
        Task<IEnumerable<Cinema>> GetllAllAsync();

        Task<Cinema> GetByIdAsync(int id);

        Task AddAsync(Cinema model);

    }
}
