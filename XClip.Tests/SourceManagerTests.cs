
using System.IO;
using System.Linq;

using NUnit.Framework;

using XClip.Config;
using XClip.DataObjects;

namespace XClip.Tests
{
    [TestFixture]
    public class SourceManagerTests
    {
        [Test]
        public void LoadSourceMedia()
        {

            var srcMgr = new SourceManager();

            var config = XClipConfig.GetSection();
            var allowedExtensions = config.AllowedExtensions.Split(',').ToList();
            var di = new DirectoryInfo(config.DirectoryIn);

            foreach (var source in from fi in di.GetFiles() where allowedExtensions.Contains(fi.Extension)
                                   select new XSource(fi.Name, fi.Extension, fi.Length, fi.CreationTime))
            {
                srcMgr.Save(1, source);
            }

        }

        [Test]
        public void LoadSourceMediaUsingCollectionManager()
        {
            const int collectionId = 1;
            var mgr = new CollectionManager();

            var config = XClipConfig.GetSection();
            var allowedExtensions = config.AllowedExtensions.Split(',').ToList();
            var di = new DirectoryInfo(config.DirectoryIn);

            var idx = 0;
            foreach (var source in from fi in di.GetFiles()
                                   where allowedExtensions.Contains(fi.Extension)
                                   select new XSource(fi.Name, fi.Extension, fi.Length, fi.CreationTime))
            {
                source.FName = source.FName.Replace(source.FExt, string.Empty);
                mgr.AddToCollection(collectionId, source, true);
                idx++;
                if (idx == 10)
                    break;
            }
        }
    }
}