using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Context;
using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Controllers
{
    [Produces("application/json")]
    public class ProfilesController : IConduitController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConduitDbContext _context;
        private readonly ILogger<ProfilesController> _logger;
        private readonly IProfileService _profileService;

        public ProfilesController(
            UserManager<IdentityUser> userManager,
            ConduitDbContext context,
            ILoggerFactory loggerFactory,
            IProfileService profileService
            )
        {
            _userManager = userManager;
            _context = context;
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _profileService = profileService;
        }

        [HttpGet("/api/profiles/{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            try
            {
                string authedUsername;
                try
                {
                    authedUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                }
                catch
                {
                    authedUsername = null;
                }
                Profile profile = await _profileService.GetProfile(username, authedUsername);
                return Ok(new ProfileHolder(profile));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpPost("/api/profiles/{username}/follow"), Authorize(Policy = "ValidUsername")]
        public async Task<IActionResult> FollowUser(string username)
        {
            try
            {
                string authedUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                Profile profile = await _profileService.SetUserIsFollowing(username, authedUsername, true);
                return Ok(new ProfileHolder(profile));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpDelete("/api/profiles/{username}/follow"), Authorize(Policy = "ValidUsername")]
        public async Task<IActionResult> UnfollowUser(string username)
        {
            try
            {
                string authedUsername = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                Profile profile = await _profileService.SetUserIsFollowing(username, authedUsername, false);
                return Ok(new ProfileHolder(profile));
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }
    }
}