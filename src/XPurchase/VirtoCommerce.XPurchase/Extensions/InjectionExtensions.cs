using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddXPurchaseSchemaTypes(this IServiceCollection services)
        {
            services.AddSchemaType<AddCartItemInputType>();
            services.AddSchemaType<AddressType>();
            services.AddSchemaType<CartShipmentItemType>();
            services.AddSchemaType<CartType>();
            services.AddSchemaType<CopuponType>();
            services.AddSchemaType<CartContextInputType>();
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
            services.AddSchemaType<ShippingMethodType>();
            services.AddSchemaType<StoreStatusEnum>();
            services.AddSchemaType<StoreType>();
            services.AddSchemaType<TaxDetailType>();
            services.AddSchemaType<UserType>();
            services.AddSchemaType<ValidationErrorType>();

            return services;
        }
    }
}
