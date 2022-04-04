using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Templates.API.BussinessLogic
{
    public class BasicModel

    {
        public int status
        {
            get; set;
        }
        public DateTime? create_date
        {
            get;set;
        }
        public DateTime? update_date
        {
            get; set;
        }
        public string create_by
        {
            get; set;
        }
        public string update_by
        {
            get; set;
        }
    }


    public class BasicQueryModel
    {
        public int status
        {
            get; set;
        }
        public string value
        {
            get; set;
        }
        public int pageIndex
        {
            get; set;
        }
        public int pageSize
        {
            get; set;
        }
    }
}
