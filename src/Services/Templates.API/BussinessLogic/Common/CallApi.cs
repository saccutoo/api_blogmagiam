using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic.Common
{
    public class CallApi<T>
    {
        private RestClient client = null;
        private RestRequest request = null;
        public CallApi()
        {
            client = new RestClient(Helpers.GetConfig("API_ACCESSTRADE:URL_BASE"));
            request = new RestRequest();
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Token " + Helpers.GetConfig("API_ACCESSTRADE:TOKEN"));
        }
        public async Task<ResponseAccessTrade<T>> GetApi(string url)
        {
            ResponseAccessTrade<T> res = new ResponseAccessTrade<T>();
            request.Resource = url;
            request.Method = Method.GET;
            //request.RequestFormat = DataFormat.Json;
            //request.AddParameter("user", model);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                res = JsonConvert.DeserializeObject<ResponseAccessTrade<T>>(response.Content);
            }
            return res;
        }
    }
}
