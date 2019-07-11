using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Core.Models;
using Application.Web.Models;
using Microsoft.AspNetCore.Authentication;
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
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            };


            return NotFound();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
             await _signInManager.SignOutAsync();
            return Ok();
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
                return  Ok(GenerateJwtToken(user));
  
            } else
            {
                //Delete User and Throw Exception
                await _userManager.DeleteAsync(user);
                throw new Exception("An Error Occured");
            }

            throw new ApplicationException("UNKNOWN_ERROR");
        }

        [HttpGet("login/{provider}")]
        public  IActionResult ExternalLogin(string provider)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("ExternalLoginCallback"),
                Items = { { "scheme", provider } }
            };

            return Challenge(props, provider);
        }

        [HttpGet("external")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            var externalUser = result.Principal.FindFirstValue("sub")
                               ?? result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                               ?? throw new Exception("Cannot Find User");

            var provider = result.Properties.Items["scheme"];
            var user = await _userManager.FindByLoginAsync(provider, externalUser);

            if(user == null)
            {
                var email = result.Principal.FindFirstValue("email")
                            ?? result.Principal.FindFirstValue(ClaimTypes.Email);

                if(email != null)
                {
                     user = await _userManager.FindByEmailAsync(email);
                    
                    if(user == null)
                    {

                        user = new UserModel { Email = email };
                        IdentityResult identityResult = await _userManager.CreateAsync(user);

                        if(identityResult.Succeeded)
                        {
                            await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, externalUser, provider));

                        }
                    }

                }


            }

            if(user == null)
            {
                return NotFound();
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            await _signInManager.SignInAsync(user, false);
            return Ok(new { token = GenerateJwtToken(user) });

        }

        private string GenerateJwtToken(UserModel user)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.FullName)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expires = DateTime.Now.AddDays(Convert.ToDouble(_options.AccessTokenExpiration));

            JwtSecurityToken token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: expires, signingCredentials: creds);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}