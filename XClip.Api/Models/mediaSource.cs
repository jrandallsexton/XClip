
using System;

namespace XClip.Api.Models
{
    public class mediaSource
    {

        public int id { get; set; }

        public Guid uId { get; set; }

        public string fName { get; set; }
        public string fExt { get; set; }
        public string mimeType { get; set; }

        public mediaSource(int msId, Guid uId, string name, string ext)
        {
            this.id = msId;
            this.uId = uId;
            this.fName = name;
            this.fExt = ext;

            this.mimeType = (ext == FileExtensions.Mp4) ? MimeTypes.Mp4 : MimeTypes.Wmv;

        }

    }
}