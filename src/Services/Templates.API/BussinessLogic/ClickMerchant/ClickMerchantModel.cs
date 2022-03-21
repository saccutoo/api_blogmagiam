using System;
using System.Collections.Generic;
using Utils;

namespace Templates.API.BussinessLogic
{
    public class ClickMerchantModel : BasicModel
    {
        public long id
        {
            get;set;
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
    }

    public class ClickMerchantQuery : BasicQueryModel
    {
        public DateTime? FirstDate
        {
            get;set;
        }
        public DateTime? LastDate
        {
            get; set;
        }
        public string Type
        {
            get; set;
        }
    }
}
