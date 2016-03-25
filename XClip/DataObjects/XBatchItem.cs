using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XClip.DataObjects
{
    public class XBatchItem
    {
        public Guid Id { get; set; }

        public int Index { get; set; }

        public string Start { get; set; }

        public string Stop { get; set; }

        public string Duration { get; set; }

        public List<int> Tags { get; set; }

        public XBatchItem() { this.Id = Guid.NewGuid(); }

        public XBatchItem(int idx, string start, string stop, string duration, List<int> tagIds) : this()
        {
            this.Index = idx;
            this.Start = start;
            this.Stop = stop;
            this.Duration = duration;
            this.Tags = tagIds;
        }
    }
}
