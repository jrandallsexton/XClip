using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using XClip.Api.Models;

namespace XClip.Tests.Api
{
    [TestFixture]
    public class MediaSourceControllerTests : ApiControllerTestBase
    {
        private string rootPath = "http://localhost/xclipApi/api/";

        [Test]
        public void GetRandomMediaSource()
        {
            var url = rootPath + "media/1/random";
            var random = base.PerformGet<mediaSource>(url, false);

            Assert.NotNull(random);
        }

        [Test]
        public void SkipSucceeds()
        {
            var mediaId = new Guid("9573BEDA-1E0F-4AF7-BA7A-F41232CF6574");

            var url = rootPath + "media/" + mediaId + "/";
            var random = base.PerformPut(url, mediaId.ToString(), false);

            Assert.NotNull(random);
        }
    }
}