using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Conduit.Context;
using Conduit.Model;
using Conduit.Model.DTO;
using Conduit.Model.Holder;
using Conduit.Model.Holder.Error;
using Conduit.Utils;
using Conduit.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Conduit.Controllers
{
    [Produces("application/json")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ConduitDbContext _context;
        private readonly IConfiguration _config;
        private ILogger _logger;

        public UsersController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ConduitDbContext context,
            IConfiguration config,
            ILoggerFactory loggerFactory
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _config = config;
            _logger = loggerFactory.CreateLogger<UsersController>();
        }

        [HttpPost("/api/users/login")]
        [LoginUserValidator]
        public async Task<IActionResult> LoginUser([FromBody] AuthUserHolder userHolder)
        {
            IdentityUser identityUser = await _userManager.FindByEmailAsync(userHolder.User.Email);
            if(null == identityUser)
            {
                LoggingUtil.LogError(_logger, $"No user was found with the email {userHolder.User.Email}");
                this.HttpContext.Response.StatusCode = 422;
                return Json(new ErrorResponseHolder($"Failed to authenticate user {userHolder.User.Email}"));
            }
            string password = userHolder.User.Password;
            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, password, false);
            if(!result.Succeeded)
            {
                LoggingUtil.LogError(_logger, $"The password for {userHolder.User.Email} was incorrect");
                this.HttpContext.Response.StatusCode = 422;
                return Json(new ErrorResponseHolder($"Failed to authenticate user {userHolder.User.Email}"));
            }
            UserPersonalizationDTO userPersonalization;
            try
            {
                userPersonalization = await _context.UserPersonalization.SingleAsync(e => e.UserId == identityUser.Id);
            } catch (Exception ex)
            {
                LoggingUtil.LogError(_logger, ex.Message);
                this.HttpContext.Response.StatusCode = 422;
                return Json(new ErrorResponseHolder($"Failed to retrieve user information for {userHolder.User.Email}"));
            }
            User user = new User(
                identityUser.Email,
                BuildToken(identityUser),
                identityUser.UserName,
                userPersonalization.Bio,
                userPersonalization.Image);
            return Ok(new UserHolder(user));
        }

        private string BuildToken(IdentityUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("/api/users")]
        [RegisterUserValidator]
        public async Task<IActionResult> RegisterUser([FromBody] AuthUserHolder userHolder)
        {
            var transaction = _context.Database.BeginTransaction();
            IdentityUser identityUser = userHolder.User.CreateIdentityUser();
            string password = userHolder.User.Password;
            var result = await _userManager.CreateAsync(identityUser, password);
            if(!result.Succeeded)
            {
                transaction.Rollback();
                LoggingUtil.LogError(_logger, $"Failed to create user {identityUser.Email}");
                this.HttpContext.Response.StatusCode = 422;
                return Json(new ErrorResponseHolder(result.Errors.Select(e => e.Description).ToList()));
            }
            try
            {
                UserPersonalizationDTO userPersonalization = new UserPersonalizationDTO(identityUser.Id, null, null);
                _context.UserPersonalization.Add(userPersonalization);
                await _context.SaveChangesAsync();
            } catch (DbUpdateException ex)
            {
                transaction.Rollback();
                LoggingUtil.LogError(_logger, ex.Message);
                this.HttpContext.Response.StatusCode = 500;
                return Json(new ErrorResponseHolder($"Failed to create user personalization {identityUser.Email}"));
            }
            transaction.Commit();
            this.HttpContext.Response.StatusCode = 201;
            User user = new User(
                identityUser.Email,
                BuildToken(identityUser),
                identityUser.UserName,
                null,
                null);
            return Json(new UserHolder(user));
        }

        [HttpGet("/api/user"), Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var jwtUser = HttpContext.User;
            if(!jwtUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Email))
            {
                LoggingUtil.LogError(_logger, "An unauthenticated user tried to access this endpoint");
                return Unauthorized();
            }
            string email = jwtUser.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            IdentityUser identityUser = await _userManager.FindByEmailAsync(email);
            if(null == identityUser)
            {
                LoggingUtil.LogError(_logger, $"No user was found with the email {email}");
                return NotFound();
            }
            UserPersonalizationDTO userPersonalization;
            try
            {
                userPersonalization = await _context.UserPersonalization.SingleAsync(e => e.UserId == identityUser.Id);
            }
            catch (Exception ex)
            {
                LoggingUtil.LogError(_logger, ex.Message);
                this.HttpContext.Response.StatusCode = 422;
                return Json(new ErrorResponseHolder($"Failed to retrieve user information for {identityUser.Email}"));
            }
            User user = new User(
                identityUser.Email,
                BuildToken(identityUser),
                identityUser.UserName,
                userPersonalization.Bio,
                userPersonalization.Image
                );
            return Ok(new UserHolder(user));
        }

        [HttpPut("/api/user"), Authorize]
        [UpdateUserValidator]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDataHolder userHolder)
        {
            var jwtUser = HttpContext.User;
            if (!jwtUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Email))
            {
                LoggingUtil.LogError(_logger, "An unauthenticated user tried to access this endpoint");
                return Unauthorized();
            }
            string email = jwtUser.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            IdentityUser identityUser = await _userManager.FindByEmailAsync(email);
            if (null == identityUser)
            {
                LoggingUtil.LogError(_logger, $"No user was found with the email {email}");
                return NotFound();
            }
            UserUpdateData updateData = userHolder.User;
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (null != updateData.Email)
                {
                    var result = await _userManager.SetEmailAsync(identityUser, updateData.Email);
                    if (!result.Succeeded)
                    {
                        transaction.Rollback();
                        LoggingUtil.LogError(_logger, $"Failed to update email {identityUser.Email} => {updateData.Email}");
                        this.HttpContext.Response.StatusCode = 422;
                        return Json(new ErrorResponseHolder(result.Errors.Select(e => e.Description).ToList()));
                    }
                }
                if (null != updateData.Username)
                {
                    var result = await _userManager.SetUserNameAsync(identityUser, updateData.Username);
                    if (!result.Succeeded)
                    {
                        transaction.Rollback();
                        LoggingUtil.LogError(_logger, $"Failed to update username {identityUser.UserName} => {updateData.Username}");
                        this.HttpContext.Response.StatusCode = 422;
                        return Json(new ErrorResponseHolder(result.Errors.Select(e => e.Description).ToList()));
                    }
                }
                if (null != updateData.Password)
                {
                    var passwordHash = _userManager.PasswordHasher.HashPassword(identityUser, updateData.Password);
                    identityUser.PasswordHash = passwordHash;
                    var result = await _userManager.UpdateAsync(identityUser);
                    if(!result.Succeeded)
                    {
                        transaction.Rollback();
                        LoggingUtil.LogError(_logger, $"Failed to update password for {identityUser.Email}");
                        this.HttpContext.Response.StatusCode = 422;
                        return Json(new ErrorResponseHolder(result.Errors.Select(e => e.Description).ToList()));
                    }
                }
                UserPersonalizationDTO userPersonalization = new UserPersonalizationDTO();
                if (null != updateData.Bio || null != updateData.Image)
                {
                    userPersonalization = await _context.UserPersonalization.FirstOrDefaultAsync(e => e.UserId == identityUser.Id);
                    if (null != updateData.Bio) userPersonalization.Bio = updateData.Bio;
                    if (null != updateData.Image) userPersonalization.Image = updateData.Image;
                    _context.UserPersonalization.Update(userPersonalization);
                    _context.SaveChanges();
                }
                transaction.Commit();
                UserHolder responseUserHolder = new UserHolder(new User(
                    identityUser.Email,
                    BuildToken(identityUser),
                    identityUser.UserName,
                    userPersonalization.Bio,
                    userPersonalization.Image));
                return Json(responseUserHolder);
            } catch (Exception)
            {
                transaction.Rollback();
                LoggingUtil.LogError(_logger, $"An exception occured updating {identityUser.Email}");
                this.HttpContext.Response.StatusCode = 422;
                return Json(new ErrorResponseHolder("Failed to update user"));
            }
        }
    }
}