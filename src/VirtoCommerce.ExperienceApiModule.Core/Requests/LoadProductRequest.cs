using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Requests
{
    public class LoadProductRequest : IRequest<LoadProductResponse>
    {
        public string[] Ids { get; set; }
        public string ResponseGroup { get; set; }
    }
}
