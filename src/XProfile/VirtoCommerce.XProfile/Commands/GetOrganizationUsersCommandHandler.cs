using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class GetOrganizationUsersCommandHandler : IRequestHandler<GetOrganizationUsersCommand, ProfileSearchResult>
    {
        private readonly IMemberServiceX _memberService;

        public GetOrganizationUsersCommandHandler(IMemberServiceX memberService)
        {
            _memberService = memberService;
        }

        public async Task<ProfileSearchResult> Handle(GetOrganizationUsersCommand request, CancellationToken cancellationToken)
        {
            //TODO: check authentication by request.UserId

            var criteria = new MembersSearchCriteria
            {
                MemberId = request.OrganizationId,
                Skip = request.Skip,
                Take = request.Take,
                Sort = request.Sort,
                ObjectType = "Member"
            };
            return await _memberService.SearchOrganizationContactsAsync(criteria);
        }
    }
}
