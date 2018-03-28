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
    public class UpdateArticleValidatorAttribute : TypeFilterAttribute
    {
        public UpdateArticleValidatorAttribute() : base(typeof(UpdateArticleValidatorImpl)) { }

        private class UpdateArticleValidatorImpl : ConduitValidatorUtils, IAsyncActionFilter
        {
            private readonly ILogger<UpdateArticleValidatorImpl> _logger;
            private readonly IArticleService _articleService;

            public UpdateArticleValidatorImpl(ILoggerFactory loggerFactory, IArticleService articleService)
            {
                _logger = loggerFactory.CreateLogger<UpdateArticleValidatorImpl>();
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
                }
                String username = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                String slug = context.ActionArguments["slug"] as String;
                Article article = await _articleService.GetArticle(slug, username);
                if(article.Author.Username != username)
                {
                    InvalidateRequest(context, "You are not authorized to edit this article", _logger, 403);
                    return;
                }
                UpdateArticleHolder updateArticleHolder = context.ActionArguments["updateArticle"] as UpdateArticleHolder;
                UpdateArticle updateArticle = updateArticleHolder.Article;
                if(null == updateArticle.Title && null == updateArticle.Description && null == updateArticle.Body)
                {
                    InvalidateRequest(context, "Nothing to update", _logger, 422);
                    return;
                }
                return;
            }
        }
    }
}
