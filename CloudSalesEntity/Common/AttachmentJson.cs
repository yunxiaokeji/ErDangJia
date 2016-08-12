using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class AttachmentJson
    {
        public string attachmentID { get; set; }

        public int type { get; set; }

        public long size { get; set; }

        public string serverUrl { get; set; }

        public string filePath { get; set; }

        public string fileName { get; set; }

        public string originalName { get; set; }
    }
}
