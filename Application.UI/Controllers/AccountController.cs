using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Core.Interface;
using Application.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.UI.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IHttpService _http;
        private readonly ApiUrl _url;

        public AccountController(IHttpService http, ApiUrl url)
        {
            _http = http;
            _url = url;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            //Call api to get token and credentials
            string token = await GetTokens(model);


            //Get Authentication Properties
            var authProps = new AuthenticationProperties
            {
                IsPersistent = true
            };
            authProps.StoreTokens(new AuthenticationToken []{new AuthenticationToken {Name = "Bearer",Value = token}});

            var claimsIdentity = new ClaimsIdentity("cookie");
            var principal = new ClaimsPrincipal(claimsIdentity);

            //cookie sign in

            await HttpContext.SignInAsync("cookie", principal, authProps);
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        private async Task<string> GetTokens(LoginModel model)
        {
            IDictionary<string, string> headers = new Dictionary<string, string>()
            {

            };
            var payload = new
            {
                username = model.UserName,
                password = model.Password
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _http.Post($"{_url.BaseUrl}/api/account/login", content, headers);

            var res = await response.Content.ReadAsStringAsync();
            string token = (string)JObject.Parse(res)["token"];
            return token;
        }
    }
}