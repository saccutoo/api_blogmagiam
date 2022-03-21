using System;
using System.Collections.Generic;
using Utils;

namespace Templates.API.BussinessLogic
{
    public class MenusModel : BasicModel
    {
        public long id
        {
            get;set;
        }
        public string name
        {
            get; set;
        }
        public long parent_id
        {
            get; set;
        }
        public string controller
        {
            get; set;
        }
        public string Class
        {
            get; set;
        }
        public string url
        {
            get; set;
        }
        public string status_name
        {
            get; set;
        }
        public string parent_name
        {
            get; set;
        }
    }
}
