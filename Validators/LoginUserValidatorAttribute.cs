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
    public class LoginUserValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AuthUserHolder userHolder = context.ActionArguments["userHolder"] as AuthUserHolder;
            if (null == userHolder)
            {
                context.HttpContext.Response.StatusCode = 422;
                context.Result = new JsonResult(new ErrorResponseHolder("The Request content is invalid."));
                return;
            }
            AuthUser user = userHolder.User;
            if (null == user)
            {
                context.HttpContext.Response.StatusCode = 422;
                context.Result = new JsonResult(new ErrorResponseHolder("There is no user in the request."));
                return;
            }
        }
    }
}
