
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using XClip.Config;
using XClip.DataObjects;

namespace XClip.Utils
{
    public class XClipUtils
    {

        public void GetFileStats()
        {

            var config = XClipConfig.GetSection();
            var allowedExtensions = config.AllowedExtensions.Split(',').ToList();
            var di = new DirectoryInfo(config.DirectoryIn);
            var counts = new SortedDictionary<string, int>();

            foreach (var fi in di.GetFiles("*.*", SearchOption.AllDirectories))
            {

                if (!allowedExtensions.Contains(fi.Extension))
                    continue;

                var tmp = fi.Extension.ToLower().Trim();

                if (!counts.ContainsKey(tmp)) { counts.Add(tmp, 0); }

                counts[tmp] += 1;

            }

            foreach (var kvp in counts)
            {
                Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            }

        }

        public void LookForDuplicateFiles()
        {
            var config = XClipConfig.GetSection();
            var di = new DirectoryInfo(config.DirectoryIn);
            var count = 0;
            foreach (var fi in di.GetFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                if (fi.Extension == string.Empty)
                    continue;

                if (!config.AllowedExtensions.Contains(fi.Extension))
                    continue;

                if (!fi.Name.Contains("(") || !fi.Name.Contains(")")) continue;

                //Console.WriteLine(fi.Name);

                // get the filename without the parens
                var left = fi.Name.IndexOf("(", StringComparison.Ordinal);
                var right = fi.Name.IndexOf(")", StringComparison.Ordinal);

                var scrubbed = fi.Name.Substring(0, left);
                scrubbed += fi.Name.Substring(right + 1, fi.Name.Length - right - 1);

                var orig = $"{config.DirectoryIn}\\{scrubbed}";
                var exists = File.Exists(orig);

                if (!exists) continue;

                var origFi = new FileInfo(orig);
                if (origFi.Length != fi.Length) continue;

                count++;

                Console.WriteLine($@"{count}. {fi.Name} => {scrubbed} ({origFi.Length})");

                var src = Path.Combine(config.DirectoryIn, fi.Name);

                if (File.Exists($"H:\\Downloads\\Dups\\{fi.Name}"))
                {
                    var unique = Guid.NewGuid().ToString().Substring(0, 5);
                    var tgt = $"H:\\Downloads\\Dups\\{unique}_{fi.Name}";
                    File.Move(src, tgt);
                }
                else
                {
                    var tgt = $"H:\\Downloads\\Dups\\{fi.Name}";
                    File.Move(src, tgt);
                }
            }
        }

        public void LoadTagsFromExternalSource()
        {
            const int userId = 1;
            const int collectionId = 1;

            var lines = File.ReadAllLines(@"f:\tags.txt");

            var tm = new TagManager();

            foreach (var line in lines.Where(line => !string.IsNullOrEmpty(line)))
            {
                tm.Save(userId, collectionId, line.Trim());
            }
        }

        public void GenerateTags()
        {
            var userId = 1;
            var collectionId = 1;

            var tagMgr = new TagManager();

            var tags = tagMgr.GetTags(userId, collectionId);
            var tagValues = tags.Values.ToList();
            var added = new List<string>();

            var blackList = tagMgr.GetBlacklist();

            //Console.WriteLine("{0} tagcount", tagValues.Count);
            for (var i = 0; i < 2005; i++)
            {
                var fileName = new BatchManager().RandomSource(1).Filename;

                var matches = Regex.Matches(fileName, @"\w(?<!\d)[\w'-]*");

                //Console.WriteLine(fileName);

                foreach (var m in matches)
                {
                    var word = m.ToString();

                    if (word.Length == 1 || word.Length > 10)
                        continue;

                    if (blackList.Contains(word.ToLower()))
                    {
                        //Console.WriteLine("\tIgnored: {0}", m);
                        continue;
                    }

                    // does this word already exist as a tag?
                    if (tagValues.Contains(word.ToLower()))
                    {
                        //Console.WriteLine("\tMatched: {0}", word);
                        continue;
                    }

                    tagValues.Add(m.ToString().ToLower());
                    added.Add(m.ToString().ToLower());
                    Console.WriteLine("\tSuggested: {0}", m.ToString().ToLower());
                }
            }

            added.Sort();

            added.ForEach(x => Console.WriteLine(x));
            //Console.WriteLine("{0} tagcount", tagValues.Count);
        }

        public void LoadDroneFootage()
        {
            var config = XClipConfig.GetSection();
            const string dirIn = @"F:\Dropbox\Drones\Clients\House";

            var cm = new CollectionManager();

            const int userId = 1;
            const int collectionId = 2;

            foreach (var tmp in new System.IO.DirectoryInfo(dirIn).EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                if (tmp.Extension == string.Empty)
                    continue;

                if (!config.AllowedExtensions.Contains(tmp.Extension.ToLower()))
                    continue;

                var xSource = new XSource()
                {
                    FileDate = tmp.CreationTimeUtc,
                    FileExt = tmp.Extension,
                    FileSize = tmp.Length,
                    Filename = tmp.Name,
                    UId = Guid.NewGuid()
                };

                cm.AddToCollection(collectionId, xSource, false, userId);

                var src = Path.Combine(tmp.DirectoryName, tmp.Name);
                var tgt = $"H:\\XClip\\Collections\\{collectionId}\\{xSource.UId}{tmp.Extension}";

                Console.WriteLine($"Copy from {src} to {tgt}");
                File.Copy(src, tgt);
            }
        }
    }
}