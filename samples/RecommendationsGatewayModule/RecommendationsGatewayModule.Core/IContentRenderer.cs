using System.Threading.Tasks;

namespace RecommendationsGatewayModule.Core
{
    public interface IContentRenderer
    {
        Task<string> RenderAsync(string template, object contextObj);
    }
}
