using GraphQL.Types;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputOrderPaymentMethodType : InputObjectGraphType<PaymentMethod>
    {
        public InputOrderPaymentMethodType()
        {
            Field(x => x.Id);
            Field<ListGraphType<InputOrderTaxDetailType>>(nameof(PaymentMethod.TaxDetails),
                "Tax details");
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
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodType),
                "Payment method type");
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodGroupType),
                "Payment method group type");

            //PT-5383: Add additional properties to XOrder types:
            //public ICollection<ObjectSettingEntry> Settings);
        }
    }
}
