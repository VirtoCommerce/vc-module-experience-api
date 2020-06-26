using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.Tools;

namespace VirtoCommerce.XPurchase.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<CartProduct, LineItem>().ConvertUsing((cartProduct, lineItem, context) =>
            {
                lineItem = AbstractTypeFactory<LineItem>.TryCreateInstance();

                //TODO:
                //lineItem.ValidationType
                //lineItem.IsReadOnly = newCartItem.CartProduct.Product.IsReadOnly;
                //lineItem.ShipmentMethodCode = newCartItem.CartProduct.Price.ShipmentMethodCode;
                //lineItem.ThumbnailImageUrl = newCartItem.CartProduct.Product.ThumbnailImageUrl;
                //lineItem.VolumetricWeight = newCartItem.CartProduct.Product.VolumetricWeight;

                lineItem.CatalogId = cartProduct.Product.CatalogId;
                lineItem.CategoryId = cartProduct.Product.CategoryId;
                lineItem.Currency = cartProduct.Price.Currency.Code;
                lineItem.DiscountAmount = cartProduct.Price.DiscountAmount.InternalAmount;
                lineItem.Discounts = cartProduct.Price.Discounts;
                lineItem.Height = cartProduct.Product.Height;
                lineItem.ImageUrl = cartProduct.Product.ImgSrc;
                lineItem.Length = cartProduct.Product.Length;
                lineItem.ListPrice = cartProduct.Price.ListPrice.InternalAmount;
                lineItem.MeasureUnit = cartProduct.Product.MeasureUnit;
                lineItem.Name = cartProduct.Product.Name;
                lineItem.PriceId = cartProduct.Price.PricelistId;
                lineItem.ProductId = cartProduct.Product.Id;
                lineItem.ProductType = cartProduct.Product.ProductType;
                lineItem.SalePrice = cartProduct.Price.SalePrice.InternalAmount;
                lineItem.Sku = cartProduct.Product.Code;
                lineItem.TaxDetails = cartProduct.Price.TaxDetails;
                lineItem.TaxPercentRate = cartProduct.Price.TaxPercentRate;
                lineItem.TaxType = cartProduct.Product.TaxType;
                lineItem.Weight = cartProduct.Product.Weight;
                lineItem.WeightUnit = cartProduct.Product.WeightUnit;
                lineItem.Width = cartProduct.Product.Width;

                return lineItem;
            });

            CreateMap<Outline, Tools.Models.Outline>();
            CreateMap<OutlineItem, Tools.Models.OutlineItem>();
            CreateMap<SeoInfo, Tools.Models.SeoInfo>();

            //TODO:
            // Check if this correct
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
                        Amount =  lineItem.Price.List > 0 ? lineItem.Price.List : lineItem.Price.Sale ?? 0M
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
                        //TODO: Is second param is shipmentRate.Rate ?
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

            CreateMap<ShoppingCart, PriceEvaluationContext>().ConvertUsing((cart, priceEvalContext, context) =>
            {
                priceEvalContext = AbstractTypeFactory<PriceEvaluationContext>.TryCreateInstance();
                priceEvalContext.Language = cart.LanguageCode;
                priceEvalContext.StoreId = cart.StoreId;
                //TODO:
                //if (cart.CustomerId != null)
                //{
                //    result.CustomerId = workContext.CurrentUser.Id;
                //    var contact = workContext.CurrentUser?.Contact;

                //    if (contact != null)
                //    {
                //        result.GeoTimeZone = contact.TimeZone;
                //        var address = contact.DefaultShippingAddress ?? contact.DefaultBillingAddress;
                //        if (address != null)
                //        {
                //            result.GeoCity = address.City;
                //            result.GeoCountry = address.CountryCode;
                //            result.GeoState = address.RegionName;
                //            result.GeoZipCode = address.PostalCode;
                //        }
                //        if (contact.UserGroups != null)
                //        {
                //            result.UserGroups = contact.UserGroups;
                //        }
                //    }
                //}
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

            CreateMap<CartAggregate, PromotionEvaluationContext>().ConvertUsing((cartAggr, promoEvalcontext, context) =>
            {
                promoEvalcontext = AbstractTypeFactory<PromotionEvaluationContext>.TryCreateInstance();
                //TODO: Add mapping config for ProductPromoEntry
                promoEvalcontext.CartPromoEntries = new List<ProductPromoEntry>();
                foreach (var lineItem in cartAggr.Cart.Items)
                {
                    var cartProduct = cartAggr.CartProductsDict[lineItem.ProductId];
                    var promoEntry = new ProductPromoEntry
                    {
                        CatalogId = lineItem.CatalogId,
                        CategoryId = lineItem.CategoryId,
                        Code = lineItem.Sku,
                        ProductId = lineItem.ProductId,
                        Discount = lineItem.DiscountTotal,
                        //Use only base price for discount evaluation
                        Price = lineItem.SalePrice,
                        Quantity = lineItem.Quantity,
                        InStockQuantity = (int)cartProduct.Inventory.InStockQuantity,
                        Outline = cartProduct.Product.Outlines.Select(x => context.Mapper.Map<Tools.Models.Outline>(x)).GetOutlinePath(cartProduct.Product.CatalogId)
                    };
                    promoEvalcontext.CartPromoEntries.Add(promoEntry);
                }
                promoEvalcontext.CartTotal = cartAggr.Cart.SubTotal;
                promoEvalcontext.StoreId = cartAggr.Cart.StoreId;
                promoEvalcontext.Coupons = cartAggr.Cart.Coupons?.ToList();
                promoEvalcontext.Currency = cartAggr.Cart.Currency;
                promoEvalcontext.CustomerId = cartAggr.Cart.CustomerId;
                //TODO:
                //promoEvalcontext.UserGroups = cart.Customer?.Contact?.UserGroups;
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
                //TODO:
                //promoEvalcontext.IsFirstTimeBuyer = cart.User.IsFirstTimeBuyer;

                return promoEvalcontext;
            });

            //TODO: Need to think about extensibility for converters
            CreateMap<CartAggregate, TaxEvaluationContext>().ConvertUsing((cartAggr, taxEvalcontext, context) =>
            {
                taxEvalcontext = AbstractTypeFactory<TaxEvaluationContext>.TryCreateInstance();
                taxEvalcontext.StoreId = cartAggr.Cart.StoreId;
                taxEvalcontext.Code = cartAggr.Cart.Name;
                taxEvalcontext.Type = "Cart";
                taxEvalcontext.CustomerId = cartAggr.Cart.CustomerId;
                //TODO: Customer

                foreach (var lineItem in cartAggr.Cart.Items)
                {
                    taxEvalcontext.Lines.Add(new TaxLine()
                    {
                        //TODO: Add Currency to tax line
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

                foreach (var shipment in cartAggr.Cart.Shipments)
                {
                    var totalTaxLine = new TaxLine
                    {
                        //TODO: Add Currency to tax line
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

                foreach (var payment in cartAggr.Cart.Payments)
                {
                    var totalTaxLine = new TaxLine
                    {
                        //TODO: Add Currency to tax line
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
        }
    }
}
