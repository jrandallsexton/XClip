using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XClip.Api.Models
{
    public class tagCreation
    {
        public int userId { get; set; }
        public int? collectionId { get; set; }
        public string tag { get; set; }
    }
}