using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;

        public UsersController(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, 
            ILoggerService logger,
            IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            var location = GetControllerActionNames();
            try
            {
                var username = userDto.EmailAddress;
                var password = userDto.Password;
                var user = new IdentityUser { Email = username, UserName = username };
                _logger.LogInfo($"{location}: Register attempt for User {username}");
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"{location}: {error.Code} - {error.Description}");
                    }
                    return InternalError($"{location}: {username} User registration Attempt failed");
                }

                await _userManager.AddToRoleAsync(user, "Customer");
                return Created("login",new { result.Succeeded });
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }

        }
        
        /// <summary>
        /// User login endpoint
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns> 
        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserDTO userDto)
        {
            var location = GetControllerActionNames();

            try
            {
                var username = userDto.EmailAddress;
                var password = userDto.Password;
                _logger.LogInfo($"{location}: Login attempt from User {username}");
                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

                if (result.Succeeded)
                {
                    _logger.LogInfo($"{location}: {username} Successfully authenticated");
                    var user = await _userManager.FindByNameAsync(username);
                    var tokenString = await GenerateJsonWebToken(user);
                    return Ok(new { token = tokenString });
                }
                _logger.LogWarn($"{location}: {username} Not authenticatied");
                return Unauthorized(userDto);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        private async Task<string> GenerateJsonWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType,r)));

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }

    }
}
