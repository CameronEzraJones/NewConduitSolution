using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Services;
using Conduit.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Controllers
{
    [Produces("application/json")]
    public class UsersController : IConduitController
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;

        public UsersController(
            ILoggerFactory loggerFactory,
            IUserService userService
            )
        {
            _logger = loggerFactory.CreateLogger<UsersController>();
            _userService = userService;
        }

        [HttpPost("/api/users/login")]
        [LoginUserValidator]
        public async Task<IActionResult> LoginUser([FromBody] AuthUserHolder userHolder)
        {
            try
            {
                User user = await _userService.AuthenticateUser(userHolder.User.Email, userHolder.User.Password);
                return Ok(new UserHolder(user));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpPost("/api/users")]
        [RegisterUserValidator]
        public async Task<IActionResult> RegisterUser([FromBody] AuthUserHolder userHolder)
        {
            try
            {
                User user = await _userService.RegisterUser(userHolder.User.Username, userHolder.User.Email, userHolder.User.Password);
                this.HttpContext.Response.StatusCode = 201;
                return Json(new UserHolder(user));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpGet("/api/user"), Authorize(Policy = "ValidUsername")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var jwtUser = HttpContext.User;
                string username = jwtUser.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                User user = await _userService.GetCurrentUser(username);
                return Ok(new UserHolder(user));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpPut("/api/user"), Authorize(Policy = "ValidUsername")]
        [UpdateUserValidator]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDataHolder userHolder)
        {
            try
            {
                var jwtUser = HttpContext.User;
                string username = jwtUser.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                User user = await _userService.UpdateUser(username, userHolder.User);
                return Ok(new UserHolder(user));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }
    }
}