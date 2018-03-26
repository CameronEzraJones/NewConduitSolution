using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Model.DAO;

namespace Conduit.Repositories
{
    public abstract class ITagsRepository
    {
        internal abstract List<TagDAO> SaveTags(List<string> tagList);
        internal abstract Task<List<string>> GetTags();
        internal abstract Int32 GetTagId(String tag);
    }
}
