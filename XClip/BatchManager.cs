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

        public bool Save(string fName, string fExt, long fileSize, DateTime created)
        {
            return _repo.Save(fName, fExt, fileSize, created);
        }

        public XSource RandomSource()
        {
            return _repo.RandomSource();
        }

        public KeyValuePair<Guid, string> Source(Guid id)
        {
            return _repo.Source(id);
        }
    }
}
