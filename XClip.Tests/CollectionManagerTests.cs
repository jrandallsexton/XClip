
using System;

using NUnit.Framework;

namespace XClip.Tests
{
    [TestFixture]
    public class CollectionManagerTests
    {
        [Test]
        public void Create_Succeeds()
        {
            var collectionId = new CollectionManager().Create(1, "Drone Footage");
            Assert.That(collectionId > 1);
        }

        [Test]
        public void CreateFailsWhenCollectionNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new CollectionManager().Create(1, string.Empty));
        }

        [Test]
        public void Delete_Succeeds()
        {
            new CollectionManager().Delete(1, 1);
        }
    }
}