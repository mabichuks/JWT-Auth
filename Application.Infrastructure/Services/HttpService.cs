using Application.Core.Interface;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Application.Infrastructure.Services
{
    public class HttpService : IHttpService
    {

        public async Task<HttpResponseMessage> Get(string url, IDictionary<string, string> headers)
        {
            using (HttpClient client = new HttpClient())
            {

                foreach (var tm in headers)
                {
                    client.DefaultRequestHeaders.Add(tm.Key, tm.Value);
                }
                return await client.GetAsync(url);
            };
        }



        public async Task<HttpResponseMessage> Post(string url, HttpContent content, IDictionary<string, string> headers)
        {
            using (HttpClient client = new HttpClient())
            {
                foreach (var tm in headers)
                {
                    client.DefaultRequestHeaders.Add(tm.Key, tm.Value);
                }
                return await client.PostAsync(url, content);
            };
        }

    }
}
