using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderPaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public OrderPaymentMethodType()
        {
            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(PaymentMethod.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field(x => x.TaxPercentRate);
            Field(x => x.TaxTotal);
            Field(x => x.TaxType);
            Field(x => x.TypeName);
            Field(x => x.StoreId);
            Field(x => x.DiscountAmountWithTax);
            Field(x => x.DiscountAmount);
            Field(x => x.TotalWithTax);
            Field(x => x.Total);
            Field(x => x.PriceWithTax);
            Field(x => x.Price);
            Field(x => x.Currency);
            Field(x => x.IsAvailableForPartial);
            Field(x => x.Priority);
            Field(x => x.IsActive);
            Field(x => x.LogoUrl);
            Field(x => x.Name);
            Field(x => x.Code);
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodType), resolve: context => (int)context.Source.PaymentMethodType);
            Field<IntGraphType>(nameof(PaymentMethod.PaymentMethodGroupType), resolve: context => (int)context.Source.PaymentMethodGroupType);

            //TODO
            //public ICollection<ObjectSettingEntry> Settings);
        }
    }
}
