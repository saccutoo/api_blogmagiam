using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utils
{
    #region Response
    public class ResponseFile<T>
    {
        public ResponseFile(T data, string fileName, string contentType)
        {
            Data = data;
            FileName = fileName;
            ContentType = contentType;
        }
        public ResponseFile()
        {
        }

        public T Data { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public StatusCode StatusCode { get; set; } = StatusCode.Success;
        public string Message { get; set; } = "Thành công";
    }

    #endregion
}