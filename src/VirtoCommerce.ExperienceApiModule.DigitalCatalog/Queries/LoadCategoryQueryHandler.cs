using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadCategoryQueryHandler : IQueryHandler<LoadCategoryQuery, LoadCategoryResponce>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;

        public LoadCategoryQueryHandler(ISearchProvider searchProvider, IMapper mapper)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
        }

        public virtual async Task<LoadCategoryResponce> Handle(LoadCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = new LoadCategoryResponce();
            var searchRequest = new SearchRequestBuilder()
                                            .WithPaging(0, 1)
                                            .WithIncludeFields(request.IncludeFields.Concat(new[] { "id" }).Distinct().Select(x => $"__object.{x}").ToArray())
                                            .WithIncludeFields((request.IncludeFields.Any(x => x.Contains("slug", System.StringComparison.OrdinalIgnoreCase))
                                                ? new[] { "__object.seoInfos" }
                                                : Enumerable.Empty<string>()).ToArray())
                                            .WithIncludeFields((request.IncludeFields.Any(x => x.Contains("parent", System.StringComparison.OrdinalIgnoreCase))
                                                ? new[] { "__object.parentId" }
                                                : Enumerable.Empty<string>()).ToArray())
                                            .AddObjectIds(new[] { request.Id })
                                            .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Category, searchRequest);
            result.Category = searchResult.Documents.Select(x => _mapper.Map<ExpCategory>(x)).FirstOrDefault();

            return result;
        }
    }
}
