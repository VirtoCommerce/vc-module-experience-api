using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;

using DynamicProperty = VirtoCommerce.ExperienceApiModule.XPurchase.Models.DynamicProperty;
using marketingDto = VirtoCommerce.MarketingModule.Core.Model.Promotions;

using cartDtos = VirtoCommerce.CartModule.Core.Model;
using coreDtos = VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Converters
{
    public static partial class CartConverter
    {
        public static cartDtos.ShoppingCartSearchCriteria ToSearchCriteriaDto(this CartSearchCriteria criteria)
        {
            var result = new cartDtos.ShoppingCartSearchCriteria
            {
                Name = criteria.Name,
                Type = criteria.Type,
                StoreId = criteria.StoreId,
                CustomerId = criteria.Customer?.Id,
                Currency = criteria.Currency?.Code,
                LanguageCode = criteria.Language?.CultureName,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.Sort
            };
            return result;
        }

        public static Platform.Core.DynamicProperties.DynamicObjectProperty ToCartDynamicPropertyDto(this DynamicProperty property)
            => property.ToDynamicPropertyDto().JsonConvert<Platform.Core.DynamicProperties.DynamicObjectProperty>();

        public static Discount ToDiscount(this CoreModule.Core.Common.Discount discountDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(discountDto.Currency)) ?? new Currency(language, discountDto.Currency);

            var result = new Discount(currency)
            {
                Coupon = discountDto.Coupon,
                Description = discountDto.Description,
                PromotionId = discountDto.PromotionId,
                Amount = new Money(discountDto.DiscountAmount, currency)
            };

            return result;
        }

        public static CoreModule.Core.Common.Discount ToCartDiscountDto(this Discount discount)
            => new CoreModule.Core.Common.Discount
            {
                PromotionId = discount.PromotionId,
                Coupon = discount.Coupon,
                Description = discount.Description,
                Currency = discount.Amount.Currency.Code,
                DiscountAmount = discount.Amount.Amount
            };

        public static ShippingMethod ToShippingMethod(this ShippingModule.Core.Model.ShippingMethod shippingMethod, Currency currency)
            => shippingMethod == null
                ? new ShippingMethod(currency)
                : new ShippingMethod(currency)
                {
                    LogoUrl = shippingMethod.LogoUrl,
                    Name = shippingMethod.Name,
                    Priority = shippingMethod.Priority,
                    TaxType = shippingMethod.TaxType,
                    ShipmentMethodCode = shippingMethod.Code,
                    Settings = shippingMethod?.Settings
                        .Where(x => !x.ValueType.ToString().EqualsInvariant("SecureString"))
                        .Select(x => x.JsonConvert<Platform.Core.Settings.ObjectSettingEntry>().ToSettingEntry())
                        .ToList()
                };

        public static Shipment ToCartShipment(this ShippingMethod shippingMethod, Currency currency)
            => new Shipment(currency)
            {
                ShipmentMethodCode = shippingMethod.ShipmentMethodCode,
                Price = shippingMethod.Price,
                DiscountAmount = shippingMethod.DiscountAmount,
                TaxType = shippingMethod.TaxType
            };

        public static TaxLine[] ToTaxLines(this ShippingMethod shipmentMethod)
            => new List<TaxLine>
            {
                new TaxLine(shipmentMethod.Currency)
                {
                    Id = shipmentMethod.BuildTaxLineId(),
                    Code = shipmentMethod.ShipmentMethodCode,
                    TaxType = shipmentMethod.TaxType,
                    //Special case when shipment method have 100% discount and need to calculate tax for old value
                    Amount = shipmentMethod.Total.Amount > 0 ? shipmentMethod.Total : shipmentMethod.Price
                }
            }.ToArray();

        public static TaxLine[] ToTaxLines(this PaymentMethod paymentMethod)
            => new List<TaxLine>
            {
                new TaxLine(paymentMethod.Currency)
                {
                    Id = paymentMethod.Code,
                    Code = paymentMethod.Code,
                    TaxType = paymentMethod.TaxType,
                     //Special case when payment method have 100% discount and need to calculate tax for old value
                    Amount = paymentMethod.Total.Amount > 0 ? paymentMethod.Total : paymentMethod.Price
                }
            }.ToArray();

        public static CartShipmentItem ToShipmentItem(this cartDtos.ShipmentItem shipmentItemDto, ShoppingCart cart)
        {
            return new CartShipmentItem
            {
                Id = shipmentItemDto.Id,
                Quantity = shipmentItemDto.Quantity,
                LineItem = cart.Items.FirstOrDefault(x => x.Id == shipmentItemDto.LineItemId)
            };
        }

        public static cartDtos.ShipmentItem ToShipmentItemDto(this CartShipmentItem shipmentItem)
        {
            var result = new cartDtos.ShipmentItem
            {
                Id = shipmentItem.Id,
                Quantity = shipmentItem.Quantity,
                LineItemId = shipmentItem.LineItem.Id,
                LineItem = shipmentItem.LineItem.ToLineItemDto()
            };

            return result;
        }

        public static Shipment ToShipment(this cartDtos.Shipment shipmentDto, ShoppingCart cart)
        {
            var retVal = new Shipment(cart.Currency)
            {
                Id = shipmentDto.Id,
                MeasureUnit = shipmentDto.MeasureUnit,
                ShipmentMethodCode = shipmentDto.ShipmentMethodCode,
                ShipmentMethodOption = shipmentDto.ShipmentMethodOption,
                WeightUnit = shipmentDto.WeightUnit,
                Height = (double?)shipmentDto.Height,
                Weight = (double?)shipmentDto.Weight,
                Width = (double?)shipmentDto.Width,
                Length = (double?)shipmentDto.Length,
                Currency = cart.Currency,
                Price = new Money(shipmentDto.Price, cart.Currency),
                PriceWithTax = new Money(shipmentDto.PriceWithTax, cart.Currency),
                DiscountAmount = new Money(shipmentDto.DiscountAmount, cart.Currency),
                Total = new Money(shipmentDto.Total, cart.Currency),
                TotalWithTax = new Money(shipmentDto.TotalWithTax, cart.Currency),
                DiscountAmountWithTax = new Money(shipmentDto.DiscountAmountWithTax, cart.Currency),
                TaxTotal = new Money(shipmentDto.TaxTotal, cart.Currency),
                TaxPercentRate = shipmentDto.TaxPercentRate,
                TaxType = shipmentDto.TaxType,
            };

            if (shipmentDto.DeliveryAddress != null)
            {
                retVal.DeliveryAddress = ToAddress(shipmentDto.DeliveryAddress);
            }

            if (shipmentDto.Items != null)
            {
                retVal.Items = shipmentDto.Items.Select(i => ToShipmentItem(i, cart)).ToList();
            }

            if (shipmentDto.TaxDetails != null)
            {
                retVal.TaxDetails = shipmentDto.TaxDetails.Select(td => ToTaxDetail(td, cart.Currency)).ToList();
            }

            if (!shipmentDto.Discounts.IsNullOrEmpty())
            {
                retVal.Discounts.AddRange(shipmentDto.Discounts.Select(x => ToDiscount(x, new[] { cart.Currency }, cart.Language)));
            }
            return retVal;
        }

        public static cartDtos.Shipment ToShipmentDto(this Shipment shipment)
            => new cartDtos.Shipment
            {
                Id = shipment.Id,
                MeasureUnit = shipment.MeasureUnit,
                ShipmentMethodCode = shipment.ShipmentMethodCode,
                ShipmentMethodOption = shipment.ShipmentMethodOption,
                WeightUnit = shipment.WeightUnit,
                Height = (decimal?)shipment.Height,
                Weight = (decimal?)shipment.Weight,
                Width = (decimal?)shipment.Width,
                Length = (decimal?)shipment.Length,

                Currency = shipment.Currency?.Code,
                DiscountAmount = shipment.DiscountAmount != null ? shipment.DiscountAmount.InternalAmount : 0,
                Price = shipment.Price != null ? shipment.Price.InternalAmount : 0,
                TaxPercentRate = shipment.TaxPercentRate,
                TaxType = shipment.TaxType,
                DeliveryAddress = shipment.DeliveryAddress?.ToCartAddressDto(),
                Discounts = shipment.Discounts?.Select(ToCartDiscountDto).ToList(),
                Items = shipment.Items?.Select(ToShipmentItemDto).ToList(),
                TaxDetails = shipment.TaxDetails?.Select(ToCartTaxDetailDto).ToList(),
            };

        public static PaymentMethod ToCartPaymentMethod(this PaymentModule.Core.Model.PaymentMethod paymentMethodDto, ShoppingCart cart)
        {
            var retVal = new PaymentMethod(cart.Currency)
            {
                Code = paymentMethodDto.Code,
                LogoUrl = paymentMethodDto.LogoUrl,
                Name = paymentMethodDto.Name,
                PaymentMethodGroupType = paymentMethodDto.PaymentMethodGroupType.ToString(),
                PaymentMethodType = paymentMethodDto.PaymentMethodType.ToString(),
                TaxType = paymentMethodDto.TaxType,

                Priority = paymentMethodDto.Priority
            };

            if (paymentMethodDto.Settings != null)
            {
                retVal.Settings = paymentMethodDto.Settings
                    .Where(x => !x.ValueType.ToString().EqualsInvariant("SecureString"))
                    .Select(x => x.JsonConvert<Platform.Core.Settings.ObjectSettingEntry>().ToSettingEntry())
                    .ToList();
            }

            retVal.Currency = cart.Currency;
            retVal.Price = new Money(paymentMethodDto.Price, cart.Currency);
            retVal.DiscountAmount = new Money(paymentMethodDto.DiscountAmount, cart.Currency);
            retVal.TaxPercentRate = paymentMethodDto.TaxPercentRate;

            if (paymentMethodDto.TaxDetails != null)
            {
                retVal.TaxDetails = paymentMethodDto.TaxDetails.Select(td => ToTaxDetail(td, cart.Currency)).ToList();
            }

            return retVal;
        }

        public static Payment ToCartPayment(this PaymentMethod paymentMethod, Money amount, ShoppingCart cart)
        {
            var result = new Payment(cart.Currency)
            {
                Amount = amount,
                PaymentGatewayCode = paymentMethod.Code,
                Price = paymentMethod.Price,
                DiscountAmount = paymentMethod.DiscountAmount,
                TaxPercentRate = paymentMethod.TaxPercentRate,
                TaxDetails = paymentMethod.TaxDetails
            };

            return result;
        }

        public static Payment ToPayment(this cartDtos.Payment paymentDto, ShoppingCart cart)
        {
            var result = new Payment(cart.Currency)
            {
                Id = paymentDto.Id,
                OuterId = paymentDto.OuterId,
                PaymentGatewayCode = paymentDto.PaymentGatewayCode,
                TaxType = paymentDto.TaxType,
                Amount = new Money(paymentDto.Amount, cart.Currency)
            };

            if (paymentDto.BillingAddress != null)
            {
                result.BillingAddress = ToAddress(paymentDto.BillingAddress);
            }

            result.Price = new Money(paymentDto.Price, cart.Currency);
            result.DiscountAmount = new Money(paymentDto.DiscountAmount, cart.Currency);
            result.PriceWithTax = new Money(paymentDto.PriceWithTax, cart.Currency);
            result.DiscountAmountWithTax = new Money(paymentDto.DiscountAmountWithTax, cart.Currency);
            result.Total = new Money(paymentDto.Total, cart.Currency);
            result.TotalWithTax = new Money(paymentDto.TotalWithTax, cart.Currency);
            result.TaxTotal = new Money(paymentDto.TaxTotal, cart.Currency);
            result.TaxPercentRate = paymentDto.TaxPercentRate;

            if (paymentDto.TaxDetails != null)
            {
                result.TaxDetails = paymentDto.TaxDetails.Select(td => ToTaxDetail(td, cart.Currency)).ToList();
            }
            if (!paymentDto.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(paymentDto.Discounts.Select(x => ToDiscount(x, new[] { cart.Currency }, cart.Language)));
            }
            return result;
        }

        public static Payment ToPayment(this cartDtos.Payment paymentDto, Currency currency, Language language)
        {
            var result = new Payment(currency)
            {
                Id = paymentDto.Id,
                OuterId = paymentDto.OuterId,
                PaymentGatewayCode = paymentDto.PaymentGatewayCode,
                TaxType = paymentDto.TaxType,
                Amount = new Money(paymentDto.Amount, currency)
            };

            if (paymentDto.BillingAddress != null)
            {
                result.BillingAddress = ToAddress(paymentDto.BillingAddress);
            }

            result.Price = new Money(paymentDto.Price, currency);
            result.DiscountAmount = new Money(paymentDto.DiscountAmount, currency);
            result.PriceWithTax = new Money(paymentDto.PriceWithTax, currency);
            result.DiscountAmountWithTax = new Money(paymentDto.DiscountAmountWithTax, currency);
            result.Total = new Money(paymentDto.Total, currency);
            result.TotalWithTax = new Money(paymentDto.TotalWithTax, currency);
            result.TaxTotal = new Money(paymentDto.TaxTotal, currency);
            result.TaxPercentRate = paymentDto.TaxPercentRate;

            if (paymentDto.TaxDetails != null)
            {
                result.TaxDetails = paymentDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }
            if (!paymentDto.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(paymentDto.Discounts.Select(x => ToDiscount(x, new[] { currency }, language)));
            }
            return result;
        }

        public static cartDtos.Payment ToPaymentDto(this Payment payment)
            => new cartDtos.Payment
            {
                Id = payment.Id,
                OuterId = payment.OuterId,
                PaymentGatewayCode = payment.PaymentGatewayCode,
                TaxType = payment.TaxType,
                Amount = payment.Amount.InternalAmount,
                Currency = payment.Currency.Code,
                Price = payment.Price.InternalAmount,
                DiscountAmount = payment.DiscountAmount.InternalAmount,
                TaxPercentRate = payment.TaxPercentRate,
                BillingAddress = payment.BillingAddress?.ToCartAddressDto(),
                Discounts = payment.Discounts?.Select(ToCartDiscountDto).ToList(),
                TaxDetails = payment.TaxDetails?.Select(ToCartTaxDetailDto).ToList()
            };

        public static cartDtos.Address ToCartAddressDto(this Address address)
            => address.ToCoreAddressDto().JsonConvert<cartDtos.Address>();

        public static Address ToAddress(this coreDtos.Address addressDto)
            => AddressConverter.ToAddress(addressDto.JsonConvert<coreDtos.Address>());

        public static PromotionEvaluationContext ToPromotionEvaluationContext(this ShoppingCart cart)
            => new PromotionEvaluationContext(cart.Language, cart.Currency)
            {
                Cart = cart,
                User = cart.Customer,
                Currency = cart.Currency,
                Language = cart.Language,
                StoreId = cart.StoreId
            };

        public static ShoppingCart ToShoppingCart(this cartDtos.ShoppingCart cartDto,
            Currency currency,
            Language language,
            User user)
        {
            var result = new ShoppingCart(currency, language)
            {
                ChannelId = cartDto.ChannelId,
                Comment = cartDto.Comment,
                CustomerId = cartDto.CustomerId,
                CustomerName = cartDto.CustomerName,
                Id = cartDto.Id,
                Name = cartDto.Name,
                ObjectType = cartDto.ObjectType,
                OrganizationId = cartDto.OrganizationId,
                Status = cartDto.Status,
                StoreId = cartDto.StoreId,
                Type = cartDto.Type,
                Customer = user,
                Coupons = cartDto.Coupons?
                    .Select(c => new Coupon
                    {
                        Code = c,
                        AppliedSuccessfully = !string.IsNullOrEmpty(c)
                    }).ToList(),

                Items = cartDto.Items
                    ?.Select(i => ToLineItem(i, currency, language))
                    .ToList(),

                HasPhysicalProducts = cartDto.Items
                    ?.Any(i => string.IsNullOrEmpty(i.ProductType)
                        || !string.IsNullOrEmpty(i.ProductType)
                        && i.ProductType.Equals("Physical", StringComparison.OrdinalIgnoreCase))
                    ?? false,

                Addresses = cartDto.Addresses?.Select(ToAddress).ToList(),
                Payments = cartDto.Payments?.Select(p => ToPayment(p, currency, language)).ToList()
            };

            if (cartDto.Shipments != null)
            {
                result.Shipments = cartDto.Shipments.Select(s => ToShipment(s, result)).ToList();
            }

            if (cartDto.DynamicProperties != null)
            {
                result.DynamicProperties = cartDto.DynamicProperties.Select(DynamicPropertyConverter.ToDynamicProperty).ToList();
            }

            if (cartDto.TaxDetails != null)
            {
                result.TaxDetails = cartDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            result.DiscountAmount = new Money(cartDto.DiscountAmount, currency);
            result.HandlingTotal = new Money(cartDto.HandlingTotal, currency);
            result.HandlingTotalWithTax = new Money(cartDto.HandlingTotalWithTax, currency);
            result.Total = new Money(cartDto.Total, currency);
            result.SubTotal = new Money(cartDto.SubTotal, currency);
            result.SubTotalWithTax = new Money(cartDto.SubTotalWithTax, currency);
            result.ShippingPrice = new Money(cartDto.ShippingSubTotal, currency);
            result.ShippingPriceWithTax = new Money(cartDto.ShippingSubTotalWithTax, currency);
            result.ShippingTotal = new Money(cartDto.ShippingTotal, currency);
            result.ShippingTotalWithTax = new Money(cartDto.ShippingTotalWithTax, currency);
            result.PaymentPrice = new Money(cartDto.PaymentSubTotal, currency);
            result.PaymentPriceWithTax = new Money(cartDto.PaymentSubTotalWithTax, currency);
            result.PaymentTotal = new Money(cartDto.PaymentTotal, currency);
            result.PaymentTotalWithTax = new Money(cartDto.PaymentTotalWithTax, currency);
            result.DiscountTotal = new Money(cartDto.DiscountTotal, currency);
            result.DiscountTotalWithTax = new Money(cartDto.DiscountTotalWithTax, currency);
            result.TaxTotal = new Money(cartDto.TaxTotal, currency);
            result.IsAnonymous = cartDto.IsAnonymous;
            result.IsRecuring = cartDto.IsRecuring == true;
            result.VolumetricWeight = cartDto.VolumetricWeight ?? 0M;
            result.Weight = cartDto.Weight ?? 0M;

            return result;
        }

        public static cartDtos.ShoppingCart ToShoppingCartDto(this ShoppingCart cart)
        {
            var result = new cartDtos.ShoppingCart
            {
                ChannelId = cart.ChannelId,
                Comment = cart.Comment,
                CustomerId = cart.CustomerId,
                CustomerName = cart.CustomerName,
                Id = cart.Id,
                Name = cart.Name,
                OrganizationId = cart.OrganizationId,
                Status = cart.Status,
                StoreId = cart.StoreId,
                Type = cart.Type,
                IsAnonymous = cart.IsAnonymous,
                LanguageCode = cart.Language?.CultureName
            };

            result.Addresses = cart.Addresses.Select(ToCartAddressDto).ToList();
            result.Coupons = cart.Coupons?.Select(c => c.Code).ToList();
            result.Currency = cart.Currency.Code;
            result.Discounts = cart.Discounts.Select(ToCartDiscountDto).ToList();
            result.HandlingTotal = cart.HandlingTotal.InternalAmount;
            result.HandlingTotalWithTax = cart.HandlingTotal.InternalAmount;
            result.DiscountAmount = cart.DiscountAmount.InternalAmount;
            result.Items = cart.Items.Select(ToLineItemDto).ToList();
            result.Payments = cart.Payments.Select(ToPaymentDto).ToList();
            result.Shipments = cart.Shipments.Select(ToShipmentDto).ToList();
            result.TaxDetails = cart.TaxDetails.Select(ToCartTaxDetailDto).ToList();
            result.DynamicProperties = cart.DynamicProperties.Select(ToCartDynamicPropertyDto).ToList();
            result.VolumetricWeight = cart.VolumetricWeight;
            result.Weight = cart.Weight;

            return result;
        }


        /// <summary>
        /// Get TaxEvaluationContext
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="taxCalculationEnabled">field from Store</param>
        /// <param name="fixedTaxRate">field from Store</param>
        public static TaxEvaluationContext ToTaxEvalContext(this ShoppingCart cart, bool taxCalculationEnabled, decimal fixedTaxRate)
        {
            var result = new TaxEvaluationContext(cart.StoreId)
            {
                Id = cart.Id,
                Code = cart.Name,
                Currency = cart.Currency,
                Type = "Cart",
                Customer = cart.Customer,
                StoreTaxCalculationEnabled = taxCalculationEnabled,
                FixedTaxRate = fixedTaxRate
            };

            foreach (var lineItem in cart.Items)
            {
                result.Lines.Add(new TaxLine(lineItem.Currency)
                {
                    Id = lineItem.Id,
                    Code = lineItem.Sku,
                    Name = lineItem.Name,
                    TaxType = lineItem.TaxType,
                    //Special case when product have 100% discount and need to calculate tax for old value
                    Amount = lineItem.ExtendedPrice.Amount > 0 ? lineItem.ExtendedPrice : lineItem.SalePrice,
                    Quantity = lineItem.Quantity,
                    Price = lineItem.PlacedPrice,
                    TypeName = "item"
                });
            }

            foreach (var shipment in cart.Shipments)
            {
                var totalTaxLine = new TaxLine(shipment.Currency)
                {
                    Id = shipment.Id,
                    Code = shipment.ShipmentMethodCode,
                    Name = shipment.ShipmentMethodOption,
                    TaxType = shipment.TaxType,
                    //Special case when shipment have 100% discount and need to calculate tax for old value
                    Amount = shipment.Total.Amount > 0 ? shipment.Total : shipment.Price,
                    TypeName = "shipment"
                };
                result.Lines.Add(totalTaxLine);

                if (shipment.DeliveryAddress != null)
                {
                    result.Address = shipment.DeliveryAddress;
                }
            }

            foreach (var payment in cart.Payments)
            {
                var totalTaxLine = new TaxLine(payment.Currency)
                {
                    Id = payment.Id,
                    Code = payment.PaymentGatewayCode,
                    Name = payment.PaymentGatewayCode,
                    TaxType = payment.TaxType,
                    //Special case when shipment have 100% discount and need to calculate tax for old value
                    Amount = payment.Total.Amount > 0 ? payment.Total : payment.Price,
                    TypeName = "payment"
                };
                result.Lines.Add(totalTaxLine);
            }
            return result;
        }

        public static TaxDetail ToTaxDetail(this CoreModule.Core.Tax.TaxDetail taxDeatilDto, Currency currency)
            => new TaxDetail(currency)
            {
                Name = taxDeatilDto.Name,
                Rate = new Money(taxDeatilDto.Rate, currency),
                Amount = new Money(taxDeatilDto.Amount, currency),
            };

        public static CoreModule.Core.Tax.TaxDetail ToCartTaxDetailDto(this TaxDetail taxDetail)
            => new CoreModule.Core.Tax.TaxDetail
            {
                Name = taxDetail.Name,
                Rate = taxDetail.Rate.Amount,
                Amount = taxDetail.Amount.Amount,
            };

        public static LineItem ToLineItem(this Product product, Language language, int quantity)
        {
            var result = new LineItem(product.Price.Currency, language)
            {
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Sku = product.Sku,
                ProductType = product.ProductType,
                TaxType = product.TaxType,
                WeightUnit = product.WeightUnit,
                MeasureUnit = product.MeasureUnit,
                Weight = product.Weight,
                Width = product.Width,
                Length = product.Length,
                Height = product.Height,

                ImageUrl = product.PrimaryImage?.Url,
                ThumbnailImageUrl = product.PrimaryImage?.Url,
                ListPrice = product.Price.ListPrice,
                SalePrice = product.Price.GetTierPrice(quantity).Price,
                TaxPercentRate = product.Price.TaxPercentRate,
                DiscountAmount = product.Price.DiscountAmount,
                ProductId = product.Id,
                Quantity = quantity
            };
            result.IsReccuring = result.PaymentPlan != null;

            return result;
        }

        public static LineItem ToLineItem(this cartDtos.LineItem lineItemDto, Currency currency, Language language)
        {
            var result = new LineItem(currency, language)
            {
                Id = lineItemDto.Id,
                IsReadOnly = lineItemDto.IsReadOnly,
                CatalogId = lineItemDto.CatalogId,
                CategoryId = lineItemDto.CategoryId,
                ImageUrl = lineItemDto.ImageUrl,
                Name = lineItemDto.Name,
                ObjectType = lineItemDto.ObjectType,
                ProductId = lineItemDto.ProductId,
                ProductType = lineItemDto.ProductType,
                Quantity = lineItemDto.Quantity < 1
                    ? 1
                    : lineItemDto.Quantity,
                ShipmentMethodCode = lineItemDto.ShipmentMethodCode,
                Sku = lineItemDto.Sku,
                TaxType = lineItemDto.TaxType,
                ThumbnailImageUrl = lineItemDto.ThumbnailImageUrl,
                WeightUnit = lineItemDto.WeightUnit,
                MeasureUnit = lineItemDto.MeasureUnit,
                Weight = lineItemDto.Weight,
                Width = lineItemDto.Width,
                Length = lineItemDto.Length,
                Height = lineItemDto.Height,
            };

            result.ImageUrl = lineItemDto.ImageUrl.RemoveLeadingUriScheme();

            if (lineItemDto.TaxDetails != null)
            {
                result.TaxDetails = lineItemDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            if (lineItemDto.DynamicProperties != null)
            {
                result.DynamicProperties = new MutablePagedList<DynamicProperty>(lineItemDto.DynamicProperties.Select(DynamicProperty).ToList());
            }

            if (!lineItemDto.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(lineItemDto.Discounts.Select(x => ToDiscount(x, new[] { currency }, language)));
            }

            result.Comment = lineItemDto.Note;
            result.IsGift = lineItemDto.IsGift;
            result.IsReccuring = lineItemDto.IsReccuring;
            result.ListPrice = new Money(lineItemDto.ListPrice, currency);
            result.RequiredShipping = lineItemDto.RequiredShipping;
            result.SalePrice = new Money(lineItemDto.SalePrice, currency);
            result.TaxPercentRate = lineItemDto.TaxPercentRate;
            result.DiscountAmount = new Money(lineItemDto.DiscountAmount, currency);
            result.TaxIncluded = lineItemDto.TaxIncluded;
            result.Weight = lineItemDto.Weight;
            result.Width = lineItemDto.Width;
            result.Height = lineItemDto.Height;
            result.Length = lineItemDto.Length;


            result.DiscountAmountWithTax = new Money(lineItemDto.DiscountAmountWithTax, currency);
            result.DiscountTotal = new Money(lineItemDto.DiscountTotal, currency);
            result.DiscountTotalWithTax = new Money(lineItemDto.DiscountTotalWithTax, currency);
            result.ListPriceWithTax = new Money(lineItemDto.ListPriceWithTax, currency);
            result.SalePriceWithTax = new Money(lineItemDto.SalePriceWithTax, currency);
            result.PlacedPrice = new Money(lineItemDto.PlacedPrice, currency);
            result.PlacedPriceWithTax = new Money(lineItemDto.PlacedPriceWithTax, currency);
            result.ExtendedPrice = new Money(lineItemDto.ExtendedPrice, currency);
            result.ExtendedPriceWithTax = new Money(lineItemDto.ExtendedPriceWithTax, currency);
            result.TaxTotal = new Money(lineItemDto.TaxTotal, currency);

            return result;
        }

        public static cartDtos.LineItem ToLineItemDto(this LineItem lineItem)
        {
            return new cartDtos.LineItem
            {
                Id = lineItem.Id,
                IsReadOnly = lineItem.IsReadOnly,
                CatalogId = lineItem.CatalogId,
                CategoryId = lineItem.CategoryId,
                ImageUrl = lineItem.ImageUrl,
                Name = lineItem.Name,
                ProductId = lineItem.ProductId,
                ProductType = lineItem.ProductType,
                Quantity = lineItem.Quantity,
                ShipmentMethodCode = lineItem.ShipmentMethodCode,
                Sku = lineItem.Sku,
                TaxType = lineItem.TaxType,
                ThumbnailImageUrl = lineItem.ThumbnailImageUrl,
                WeightUnit = lineItem.WeightUnit,
                MeasureUnit = lineItem.MeasureUnit,
                Weight = lineItem.Weight,
                Width = lineItem.Width,
                Length = lineItem.Length,
                Height = lineItem.Height,

                Currency = lineItem.Currency.Code,
                Discounts = lineItem.Discounts.Select(ToCartDiscountDto).ToList(),

                ListPrice = lineItem.ListPrice.InternalAmount,
                SalePrice = lineItem.SalePrice.InternalAmount,
                TaxPercentRate = lineItem.TaxPercentRate,
                DiscountAmount = lineItem.DiscountAmount.InternalAmount,
                TaxDetails = lineItem.TaxDetails.Select(ToCartTaxDetailDto).ToList(),
                DynamicProperties = lineItem.DynamicProperties.Select(ToCartDynamicPropertyDto).ToList(),
                VolumetricWeight = lineItem.VolumetricWeight ?? 0M
            };
        }

        public static CartShipmentItem ToShipmentItem(this LineItem lineItem)
        {
            var shipmentItem = new CartShipmentItem
            {
                LineItem = lineItem,
                Quantity = lineItem.Quantity
            };
            return shipmentItem;
        }

        public static marketingDto.ProductPromoEntry ToProductPromoEntryDto(this LineItem lineItem)
        {
            var result = new marketingDto.ProductPromoEntry
            {
                CatalogId = lineItem.CatalogId,
                CategoryId = lineItem.CategoryId,
                Code = lineItem.Sku,
                ProductId = lineItem.ProductId,
                Discount = lineItem.DiscountTotal.Amount,
                //Use only base price for discount evaluation
                Price = lineItem.SalePrice.Amount,
                Quantity = lineItem.Quantity,
                InStockQuantity = lineItem.InStockQuantity,
                Outline = lineItem.Product.Outline,
                Variations = null // TODO
            };

            return result;
        }
    }
}
