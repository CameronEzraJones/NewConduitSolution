﻿using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Model.Holder.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Validators
{
    public class LoginUserValidatorAttribute : TypeFilterAttribute
    {
        public LoginUserValidatorAttribute():base(typeof(LoginUserValidatorImpl)) { }

        private class LoginUserValidatorImpl : ConduitValidatorUtils, IActionFilter
        {
            private readonly ILogger<LoginUserValidatorAttribute> _logger;

            public LoginUserValidatorImpl(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<LoginUserValidatorAttribute>();
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
                    InvalidateRequest(context, "There is no user present in the user holder.", _logger, 422);
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
