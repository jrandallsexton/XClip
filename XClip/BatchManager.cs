using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XClip.DataObjects;
using XClip.Repositories;

namespace XClip
{
    public class BatchManager
    {
        private readonly BatchRepository _repo = new BatchRepository();

        public bool FileExists(string fName)
        {
            return _repo.FileExists(fName);
        }

        //public bool Save(string fName, string fExt, long fileSize, DateTime created)
        //{
        //    return _repo.Save(fName, fExt, fileSize, created);
        //}

        public XSource RandomSource(int collectionId)
        {
            return _repo.RandomSource(collectionId);
        }

        public KeyValuePair<Guid, string> Source(Guid id)
        {
            return _repo.Source(id);
        }

        public bool Save(XBatch batch)
        {
            return _repo.BatchInfoSave(batch);
        }

        public List<XBatch> GetOpen(int userId, int collectionId)
        {
            var openBatches = new List<XBatch>();

            var openBatchIds = _repo.OpenBatchIds(collectionId);

            if (openBatchIds == null || openBatchIds.Count == 0)
                return openBatches;

            openBatchIds.ForEach(x =>
            {
                openBatches.Add(_repo.Get(x));
            });

            return openBatches;

        }

        public void MarkStarted(Guid batchUId)
        {
            _repo.MarkStarted(batchUId);
        }

        public void MarkCompleted(Guid batchUId, DateTime end)
        {
            _repo.MarkCompleted(batchUId, end);
        }
    }
}
