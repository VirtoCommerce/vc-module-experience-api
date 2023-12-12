using System;
using System.Collections.Generic;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.XOrder.Mapping;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Facets;
using VirtoCommerce.XDigitalCatalog.Mapping;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests
{
    public class MappingTermFilterTests
    {
        [Fact]
        public void OrderMappingProfileTest()
        {
            // Arrange
            var mapperCfg = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new OrderMappingProfile());
            });

            var mapper = mapperCfg.CreateMapper();
            var terms = new List<IFilter>
            {
                new TermFilter { FieldName = "CustomerId", Values = new[] { Guid.NewGuid().ToString() } },
                new TermFilter { FieldName = "CustomerIds", Values = new[] { Guid.NewGuid().ToString() } },
                new TermFilter { FieldName = "SubscriptionIds", Values = new string [] { } },
                new TermFilter { FieldName = "SubscriptionIds", Values = null }
            };

            // Action
            var criteria = new CustomerOrderSearchCriteria();
            mapper.Map(terms, criteria);

            // Assert
            Assert.NotNull(criteria);
            Assert.NotNull(criteria.CustomerId);
            Assert.NotNull(criteria.CustomerIds);
            Assert.NotNull(criteria.CustomerId);
            Assert.Null(criteria.SubscriptionIds);
        }

        [Fact]
        public void FacetMappingProfileTest()
        {
            var mapperCfg = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FacetMappingProfile());
            });

            var mapper = mapperCfg.CreateMapper();

            var source = new Aggregation
            {
                AggregationType = "attr",
                Items =
                [
                    new AggregationItem
                    {
                        Count = 1,
                        Value = "value",
                        IsApplied = true,
                    }
                ]
            };

            var destination = mapper.Map<FacetResult>(source, options => { options.Items["cultureName"] = "en-US"; });

            var result = destination as TermFacetResult;

            Assert.NotEmpty(result.Terms);
        }

    }
}
