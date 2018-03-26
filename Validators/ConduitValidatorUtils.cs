using Conduit.Model.Holder.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Conduit.Validators
{
    public class ConduitValidatorUtils
    {
        protected virtual void InvalidateRequest(ActionExecutingContext context, String message, ILogger logger, int statusCode)
        {
            String trace = GenerateTrace();
            logger.LogError(message + "\nTrace is " + trace);
            context.HttpContext.Response.StatusCode = statusCode;
            context.Result = new JsonResult(new ErrorResponseHolder(message, trace));
        }

        protected virtual void InvalidateRequest(ActionExecutingContext context, List<String> messages, ILogger logger, int statusCode)
        {
            String trace = GenerateTrace();
            logger.LogError(String.Join("\n", messages) + "\nTrace is " + trace);
            context.HttpContext.Response.StatusCode = statusCode;
            context.Result = new JsonResult(new ErrorResponseHolder(messages, trace));
        }

        protected virtual bool ValidateTokenHasClaims(ClaimsPrincipal jwtUser)
        {
            return jwtUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Email);
        }

        private String GenerateTrace()
        {
            Guid guid = Guid.NewGuid();
            return Convert.ToBase64String(guid.ToByteArray());
        }
    }
}
