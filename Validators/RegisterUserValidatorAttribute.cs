using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Model.Holder.Error;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Conduit.Validators
{
    public class RegisterUserValidatorAttribute : TypeFilterAttribute
    {
        public RegisterUserValidatorAttribute():base(typeof(RegisterUserValidatorImpl)) { }

        private class RegisterUserValidatorImpl : ConduitValidatorUtils, IActionFilter
        {
            private readonly ILogger<RegisterUserValidatorAttribute> _logger;
            private readonly UserManager<IdentityUser> _userManager;

            public RegisterUserValidatorImpl(ILoggerFactory loggerFactory, UserManager<IdentityUser> userManager)
            {
                _logger = loggerFactory.CreateLogger<RegisterUserValidatorAttribute>();
                _userManager = userManager;
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                AuthUserHolder userHolder = context.ActionArguments["userHolder"] as AuthUserHolder;
                if (null == userHolder)
                {
                    InvalidateRequest(context, "No user holder is present.", _logger, 422);
                }
                AuthUser user = userHolder.User;
                if (null == user)
                {
                    InvalidateRequest(context, "No user is present in the user holder.", _logger, 422);
                }
                if (null == user.Username)
                {
                    InvalidateRequest(context, "No username is present in the user.", _logger, 422);
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
