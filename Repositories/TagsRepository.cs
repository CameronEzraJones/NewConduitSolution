using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Context;
using Conduit.Model.DAO;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Repositories
{
    public class TagsRepository : ITagsRepository
    {
        private readonly DbSet<TagDAO> _tag;

        public TagsRepository(ConduitDbContext context)
        {
            _tag = context.Tag;
        }

        internal override Int32 GetTagId(string tag)
        {
            return _tag.Where(e => e.Tag == tag).Select(e => e.Id).SingleOrDefault();
        }

        internal override async Task<List<string>> GetTags()
        {
            return await _tag.Select(e => e.Tag).ToListAsync();
        }

        internal override List<TagDAO> SaveTags(List<string> tagList)
        {
            List<TagDAO> tags = new List<TagDAO>();
            foreach(String tag in tagList)
            {
                TagDAO tagDao = _tag.Where(e => e.Tag == tag).Single();
                if(tagDao == null)
                {
                    tagDao = new TagDAO
                    {
                        Tag = tag
                    };
                    _tag.Add(tagDao);
                }
                tags.Add(tagDao);
            }
            return tags;
        }
    }
}
