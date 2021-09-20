using FluentValidation;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Validators
{
    /// <summary>
    /// Example of derived CartValidator
    /// </summary>
    public class CartValidator2 : CartValidator
    {
        public CartValidator2()
        {
            // Some additional rules (to the basic) can be provided there
            RuleFor(x => x.CartAggregate.Cart.Id).NotEmpty(); // Just example
        }
    }
}
