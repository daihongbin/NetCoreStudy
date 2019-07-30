using Michael.JwtWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Michael.JwtWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        public string GetCurrentTime()
        {
            var sub = User.FindFirst(d => d.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
