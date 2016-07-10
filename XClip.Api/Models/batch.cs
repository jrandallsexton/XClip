
using System;
using System.Collections.Generic;

namespace XClip.Api.Models
{
    public class batch
    {
        public Guid sourceId { get; set; }
        public List<int> tags { get; set; }
        public List<batchItem> items { get; set; }
    }
}