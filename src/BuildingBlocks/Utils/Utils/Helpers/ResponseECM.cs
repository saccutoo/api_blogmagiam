using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace Utils
{
    public class ResponseECM
    {
        public List<List<Property>> properties { get; set; }
        public StatusCode StatusCode { get; set; } = StatusCode.Success;
        public string Message { get; set; } = "Thành công";
        public string Code { get; set; }
        public ResponseFile<MemoryStream> ResponseFile { get; set; } = new ResponseFile<MemoryStream>();
    }


    public class Property
    {
        public string docName { get; set; }
        public string cifNo { get; set; }
        public string id { get; set; }
        public string fileType { get; set; }
        public string docContent { get; set; }
    }
}
