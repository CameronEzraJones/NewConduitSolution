using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Model.Holder.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Validators
{
    public class UpdateUserValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            UserUpdateDataHolder userHolder = context.ActionArguments["userHolder"] as UserUpdateDataHolder;
            if (null == userHolder)
            {
                context.HttpContext.Response.StatusCode = 422;
                context.Result = new JsonResult(new ErrorResponseHolder("The Request content is invalid."));
                return;
            }
            UserUpdateData user = userHolder.User;
            if (null == user)
            {
                context.HttpContext.Response.StatusCode = 422;
                context.Result = new JsonResult(new ErrorResponseHolder("There is no user information in the request."));
                return;
            }
            if (!context.ModelState.IsValid)
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
