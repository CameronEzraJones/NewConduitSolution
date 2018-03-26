using Conduit.Context;
using Conduit.Model.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public class FavoriteArticleRepository : IFavoriteArticleRepository
    {
        private readonly DbSet<FavoriteArticleDAO> _favoriteArticle;

        public FavoriteArticleRepository(ConduitDbContext context)
        {
            _favoriteArticle = context.FavoriteArticle;
        }

        internal override void FavoriteArticle(FavoriteArticleDAO favoriteArticle)
        {
            _favoriteArticle.Add(favoriteArticle);
        }

        internal override FavoriteArticleDAO GetFavoriteArticleDAO(int id, string userId)
        {
            return _favoriteArticle.Where(e => e.ArticleId == id && e.UserId == userId).Single();
        }

        internal override List<int> GetFavoriteArticlesForUser(string userId)
        {
            return _favoriteArticle.Where(e => e.UserId == userId).Select(e => e.ArticleId).ToList();
        }

        internal override int GetFavoritesCountForArticle(int id)
        {
            return _favoriteArticle.Where(e => e.ArticleId == id).ToList<FavoriteArticleDAO>().Count;
        }

        internal override Boolean IsArticleFavorited(int id, string userId)
        {
            try
            {
                _favoriteArticle.Where(e => e.UserId == userId && e.ArticleId == id).Single();
                return true;
            } catch
            {
                return false;
            }
        }

        internal override void UnfavoriteArticle(int id, string userId)
        {
            FavoriteArticleDAO favoriteArticle = _favoriteArticle.Where(e => e.UserId == userId && e.ArticleId == id).Single();
            _favoriteArticle.Remove(favoriteArticle);
        }
    }
}
