using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nest;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Extensions;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Extensions
{
    public class GetBreadcrumbsFromOutLineTests
    {
        [Fact]
        public void GetProductBredCrumbs_Success()
        {
            var productOutline = GetProductOutlines();
            var store = new Store()
            {
                Id = "StoreId",
                Catalog = "4974648a41df4e6ea67ef2ad76d7bbd4",
                DefaultLanguage = "en-US",
                DefaultCurrency = "USD",
                Settings = new List<ObjectSettingEntry>()
            };
            //Act
            var breadcrumbs = productOutline.GetBreadcrumbsFromOutLine(store, null);

            // Assert
            breadcrumbs.Should().HaveCount(3);

            breadcrumbs.ToArray()[0].SeoPath.Should().Be("Camcorders");
            breadcrumbs.ToArray()[1].SeoPath.Should().Be("Camcorders/Aerial Imaging & Drones");
            breadcrumbs.ToArray()[2].SeoPath.Should().Be("Camcorders/Aerial Imaging & Drones/3dr-x8-m-octocopter-for-visual-spectrum-aeria");
        }

        [Fact]
        public void GetProductBredCrumbs_WrongStoreCatalog_ShouldBeEmpty()
        {
            var productOutline = GetProductOutlines();
            var store = new Store()
            {
                Id = "StoreId",
                Catalog = "WrongCatalogId",
                DefaultLanguage = "en-US",
                DefaultCurrency = "USD",
                Settings = new List<ObjectSettingEntry>()
            };
            //Act
            var breadcrumbs = productOutline.GetBreadcrumbsFromOutLine(store, null);

            // Assert
            breadcrumbs.Should().BeEmpty();
        }


        private List<Outline> GetProductOutlines()
        {
            return new List<Outline>() {
                new Outline
                {
                    Items = new List<OutlineItem>()
                    {
                        new OutlineItem(){
                            Id= "4974648a41df4e6ea67ef2ad76d7bbd4",
                            Name= "Electronics",
                            SeoObjectType= "Catalog",
                            SeoInfos = new List<SeoInfo>()
    
                        },
                        new OutlineItem(){
                            Id= "45d3fc9a913d4610a5c7d0470558c4b2",
                            Name= "Camcorders",
                            SeoObjectType= "Category",
                            SeoInfos= new List<SeoInfo> {
                                new SeoInfo(){
                                    Id= "ddf4dd0c093740f5a7b5023b34f4bdbe",
                                    ObjectId = "45d3fc9a913d4610a5c7d0470558c4b2",
                                    ObjectType = "Category",
                                    SemanticUrl = "Camcorders"
    
                                }
                            }
                        },
                        new OutlineItem(){
                            Id= "e51b5f9eea094a44939c11d4d4fa3bb1",
                            Name= "Aerial Imaging & Drones",
                            SeoObjectType= "Category",
                            SeoInfos= new List<SeoInfo> {
                                new SeoInfo(){
                                    Id= "42673c31224e46c4b51b4c026401e8f3",
                                    ObjectId = "e51b5f9eea094a44939c11d4d4fa3bb1",
                                    ObjectType = "Category",
                                    SemanticUrl = "Aerial Imaging & Drones"
                                }
                            }
                        },
                        new OutlineItem(){
                            Id= "e7eee66223da43109502891b54bc33d3",
                            Name= "e7eee66223da43109502891b54bc33d3",
                            SeoObjectType= "CatalogProduct",
                            SeoInfos= new List<SeoInfo> {
                                new SeoInfo(){
                                    
                                    Id= "42673c31224e46c4b51b4c4353453453",
                                    ObjectId = "e7eee66223da43109502891b54bc33d3",
                                    ObjectType = "CatalogProduct",
                                    SemanticUrl = "3dr-x8-m-octocopter-for-visual-spectrum-aeria"
    
                                }
                            }
                        }
                    }
                }

            };
        } 
    }
}
