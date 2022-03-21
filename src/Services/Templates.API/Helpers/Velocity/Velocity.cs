using Newtonsoft.Json;
using NVelocity;
using NVelocity.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.Velocity
{
    public static class Velocity
    {
        public static async Task<Response> RenderTemplate(string templateContent,string param)
        {
            ResponseVelocity resVelocity = new ResponseVelocity();
            ResponseObject<ResponseVelocity> response = new ResponseObject<ResponseVelocity>(resVelocity);
          
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                var dicValue = new  Dictionary<string,object>();
                if (!string.IsNullOrEmpty(param))
                {
                    dicValue= JsonConvert.DeserializeObject<Dictionary<string, object>>(param);
                }              
                if (string.IsNullOrEmpty(templateContent))
                {
                    response.StatusCode = StatusCode.Fail;
                    response.Message ="File không có nội dung";
                    return response;
                }

                var name = templateContent;

                var engine = new VelocityEngine();
                engine.Init();

                var context = new VelocityContext();

                var templateData = dicValue ?? null;
                if (templateData!=null && templateData.Count()>0)
                {
                    foreach (var item in dicValue)
                    {
                        if (item.Key.IndexOf("BM_") > -1)
                        {
                            context.Put(item.Key, item.Value!=null ? item.Value : "");
                        }
                        else if(item.Key.IndexOf("DATA_TABLE") > -1 && item.Value!=null)
                        {
                            var dicValueTable  = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Value.ToString());
                            if (dicValueTable!=null && dicValueTable.Count()>0)
                            {
                                foreach (var dic in dicValueTable)
                                {
                                    if (dic.Value!=null)
                                    {
                                        context.Put(dic.Key, JsonConvert.DeserializeObject<object>(dic.Value.ToString()));
                                    }
                                }
                            }                          
                        }
                    }
                }


                Helper helper = new Helper();
                context.Put("helper", helper);

                using (var writer = new StringWriter())
                {
                    engine.Evaluate(context, writer, "", templateContent);
                    response.Data.Content= writer.GetStringBuilder().ToString().Trim();
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.Fail;
                response.Message = ex.Message;
            }
            return  response;
        }
    }
  
    public class ResponseVelocity
    {
        public string Content { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
