using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetContactByIdQuery : IQuery<ContactAggregate>
    {
        public GetContactByIdQuery(string contactId)
        {
            ContactId = contactId;
        }

        public string ContactId { get; set; }
    }
}
