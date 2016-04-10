
using System;
using XClip.DataObjects;
using XClip.Repositories;

namespace XClip
{
    public class SourceManager
    {

        private readonly SourceRepository _sourceRepository = new SourceRepository();

        public int Save(int collectionId, XSource source)
        {
            return _sourceRepository.Save(collectionId, source);   
        }

        public Guid UId(int collectionId, int sourceId)
        {
            return _sourceRepository.UId(collectionId, sourceId);
        }

        public void MarkAsSkipped(Guid uId)
        {
            _sourceRepository.MarkAsSkipped(uId);
        }
    }
}