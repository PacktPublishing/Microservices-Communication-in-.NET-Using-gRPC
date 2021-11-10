using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserFacingApp.Models;

namespace UserFacingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly GrpcClientWrapper clientWrapper;

        public HomeController(GrpcClientWrapper clientWrapper)
        {
            this.clientWrapper = clientWrapper;
        }

        public async Task<IActionResult> Index()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            Console.WriteLine($"Access token: {accessToken}");
            return View();
        }

        public IActionResult LogOut()
        {
            return new SignOutResult(new[]
            {
                CookieAuthenticationDefaults.AuthenticationScheme,
                "oidc"
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var secretDetails = await this.clientWrapper.GetSecret(id, accessToken);
            return View(secretDetails);
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
    }
}
