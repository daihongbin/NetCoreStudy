using CoreBackend.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CoreBackend.Api.Controllers
{
    [Route("api/[controller]")]
    public class TestController:Controller
    {
        private readonly MyContext _myContext;

        public TestController(MyContext context)
        {
            _myContext = context;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
