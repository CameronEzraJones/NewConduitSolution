using Conduit.Model;
using Conduit.Model.Holder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Conduit.Validators
{
    public class UpdateUserValidatorAttribute : TypeFilterAttribute
    {
        public UpdateUserValidatorAttribute():base(typeof(UpdateUserValidatorImpl)) { }

        private class UpdateUserValidatorImpl : ConduitValidatorUtils, IActionFilter
        {
            private readonly ILogger<UpdateUserValidatorAttribute> _logger;

            public UpdateUserValidatorImpl(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<UpdateUserValidatorAttribute>();
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var jwtUser = context.HttpContext.User;
                if(!jwtUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Email))
                {
                    InvalidateRequest(context, "Invalid token for request", _logger, 401);
                }
                UserUpdateDataHolder userHolder = context.ActionArguments["userHolder"] as UserUpdateDataHolder;
                if (null == userHolder)
                {
                    InvalidateRequest(context, "No user holder is present.", _logger, 422);
                }
                UserUpdateData user = userHolder.User;
                if (null == user)
                {
                    InvalidateRequest(context, "No user is present in the user holder.", _logger, 422);
                }
                if (!context.ModelState.IsValid)
                {
                    List<string> errors = context
                        .ModelState
                        .Values
                        .SelectMany(e => e.Errors)
                        .Select(e => e.ErrorMessage)
                        .Distinct()
                        .Cast<string>()
                        .ToList();
                    InvalidateRequest(context, errors, _logger, 422);
                }
            }
        }
    }
}
