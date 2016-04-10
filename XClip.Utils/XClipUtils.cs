
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using XClip.Config;

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

                if (fi.Name.Contains("(") && fi.Name.Contains(")"))
                {
                    //Console.WriteLine(fi.Name);

                    // get the filename without the parens
                    var left = fi.Name.IndexOf("(");
                    var right = fi.Name.IndexOf(")");

                    var scrubbed = fi.Name.Substring(0, left);
                    scrubbed += fi.Name.Substring(right + 1, fi.Name.Length - right - 1);

                    var orig = string.Format("{0}\\{1}", config.DirectoryIn, scrubbed);
                    var exists = File.Exists(orig);

                    if (exists)
                    {
                        var origFi = new FileInfo(orig);
                        if (origFi.Length == fi.Length)
                        {
                            count++;
                            Console.WriteLine("{0}. {1} => {2} ({3})", count, fi.Name, scrubbed, origFi.Length);
                            var src = Path.Combine(config.DirectoryIn, fi.Name);
                            var tgt = string.Format("H:\\Downloads\\Dups\\{0}", fi.Name);
                            File.Move(src, tgt);
                        }
                        
                    }    
                    
                }
            }
        }

        public void LoadTagsFromExternalSource()
        {
            var lines = File.ReadAllLines(@"f:\tags.txt");

            var tm = new TagManager();

            foreach (var line in lines.Where(line => !string.IsNullOrEmpty(line)))
            {
                tm.Save(line.Trim());
            }
        }

        public void GenerateTags()
        {
            var tagMgr = new TagManager();

            var tags = tagMgr.GetTags();
            var tagValues = tags.Values.ToList();
            var added = new List<string>();

            var blackList = tagMgr.GetBlacklist();

            //Console.WriteLine("{0} tagcount", tagValues.Count);
            for (var i = 0; i < 2005; i++)
            {
                var fileName = new BatchManager().RandomSource(1).FName;

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

    }
}