using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ModernHttpClient;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Common
{
    public class HttpRequestHandler
    {
        public string AccessToken { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }

        public TResult SendRequest<T,TResult>(T t)
        {
            HttpClient httpClinet = new HttpClient(new NativeMessageHandler());
            var request = new HttpRequestMessage();
            request.Method = Method == "POST" ? HttpMethod.Post : HttpMethod.Get;
            httpClinet.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.RequestUri = new Uri(Url);

            if (!String.IsNullOrEmpty(AccessToken))
            {
                request.Headers.Add("x-access-token", AccessToken);
            }

            if (t != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(t, Formatting.Indented), Encoding.UTF8, "application/json");
            }
            try
            {
                
                var result = httpClinet.SendAsync(request).Result;
                if(result.IsSuccessStatusCode)
                {
                    string resultJson = result.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<TResult>(resultJson);
                }
                else
                {
                    throw new HttpClientException((int)result.StatusCode);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
          
                       
        }

        public TResult SendRequest<TResult>()
        {
            return SendRequest<object,TResult>(null);
        }
    }

    public class HttpClientException : Exception
    {
        public HttpClientException(int errorCode)
        {
            ErrorCode = errorCode;
            HttpErrorMessage = GetErrorMessage(errorCode);
        }
        public int ErrorCode { get; set; }
        public string HttpErrorMessage { get; set; }

        public string GetErrorMessage(int errorCode)
        {
            switch(errorCode)
            {
                case 204: return "The request has been successfully processed and the response is intentionally blank";
                case 400 : return "The requested resource does not exist on the server";
                case 401: return "Authentication failed";
                case 404: return "The request has been successfully processed and the response is intentionally blank";
                case 409: return "The request could not be carried out because of a conflict on the server";
                case 500: return "Internal Server Error";
                case 501: return "Not Implemented";
                case 503: return "Service Unavailable";
                default:return String.Empty;
            }
        }
    }

}
