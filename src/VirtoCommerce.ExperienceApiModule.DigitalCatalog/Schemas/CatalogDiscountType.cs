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
            FieldAsync<PromotionType>("promotion", resolve: async context =>
            {
                var loader = dataLoader.Context.GetOrAddBatchLoader<string, Promotion>("promotionsLoader", (ids) => LoadPromotionsAsync(mediator, ids));

                // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
                var loadHandle = loader.LoadAsync(context.Source.PromotionId).GetResultAsync();
                return await loadHandle;
            });
        }

        protected virtual async Task<IDictionary<string, Promotion>> LoadPromotionsAsync(IMediator mediator, IEnumerable<string> ids)
        {
            var result = await mediator.Send(new LoadPromotionsQuery { Ids = ids });

            return result.Promotions;
        }
    }
}
