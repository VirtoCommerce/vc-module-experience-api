using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Data.Middlewares;

/// <summary>
/// Loads vendors for products
/// </summary>
public class EvalProductsVendorMiddleware : IAsyncMiddleware<SearchProductResponse>
{
    private readonly IMapper _mapper;
    private readonly IMemberService _memberService;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="memberService"></param>
    public EvalProductsVendorMiddleware(IMapper mapper, IMemberService memberService)
    {
        _mapper = mapper;
        _memberService = memberService;
    }

    /// <summary>
    /// Runs the middleware
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (parameter.Query == null)
        {
            throw new ArgumentException("Query is null", nameof(parameter));
        }

        if (ShouldLoadVendors(parameter))
        {
            var vendorIds = ExtractVendorIds(parameter.Results);
            var vendorsByIds = await LoadVendors(vendorIds);
            UpdateVendorsInProducts(parameter.Results, vendorsByIds);
        }


        await next(parameter);
    }

    /// <summary>
    /// Checks if vendors should be loaded
    /// </summary>
    /// <param name="searchProductResponse"></param>
    /// <returns></returns>
    protected virtual bool ShouldLoadVendors(SearchProductResponse searchProductResponse)
    {
        var responseGroup = EnumUtility.SafeParse(searchProductResponse.Query.GetResponseGroup(), ExpProductResponseGroup.None);
        return responseGroup.HasFlag(ExpProductResponseGroup.LoadVendors) && searchProductResponse.Results.Any();
    }

    /// <summary>
    /// Extracts vendor ids from products
    /// </summary>
    /// <param name="products"></param>
    /// <returns></returns>
    protected virtual IList<string> ExtractVendorIds(IList<ExpProduct> products)
    {
        return products
            .Where(x => x.IndexedProduct?.Vendor != null)
            .Select(x => x.IndexedProduct.Vendor)
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Loads vendors by ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    protected virtual async Task<IDictionary<string, Member>> LoadVendors(IList<string> ids)
    {
        var result = new Dictionary<string, Member>();
        const int pageSize = 10;

        foreach (var idsPage in ids.Paginate(pageSize))
        {
            var members = await _memberService.GetByIdsAsync(idsPage.ToArray());
            foreach (var member in members)
            {
                result.TryAdd(member.Id, member);
            }
        }

        return result;
    }

    /// <summary>
    /// Updates vendors in products.
    /// </summary>
    /// <param name="products"></param>
    /// <param name="vendorsByIds"></param>
    protected virtual void UpdateVendorsInProducts(IList<ExpProduct> products, IDictionary<string, Member> vendorsByIds)
    {
        if (!vendorsByIds.Any())
        {
            return;
        }

        products
            .Where(x => x.IndexedProduct?.Vendor != null)
            .Apply(product =>
            {
                if (vendorsByIds.TryGetValue(product.IndexedProduct.Vendor, out var member))
                {
                    product.Vendor = _mapper.Map<ExpVendor>(member);
                }
            });
    }
}
