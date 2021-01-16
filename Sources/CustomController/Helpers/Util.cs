using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CustomController.Helpers
{
    public static class Util
    {
        public class ErrorMessage : IHttpActionResult
        {
            public bool Success { get { return false; } }
            public string Message { get; set; }

            public static implicit operator HttpResponseMessage(ErrorMessage obj)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(JsonConvert.SerializeObject(obj)) };
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult<HttpResponseMessage>(this);
            }
        }

        public class OkMessage : IHttpActionResult
        {
            public bool Success { get { return true; } }

            public static implicit operator HttpResponseMessage(OkMessage obj)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(obj)) };
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult<HttpResponseMessage>(this);
            }
        }

        public class OkMessage<T> : IHttpActionResult
        {
            public bool Success { get { return true; } }
            public T Data { get; set; }

            public static implicit operator HttpResponseMessage(OkMessage<T> obj)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(obj)) };
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult<HttpResponseMessage>(this);
            }
        }

        public class UnauthorizedMessage : IHttpActionResult
        {
            public bool Success { get { return false; } }
            public string Message { get { return "Invalid company/username/password."; } }

            public static implicit operator HttpResponseMessage(UnauthorizedMessage obj)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized, Content = new StringContent(JsonConvert.SerializeObject(obj)) };
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult<HttpResponseMessage>(this);
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
