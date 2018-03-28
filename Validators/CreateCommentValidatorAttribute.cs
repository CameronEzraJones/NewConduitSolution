using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Validators
{
    public class CreateCommentValidatorAttribute : TypeFilterAttribute
    {
        public CreateCommentValidatorAttribute() : base(typeof(CreateCommentValidatorImpl)) { }

        private class CreateCommentValidatorImpl : ConduitValidatorUtils, IAsyncActionFilter
        {
            private readonly ILogger<CreateCommentValidatorImpl> _logger;
            private readonly IArticleService _articleService;

            public CreateCommentValidatorImpl(ILoggerFactory loggerFactory,
                IArticleService articleService)
            {
                _logger = loggerFactory.CreateLogger<CreateCommentValidatorImpl>();
                _articleService = articleService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                string slug = context.ActionArguments["slug"] as string;
                Article article = await _articleService.GetArticle(slug, null);
                if(null == article || null == article.Slug)
                {
                    InvalidateRequest(context, $"No article with the specified slug {slug}", _logger, 404);
                    await next();
                }
                NewCommentHolder newCommentHolder = context.ActionArguments["newCommentHolder"] as NewCommentHolder;
                if(null == newCommentHolder)
                {
                    InvalidateRequest(context, "No comment holder is present", _logger, 404);
                    await next();
                }
                NewComment newComment = newCommentHolder.Comment;
                if(null == newComment)
                {
                    InvalidateRequest(context, "No comment in the holder", _logger, 404);
                    await next();
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
                    await next();
                }
                await next();
            }
        }
    }
}
