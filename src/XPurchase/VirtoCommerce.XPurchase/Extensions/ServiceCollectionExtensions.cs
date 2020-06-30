using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Schemas;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXPurchase(this IServiceCollection services)
        {
            //TODO:
            //services.AddSchemaType<PaymentPlanType>();
            //services.AddSchemaType<SettingType>();
            //services.AddSchemaType<StoreStatusEnum>();
            //services.AddSchemaType<StoreType>();
            //services.AddSchemaType<UserType>();

            services.AddSchemaType<AddressType>();
            services.AddSchemaType<CartShipmentItemType>();
            services.AddSchemaType<CartType>();
            services.AddSchemaType<CouponType>();
            services.AddSchemaType<CurrencyType>();
            services.AddSchemaType<DiscountType>();
            services.AddSchemaType<DynamicPropertyType>();
            services.AddSchemaType<InputAddCouponType>();
            services.AddSchemaType<InputAddItemType>();
            services.AddSchemaType<InputAddOrUpdateCartPaymentType>();
            services.AddSchemaType<InputAddOrUpdateCartShipmentType>();
            services.AddSchemaType<InputCartShipmentItemType>();
            services.AddSchemaType<InputChangeCartItemPriceType>();
            services.AddSchemaType<InputChangeCartItemQuantityType>();
            services.AddSchemaType<InputChangeCommentType>();
            services.AddSchemaType<InputClearCartType>();
            services.AddSchemaType<InputPaymentType>();
            services.AddSchemaType<InputRemoveCouponType>();
            services.AddSchemaType<InputRemoveItemType>();
            services.AddSchemaType<InputShipmentType>();
            services.AddSchemaType<InputAddressType>();
            services.AddSchemaType<InputValidateCouponType>();
            services.AddSchemaType<LanguageType>();
            services.AddSchemaType<LineItemType>();
            services.AddSchemaType<MoneyType>();
            services.AddSchemaType<PaymentMethodType>();
            services.AddSchemaType<PaymentType>();
            services.AddSchemaType<ShipmentType>();
            services.AddSchemaType<ShippingMethodType>();
            services.AddSchemaType<TaxDetailType>();
            services.AddSchemaType<ValidationErrorType>();

            services.AddSchemaBuilder<PurchaseSchema>();

            //TODO: Move to single method serviceCollectionExtensions.AddPurchase in  the Purchase project
            services.AddTransient<ICartAggregateRepository, CartAggregateRepository>();

            services.AddMediatR(typeof(XPurchaseAnchor));

            //TODO: Not work
            services.AddAutoMapper(typeof(XPurchaseAnchor));

            services.AddTransient<ICartProductService, CartProductService>();

            services.AddTransient<CartAggregate>();
            services.AddTransient<Func<CartAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<CartAggregate>());

            return services;
        }
    }
}
