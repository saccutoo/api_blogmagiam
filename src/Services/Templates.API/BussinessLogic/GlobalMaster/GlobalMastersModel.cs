using System;
using System.Collections.Generic;
using Utils;

namespace Templates.API.BussinessLogic
{
    public class GlobalMastersModel : BasicModel
    {
        public long id
        {
            get;set;
        }
        public string name
        {
            get; set;
        }
        public string code
        {
            get; set;
        }
        public string parent_code
        {
            get; set;
        }
        public string value
        {
            get; set;
        }
        public string status_name
        {
            get; set;
        }
    }
}
