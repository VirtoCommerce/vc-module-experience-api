using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace RecommendationsGatewayModule.Core
{
    public class DownstreamResponse
    {
        public static async Task<DownstreamResponse> FromHttpResponseAsync(HttpResponseMessage httpResponse, DownstreamRequest request)
        {
            var response = AbstractTypeFactory<DownstreamResponse>.TryCreateInstance(request.Scenario.Connection.ResponseType.Name);
            response.HttpResponse = httpResponse;

            // try to read content
            var content = await httpResponse.Content.ReadAsStringAsync();
            response.Raw = content;

            // some HTTP error - try to parse body as JSON but allow non-JSON as well
            if (httpResponse.IsSuccessStatusCode)
            {
                try
                {
                    if (content != null)
                    {
                        response.Json = JObject.Parse(content);
                    }
                    await response.InitializeAsync(request);
                }
                catch (Exception ex)
                {
                    response.ErrorType = ResponseErrorType.Exception;
                    response.Exception = ex;
                }
            }
            else
            {
                response.ErrorType = httpResponse.StatusCode == HttpStatusCode.BadRequest ? ResponseErrorType.Application : ResponseErrorType.Http;
            }
            return response;
        }

      
        public static DownstreamResponse FromException(Exception ex, string errorMessage = null)
        {
            var response = new DownstreamResponse
            {
                Exception = ex,
                ErrorType = ResponseErrorType.Exception,
                ErrorMessage = errorMessage
            };

            return response;
        }


        protected virtual Task InitializeAsync(DownstreamRequest request)
        {
            return Task.CompletedTask;
        }


        protected HttpResponseMessage HttpResponse { get; set; }
        protected string Raw { get; set; }
        protected JObject Json { get; set; }
        public Exception Exception { get; protected set; }

        public bool IsError => Error != null;

        protected ResponseErrorType ErrorType { get; set; } = ResponseErrorType.None;

        protected string ErrorMessage { get; set; }

        protected HttpStatusCode HttpStatusCode => HttpResponse.StatusCode;
        protected string HttpErrorReason => HttpResponse.ReasonPhrase;

        public string Error
        {
            get
            {
                if (ErrorType == ResponseErrorType.Http)
                {
                    return HttpErrorReason;
                }
                if (ErrorType == ResponseErrorType.Exception)
                {
                    return Exception.Message;
                }

                return ErrorMessage;
            }
        }

    }
}
