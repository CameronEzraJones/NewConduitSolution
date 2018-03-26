using Conduit.Exceptions;
using Conduit.Model;
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
    public class CreateArticleValidatorAttribute : TypeFilterAttribute
    {
        public CreateArticleValidatorAttribute() : base(typeof(CreateArticleValidatorImpl)) { }

        private class CreateArticleValidatorImpl : ConduitValidatorUtils, IActionFilter
        {
            private readonly ILogger<CreateArticleValidatorAttribute> _logger;

            public CreateArticleValidatorImpl(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<CreateArticleValidatorAttribute>();
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                context.Exception = null;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                NewArticleHolder articleHolder = context.ActionArguments["newArticle"] as NewArticleHolder;
                if (null == articleHolder)
                {
                    InvalidateRequest(context, "No article holder is present", _logger, 422);
                    return;
                }
                NewArticle article = articleHolder.Article;
                if (null == article)
                {
                    InvalidateRequest(context, "There is no article in the holder", _logger, 422);
                    return;
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
