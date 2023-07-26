using AutoMapper;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;
using Money = VirtoCommerce.CoreModule.Core.Currency.Money;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShipmentType : ExtendableGraphType<Shipment>
    {
        public OrderShipmentType(IMapper mapper, IMemberService memberService, IDataLoaderContextAccessor dataLoader, IDynamicPropertyResolverService dynamicPropertyResolverService)
        {
            Field(x => x.Id);
            Field(x => x.OperationType);
            Field(x => x.ParentOperationId, true);
            Field(x => x.Number);
            Field(x => x.IsApproved);
            Field(x => x.Status, true);
            Field(x => x.Comment, true);
            Field(x => x.OuterId, true);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate, true);
            Field(x => x.CancelReason, true);
            Field(x => x.ObjectType);

            Field(x => x.OrganizationId, true);
            Field(x => x.OrganizationName, true);
            Field(x => x.FulfillmentCenterId, true);
            Field(x => x.FulfillmentCenterName, true);
            Field(x => x.EmployeeId, true);
            Field(x => x.EmployeeName, true);
            Field(x => x.ShipmentMethodCode, true);
            Field(x => x.ShipmentMethodOption, true);
            Field<OrderShippingMethodType>(nameof(Shipment.ShippingMethod).ToCamelCase(), resolve: x => x.Source.ShippingMethod);
            Field(x => x.CustomerOrderId, true);
            Field(x => x.WeightUnit, true);
            Field(x => x.Weight, true);
            Field(x => x.MeasureUnit, true);
            Field(x => x.Height, true);
            Field(x => x.Length, true);
            Field(x => x.Width, true);
            ExtendableField<OrderAddressType>(nameof(Shipment.DeliveryAddress).ToCamelCase(), resolve: x => x.Source.DeliveryAddress);

            Field(x => x.TaxType, true);
            Field(x => x.TaxPercentRate);

            Field(x => x.TrackingNumber, true);
            Field(x => x.TrackingUrl, true);
            Field(x => x.DeliveryDate, true);

            Field<MoneyType>(nameof(Shipment.Price).ToCamelCase(), resolve: context => new Money(context.Source.Price, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(Shipment.PriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.PriceWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(Shipment.Total).ToCamelCase(), resolve: context => new Money(context.Source.Total, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(Shipment.TotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.TotalWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(Shipment.DiscountAmount).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmount, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(Shipment.DiscountAmountWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(Shipment.TaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrency()));
            Field<CurrencyType>(nameof(Shipment.Currency).ToCamelCase(), resolve: context => context.GetOrderCurrency());

            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(Shipment.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<OrderShipmentItemType>>>(nameof(Shipment.Items), resolve: x => x.Source.Items);
            Field<NonNullGraphType<ListGraphType<OrderShipmentPackageType>>>(nameof(Shipment.Packages), resolve: x => x.Source.Packages);
            ExtendableField<NonNullGraphType<ListGraphType<PaymentInType>>>(nameof(Shipment.InPayments), resolve: x => x.Source.InPayments);
            Field<NonNullGraphType<ListGraphType<OrderDiscountType>>>(nameof(Shipment.Discounts), resolve: x => x.Source.Discounts);

            var vendorField = new FieldType
            {
                Name = "vendor",
                Type = GraphTypeExtenstionHelper.GetActualType<VendorType>(),
                Resolver = new FuncFieldResolver<Shipment, IDataLoaderResult<ExpVendor>>(context =>
                {
                    return dataLoader.LoadVendor(memberService, mapper, loaderKey: "order_vendor", vendorId: context.Source.VendorId);
                })
            };
            AddField(vendorField);

            ExtendableField<ListGraphType<DynamicPropertyValueType>>(
                "dynamicProperties",
                "Customer order Shipment dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetArgumentOrValue<string>("cultureName")));
        }
    }
}
