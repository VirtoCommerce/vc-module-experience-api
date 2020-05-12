using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations
{
    public interface IDownstreamRequestSender
    {
        Task<DownstreamResponse> SendRequestAsync(DownstreamRequest request);

    }
}
