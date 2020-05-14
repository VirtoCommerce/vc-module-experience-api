using System;
using System.Net.Http;
using System.Threading.Tasks;
using RecommendationsGatewayModule.Core;

namespace RecommendationsGatewayModule.Data
{
    public class DownstreamRequestSender : IDownstreamRequestSender
    {
        private readonly HttpClient _httpClient;

        public DownstreamRequestSender(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<DownstreamResponse> SendRequestAsync(DownstreamRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                var response = await _httpClient.SendAsync(request);
                return await DownstreamResponse.FromHttpResponseAsync(response, request);
            }
            catch (Exception ex)
            {
                return DownstreamResponse.FromException(ex);
            }
        }
    }
}
