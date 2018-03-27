using Conduit.Controllers;
using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Validators
{
    public class DeleteArticleValidatorAttribute : TypeFilterAttribute
    {
        public DeleteArticleValidatorAttribute() : base(typeof(DeleteArticleValidatorImpl)) { }

        private class DeleteArticleValidatorImpl : ConduitValidatorUtils, IAsyncActionFilter
        {
            private readonly ILogger<DeleteArticleValidatorImpl> _logger;
            private readonly IArticleService _articleService;

            public DeleteArticleValidatorImpl(ILoggerFactory loggerFactory, IArticleService articleService)
            {
                _logger = loggerFactory.CreateLogger<DeleteArticleValidatorImpl>();
                _articleService = articleService;
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                throw new NotImplementedException();
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var jwtUser = context.HttpContext.User;
                if (!jwtUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub))
                {
                    InvalidateRequest(context, "Invalid token for request", _logger, 401);
                    await next();
                }
                String username = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                String slug = context.ActionArguments["slug"] as String;
                Article article = await _articleService.GetArticle(username, slug);
                if(article.Author.Username != username)
                {
                    InvalidateRequest(context, "You are not authorized to delete this article", _logger, 403);
                    await next();
                }
                await next();
            }
        }
    }
}
