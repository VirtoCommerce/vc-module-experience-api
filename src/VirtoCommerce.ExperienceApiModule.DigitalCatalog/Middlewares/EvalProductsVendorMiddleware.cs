using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares;

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
            throw new OperationCanceledException("Query must be set");
        }

        if (ShouldLoadVendors(parameter))
        {
            var vendorIds = ExtractVendorIds(parameter.Results);
            var vendors = await LoadVendors(vendorIds);
            UpdateVendorsInResults(parameter.Results, vendors);
        }


        await next(parameter);
    }

    /// <summary>
    /// Checks if vendors should be loaded
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    protected virtual bool ShouldLoadVendors(SearchProductResponse parameter)
    {
        var responseGroup = EnumUtility.SafeParse(parameter.Query.GetResponseGroup(), ExpProductResponseGroup.None);
        return responseGroup.HasFlag(ExpProductResponseGroup.LoadVendors) && parameter.Results.Any();
    }

    /// <summary>
    /// Extracts vendor ids from products
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    protected virtual string[] ExtractVendorIds(IEnumerable<ExpProduct> results)
    {
        return results
            .Where(x => x.IndexedProduct.Vendor != null)
            .Select(x => x.IndexedProduct.Vendor)
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Loads vendors by ids
    /// </summary>
    /// <param name="vendorIds"></param>
    /// <returns></returns>
    protected virtual async Task<Dictionary<string, Member>> LoadVendors(string[] vendorIds)
    {
        var vendors = new Dictionary<string, Member>();

        var countResult = vendorIds.Length;
        const int pageSize = 10;
        var currentIndex = 0;

        while (currentIndex < countResult)
        {
            var pageMemberIds = vendorIds.Skip(currentIndex).Take(pageSize).ToArray();

            var members = await _memberService.GetByIdsAsync(pageMemberIds);
            foreach (var member in members)
            {
                vendors.TryAdd(member.Id, member);
            }
            currentIndex += pageSize;
        }

        return vendors;
    }

    /// <summary>
    /// Updates vendors in results.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="memberByIds"></param>
    protected virtual void UpdateVendorsInResults(IEnumerable<ExpProduct> results, Dictionary<string, Member> vendors)
    {
        if (!vendors.Any())
        {
            return;
        }

        results
            .Where(x => x.IndexedProduct.Vendor != null)
            .Apply(product =>
            {
                vendors.TryGetValue(product.IndexedProduct.Vendor, out var member);
                product.Vendor = _mapper.Map<ExpProductVendor>(member);
            });
    }
}
