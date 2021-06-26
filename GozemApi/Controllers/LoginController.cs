using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;

using AutoMapper;

using GozemApi.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Raven.Client.Documents.Session;

namespace GozemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        private readonly IAsyncDocumentSession _session;
        private readonly FileServerOptions _fileServerOptions;

        public LoginController(IOptions<FileServerOptions> fileServerOptions,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> options, IMapper mapper, IAsyncDocumentSession session)
        {
            _fileServerOptions = fileServerOptions.Value;
            this._userManager = userManager;
            this._signInManager = signInManager;
            _mapper = mapper;
            _session = session;
            _jwtSettings = options.Value;
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().Wait();
            return Ok();
        }

        [HttpPost("signup")]
        public ActionResult<ApplicationUserViewModel> SignUp([FromBody] NewApplicationUser vm)
        {
            if (!TryValidateModel(vm)) return ValidationProblem();
            var user = _mapper.Map<ApplicationUser>(vm);
            var createResult = _userManager.CreateAsync(user, vm.Password).Result;
            if (createResult.Succeeded)
            {
                createResult = _userManager.AddToRoleAsync(user, "User").Result;
                return Created(string.Empty, _mapper.Map<ApplicationUserViewModel>(user));
            }
            return BadRequest();
        }

        [HttpPost]
        public ActionResult<string> UserLogin(LoginViewModel loginModel)
        {
            if (!TryValidateModel(loginModel)) return ValidationProblem();

            ApplicationUser user;
            if (Regex.IsMatch(loginModel.UsernameOrEmail, @"^.+@.+\.(.+)$"))
            {
                user = _userManager.FindByEmailAsync(loginModel.UsernameOrEmail).Result;
            }
            else
            {
                user = _userManager.FindByNameAsync(loginModel.UsernameOrEmail).Result;
            }

            if (user is null)
            {
                return NotFound();
            }

            var signInResult = _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false).Result;
            if (signInResult.Succeeded)
            {
                signInResult = _signInManager.PasswordSignInAsync(user, loginModel.Password, true, false).Result;

                if (!signInResult.Succeeded)
                {
                    return Problem("Invalid username or password");
                }
            }

            string issuer = $"{Request.Scheme}://{Request.Host}";
            var claims = new List<Claim>
            {
                {new Claim(JwtRegisteredClaimNames.Sub, user.Id) },
                {new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}") },
                {new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) },
                {new Claim(JwtRegisteredClaimNames.Iss, issuer) },
                {new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) },
                {new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)) },
                {new Claim(JwtRegisteredClaimNames.Email, user.Email) },
                {new Claim("profile_photo", user.ProfilePhoto ?? string.Empty) }
            };

            var jwtToken = JwtTokenGenerator.GenerateJwtToken(claims, _jwtSettings.SigningKey, _jwtSettings.TokenExpirationInDays, _jwtSettings.ValidAudience, issuer);

            return Json(jwtToken);
        }
    }
}