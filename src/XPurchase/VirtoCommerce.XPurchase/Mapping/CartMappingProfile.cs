using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.Tools;

namespace VirtoCommerce.XPurchase.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            //TODO: NewCartItem -> LineItem
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
                foreach(var lineItem in cartAggr.Cart.Items)
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
                        Outline = cartProduct.Product.Outlines.Select(x=> context.Mapper.Map<Tools.Models.Outline>(x)).GetOutlinePath(cartProduct.Product.CatalogId)
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
