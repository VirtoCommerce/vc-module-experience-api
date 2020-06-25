using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XPurchase.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<NewCartItem, LineItem>().ConvertUsing((newCartItem, lineItem, context) =>
            {
                lineItem = AbstractTypeFactory<LineItem>.TryCreateInstance();

                //TODO:
                //lineItem.ValidationType

                //TODO:
                //lineItem.IsReadOnly = newCartItem.CartProduct.Product.IsReadOnly;

                //TODO:
                //lineItem.ShipmentMethodCode = newCartItem.CartProduct.Price.ShipmentMethodCode;

                //TODO:
                //lineItem.ThumbnailImageUrl = newCartItem.CartProduct.Product.ThumbnailImageUrl;

                //TODO:
                //lineItem.VolumetricWeight = newCartItem.CartProduct.Product.VolumetricWeight;

                lineItem.CatalogId = newCartItem.CartProduct.Product.CatalogId;
                lineItem.CategoryId = newCartItem.CartProduct.Product.CategoryId;
                lineItem.Currency = newCartItem.CartProduct.Price.Currency.Code;
                lineItem.DiscountAmount = newCartItem.CartProduct.Price.DiscountAmount.InternalAmount;
                lineItem.Discounts = newCartItem.CartProduct.Price.Discounts;
                lineItem.DynamicProperties = context.Mapper.Map<Dictionary<string, string>, ICollection<DynamicObjectProperty>>(newCartItem.DynamicProperties);
                lineItem.Height = newCartItem.CartProduct.Product.Height;
                lineItem.Id = newCartItem.CartProduct.Id;
                lineItem.ImageUrl = newCartItem.CartProduct.Product.ImgSrc;
                lineItem.Length = newCartItem.CartProduct.Product.Length;
                lineItem.ListPrice = newCartItem.CartProduct.Price.ListPrice.InternalAmount;
                lineItem.MeasureUnit = newCartItem.CartProduct.Product.MeasureUnit;
                lineItem.Name = newCartItem.CartProduct.Product.Name;
                lineItem.Note = newCartItem.Comment;
                lineItem.Price = context.Mapper.Map<ProductPrice, PricingModule.Core.Model.Price>(newCartItem.CartProduct.Price);
                lineItem.PriceId = newCartItem.CartProduct.Price.PricelistId;
                lineItem.ProductId = newCartItem.ProductId;
                lineItem.ProductId = newCartItem.ProductId;
                lineItem.ProductType = newCartItem.CartProduct.Product.ProductType;
                lineItem.Quantity = newCartItem.Quantity;
                lineItem.Quantity = newCartItem.Quantity;
                lineItem.SalePrice = newCartItem.CartProduct.Price.SalePrice.InternalAmount;
                lineItem.Sku = newCartItem.CartProduct.Product.Code;
                lineItem.TaxDetails = newCartItem.CartProduct.Price.TaxDetails;
                lineItem.TaxPercentRate = newCartItem.CartProduct.Price.TaxPercentRate;
                lineItem.TaxType = newCartItem.CartProduct.Product.TaxType;
                lineItem.Weight = newCartItem.CartProduct.Product.Weight;
                lineItem.WeightUnit = newCartItem.CartProduct.Product.WeightUnit;
                lineItem.Width = newCartItem.CartProduct.Product.Width;

                return lineItem;
            });

            //TODO: LineItem -> IEnumerable<TaxLine>
            //TODO: ShipingRate -> IEnumerable<TaxLine>
            //TODO: PaymentMethod -> IEnumerable<TaxLine>
            //TODO: ShoppingCart -> PriceEvaluationContext

            CreateMap<ExpProduct, LineItem>().ConvertUsing((cart, promoEvalcontext, context) =>
            {
                //TODO:
                var result = AbstractTypeFactory<LineItem>.TryCreateInstance();
                return result;
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
                        //Outline = cartProduct.Product.Outlines.Select(x => context.Mapper.Map<Tools.Models.Outline>(x)).GetOutlinePath(cartProduct.Product.CatalogId)
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
