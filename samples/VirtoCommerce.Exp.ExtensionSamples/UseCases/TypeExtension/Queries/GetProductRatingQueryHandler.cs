using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class GetProductRatingQueryHandler : IRequestHandler<GetProductRatingQuery, ProductRating>
    {
        public Task<ProductRating> Handle(GetProductRatingQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ProductRating { Rating = 1 });
        }
    }

}
