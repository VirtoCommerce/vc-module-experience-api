using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetContactByIdQuery : IQuery<ContactAggregate>
    {
        public GetContactByIdQuery(string contactId, string userId)
        {
            ContactId = contactId;
            UserId = userId;
        }

        public string ContactId { get; set; }
        public string UserId { get; set; }
    }
}
