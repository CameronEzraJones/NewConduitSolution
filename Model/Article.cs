using System;
using System.Collections.Generic;

namespace Conduit.Model
{
    public class Article
    {
        public String Slug { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Body { get; set; }
        public List<String> TagList { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Boolean Favorited { get; set; }
        public int FavoritesCount { get; set; }
        public Profile Author { get; set; }

        public Article()
        {

        }

        public Article(
            String slug,
            String title,
            String description,
            String body,
            List<String> tagList,
            DateTime createdAt,
            DateTime updatedAt,
            Boolean favorited,
            int favoritesCount,
            Profile author)
        {
            Slug = slug;
            Title = title;
            Description = description;
            Body = body;
            TagList = tagList;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Favorited = favorited;
            FavoritesCount = favoritesCount;
            Author = author;
        }
    }
}
