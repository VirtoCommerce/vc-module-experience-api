using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.Tools;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Data.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<CartModule.Core.Model.Address, TaxModule.Core.Model.Address>();

            CreateMap<GiftReward, GiftItem>();
            CreateMap<GiftItem, LineItem>();
            CreateMap<GiftItem, GiftLineItem>();

            CreateMap<CartProduct, LineItem>().ConvertUsing((cartProduct, lineItem, context) =>
            {
                if (lineItem == null)
                {
                    lineItem = AbstractTypeFactory<LineItem>.TryCreateInstance();
                }
                //PT-5453: Add mapping to CartMappingProfile
                //lineItem.ValidationType
                //lineItem.IsReadOnly = newCartItem.CartProduct.Product.IsReadOnly;
                //lineItem.ShipmentMethodCode = newCartItem.CartProduct.Price.ShipmentMethodCode;
                //lineItem.ThumbnailImageUrl = newCartItem.CartProduct.Product.ThumbnailImageUrl;
                //lineItem.VolumetricWeight = newCartItem.CartProduct.Product.VolumetricWeight;

                lineItem.CatalogId = cartProduct.Product.CatalogId;
                lineItem.CategoryId = cartProduct.Product.CategoryId;
                if (cartProduct.Price != null)
                {
                    lineItem.Currency = cartProduct.Price.Currency.Code;
                    lineItem.DiscountAmount = cartProduct.Price.DiscountAmount.InternalAmount;
                    lineItem.PriceId = cartProduct.Price.PricelistId;
                    lineItem.SalePrice = cartProduct.Price.SalePrice.InternalAmount;
                    lineItem.TaxDetails = cartProduct.Price.TaxDetails;
                    lineItem.TaxPercentRate = cartProduct.Price.TaxPercentRate;
                    lineItem.Discounts = cartProduct.Price.Discounts;
                    lineItem.ListPrice = cartProduct.Price.ListPrice.InternalAmount;
                }

                lineItem.Height = cartProduct.Product.Height;
                lineItem.ImageUrl = cartProduct.Product.ImgSrc;
                lineItem.Length = cartProduct.Product.Length;
                lineItem.MeasureUnit = cartProduct.Product.MeasureUnit;
                lineItem.Name = cartProduct.Product.Name;
                lineItem.ProductOuterId = cartProduct.Product.OuterId;
                lineItem.ProductId = cartProduct.Product.Id;
                lineItem.ProductType = cartProduct.Product.ProductType;
                lineItem.Sku = cartProduct.Product.Code;
                lineItem.TaxType = cartProduct.Product.TaxType;
                lineItem.Weight = cartProduct.Product.Weight;
                lineItem.WeightUnit = cartProduct.Product.WeightUnit;
                lineItem.Width = cartProduct.Product.Width;
                lineItem.FulfillmentCenterId = cartProduct.Inventory?.FulfillmentCenterId;
                lineItem.FulfillmentCenterName = cartProduct.Inventory?.FulfillmentCenterName;
                lineItem.VendorId = cartProduct.Product.Vendor;

                return lineItem;
            });

            CreateMap<Outline, Tools.Models.Outline>();
            CreateMap<OutlineItem, Tools.Models.OutlineItem>();
            CreateMap<SeoInfo, Tools.Models.SeoInfo>();

            CreateMap<LineItem, IEnumerable<TaxLine>>().ConvertUsing((lineItem, taxLines, context) =>
            {
                return new[]
                {
                    new TaxLine
                    {
                        Id = lineItem.Id,
                        Code = lineItem.Sku,
                        Name = lineItem.Name,
                        TaxType = lineItem.TaxType,
                        //Special case when product have 100% discount and need to calculate tax for old value
                        Amount =  lineItem.ListPrice > 0 ? lineItem.ListPrice : lineItem.SalePrice
                    }
                };
            });

            CreateMap<ShippingRate, IEnumerable<TaxLine>>().ConvertUsing((shipmentRate, taxLines, context) =>
            {
                return new[]
                {
                    new TaxLine
                    {
                        Id = string.Join("&", shipmentRate.ShippingMethod.Code, shipmentRate.OptionName),
                        Code = shipmentRate.ShippingMethod.Code,
                        TaxType = shipmentRate.ShippingMethod.TaxType,
                        Amount = shipmentRate.DiscountAmount > 0 ? shipmentRate.DiscountAmount : shipmentRate.Rate
                    }
                };
            });

            CreateMap<PaymentMethod, IEnumerable<TaxLine>>().ConvertUsing((paymentMethod, taxLines, context) =>
            {
                return new[]
                {
                    new TaxLine
                    {
                        Id = paymentMethod.Code,
                        Code = paymentMethod.Code,
                        TaxType = paymentMethod.TaxType,
                        Amount = paymentMethod.Total > 0 ? paymentMethod.Total : paymentMethod.Price
                    }
                };
            });

            CreateMap<CartAggregate, PriceEvaluationContext>().ConvertUsing((cartAggr, priceEvalContext, context) =>
            {
                priceEvalContext = AbstractTypeFactory<PriceEvaluationContext>.TryCreateInstance();
                priceEvalContext.Language = cartAggr.Cart.LanguageCode;
                priceEvalContext.StoreId = cartAggr.Cart.StoreId;
                priceEvalContext.CatalogId = cartAggr.Store.Catalog;
                priceEvalContext.Currency = cartAggr.Cart.Currency;

                var contact = cartAggr.Member;
                if (contact != null)
                {
                    priceEvalContext.CustomerId = contact.Id;

                    //priceEvalContext.GeoTimeZone = contact.TimeZome;

                    var address = contact.Addresses.FirstOrDefault(x => x.AddressType == CoreModule.Core.Common.AddressType.Shipping)
                               ?? contact.Addresses.FirstOrDefault(x => x.AddressType == CoreModule.Core.Common.AddressType.Billing);

                    if (address != null)
                    {
                        priceEvalContext.GeoCity = address.City;
                        priceEvalContext.GeoCountry = address.CountryCode;
                        priceEvalContext.GeoState = address.RegionName;
                        priceEvalContext.GeoZipCode = address.PostalCode;
                    }
                    if (contact.Groups != null)
                    {
                        priceEvalContext.UserGroups = contact.Groups.ToArray();
                    }
                }

                //if (pricelists != null)
                //{
                //    result.PricelistIds = pricelists.Select(p => p.Id).ToList();
                //}
                //if (products != null)
                //{
                //    result.ProductIds = products.Select(p => p.Id).ToList();
                //}
                return priceEvalContext;
            });

            CreateMap<LineItem, ProductPromoEntry>()
                .ConvertUsing((lineItem, productPromoEntry, context) =>
                {
                    if (productPromoEntry == null)
                    {
                        productPromoEntry = AbstractTypeFactory<ProductPromoEntry>.TryCreateInstance();
                    }
                    //PT-5453: Add mapping to CartMappingProfile
                    // productPromoEntry.InStockQuantity = lineItem.InStockQuantity;
                    // productPromoEntry.Outline = lineItem.Product.Outline;
                    // productPromoEntry.Variations = null;

                    productPromoEntry.CatalogId = lineItem.CatalogId;
                    productPromoEntry.CategoryId = lineItem.CategoryId;
                    productPromoEntry.Code = lineItem.Sku;
                    productPromoEntry.Discount = lineItem.DiscountTotal;
                    //Use only base price for discount evaluation
                    productPromoEntry.Price = lineItem.SalePrice;
                    productPromoEntry.ProductId = lineItem.ProductId;
                    productPromoEntry.Quantity = lineItem.Quantity;

                    return productPromoEntry;
                });

            CreateMap<CartAggregate, PromotionEvaluationContext>().ConvertUsing((cartAggr, promoEvalcontext, context) =>
            {
                if (promoEvalcontext == null)
                {
                    promoEvalcontext = AbstractTypeFactory<PromotionEvaluationContext>.TryCreateInstance();
                }

                promoEvalcontext.CartPromoEntries = new List<ProductPromoEntry>();

                foreach (var lineItem in cartAggr.SelectedLineItems)
                {
                    var promoEntry = context.Mapper.Map<ProductPromoEntry>(lineItem);
                    var cartProduct = cartAggr.CartProducts[lineItem.ProductId];
                    if (cartProduct != null)
                    {
                        promoEntry.InStockQuantity = (int)(cartProduct.Inventory?.InStockQuantity ?? 0);
                        promoEntry.Outline = cartProduct.Product.Outlines?.Select(x => context.Mapper.Map<Tools.Models.Outline>(x)).GetOutlinePath(cartProduct.Product.CatalogId);
                    }
                    promoEvalcontext.CartPromoEntries.Add(promoEntry);
                }

                promoEvalcontext.CartTotal = cartAggr.Cart.SubTotal;
                promoEvalcontext.StoreId = cartAggr.Cart.StoreId;
                promoEvalcontext.Coupons = cartAggr.Cart.Coupons?.ToList();
                promoEvalcontext.Currency = cartAggr.Cart.Currency;
                promoEvalcontext.CustomerId = promoEvalcontext.UserId = cartAggr.Cart.CustomerId;
                promoEvalcontext.ContactId = cartAggr.Member?.Id;
                promoEvalcontext.OrganizaitonId = cartAggr.Cart.OrganizationId;
                promoEvalcontext.UserGroups = cartAggr.Member?.Groups.ToArray();
                promoEvalcontext.IsRegisteredUser = !cartAggr.Cart.IsAnonymous;
                promoEvalcontext.Language = cartAggr.Cart.LanguageCode;
                //Set cart line items as default promo items
                promoEvalcontext.PromoEntries = promoEvalcontext.CartPromoEntries;

                if (!cartAggr.Cart.Shipments.IsNullOrEmpty())
                {
                    var shipment = cartAggr.Cart.Shipments.First();
                    promoEvalcontext.ShipmentMethodCode = shipment.ShipmentMethodCode;
                    promoEvalcontext.ShipmentMethodOption = shipment.ShipmentMethodOption;
                    promoEvalcontext.ShipmentMethodPrice = shipment.Price;
                }
                if (!cartAggr.Cart.Payments.IsNullOrEmpty())
                {
                    var payment = cartAggr.Cart.Payments.First();
                    promoEvalcontext.PaymentMethodCode = payment.PaymentGatewayCode;
                    promoEvalcontext.PaymentMethodPrice = payment.Price;
                }

                promoEvalcontext.IsEveryone = true;
                //promoEvalcontext.IsFirstTimeBuyer = cartAggr.IsFirstBuyer;

                return promoEvalcontext;
            });

            //PT-5457: Need to think about extensibility for converters
            CreateMap<CartAggregate, TaxEvaluationContext>().ConvertUsing((cartAggr, taxEvalcontext, context) =>
            {
                if (taxEvalcontext == null)
                {
                    taxEvalcontext = AbstractTypeFactory<TaxEvaluationContext>.TryCreateInstance();
                }
                taxEvalcontext.StoreId = cartAggr.Cart.StoreId;
                taxEvalcontext.Code = cartAggr.Cart.Name;
                taxEvalcontext.Type = "Cart";
                taxEvalcontext.CustomerId = cartAggr.Cart.CustomerId;
                //map customer after PT-5425

                foreach (var lineItem in cartAggr.SelectedLineItems)
                {
                    taxEvalcontext.Lines.Add(new TaxLine()
                    {
                        //PT-5339: Add Currency to tax line
                        Id = lineItem.Id,
                        Code = lineItem.Sku,
                        Name = lineItem.Name,
                        TaxType = lineItem.TaxType,
                        //Special case when product have 100% discount and need to calculate tax for old value
                        Amount = lineItem.ExtendedPrice > 0 ? lineItem.ExtendedPrice : lineItem.SalePrice,
                        Quantity = lineItem.Quantity,
                        Price = lineItem.PlacedPrice,
                        TypeName = "item"
                    });
                }

                foreach (var shipment in cartAggr.Cart.Shipments ?? Array.Empty<Shipment>())
                {
                    var totalTaxLine = new TaxLine
                    {
                        //PT-5339: Add Currency to tax line
                        Id = shipment.Id,
                        Code = shipment.ShipmentMethodCode,
                        Name = shipment.ShipmentMethodOption,
                        TaxType = shipment.TaxType,
                        //Special case when shipment have 100% discount and need to calculate tax for old value
                        Amount = shipment.Total > 0 ? shipment.Total : shipment.Price,
                        TypeName = "shipment"
                    };
                    taxEvalcontext.Lines.Add(totalTaxLine);

                    if (shipment.DeliveryAddress != null)
                    {
                        taxEvalcontext.Address = context.Mapper.Map<TaxModule.Core.Model.Address>(shipment.DeliveryAddress);
                    }
                }

                foreach (var payment in cartAggr.Cart.Payments ?? Array.Empty<Payment>())
                {
                    var totalTaxLine = new TaxLine
                    {
                        //PT-5339: Add Currency to tax line
                        Id = payment.Id,
                        Code = payment.PaymentGatewayCode,
                        Name = payment.PaymentGatewayCode,
                        TaxType = payment.TaxType,
                        //Special case when shipment have 100% discount and need to calculate tax for old value
                        Amount = payment.Total > 0 ? payment.Total : payment.Price,
                        TypeName = "payment"
                    };
                    taxEvalcontext.Lines.Add(totalTaxLine);
                }
                return taxEvalcontext;
            });

            CreateMap<IList<IFilter>, ShoppingCartSearchCriteria>()
              .ConvertUsing((terms, criteria, context) =>
              {
                  foreach (var term in terms.OfType<TermFilter>())
                  {
                      term.MapTo(criteria);
                  }

                  return criteria;
              });
        }
    }

}
