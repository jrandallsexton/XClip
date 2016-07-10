

using System;
using System.Collections.Generic;

using XClip.Repositories;

namespace XClip
{
    public interface ITagManager
    {
        Dictionary<int, string> GetTags(int userId, int? collectionId);
        IEnumerable<string> GetBlacklist();
        int Save(int userId, int? collectionId, string tagText);
        void AddTag(int sourceId, int tagId);
        Dictionary<int, string> GetTags(Guid mediaId);
    }

    public class TagManager : ITagManager
    {

        private readonly ITagRepository _tagRepository = new TagRepository();

        public Dictionary<int, string> GetTags(int userId, int? collectionId)
        {
            return _tagRepository.Tags(userId, collectionId);
        }

        public IEnumerable<string> GetBlacklist()
        {
            return _tagRepository.GetBlacklist();
        } 

        public int Save(int userId, int? collectionId, string tagText)
        {
            return _tagRepository.Save(userId, collectionId, tagText);
        }

        public void AddTag(int sourceId, int tagId)
        {
            if (_tagRepository.TagExists(sourceId, tagId))
                return;
            _tagRepository.AddTag(sourceId, tagId);
        }

        public Dictionary<int, string> GetTags(Guid mediaId)
        {
            return _tagRepository.GetTags(mediaId);
        }
    }
}