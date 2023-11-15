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
using VirtoCommerce.Platform.Core.Settings;
using Money = VirtoCommerce.CoreModule.Core.Currency.Money;
using OrderSettings = VirtoCommerce.OrdersModule.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShipmentType : ExtendableGraphType<Shipment>
    {
        public OrderShipmentType(
            IMapper mapper,
            IMemberService memberService,
            IDataLoaderContextAccessor dataLoader,
            IDynamicPropertyResolverService dynamicPropertyResolverService,
            ILocalizableSettingService localizableSettingService)
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.OperationType, nullable: false);
            Field(x => x.ParentOperationId, nullable: true);
            Field(x => x.Number, nullable: false);
            Field(x => x.IsApproved, nullable: false);
            LocalizedField(x => x.Status, OrderSettings.ShipmentStatus, localizableSettingService, nullable: true);
            Field(x => x.Comment, nullable: true);
            Field(x => x.OuterId, nullable: true);
            Field(x => x.IsCancelled, nullable: false);
            Field(x => x.CancelledDate, nullable: true);
            Field(x => x.CancelReason, nullable: true);
            Field(x => x.ObjectType, nullable: false);

            Field(x => x.OrganizationId, nullable: true);
            Field(x => x.OrganizationName, nullable: true);
            Field(x => x.FulfillmentCenterId, nullable: true);
            Field(x => x.FulfillmentCenterName, nullable: true);
            Field(x => x.EmployeeId, nullable: true);
            Field(x => x.EmployeeName, nullable: true);
            Field(x => x.ShipmentMethodCode, nullable: true);
            Field(x => x.ShipmentMethodOption, nullable: true);
            Field<OrderShippingMethodType>(nameof(Shipment.ShippingMethod).ToCamelCase(), resolve: x => x.Source.ShippingMethod);
            Field(x => x.CustomerOrderId, nullable: true);
            Field(x => x.WeightUnit, nullable: true);
            Field(x => x.Weight, nullable: true);
            Field(x => x.MeasureUnit, nullable: true);
            Field(x => x.Height, nullable: true);
            Field(x => x.Length, nullable: true);
            Field(x => x.Width, nullable: true);
            ExtendableField<OrderAddressType>(nameof(Shipment.DeliveryAddress).ToCamelCase(), resolve: x => x.Source.DeliveryAddress);

            Field(x => x.TaxType, nullable: true);
            Field(x => x.TaxPercentRate, nullable: false);

            Field(x => x.TrackingNumber, nullable: true);
            Field(x => x.TrackingUrl, nullable: true);
            Field(x => x.DeliveryDate, nullable: true);

            Field<NonNullGraphType<MoneyType>>(nameof(Shipment.Price).ToCamelCase(),
                resolve: context => new Money(context.Source.Price, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(Shipment.PriceWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.PriceWithTax, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(Shipment.Total).ToCamelCase(),
                resolve: context => new Money(context.Source.Total, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(Shipment.TotalWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.TotalWithTax, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(Shipment.DiscountAmount).ToCamelCase(),
                resolve: context => new Money(context.Source.DiscountAmount, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(Shipment.DiscountAmountWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(Shipment.TaxTotal).ToCamelCase(),
                resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrency()));
            Field<NonNullGraphType<CurrencyType>>(nameof(Shipment.Currency).ToCamelCase(),
                resolve: context => context.GetOrderCurrency());

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<OrderTaxDetailType>>>>(nameof(Shipment.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<OrderShipmentItemType>>>>(nameof(Shipment.Items), resolve: x => x.Source.Items);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<OrderShipmentPackageType>>>>(nameof(Shipment.Packages), resolve: x => x.Source.Packages);
            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<PaymentInType>>>>(nameof(Shipment.InPayments), resolve: x => x.Source.InPayments);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<OrderDiscountType>>>>(nameof(Shipment.Discounts), resolve: x => x.Source.Discounts);

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

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<DynamicPropertyValueType>>>>(
                "dynamicProperties",
                "Customer order Shipment dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetCultureName()));
        }
    }
}
