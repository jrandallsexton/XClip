
using System.Collections.Generic;

using NUnit.Framework;

using XClip.Api.Models;

namespace XClip.Tests.Api
{
    [TestFixture]
    public class TagControllerTests : ApiControllerTestBase
    {
        private string rootPath = "http://localhost/xclipApi/api/";

        [Test]
        public void GetAllTags()
        {
            var url = rootPath + "tags";
            var tags = base.PerformGet<List<tag>>(url, false);

            Assert.NotNull(tags);
            Assert.That(tags.Count > 0);
        }
    }
}