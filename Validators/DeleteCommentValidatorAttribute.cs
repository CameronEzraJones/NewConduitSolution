using Conduit.Model;
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
    public class DeleteCommentValidatorAttribute : TypeFilterAttribute
    {
        public DeleteCommentValidatorAttribute() : base(typeof(DeleteCommentValidatorImpl)) { }

        private class DeleteCommentValidatorImpl : ConduitValidatorUtils, IAsyncActionFilter
        {
            private readonly ILogger<DeleteCommentValidatorImpl> _logger;
            private readonly IArticleService _articleService;
            private readonly ICommentService _commentService;

            public DeleteCommentValidatorImpl(ILoggerFactory loggerFactory,
                IArticleService articleService,
                ICommentService commentService)
            {
                _logger = loggerFactory.CreateLogger<DeleteCommentValidatorImpl>();
                _articleService = articleService;
                _commentService = commentService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var jwtUser = context.HttpContext.User;
                if (!jwtUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub))
                {
                    InvalidateRequest(context, "Invalid token for request", _logger, 401);
                    return;
                }
                String username = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                string slug = context.ActionArguments["slug"] as string;
                int? id = Convert.ToInt32(context.RouteData.Values["id"]);
                if(null == id)
                {
                    InvalidateRequest(context, "id must be a number", _logger, 422);
                    return;
                }
                Article article = await _articleService.GetArticle(slug, null);
                if (null == article || null == article.Slug)
                {
                    InvalidateRequest(context, $"No article with the given slug {slug}", _logger, 404);
                    return;
                }
                List<Comment> comments = await _commentService.GetComments(null, slug);
                Comment comment = null;
                try
                {
                    comment = comments.Where(e => e.Id == id).Single();
                } catch (Exception)
                {
                    InvalidateRequest(context, $"No comment with id {id} in article with slug {slug}", _logger, 404);
                    return;
                }
                if(comment.Author.Username != username)
                {
                    InvalidateRequest(context, $"You are not authorized to delete this comment", _logger, 404);
                    return;
                }
                await next();
            }
        }
    }
}
