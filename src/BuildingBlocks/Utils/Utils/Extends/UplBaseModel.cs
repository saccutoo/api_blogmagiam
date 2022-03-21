using System;
using System.Runtime.Serialization;

namespace Utils
{
    public class UplBaseModel
    {
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}