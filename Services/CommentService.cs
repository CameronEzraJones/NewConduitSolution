using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Model;
using Conduit.Model.DAO;
using Conduit.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Conduit.Services
{
    public class CommentService : ICommentService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IArticleRepository _articleRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IProfileService _profileService;

        public CommentService(UserManager<IdentityUser> userManager,
            IArticleRepository articleRepository,
            ICommentRepository commentRepository,
            IProfileService profileService)
        {
            _userManager = userManager;
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _profileService = profileService;
        }

        public override async Task<Comment> CreateComment(string username, string commentBody, string articleSlug)
        {
            DateTime now = DateTime.Now;
            int articleId = _articleRepository.GetArticleBySlug(articleSlug).Id;
            IdentityUser user = await _userManager.FindByNameAsync(username);
            string authorId = user.Id;
            CommentDAO commentDao = _commentRepository.CreateComment(commentBody, authorId, articleId);
            return await CreateCommentFromDAO(commentDao, username);
        }

        public override void DeleteComment(int commentId)
        {
            _commentRepository.DeleteComment(commentId);
        }

        public override async Task<List<Comment>> GetComments(string username, string articleSlug)
        {
            int articleId = _articleRepository.GetArticleBySlug(articleSlug).Id;
            List<CommentDAO> commentDaos = _commentRepository.GetCommentsForArticle(articleId);
            List<Comment> comments = new List<Comment>();
            foreach(CommentDAO commentDao in commentDaos)
            {
                comments.Add(await CreateCommentFromDAO(commentDao, username));
            }
            return comments;
        }

        private async Task<Comment> CreateCommentFromDAO (CommentDAO commentDao, string authedUsername = null)
        {
            IdentityUser author = await _userManager.FindByIdAsync(commentDao.AuthorId);
            return new Comment
            {
                Author = await _profileService.GetProfile(author.UserName, authedUsername),
                Body = commentDao.Body,
                Id = commentDao.Id,
                CreatedAt = commentDao.CreatedAt,
                UpdatedAt = commentDao.UpdatedAt
            };
        }
    }
}
