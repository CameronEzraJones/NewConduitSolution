using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Validators.AuthorizationHandler
{
    public class ValidJWTAuthorizationHandler : AuthorizationHandler<ValidUsernameRequirement>
    {
        UserManager<IdentityUser> _userManager;
        ILogger<ValidJWTAuthorizationHandler> _logger;

        public ValidJWTAuthorizationHandler(UserManager<IdentityUser> userManager, ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ValidJWTAuthorizationHandler>();
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidUsernameRequirement requirement)
        {
            var jwtUser = context.User;
            if(!jwtUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub))
            {
                return;
            }
            string username = jwtUser.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
            if(null == username)
            {
                return;
            }
            try
            {
                IdentityUser user = await _userManager.FindByNameAsync(username);
                context.Succeed(requirement);
                return;
            } catch
            {
                return;
            }
        }
    }

    public class ValidUsernameRequirement : IAuthorizationRequirement
    {

    }
}
