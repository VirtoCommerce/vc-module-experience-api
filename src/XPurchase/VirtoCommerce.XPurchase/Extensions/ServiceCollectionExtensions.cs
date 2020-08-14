using System;
using AutoMapper;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.XPurchase.Authorization;
using VirtoCommerce.XPurchase.Schemas;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXPurchase(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSingleton<Func<SignInManager<ApplicationUser>>>(provider => () => provider.GetService<SignInManager<ApplicationUser>>());
            //TODO:
            //services.AddSchemaType<PaymentPlanType>();
            //services.AddSchemaType<SettingType>();
            //services.AddSchemaType<StoreStatusEnum>();
            //services.AddSchemaType<StoreType>();
            //services.AddSchemaType<UserType>();

            graphQlbuilder.AddGraphTypes(typeof(XPurchaseAnchor));


            //services.AddSchemaType<AddressType>();
            //services.AddSchemaType<CartShipmentItemType>();
            //services.AddSchemaType<CartType>();
            //services.AddSchemaType<CouponType>();
            //services.AddSchemaType<CurrencyType>();
            //services.AddSchemaType<DiscountType>();
            //services.AddSchemaType<DynamicPropertyType>();
            //services.AddSchemaType<InputAddCouponType>();
            //services.AddSchemaType<InputAddItemType>();
            //services.AddSchemaType<InputAddOrUpdateCartPaymentType>();
            //services.AddSchemaType<InputAddOrUpdateCartShipmentType>();
            //services.AddSchemaType<InputAddressType>();
            //services.AddSchemaType<InputCartShipmentItemType>();
            //services.AddSchemaType<InputChangeCartItemCommentType>();
            //services.AddSchemaType<InputChangeCartItemPriceType>();
            //services.AddSchemaType<InputChangeCartItemQuantityType>();
            //services.AddSchemaType<InputChangeCommentType>();
            //services.AddSchemaType<InputClearCartType>();
            //services.AddSchemaType<InputClearPaymentsType>();
            //services.AddSchemaType<InputClearShipmentsType>();
            //services.AddSchemaType<InputMergeCartType>();
            //services.AddSchemaType<InputPaymentType>();
            //services.AddSchemaType<InputRemoveCartType>();
            //services.AddSchemaType<InputRemoveCouponType>();
            //services.AddSchemaType<InputRemoveItemType>();
            //services.AddSchemaType<InputRemoveShipmentType>();
            //services.AddSchemaType<InputShipmentType>();
            //services.AddSchemaType<InputValidateCouponType>();
            //services.AddSchemaType<LanguageType>();
            //services.AddSchemaType<LineItemType>();
            //services.AddSchemaType<MoneyType>();
            //services.AddSchemaType<PaymentMethodType>();
            //services.AddSchemaType<PaymentType>();
            //services.AddSchemaType<ShipmentType>();
            //services.AddSchemaType<ShippingMethodType>();
            //services.AddSchemaType<TaxDetailType>();
            //services.AddSchemaType<ValidationErrorType>();

            services.AddSchemaBuilder<PurchaseSchema>();
            services.AddSingleton<IAuthorizationHandler, CanAccessCartAuthorizationHandler>();
            services.AddTransient<ICartAggregateRepository, CartAggregateRepository>();

            services.AddTransient<ICartValidationContextFactory, CartValidationContextFactory>();
            services.AddTransient<ICartAvailMethodsService, CartAvailMethodsService>();

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
