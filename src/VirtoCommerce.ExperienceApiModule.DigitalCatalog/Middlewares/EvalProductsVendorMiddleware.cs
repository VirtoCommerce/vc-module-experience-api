using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerModule.Core.Model;
using AutoMapper;

namespace VirtoCommerce.XDigitalCatalog.Middlewares;

public class EvalProductsVendorMiddleware: IAsyncMiddleware<SearchProductResponse>
{
    private readonly IMapper _mapper;
    private readonly IMemberService _memberService;

    public EvalProductsVendorMiddleware(IMapper mapper, IMemberService memberService)
    {
        _mapper = mapper;
        _memberService = memberService;
    }

    public virtual async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        var query = parameter.Query;
        if (query == null)
        {
            throw new OperationCanceledException("Query must be set");
        }

        var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
        if (responseGroup.HasFlag(ExpProductResponseGroup.LoadVendors) && parameter.Results.Any())
        {
            var members = new List<Member>();

            var memberIds = parameter.Results
                .Where(x => x.IndexedProduct.Vendor != null)
                .Select(x => x.IndexedProduct.Vendor)
                .Distinct()
                .ToArray();

            var countResult = memberIds.Count();

            var pageSize = 10;

            for (var i = 0; i < countResult; i += pageSize)
            {
                members.AddRange(await _memberService.GetByIdsAsync(memberIds));
            }

            if (members.Any())
            {
                parameter.Results.Apply(product =>
                    product.Vendor =
                        _mapper.Map<ExpProductVendor>(members.FirstOrDefault(member =>
                            member.Id == product.IndexedProduct.Vendor)));
            }
        }

        await next(parameter);
    }
}
