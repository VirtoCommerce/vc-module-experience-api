//using System.Threading;
//using System.Threading.Tasks;
//using MediatR;
//using VirtoCommerce.CustomerModule.Core.Model.Search;

//namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
//{
//    public class SearchOrganizationMembersHandler : IRequestHandler<SearchOrganizationMembersQuery, MemberSearchResult>
//    {
//        public async Task<MemberSearchResult> Handle(SearchOrganizationMembersQuery request, CancellationToken cancellationToken)
//        {
//            //TODO: check authentication by request.UserId

//            var criteria = new MembersSearchCriteria
//            {
//                MemberId = request.OrganizationId,
//                Skip = request.Skip,
//                Take = request.Take,
//                Sort = request.Sort,
//                ObjectType = "Member"
//            };
//            return await _memberService.SearchOrganizationContactsAsync(criteria);
//        }
//    }
//}
