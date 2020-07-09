using FluentValidation.Results;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Validators
{
    static class ErrorDescriber
    {
        public static ValidationFailure AddressesMissingError(Member entity)
        {
            return new ValidationFailure(nameof(entity.Addresses), $"Addresses are missing for Member of type '{entity.MemberType}'");
        }
    }
}
