
using System;

namespace XClip.DataObjects
{
    public class XSource
    {
        public int Id { get; set; }

        public Guid UId { get; set; }

        public string FName { get; set; }

        public string FExt { get; set; }

        public long FSize { get; set; }

        public DateTime FDate { get; set; }

        public XSource(string fName, string fExt, long fSize, DateTime fDate)
        {
            this.FName = fName;
            this.FExt = fExt;
            this.FSize = fSize;
            this.FDate = fDate;
        }

        public XSource(int id, Guid uId, string fName, string fExt, long fSize, DateTime fDate)
        {
            this.Id = id;
            this.UId = uId;
            this.FName = fName;
            this.FExt = fExt;
            this.FSize = fSize;
            this.FDate = fDate;
        }
    }
}