using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Templates.API.Velocity
{
    public class Helper
    {
        public DateTime GetDate()
        {
            DateTime date = new DateTime();
            return date;
        }

        public object ConvertStringToListObject(string value)
        {
            return JsonConvert.DeserializeObject<object>(value);
        }
    }
}
