using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Domain.Factories;
using VirtoCommerce.XPurchase.Schemas;
using MediatR;
using AutoMapper;
namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXPurchase(this IServiceCollection services)
        {
            services.AddSchemaType<AddressType>();
            services.AddSchemaType<CartShipmentItemType>();
            services.AddSchemaType<CartType>();
            services.AddSchemaType<CopuponType>();
            services.AddSchemaType<CurrencyType>();
            services.AddSchemaType<DiscountType>();
            services.AddSchemaType<DynamicPropertyType>();
            services.AddSchemaType<LanguageType>();
            services.AddSchemaType<LineItemType>();
            services.AddSchemaType<MoneyType>();
            services.AddSchemaType<PaymentMethodType>();
            services.AddSchemaType<ShippingMethodType>();
            services.AddSchemaType<PaymentPlanType>();
            services.AddSchemaType<PaymentType>();
            services.AddSchemaType<SettingType>();
            services.AddSchemaType<ShipmentType>();
            services.AddSchemaType<StoreStatusEnum>();
            services.AddSchemaType<StoreType>();
            services.AddSchemaType<TaxDetailType>();
            services.AddSchemaType<UserType>();
            services.AddSchemaType<ValidationErrorType>();

            services.AddSchemaBuilder<PurchaseSchema>();

            //TODO: Move to single method serviceCollectionExtensions.AddPurchase in  the Purchase project
            services.AddTransient<ICartAggregateRepository, CartAggregateRepository>();

            services.AddMediatR(typeof(XPurchaseAnchor));

            services.AddAutoMapper(typeof(XPurchaseAnchor));

            return services;
        }
    }
}
