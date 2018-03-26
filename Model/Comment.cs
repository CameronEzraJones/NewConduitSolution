using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public String Body { get; set; }
        public Profile Author { get; set; }

        public Comment()
        {

        }

        public Comment(int id, DateTime createdAt, DateTime updatedAt, String body, Profile author)
        {
            Id = id;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Body = body;
            Author = author;
        }
    }
}
