using System;
using System.Collections.Generic;
using Utils;

namespace Templates.API.BussinessLogic
{
    public class CouponModel: BasicModel
    {

        public string aff_link
        {
            get; set;
        }

        public string aff_link_campaign_tag
        {
            get; set;
        }

        public long campaign
        {
            get; set;
        }

        public string campaign_id
        {
            get; set;
        }
        public string campaign_name
        {
            get; set;
        }
        public List<CategoriesModel> categories
        {
            get; set;
        } = new List<CategoriesModel>();

        public long coin_cap
        {
            get; set;
        }

        public long coin_percentage
        {
            get; set;
        }

        public string content
        {
            get; set;
        }
        public List<CouponsModel> coupons
        {
            get; set;
        } = new List<CouponsModel>();

        public long discount_percentage
        {
            get; set;
        }
        public long discount_value
        {
            get; set;
        }
        public string domain
        {
            get; set;
        }

        public string end_time
        {
            get; set;
        }

        public string id
        {
            get; set;
        }
        public string image
        {
            get; set;
        }
        public bool is_hot
        {
            get; set;
        }

        public List<string> keyword
        {
            get; set;
        }
        public string link
        {
            get; set;
        }
        public long max_value
        {
            get; set;
        }
        public string merchant
        {
            get; set;
        }
        public string name
        {
            get; set;
        }

        public long min_spend
        {
            get; set;
        }

        public long prior_type
        {
            get; set;
        }
        public long remain
        {
            get; set;
        }

        public bool remain_true
        {
            get; set;
        }

        public long shop_id
        {
            get; set;
        }

        public DateTime start_time
        {
            get; set;
        }

        public long status
        {
            get; set;
        }

        public string time_left
        {
            get; set;
        }
    }

    public class CouponsModel
    {
        public string coupon_code
        {
            get; set;
        }

        public string coupon_desc
        {
            get; set;
        }
    }
    public class CouponLazadaModel : BasicModel
    {

        public string aff_link
        {
            get; set;
        }

        public string aff_link_campaign_tag
        {
            get; set;
        }

        public long campaign
        {
            get; set;
        }

        public string campaign_id
        {
            get; set;
        }
        public string campaign_name
        {
            get; set;
        }
        public List<CategoriesModel> categories
        {
            get; set;
        } = new List<CategoriesModel>();

        public long coin_cap
        {
            get; set;
        }

        public long coin_percentage
        {
            get; set;
        }

        public string content
        {
            get; set;
        }
        public List<CouponsModel> coupons
        {
            get; set;
        } = new List<CouponsModel>();

        public long discount_percentage
        {
            get; set;
        }
        public long discount_value
        {
            get; set;
        }
        public string domain
        {
            get; set;
        }

        public string end_time
        {
            get; set;
        }

        public string id
        {
            get; set;
        }
        public string image
        {
            get; set;
        }
        public bool is_hot
        {
            get; set;
        }

        public string keyword
        {
            get; set;
        }
        public string link
        {
            get; set;
        }
        public long max_value
        {
            get; set;
        }
        public string merchant
        {
            get; set;
        }
        public string name
        {
            get; set;
        }

        public long min_spend
        {
            get; set;
        }

        public long prior_type
        {
            get; set;
        }
        public long remain
        {
            get; set;
        }

        public bool remain_true
        {
            get; set;
        }

        public long shop_id
        {
            get; set;
        }

        public DateTime start_time
        {
            get; set;
        }

        public long status
        {
            get; set;
        }

        public string time_left
        {
            get; set;
        }
    }
}
