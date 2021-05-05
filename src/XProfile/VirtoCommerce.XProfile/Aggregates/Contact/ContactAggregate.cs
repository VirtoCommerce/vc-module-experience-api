using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates.Contact;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class ContactAggregate : MemberAggregateRootBase
    {
        public Contact Contact => Member as Contact;

        public override Member Member
        {
            get => base.Member;
            set
            {
                base.Member = value;

                if (string.IsNullOrEmpty(Contact?.FullName))
                {
                    Contact.FullName = string.Join(" ", Contact.FirstName, Contact.LastName);
                }
            }
        }

        public virtual ContactAggregate UpdatePersonalDetails(PersonalData personalDetails)
        {
            Contact.FirstName = personalDetails.FirstName;
            Contact.LastName = personalDetails.LastName;
            Contact.MiddleName = personalDetails.MiddleName;
            Contact.FullName = personalDetails.FullName;

            return this;
        }
    }
}
