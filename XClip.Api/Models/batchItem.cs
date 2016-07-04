
using System;
using System.Collections.Generic;

namespace XClip.Api.Models
{
    public class batchItem
    {
        public Guid Id { get; set; }
        public int index { get; set; }
        public string start { get; set; }
        public string stop { get; set; }
        public string duration { get; set; }
        public List<int> tags { get; set; } 
    }
}