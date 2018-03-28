using Conduit.Model;
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
    public class GetCommentsValidatorAttribute : TypeFilterAttribute
    {
        public GetCommentsValidatorAttribute() : base(typeof(GetCommentsValidatorImpl)) { }

        private class GetCommentsValidatorImpl : ConduitValidatorUtils, IAsyncActionFilter
        {
            private readonly ILogger<GetCommentsValidatorImpl> _logger;
            private readonly IArticleService _articleService;

            public GetCommentsValidatorImpl(ILoggerFactory loggerFactory,
                IArticleService articleService)
            {
                _logger = loggerFactory.CreateLogger<GetCommentsValidatorImpl>();
                _articleService = articleService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                string slug = context.ActionArguments["slug"] as string;
                Article article = await _articleService.GetArticle(slug, null);
                if(null == article || null == article.Slug)
                {
                    InvalidateRequest(context, $"No article with the given slug {slug}", _logger, 404);
                    await next();
                }
                await next();
            }
        }
    }
}
