using Conduit.Exceptions;
using Conduit.Model.Holder.Error;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Conduit.Controllers
{
    public class IConduitController : Controller
    {
        protected virtual IActionResult HandleException(Exception ex, ILogger logger)
        {
            String trace = GenerateTrace();
            logger.LogError(ex.ToString() + "\nTrace is " + trace);
            if(ex is ConduitUnauthorizedException)
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(ex.Message + "\nTrace number is " + trace);
            }
            if(ex is ConduitForbiddenException)
            {
                this.HttpContext.Response.StatusCode = 403;
                return Json(ex.Message + "\nTrace number is " + trace);
            }
            if(ex is ConduitNotFoundException)
            {
                this.HttpContext.Response.StatusCode = 404;
                return Json(ex.Message + "\nTrace number is " + trace);
            }
            if(ex is ConduitValidationException)
            {
                var conduitValidationException = (ConduitValidationException)ex;
                this.HttpContext.Response.StatusCode = 422;
                if(null != (conduitValidationException.Errors))
                {
                    return Json(new ErrorResponseHolder(conduitValidationException.Errors, trace));
                }
                return Json(new ErrorResponseHolder(ex.Message, trace));
            }
            this.HttpContext.Response.StatusCode = 500;
            return Json("An exception occured on the server. Trace is " + trace);
        }

        protected virtual bool ValidateToken (ClaimsPrincipal jwtUser)
        {
            return jwtUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Email) ? true : false;
        }

        private String GenerateTrace()
        {
            Guid guid = Guid.NewGuid();
            return Convert.ToBase64String(guid.ToByteArray());
        }
    }
}
