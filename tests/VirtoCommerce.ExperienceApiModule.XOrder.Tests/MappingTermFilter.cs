using System;
using System.Collections.Generic;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.XOrder.Mapping;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.SearchModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests
{
    public class MappingTermFilter
    {
        [Fact]
        public void Map_TermFilter()
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
                new TermFilter { FieldName = "CustomerIds", Values = new[] { Guid.NewGuid().ToString() } }
            };

            // Action
            var criteria = new CustomerOrderSearchCriteria();
            mapper.Map(terms, criteria);

            // Assert
            Assert.NotNull(criteria);
            Assert.NotNull(criteria.CustomerId);
            Assert.NotNull(criteria.CustomerIds);
        }
    }
}
