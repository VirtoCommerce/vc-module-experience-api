using System.Threading.Tasks;

namespace RecommendationsGatewayModule.Core
{
    public interface IDownstreamRequestSender
    {
        Task<DownstreamResponse> SendRequestAsync(DownstreamRequest request);

    }
}
