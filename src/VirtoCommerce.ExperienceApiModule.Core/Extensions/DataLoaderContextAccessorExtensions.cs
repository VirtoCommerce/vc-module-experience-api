using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GraphQL.DataLoader;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class DataLoaderContextAccessorExtensions
    {
        public static IDataLoader<string, ExpVendor> GetVendorDataLoader(
            this IDataLoaderContextAccessor dataLoader,
            IMemberService memberService,
            IMapper mapper,
            string loaderKey)
        {
            var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpVendor>(loaderKey, async (ids) =>
            {
                var memberByIds = new Dictionary<string, ExpVendor>();

                var members = await memberService.GetByIdsAsync(ids.ToArray());
                foreach (var member in members)
                {
                    var vendor = mapper.Map<ExpVendor>(member);
                    memberByIds.Add(member.Id, vendor);
                }

                return memberByIds;
            });
            return loader;
        }
    }
}
