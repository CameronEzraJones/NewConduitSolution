using Conduit.Context;
using Conduit.Model;
using Conduit.Model.DAO;
using Conduit.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ConduitDbContext _context;
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleTagsRepository _articleTagsRepository;
        private readonly IFavoriteArticleRepository _favoriteArticleRepository;
        private readonly IProfileService _profileService;
        private readonly ITagsRepository _tagsRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserIsFollowingRepository _userIsFollowingRepository;

        public ArticleService(
            ConduitDbContext context,
            IArticleRepository articleRepository,
            IArticleTagsRepository articleTagsRepository,
            IFavoriteArticleRepository favoriteArticleRepository,
            IProfileService profileService,
            ITagsRepository tagsRepository,
            UserManager<IdentityUser> userManager,
            IUserIsFollowingRepository userIsFollowingRepository)
        {
            _context = context;
            _articleRepository = articleRepository;
            _articleTagsRepository = articleTagsRepository;
            _favoriteArticleRepository = favoriteArticleRepository;
            _profileService = profileService;
            _tagsRepository = tagsRepository;
            _userManager = userManager;
            _userIsFollowingRepository = userIsFollowingRepository;
        }

        public override async Task<Article> CreateArticle(string authedUsername, NewArticle newArticle)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                IdentityUser author = await _userManager.FindByNameAsync(authedUsername);
                String authorId = author.Id;
                ArticleDAO createdArticle = _articleRepository.CreateArticle(
                    title: newArticle.Title,
                    description: newArticle.Description,
                    body: newArticle.Body,
                    authorId: authorId);
                _context.SaveChanges();
                List<TagDAO> tags = _tagsRepository.SaveTags(newArticle.TagList);
                _context.SaveChanges();
                _articleTagsRepository.SaveArticleTags(tags.Select(e => e.Id).ToList(), createdArticle.Id);
                _context.SaveChanges();
                Article article = await CreateArticleFromDAO(createdArticle, authedUsername);
                transaction.Commit();
                return article;
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        public override async Task DeleteArticle(string authedUsername, string slug)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                ArticleDAO article = await _articleRepository.GetArticleBySlug(slug);
                _articleRepository.DeleteArticle(article.Id);
                _context.SaveChanges();
                transaction.Commit();
                return;
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        public override async Task<Article> GetArticle(string slug, string authedUsername = null)
        {
            try
            {
                ArticleDAO articleDao = await _articleRepository.GetArticleBySlug(slug);
                Article article = await CreateArticleFromDAO(articleDao, authedUsername);
                return article;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public override async Task<List<Article>> GetArticles(Dictionary<string, string> query, string authedUsername = null)
        {
            List<int> articleIds = _articleRepository.GetAllArticleIds();
            if(null != query.GetValueOrDefault("tag"))
            {
                Int32 tagId = _tagsRepository.GetTagId(query.GetValueOrDefault("tag"));
                articleIds = articleIds.Intersect(_articleTagsRepository.GetArticlesWithTag(tagId)).ToList();
            }
            if(null != query.GetValueOrDefault("author"))
            {
                IdentityUser author = await _userManager.FindByNameAsync(query.GetValueOrDefault("author"));
                articleIds = articleIds.Intersect(_articleRepository.GetArticlesByAuthorId(author?.Id).Select(e => e.Id).ToList()).ToList();
            }
            if(null != query.GetValueOrDefault("favorited"))
            {
                IdentityUser favoritedUser = await _userManager.FindByNameAsync(query.GetValueOrDefault("favorited"));
                articleIds = articleIds.Intersect(_favoriteArticleRepository.GetFavoriteArticlesForUser(favoritedUser?.Id)).ToList();
            }
            List<ArticleDAO> articleDaos = _articleRepository.GetArticlesByArticleIds(articleIds);
            if(null != query.GetValueOrDefault("offset"))
            {
                articleDaos = (List<ArticleDAO>) articleDaos.Skip(Convert.ToInt32(query.GetValueOrDefault("offset")));
            }
            if (null != query.GetValueOrDefault("limit"))
            {
                articleDaos = (List<ArticleDAO>)articleDaos.Take(Convert.ToInt32(query.GetValueOrDefault("limit")));
            }
            List<Article> articles = new List<Article>();
            foreach(ArticleDAO articleDao in articleDaos)
            {
                articles.Add(await CreateArticleFromDAO(articleDao, authedUsername));
            }
            return articles;
        }

        public override async Task<List<Article>> GetFeedArticles(Dictionary<string, string> query, string authedUsername)
        {
            try
            {
                IdentityUser user = await _userManager.FindByNameAsync(authedUsername);
                List<String> followeeIds = _userIsFollowingRepository.GetListOfFollowees(user.Id);
                List<ArticleDAO> articleDaos = _articleRepository.GetArticlesByAuthorIds(followeeIds);
                if (null != query.GetValueOrDefault("offset"))
                {
                    articleDaos = (List<ArticleDAO>)articleDaos.Skip(Convert.ToInt32(query.GetValueOrDefault("offset")));
                }
                if (null != query.GetValueOrDefault("limit"))
                {
                    articleDaos = (List<ArticleDAO>)articleDaos.Take(Convert.ToInt32(query.GetValueOrDefault("limit")));
                }
                List<Article> articles = new List<Article>();
                foreach (ArticleDAO articleDao in articleDaos)
                {
                    articles.Add(await CreateArticleFromDAO(articleDao, authedUsername));
                }
                return articles;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public override Task<List<string>> GetTags()
        {
            try
            {
                return _tagsRepository.GetTags();
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public override async Task<Article> UpdateArticle(string authedUsername, String articleSlug, UpdateArticle updateArticle)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                ArticleDAO articleDao = await _articleRepository.GetArticleBySlug(articleSlug);
                _articleRepository.UpdateArticle(articleDao.Id, updateArticle);
                _context.SaveChanges();
                Article article = await CreateArticleFromDAO(articleDao, authedUsername);
                transaction.Commit();
                return article;
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        public override async Task<Article> FavoriteArticle(string authedUsername, string slug)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                IdentityUser user = await _userManager.FindByNameAsync(authedUsername);
                ArticleDAO articleDao = await _articleRepository.GetArticleBySlug(slug);
                FavoriteArticleDAO favoriteArticle = new FavoriteArticleDAO
                {
                    ArticleId = articleDao.Id,
                    UserId = user.Id
                };
                _favoriteArticleRepository.FavoriteArticle(favoriteArticle);
                _context.SaveChanges();
                articleDao = await _articleRepository.GetArticleBySlug(slug);
                transaction.Commit();
                Article article = await CreateArticleFromDAO(articleDao, authedUsername);
                return article;
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        public override async Task<Article> UnfavoriteArticle(string authedUsername, string slug)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                IdentityUser user = await _userManager.FindByNameAsync(authedUsername);
                ArticleDAO articleDao = await _articleRepository.GetArticleBySlug(slug);
                _favoriteArticleRepository.UnfavoriteArticle(articleDao.Id, user.Id);
                _context.SaveChanges();
                articleDao = await _articleRepository.GetArticleBySlug(slug);
                transaction.Commit();
                Article article = await CreateArticleFromDAO(articleDao, authedUsername);
                return article;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        //Private Methods

        private List<string> GetTagListForArticle(int id)
        {
            return _articleTagsRepository.GetTagListForArticle(id);
        }

        private bool GetFavoritedStatus(int id, string userId)
        {
            return _favoriteArticleRepository.IsArticleFavorited(id, userId);
        }

        private int GetFavoritesCountForArticle(int id)
        {
            return _favoriteArticleRepository.GetFavoritesCountForArticle(id);
        }

        private async Task<Article> CreateArticleFromDAO(ArticleDAO articleDAO, String authedUsername)
        {
            try
            {
                IdentityUser user = null;
                if(null != authedUsername)
                {
                    user = await _userManager.FindByNameAsync(authedUsername);
                }
                IdentityUser author = await _userManager.FindByIdAsync(articleDAO.AuthorId);
                return new Article
                {
                    Author = await _profileService.GetProfile(author.UserName, authedUsername),
                    Body = articleDAO.Body,
                    CreatedAt = articleDAO.CreatedAt,
                    Description = articleDAO.Description,
                    Favorited = _favoriteArticleRepository.IsArticleFavorited(articleDAO.Id, user?.Id),
                    FavoritesCount = _favoriteArticleRepository.GetFavoritesCountForArticle(articleDAO.Id),
                    Slug = articleDAO.Slug,
                    TagList = _articleTagsRepository.GetTagListForArticle(articleDAO.Id),
                    Title = articleDAO.Title,
                    UpdatedAt = articleDAO.UpdatedAt
                };
            } catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
