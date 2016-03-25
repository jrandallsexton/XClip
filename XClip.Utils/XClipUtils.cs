
using System;
using System.Collections.Generic;
using System.IO;

using XClip.Config;

namespace XClip.Utils
{
    public class XClipUtils
    {
        private readonly List<string> _allowedExtensions = new List<string>() { ".mp4", ".wmv", ".flv", ".mpg", ".avi" };

        public void GetFileStats()
        {

            var config = XClipConfig.GetSection();
            var di = new DirectoryInfo(config.DirectoryIn);
            var counts = new SortedDictionary<string, int>();

            foreach (var fi in di.GetFiles("*.*", SearchOption.AllDirectories))
            {
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
                if (!_allowedExtensions.Contains(fi.Extension))
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
    }
}