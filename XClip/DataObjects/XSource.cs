using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XClip.DataObjects
{
    public class XSource
    {
        public Guid Id { get; set; }
        public string FName { get; set; }
        public string FExt { get; set; }

        public XSource(Guid id, string fName, string fExt)
        {
            this.Id = id;
            this.FName = fName;
            this.FExt = fExt;
        }
    }
}
