using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MVCClient.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MVCClient.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "管理员")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "管理员")]
        public async Task<IActionResult> About()
        {
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            ViewData["idToken"] = idToken;

            var discoveryClient = new DiscoveryClient("https://localhost:5001");
            var doc = await discoveryClient.GetAsync();

            var userInfoClient = new UserInfoClient(doc.UserInfoEndpoint);
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var response = await userInfoClient.GetAsync(accessToken);
            var claims = response.Claims;
            var email = claims.FirstOrDefault(f => f.Type == "email")?.Value;
            ViewData["email"] = email;

            return View();
        }

        [Authorize(Roles = "管理员")]
        public async Task<IActionResult> Contact()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:6001");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.solenovex.hateoas+json"));

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            ViewData["accessToken"] = accessToken;
            httpClient.SetBearerToken(accessToken);

            var res = await httpClient.GetAsync("api/values").ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var objects = JsonConvert.DeserializeObject<dynamic>(json);
                ViewData["json"] = objects;
                return View();
            }

            throw new Exception($"Error Occurred: ${res.ReasonPhrase}");
        }

        [Authorize(Roles = "管理员")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "管理员")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public async Task Logout()
        {
            //清除本地Cookies
            await HttpContext.SignOutAsync("Cookies");
            //清除IdentityServer4
            await HttpContext.SignOutAsync("oidc");
        }
    }
}
