
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XClip.DataObjects
{
    public class XVideo
    {
        public int Id { get; set; }

        public Guid UId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<XVideoItem> Items { get; set; }

        public XVideo()
        {
            this.Items = new List<XVideoItem>();
        }
    }
}