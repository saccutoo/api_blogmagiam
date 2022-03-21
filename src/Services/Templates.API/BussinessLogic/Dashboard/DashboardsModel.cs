using System;
using System.Collections.Generic;
using Utils;

namespace Templates.API.BussinessLogic
{
    public class DashboardsModel : BasicModel
    {
        public long id
        {
            get; set;
        }
        public string ip
        {
            get; set;
        }
        public string merchant_name
        {
            get; set;
        }
        public string aff_link
        {
            get; set;
        }

        public int count_click
        {
            get; set;
        }
    }
}
