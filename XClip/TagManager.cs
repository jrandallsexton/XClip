

using System;
using System.Collections.Generic;

using XClip.Repositories;

namespace XClip
{
    public class TagManager
    {

        private TagRepository _tagRepository = new TagRepository();

        public Dictionary<int, string> GetTags()
        {
            return _tagRepository.Tags();
        }

        public IEnumerable<string> GetBlacklist()
        {
            return _tagRepository.GetBlacklist();
        } 

        public int Save(string tagText)
        {
            return _tagRepository.Save(tagText);
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