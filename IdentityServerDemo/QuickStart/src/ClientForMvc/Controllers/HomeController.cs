using ClientForMvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientForMvc.Controllers
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

        //对视图进行保护
        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies","oidc");
        }


        public async Task<IActionResult> CallApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("http://localhost:5001/identity");

            ViewBag.Json = JArray.Parse(content).ToString();

            return View("Json");

        }
    }
}
