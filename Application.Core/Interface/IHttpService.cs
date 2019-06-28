using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core.Interface
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> Get(string url, IDictionary<string, string> headers);
        Task<HttpResponseMessage> Post(string url, HttpContent content, IDictionary<string, string> headers);
    }
}
