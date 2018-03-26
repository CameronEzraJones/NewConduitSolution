using Conduit.Model.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public abstract class IFavoriteArticleRepository
    {
        internal abstract Boolean IsArticleFavorited(int id, string userId);
        internal abstract int GetFavoritesCountForArticle(int id);
        internal abstract FavoriteArticleDAO GetFavoriteArticleDAO(int id, string userId);
        internal abstract List<int> GetFavoriteArticlesForUser(string userId);
        internal abstract void FavoriteArticle(FavoriteArticleDAO favoriteArticle);
        internal abstract void UnfavoriteArticle(int id, string userId);
    }
}
