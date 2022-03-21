using System;
using System.Collections.Generic;
using Utils;

namespace Templates.API.BussinessLogic
{
    public class MerchantListModel : BasicModel
    {
        public string Id { get; set; }
        public string merchant_id { get; set; }
        public string display_name { get; set; }
        public string login_name { get; set; }
        public string logo { get; set; }
        public string logo_coupon { get; set; }
        public long total_offer { get; set; }
        public long is_hide { get; set; }

    }
}
