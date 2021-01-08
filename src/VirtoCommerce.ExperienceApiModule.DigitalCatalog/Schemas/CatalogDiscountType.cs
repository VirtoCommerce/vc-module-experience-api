using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class CatalogDiscountType : DiscountType
    {
        public CatalogDiscountType(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            Field<PromotionType, Promotion>()
                .Name("promotion")
                .ResolveAsync(ctx =>
                {
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, Promotion>("promotionsLoader", (ids) => LoadPromotionsAsync(mediator, ids));
                    return loader.LoadAsync(ctx.Source.PromotionId);
                });
        }

        protected virtual async Task<IDictionary<string, Promotion>> LoadPromotionsAsync(IMediator mediator, IEnumerable<string> ids)
        {
            var result = await mediator.Send(new LoadPromotionsQuery { Ids = ids });

            return result.Promotions;
        }
    }
}
