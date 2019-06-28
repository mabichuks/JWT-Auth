using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Core.Models;
using Application.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Application.Web.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly Core.Models.TokenOptions _options;

        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, Core.Models.TokenOptions options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _options = options;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
          
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);



            if (result.Succeeded)
            {
                UserModel user = await _userManager.FindByNameAsync(model.UserName);
                var token = GenerateJwtToken(model.UserName, user);
                return Ok(new { token });
            };


            return NotFound();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new UserModel
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var existing = await _userManager.FindByNameAsync(model.Email);
            if (existing != null)
            {
                throw new Exception("User already Exists");
            }

            var createUser = await _userManager.CreateAsync(user);
            if (!createUser.Succeeded)
            {
                throw new Exception("An Error Occured");
            }

            var addPassword = await _userManager.AddPasswordAsync(user, model.Password);
            if (addPassword.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return  Ok(GenerateJwtToken(model.Email, user));
  
            } else
            {
                //Delete User and Throw Exception
                await _userManager.DeleteAsync(user);
                throw new Exception("An Error Occured");
            }
            //var result = await _userManager.CreateAsync(user, model.Password);

            //if (result.Succeeded)
            //{
            //    await _signInManager.SignInAsync(user, false);
            //    return  Ok(GenerateJwtToken(model.Email, user));
            //}

            throw new ApplicationException("UNKNOWN_ERROR");
        }

        private string GenerateJwtToken(string email, UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.FullName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_options.AccessTokenExpiration));

            var token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: expires, signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}