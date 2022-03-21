using System;
using System.Collections.Generic;
using Utils;

namespace Templates.API.BussinessLogic
{
    public class UsersModel : BasicModel
    {
        public long id
        {
            get;set;
        }
        public string user_name
        {
            get; set;
        }
        public string pass_word
        {
            get; set;
        }
        public int block
        {
            get; set;
        }

        public string status_name
        {
            get; set;
        }
        public string token
        {
            get; set;
        }
    }
}
