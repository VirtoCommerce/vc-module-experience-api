using GraphQL.Types;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputOrderPaymentMethodType : InputObjectGraphType<PaymentMethod>
    {
        public InputOrderPaymentMethodType()
        {
            Field(x => x.Id);
            Field<ListGraphType<InputOrderTaxDetailType>>(nameof(PaymentMethod.TaxDetails));
            Field(x => x.TaxTotal);
            Field(x => x.TypeName, true);
            Field(x => x.StoreId, true);
            Field(x => x.DiscountAmountWithTax);
            Field(x => x.DiscountAmount);
            Field(x => x.TotalWithTax);
            Field(x => x.Total);
            Field(x => x.PriceWithTax);
            Field(x => x.Price);
            Field(x => x.Currency, true);
            Field(x => x.IsAvailableForPartial);
            Field(x => x.Priority);
            Field(x => x.IsActive);
            Field(x => x.LogoUrl, true);
            Field(x => x.Name, true);
            Field(x => x.Code);
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodType));
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodGroupType));

            //TODO
            //public ICollection<ObjectSettingEntry> Settings);
        }
    }
}
