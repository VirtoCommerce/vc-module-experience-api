using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderPaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public OrderPaymentMethodType()
        {
            Field<ListGraphType<OrderTaxDetailType>>(nameof(PaymentMethod.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field(x => x.TaxPercentRate);
            Field(x => x.TaxTotal);
            Field(x => x.TaxType, true);
            Field(x => x.TypeName);
            Field(x => x.StoreId);
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
            Field(x => x.LogoUrl);
            Field(x => x.Code);
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodType), resolve: context => (int)context.Source.PaymentMethodType);
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodGroupType), resolve: context => (int)context.Source.PaymentMethodGroupType);

            //TODO
            //public ICollection<ObjectSettingEntry> Settings);
        }
    }
}
