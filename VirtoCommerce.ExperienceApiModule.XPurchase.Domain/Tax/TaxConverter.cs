using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Customer;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;
using dto = VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Tax
{
    public static partial class TaxConverter
    {
        public static TaxRate ToTaxRate(this dto.TaxRate taxRateDto, Currency currency)
        {
            var result = new TaxRate(currency)
            {
                Rate = new Money(taxRateDto.Rate, currency),
                PercentRate = taxRateDto.PercentRate
            };

            if (taxRateDto.Line != null)
            {
                result.Line = new TaxLine(currency)
                {
                    Code = taxRateDto.Line.Code,
                    Id = taxRateDto.Line.Id,
                    Name = taxRateDto.Line.Name,
                    Quantity = taxRateDto.Line.Quantity,
                    TaxType = taxRateDto.Line.TaxType,
                    TypeName = taxRateDto.Line.TypeName,

                    Amount = new Money(taxRateDto.Line.Amount, currency),
                    Price = new Money(taxRateDto.Line.Price, currency)
                };

                if (taxRateDto.TaxDetails != null)
                {
                    result.Line.TaxDetails = taxRateDto.TaxDetails.Select(x => x.ToTaxDetail(currency)).ToList();
                }
            }

            return result;
        }

        public static dto.TaxEvaluationContext ToTaxEvaluationContextDto(this TaxEvaluationContext taxContext)
        {
            var retVal = new dto.TaxEvaluationContext
            {
                Code = taxContext.Code,
                Id = taxContext.Id,
                Type = taxContext.Type
            };

            if (taxContext.Address != null)
            {
                retVal.Address = taxContext.Address.ToAddressDto();
            }

            retVal.Customer = taxContext?.Customer?.Contact?.ToCustomerDto();

            if (taxContext.Currency != null)
            {
                retVal.Currency = taxContext.Currency.Code;
            }

            retVal.Lines = new List<dto.TaxLine>();

            if (!taxContext.Lines.IsNullOrEmpty())
            {
                retVal.Lines = taxContext.Lines.Select(x => x.ToTaxLineDto()).ToList();

            }

            return retVal;
        }

        public static dto.TaxLine ToTaxLineDto(this TaxLine taxLine)
        {
            return new dto.TaxLine
            {
                Id = taxLine.Id,
                Code = taxLine.Code,
                Name = taxLine.Name,
                Quantity = taxLine.Quantity,
                TaxType = taxLine.TaxType,
                Amount = taxLine.Amount.Amount,
                Price = taxLine.Price.Amount,
                TypeName = taxLine.TypeName
            };
        }

        public static dto.Address ToAddressDto(this Address address)
        {
            var retVal = new dto.Address
            {
                Key = address.Key,
                City = address.City,
                CountryCode = address.CountryCode,
                CountryName = address.CountryName,
                Email = address.Email,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Line1 = address.Line1,
                Line2 = address.Line2,
                MiddleName = address.MiddleName,
                Name = address.Name,
                Organization = address.Organization,
                Phone = address.Phone,
                PostalCode = address.PostalCode,
                RegionId = address.RegionId,
                RegionName = address.RegionName,
                Zip = address.Zip,

                AddressType = (CoreModule.Core.Common.AddressType)Enum.Parse(typeof(CoreModule.Core.Common.AddressType), address.Type.ToString(), true),
            };

            return retVal;
        }

        public static dto.Customer ToCustomerDto(this Contact contact)
        {
            var retVal = new dto.Customer
            {
                Id = contact.Id,
                Name = contact.Name,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                MiddleName = contact.MiddleName,
            };

            if (!contact.UserGroups.IsNullOrEmpty())
            {
                retVal.Groups = contact.UserGroups.ToArray();
            }

            if (!contact.Addresses.IsNullOrEmpty())
            {
                retVal.Addresses = new List<dto.Address>();

                foreach (var address in contact.Addresses)
                {
                    var addressDto = address.ToAddressDto();
                    if (string.IsNullOrEmpty(addressDto.FirstName))
                    {
                        addressDto.FirstName = contact.FirstName;
                    }
                    if (string.IsNullOrEmpty(addressDto.LastName))
                    {
                        addressDto.LastName = contact.LastName;
                    }
                    if (string.IsNullOrEmpty(addressDto.Email))
                    {
                        addressDto.Email = contact.Email;
                    }
                    retVal.Addresses.Add(addressDto);
                }
            }

            if (!contact.Emails.IsNullOrEmpty())
            {
                retVal.Emails = contact.Emails;
            }

            // TODO: It needs to be rework to support only a multiple  organizations for a customer by design.
            if (contact.OrganizationId != null)
            {
                retVal.Organizations = new List<string>() { contact.OrganizationId };
            }

            if (contact.OrganizationsIds != null)
            {
                retVal.Organizations = contact.OrganizationsIds.Concat(retVal.Organizations ?? Array.Empty<string>()).Distinct().ToArray();
            }

            return retVal;
        }

        public static TaxDetail ToTaxDetail(this CoreModule.Core.Tax.TaxDetail taxDetailDto, Currency currency)
        {
            return new TaxDetail(currency)
            {
                Name = taxDetailDto.Name,
                Amount = new Money(taxDetailDto.Amount, currency),
                Rate = new Money(taxDetailDto.Rate, currency),
            };
        }
    }
}
