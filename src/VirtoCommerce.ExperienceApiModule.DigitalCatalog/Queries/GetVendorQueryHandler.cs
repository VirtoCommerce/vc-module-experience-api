using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class GetVendorQueryHandler: IQueryHandler<GetVendorQuery, Member>
    {
        private readonly IMemberService _memberService;

        public GetVendorQueryHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public Task<Member> Handle(GetVendorQuery request, CancellationToken cancellationToken)
        {
            var member = _memberService.GetByIdAsync(request.Id);

            return member;
        }
    }
}
