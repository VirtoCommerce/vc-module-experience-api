using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class GetVendorQueryHandler: IQueryHandler<GetVendorQuery, ExpVendorType>
    {
        private readonly IMemberService _memberService;

        public GetVendorQueryHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public virtual async Task<ExpVendorType> Handle(GetVendorQuery request, CancellationToken cancellationToken)
        {
            var member = await _memberService.GetByIdAsync(request.Id);

            var result = new ExpVendorType { Id = member.Id, Type = member.MemberType, Name = member.Name };

            return result;
        }
    }
}
