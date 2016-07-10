
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using XClip.Config;
using XClip.DataObjects;
using XClip.Repositories;

namespace XClip
{
    public class CollectionManager
    {
        private readonly ICollectionRepository _collectionRepository = new CollectionRepository();
        private readonly ISourceManager _sourceManager = new SourceManager();
        private readonly ITagManager _tagManager = new TagManager();

        public int Create(int userId, string name)
        {
            if (string.IsNullOrEmpty((name)))
                throw new ArgumentException("Collection Name cannot be empty");

            if (userId < 1)
                throw new ArgumentException("UserId must be specified");

            return _collectionRepository.Create(userId, name);
        }

        public void Delete(int id, int userId)
        {
            if (id < 1)
                throw new ArgumentException("CollectionId must be specified");
            if (userId < 1)
                throw new ArgumentException("UserId must be specified");

            _collectionRepository.Delete(id, userId);
        }

        public void AddToCollection(int collectionId, XSource source, bool moveFiles, int userId)
        {
            var id = _sourceManager.Save(collectionId, source);

            // auto-generate some tags based on the filename (if any words are found within the title)
            this.AutoTag(id, source.Filename, userId, collectionId);

            if (!moveFiles)
                return;
            // now that the file has been saved
            // move the physical file into the folder for the collection

            // create the target folder if it does not exist
            var config = XClipConfig.GetSection();

            var target = Path.Combine(config.DirectoryRoot, "Collections");
            target = Path.Combine(target, collectionId.ToString());

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);

            var srcPath = Path.Combine(config.DirectoryIn, $"{source.Filename}{source.FileExt}");
            var tgtPath = Path.Combine(target, $"{source.UId}{source.FileExt}");

            File.Move(srcPath, tgtPath);
        }

        private void AutoTag(int fileId, string fileName, int userId, int collectionId)
        {
            var tags = _tagManager.GetTags(userId, collectionId);
            var existingTags = tags.Values.ToList();

            // http://stackoverflow.com/questions/2159026/regex-how-to-get-words-from-a-string-c
            var matches = Regex.Matches(fileName, @"\w(?<!\d)[\w'-]*");
            var blackList = _tagManager.GetBlacklist();

            var newTags = new List<string>();

            var fileTags = new List<string>();

            foreach (var m in matches)
            {
                var word = m.ToString();

                if (word.Length == 1 || word.Length > 15)
                    continue;

                if (blackList.Contains(word.ToLower()))
                    continue;

                if (existingTags.Contains(word.ToLower()))
                {
                    fileTags.Add(word.ToLower());
                    continue;
                }

                if (m.ToString().Length > 15)
                    continue;
                
                newTags.Add(m.ToString().ToLower());
            }

            // try to find proper names (for "stars")
            var talent = Regex.Matches(fileName, @"^[a-z -']+$");

            foreach (var t in talent)
            {
                Console.WriteLine("talent =>" + t);
            }

            // save the new tags
            newTags.ForEach(x =>
            {
                _tagManager.Save(userId, collectionId, x);
            });

            // associate the new file with the tags
            var tagDictionary = _tagManager.GetTags(userId, collectionId);

            foreach (var kvp in tagDictionary)
            {
                if (fileTags.Contains(kvp.Value))
                {
                    _tagManager.AddTag(fileId, kvp.Key);
                    continue;
                }

                if (newTags.Contains(kvp.Value))
                {
                    _tagManager.AddTag(fileId, kvp.Key);
                    continue;
                }
            }

        }
    }
}