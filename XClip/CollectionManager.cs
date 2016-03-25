
using System;
using XClip.Repositories;

namespace XClip
{
    public class CollectionManager
    {
        private readonly CollectionRepository _collectionRepository = new CollectionRepository();

        public int Create(int userId, string name)
        {
            if (string.IsNullOrEmpty((name)))
                throw new ArgumentException("Collection Name cannot be empty");

            if (userId < 1)
                throw new ArgumentException("UserId must be specified");

            return _collectionRepository.Create(userId, name);
        }
    }
}