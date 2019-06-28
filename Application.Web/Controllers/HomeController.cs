using Application.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Application.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContext;


        public HomeController(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        [Authorize]

        public IActionResult Index()
        {
            //var headers = _httpContext.HttpContext.Request.Cookies;
            //if (headers.Count() < 3) return Unauthorized();
            //JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            //var user = handler.ValidateToken(headers, );

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
    }
}
