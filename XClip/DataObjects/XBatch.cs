
using System;
using System.Collections.Generic;

namespace XClip.DataObjects
{
    public class XBatch
    {
        public Guid Id { get; set; }

        public Guid SrcId { get; set; }

        public string InputDir { get; set; }

        public string OutputDir { get; set; }

        private string _fName = string.Empty;

        public string Filename
        {
            get { return this._fName; }
            set
            {
                this._fName = value;
                this.OutputMask = _fName.Insert(_fName.LastIndexOf('.'), "_{0}");
            }
        }

        public string OutputMask { get; set; }

        public List<XBatchItem> Items { get; set; }

        public XBatch()
        {
            this.Id = Guid.NewGuid();
            this.Items = new List<XBatchItem>();
        }

        public XBatch(Guid srcId) : this()
        {
            this.SrcId = srcId;
        }

        // http://stackoverflow.com/questions/19402024/c-sharp-how-to-inherit-from-default-constructor
        public XBatch(string dirIn, string dirOout, string fileName, string mask) : this()
        {
            this.InputDir = dirIn;
            this.OutputDir = dirOout;
            this.Filename = fileName;
            this.OutputMask = mask;
        }
    }
}
