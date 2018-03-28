using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Context;
using Conduit.Model;
using Conduit.Model.Holder;
using Conduit.Services;
using Conduit.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Conduit.Controllers
{
    [Produces("application/json")]
    public class ArticlesController : IConduitController
    {
        private readonly ILogger _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConduitDbContext _context;
        private readonly IProfileService _profileService;
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;

        public ArticlesController(
            ILoggerFactory loggerFactory,
            UserManager<IdentityUser> userManager,
            ConduitDbContext context,
            IProfileService profileService,
            IArticleService articleService,
            ICommentService commentService)
        {
            _logger = loggerFactory.CreateLogger<ArticlesController>();
            _userManager = userManager;
            _context = context;
            _profileService = profileService;
            _articleService = articleService;
            _commentService = commentService;
        }

        [HttpPost("/api/articles"), Authorize(Policy = "ValidUsername")]
        [CreateArticleValidator]
        public async Task<IActionResult> CreateArticle([FromBody] NewArticleHolder newArticle)
        {
            try
            {
                string username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                Article article = await _articleService.CreateArticle(username, newArticle.Article);
                this.HttpContext.Response.StatusCode = 201;
                return Json(new SingleArticleHolder(article));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpGet("/api/articles")]
        [GetArticlesValidator]
        public async Task<IActionResult> GetArticles()
        {
            try
            {
                string username;
                try
                {
                    username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                } catch
                {
                    username = null;
                }
                var query = this.HttpContext.Request.Query.ToDictionary(t => t.Key, t => t.Value.ToString());
                List<Article> articles = await _articleService.GetArticles(query, username);
                return Ok(new MultipleArticleHolder(articles));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpGet("/api/articles/feed"), Authorize(Policy = "ValidUsername")]
        [GetFeedArticlesValidator]
        public async Task<IActionResult> GetFeedArticles()
        {
            try
            {
                string username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                var query = this.HttpContext.Request.Query.ToDictionary(t => t.Key, t => t.Value.ToString());
                List<Article> articles = await _articleService.GetFeedArticles(query, username);
                return Ok(new MultipleArticleHolder(articles));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpGet("/api/articles/{slug}")]
        public async Task<IActionResult> GetArticleBySlug(string slug)
        {
            try
            {
                string username;
                try
                {
                    username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                }
                catch
                {
                    username = null;
                }
                Article article = await _articleService.GetArticle(slug, username);
                return Ok(new SingleArticleHolder(article));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpPut("/api/articles/{slug}"), Authorize(Policy = "ValidUsername")]
        [UpdateArticleValidator]
        public async Task<IActionResult> UpdateArticle(string slug, [FromBody] UpdateArticleHolder updateArticleHolder)
        {
            try
            {
                string username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                Article article = await _articleService.UpdateArticle(username, slug, updateArticleHolder.Article);
                return Ok(new SingleArticleHolder(article));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpDelete("/api/articles/{slug}"), Authorize(Policy = "ValidUsername")]
        [DeleteArticleValidator]
        public async Task<IActionResult> DeleteArticle(string slug)
        {
            try
            {
                string username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                await _articleService.DeleteArticle(username, slug);
                return Ok();
            } catch(Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpPost("/api/articles/{slug}/comments"), Authorize(Policy = "ValidUsername")]
        [CreateCommentValidator]
        public async Task<IActionResult> CreateComment(string slug, [FromBody] NewCommentHolder newCommentHolder)
        {
            try
            {
                string username  = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                Comment comment = await _commentService.CreateComment(username, newCommentHolder.Comment.Body, slug);
                this.HttpContext.Response.StatusCode = 201;
                return Json(new SingleCommentHolder(comment));
            } catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpGet("/api/articles/{slug}/comments")]
        [GetCommentsValidator]
        public async Task<IActionResult> GetComments(string slug)
        {
            try
            {
                string username;
                try
                {
                    username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                }
                catch
                {
                    username = null;
                }
                List<Comment> comments = await _commentService.GetComments(username, slug);
                return Ok(new MultipleCommentsHolder(comments));
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpDelete("/api/articles/{slug}/comments/{id}"), Authorize(Policy = "ValidUsername")]
        [DeleteCommentValidator]
        public IActionResult DeleteComment(string slug, int id)
        {
            try
            {
                _commentService.DeleteComment(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }

        [HttpGet("/api/tags")]
        public async Task<IActionResult> GetTags()
        {
            try
            {
                List<string> tags = await _articleService.GetTags();
                return Ok(new TagsHolder(tags));
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger);
            }
        }
    }
}