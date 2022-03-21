using System;
using System.Collections.Generic;
using Utils;

namespace Templates.API.BussinessLogic
{
    public class TypePromotionsModel : BasicModel
    {
        public long id
        {
            get;set;
        }
        public string name
        {
            get; set;
        }
        public string status_name
        {
            get; set;
        }
    }
}
