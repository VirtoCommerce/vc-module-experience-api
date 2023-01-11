using AutoMapper;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PaymentType : ExtendableGraphType<Payment>
    {
        private readonly IMemberService _memberService;
        private readonly IMapper _mapper;

        public PaymentType(IMapper mapper, IMemberService memberService, IDataLoaderContextAccessor dataLoader, IDynamicPropertyResolverService dynamicPropertyResolverService)
        {
            _mapper = mapper;
            _memberService = memberService;

            Field(x => x.Id, nullable: true).Description("Payment Id");
            Field(x => x.OuterId, nullable: true).Description("Value of payment outer id");
            Field(x => x.PaymentGatewayCode, nullable: true).Description("Value of payment gateway code");
            Field<CurrencyType>("currency",
                "Currency",
                resolve: context => context.GetCart().Currency);
            Field<MoneyType>("amount",
                "Amount",
                resolve: context => context.Source.Amount.ToMoney(context.GetCart().Currency));
            ExtendableField<CartAddressType>("billingAddress",
                "Billing address",
                resolve: context => context.Source.BillingAddress);
            Field<MoneyType>("price",
                "Price",
                resolve: context => context.Source.Price.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("priceWithTax",
                "Price with tax",
                resolve: context => context.Source.PriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("total",
                "Total",
                resolve: context => context.Source.Total.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("totalWithTax",
                "Total with tax",
                resolve: context => context.Source.TotalWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountAmount",
                "Discount amount",
                resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountAmountWithTax",
                "Discount amount with tax",
                resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("taxTotal",
                "Tax total",
                resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails",
                "Tax details",
                resolve: context => context.Source.TaxDetails);
            Field<ListGraphType<DiscountType>>("discounts",
                "Discounts",
                resolve: context => context.Source.Discounts);

            var vendorField = new FieldType
            {
                Name = "vendor",
                Type = GraphTypeExtenstionHelper.GetActualType<VendorType>(),
                Resolver = new FuncFieldResolver<Payment, IDataLoaderResult<ExpVendor>>(context =>
                {
                    var loader = dataLoader.GetVendorDataLoader(_memberService, _mapper, "cart_vendor");
                    return context.Source.VendorId != null ? loader.LoadAsync(context.Source.VendorId) : null;
                })
            };
            AddField(vendorField);

            ExtendableField<ListGraphType<DynamicPropertyValueType>>(
                "dynamicProperties",
                "Cart payment dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetArgumentOrValue<string>("cultureName")));
        }
    }
}
