using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.DataLoader;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Models;

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

        public static IDataLoaderResult<ExpVendor> LoadVendor(
            this IDataLoaderContextAccessor dataLoader,
            IMemberService memberService,
            IMapper mapper,
            string loaderKey,
            string vendorId)
        {
            var loader = dataLoader.GetVendorDataLoader(memberService, mapper, loaderKey);

            return vendorId != null
                ? loader.LoadAsync(vendorId)
                : new DataLoaderResult<ExpVendor>(Task.FromResult<ExpVendor>(null));
        }
    }
}
