using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class GetVendorQueryHandler: IQueryHandler<GetVendorQuery, ExpVendor>
    {
        private readonly IMemberService _memberService;

        public GetVendorQueryHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public virtual async Task<ExpVendor> Handle(GetVendorQuery request, CancellationToken cancellationToken)
        {
            var member = await _memberService.GetByIdAsync(request.Id);

            var result = AbstractTypeFactory<ExpVendor>.TryCreateInstance();
            result.Id = member.Id;
            result.Name = member.Name;
            result.Type = member.MemberType;
            return result;
        }
    }
}
