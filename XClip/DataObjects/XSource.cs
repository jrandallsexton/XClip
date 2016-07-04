
using System;

namespace XClip.DataObjects
{
    public class XSource
    {
        public int Id { get; set; }

        public Guid UId { get; set; }

        public string Filename { get; set; }

        public string FileExt { get; set; }

        public long FileSize { get; set; }

        public DateTime FileDate { get; set; }

        public XSource()
        {
            this.UId = Guid.NewGuid();
        }

        public XSource(string fName, string fExt, long fSize, DateTime fDate)
        {
            this.Filename = fName;
            this.FileExt = fExt;
            this.FileSize = fSize;
            this.FileDate = fDate;
        }

        public XSource(int id, Guid uId, string fName, string fExt, long fSize, DateTime fDate)
        {
            this.Id = id;
            this.UId = uId;
            this.Filename = fName;
            this.FileExt = fExt;
            this.FileSize = fSize;
            this.FileDate = fDate;
        }
    }
}