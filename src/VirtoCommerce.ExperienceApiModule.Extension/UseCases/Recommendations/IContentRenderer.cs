using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations
{
    public interface IContentRenderer
    {
        Task<string> RenderAsync(string template, object contextObj);
    }
}
