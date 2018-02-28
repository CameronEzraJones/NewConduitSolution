using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Model.Holder.Error;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Conduit.Validators
{
    public class RegisterUserValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AuthUserHolder userHolder = context.ActionArguments["userHolder"] as AuthUserHolder;
            if(null == userHolder)
            {
                context.HttpContext.Response.StatusCode = 422;
                context.Result = new JsonResult(new ErrorResponseHolder("The Request content is invalid."));
                return;
            }
            AuthUser user = userHolder.User;
            if(null == user)
            {
                context.HttpContext.Response.StatusCode = 422;
                context.Result = new JsonResult(new ErrorResponseHolder("There is no user in the request."));
                return;
            }
            if(null == user.Username)
            {
                context.HttpContext.Response.StatusCode = 422;
                context.Result = new JsonResult(new ErrorResponseHolder("The Username field is required."));
            }
            if(!context.ModelState.IsValid)
            {
                context.HttpContext.Response.StatusCode = 422;
                List<string> errors = context
                    .ModelState
                    .Values
                    .SelectMany(e => e.Errors)
                    .Select(e => e.ErrorMessage)
                    .Distinct()
                    .Cast<string>()
                    .ToList();
                ErrorResponseHolder error = new ErrorResponseHolder(errors);
                context.Result = new JsonResult(error);
            }
        }
    }
}
