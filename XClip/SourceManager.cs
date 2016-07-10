
using System;
using XClip.DataObjects;
using XClip.Repositories;

namespace XClip
{
    public interface ISourceManager
    {
        int Save(int collectionId, XSource source);
        Guid UId(int collectionId, int sourceId);
        void MarkAsSkipped(Guid uId);
        void MarkAsDeleted(Guid uId);
        XSource Get(Guid uId);
    }

    public class SourceManager : ISourceManager
    {

        private readonly ISourceRepository _sourceRepository = new SourceRepository();

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

        public void MarkAsDeleted(Guid uId)
        {
            _sourceRepository.MarkAsDeleted(uId);
        }

        public XSource Get(Guid uId)
        {
            return _sourceRepository.Get(uId);
        }
    }
}