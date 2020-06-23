using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.XPurchase.Models;
using VirtoCommerce.XPurchase.Models.Catalog;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Domain.Converters
{
    public static class ProductExtensions
    {
        public static Product ToProduct(this CatalogProduct productDto, Language currentLanguage, Currency currentCurrency/*, Store store*/)
        {
            var result = new Product(currentCurrency)
            {
                Id = productDto.Id,
                TitularItemId = productDto.TitularItemId,
                CatalogId = productDto.CatalogId,
                CategoryId = productDto.CategoryId,
                DownloadExpiration = productDto.DownloadExpiration,
                DownloadType = productDto.DownloadType,
                EnableReview = productDto.EnableReview ?? false,
                Gtin = productDto.Gtin,
                HasUserAgreement = productDto.HasUserAgreement ?? false,
                IsActive = productDto.IsActive ?? false,
                IsBuyable = productDto.IsBuyable ?? false,
                ManufacturerPartNumber = productDto.ManufacturerPartNumber,
                MaxNumberOfDownload = productDto.MaxNumberOfDownload ?? 0,
                MaxQuantity = productDto.MaxQuantity ?? 0,
                MeasureUnit = productDto.MeasureUnit,
                MinQuantity = productDto.MinQuantity ?? 0,
                Name = productDto.Name,
                PackageType = productDto.PackageType,
                ProductType = productDto.ProductType,
                ShippingType = productDto.ShippingType,
                TaxType = productDto.TaxType,
                TrackInventory = productDto.TrackInventory ?? false,
                VendorId = productDto.Vendor,
                WeightUnit = productDto.WeightUnit,
                Weight = productDto.Weight,
                Height = productDto.Height,
                Width = productDto.Width,
                Length = productDto.Length,
                Sku = productDto.Code,
                //Outline = productDto.Outlines.GetOutlinePath(store.Catalog),
                //SeoPath = productDto.Outlines.GetSeoPath(store, currentLanguage, null),
            };
            result.Url = "/" + (result.SeoPath ?? "product/" + result.Id);

            //if (productDto.Properties != null)
            //{
            //    result.Properties = new MutablePagedList<CatalogProperty>(productDto.Properties
            //        .Where(x => string.Equals(x.Type.ToString(), "Product", StringComparison.InvariantCultureIgnoreCase))
            //        .Select(p => ToProperty(p, currentLanguage))
            //        .ToList());

            //    if (productDto.IsActive.GetValueOrDefault())
            //    {
            //        result.VariationProperties = new MutablePagedList<CatalogProperty>(productDto.Properties
            //            .Where(x => string.Equals(x.Type, "Variation", StringComparison.InvariantCultureIgnoreCase))
            //            .Select(p => ToProperty(p, currentLanguage))
            //            .ToList());
            //    }
            //}

            //if (productDto.Images != null)
            //{
            //    result.Images = productDto.Images.Select(ToImage).ToArray();
            //    result.PrimaryImage = result.Images.FirstOrDefault();
            //}

            //if (productDto.Assets != null)
            //{
            //    result.Assets = productDto.Assets.Select(ToAsset).ToList();
            //}

            //if (productDto.Variations != null)
            //{
            //    result.Variations = productDto.Variations.Select(v => ToProduct(v, currentLanguage, currentCurrency, store)).ToList();
            //}

            //if (!productDto.SeoInfos.IsNullOrEmpty())
            //{
            //    var seoInfoDto = productDto.SeoInfos.Select(x => x.JsonConvert<coreDto.SeoInfo>())
            //        .GetBestMatchingSeoInfos(store, currentLanguage)
            //        .FirstOrDefault();

            //    if (seoInfoDto != null)
            //    {
            //        result.SeoInfo = seoInfoDto.ToSeoInfo();
            //    }
            //}

            if (result.SeoInfo == null)
            {
                result.SeoInfo = new SeoInfo
                {
                    Title = productDto.Id,
                    Language = currentLanguage,
                    Slug = productDto.Code
                };
            }

            //if (productDto.Reviews != null)
            //{
            //    // Reviews for currentLanguage (or Invariant language as fall-back) for each ReviewType
            //    var descriptions = productDto.Reviews
            //                            .Where(r => !string.IsNullOrEmpty(r.Content))
            //                            .Select(r => new EditorialReview
            //                            {
            //                                Language = new Language(r.LanguageCode),
            //                                ReviewType = r.ReviewType,
            //                                Value = Markdown.ToHtml(r.Content, _markdownPipeline)
            //                            });
            //    //Select only best matched description for current language in the each description type
            //    var tmpDescriptionList = new List<EditorialReview>();
            //    foreach (var descriptionGroup in descriptions.GroupBy(x => x.ReviewType))
            //    {
            //        var description = descriptionGroup.FindWithLanguage(currentLanguage);
            //        if (description != null)
            //        {
            //            tmpDescriptionList.Add(description);
            //        }
            //    }
            //    result.Descriptions = new MutablePagedList<EditorialReview>(tmpDescriptionList);
            //    result.Description = (result.Descriptions.FirstOrDefault(x => x.ReviewType.EqualsInvariant("FullReview")) ?? result.Descriptions.FirstOrDefault())?.Value;
            //}

            return result;
        }
    }
}
