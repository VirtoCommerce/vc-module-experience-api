using System.Linq;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XPurchase.Domain.Mapping
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            //TODO: NewCartItem -> LineItem
            //TODO: LineItem -> IEnumerable<TaxLine>
            //TODO: ShipingRate -> IEnumerable<TaxLine>
            //TODO: PaymentMethod -> IEnumerable<TaxLine>


            CreateMap<ExpProduct, LineItem>().ConvertUsing((cart, promoEvalcontext, context) =>
            {
                //TODO:
                var result = AbstractTypeFactory<LineItem>.TryCreateInstance();
                return result;
            });

            CreateMap<ShoppingCart, PromotionEvaluationContext>().ConvertUsing((cart, promoEvalcontext, context) =>
            {
                promoEvalcontext = AbstractTypeFactory<PromotionEvaluationContext>.TryCreateInstance();
                //TODO: Add mapping config for ProductPromoEntry
                promoEvalcontext.CartPromoEntries = cart.Items.Select(x => context.Mapper.Map<ProductPromoEntry>(x)).ToList();

                promoEvalcontext.CartTotal = cart.SubTotal;
                promoEvalcontext.StoreId = cart.StoreId;
                promoEvalcontext.Coupons = cart.Coupons?.ToList();
                promoEvalcontext.Currency = cart.Currency;
                promoEvalcontext.CustomerId = cart.CustomerId;
                //TODO:
                //promoEvalcontext.UserGroups = cart.Customer?.Contact?.UserGroups;
                promoEvalcontext.IsRegisteredUser = !cart.IsAnonymous;
                promoEvalcontext.Language = cart.LanguageCode;
                //Set cart line items as default promo items
                promoEvalcontext.PromoEntries = promoEvalcontext.CartPromoEntries;

                if (!cart.Shipments.IsNullOrEmpty())
                {
                    var shipment = cart.Shipments.First();
                    promoEvalcontext.ShipmentMethodCode = shipment.ShipmentMethodCode;
                    promoEvalcontext.ShipmentMethodOption = shipment.ShipmentMethodOption;
                    promoEvalcontext.ShipmentMethodPrice = shipment.Price;
                }
                if (!cart.Payments.IsNullOrEmpty())
                {
                    var payment = cart.Payments.First();
                    promoEvalcontext.PaymentMethodCode = payment.PaymentGatewayCode;
                    promoEvalcontext.PaymentMethodPrice = payment.Price;
                }

                promoEvalcontext.IsEveryone = true;
                //TODO:
                //promoEvalcontext.IsFirstTimeBuyer = cart.User.IsFirstTimeBuyer;

                return promoEvalcontext;
            });

            //TODO: Need to think about extensibility for converters
            CreateMap<ShoppingCart, TaxEvaluationContext>().ConvertUsing((cart, taxEvalcontext, context) =>
            {
                taxEvalcontext = AbstractTypeFactory<TaxEvaluationContext>.TryCreateInstance();
                taxEvalcontext.StoreId = cart.StoreId;
                taxEvalcontext.Code = cart.Name;
                taxEvalcontext.Type = "Cart";
                taxEvalcontext.CustomerId = cart.CustomerId;
                //TODO: Customer


                foreach (var lineItem in cart.Items)
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

                foreach (var shipment in cart.Shipments)
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

                foreach (var payment in cart.Payments)
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
