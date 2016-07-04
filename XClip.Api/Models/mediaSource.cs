
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

        public mediaSource()
        {
            
        }

        public mediaSource(int msId, Guid uId, string name, string ext)
        {
            this.id = msId;
            this.uId = uId;
            this.fName = name;
            this.fExt = ext;

            ext = ext.ToLower();

            if (ext == FileExtensions.Mp4)
            {
                this.mimeType = MimeTypes.Mp4;
            }
            else if (ext == FileExtensions.Mov)
            {
                this.mimeType = MimeTypes.Mov;
            }
            else
            {
                this.mimeType = MimeTypes.Wmv;
            }

        }

    }
}