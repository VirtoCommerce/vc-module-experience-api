using FluentValidation;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Validators;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class NewContactValidator : AbstractValidator<Contact>
    {
        public NewContactValidator()
        {
            RuleFor(x => x.FirstName).NotNull();
            RuleFor(x => x.FullName).NotNull();
            RuleFor(x => x.LastName).NotNull();
            RuleFor(x => x.Name).NotNull();

            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((member, context) =>
                {
                    if (member.Addresses.IsNullOrEmpty())
                    {
                        context.AddFailure(ErrorDescriber.AddressesMissingError(member));
                    }
                });
            });
        }
    }
}
