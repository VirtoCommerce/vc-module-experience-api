using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Contracts
{
    public class LoadProductRequest : IRequest<LoadProductResponse>
    {
        public string[] Ids { get; set; }
        public string ResponseGroup { get; set; }
    }
}
