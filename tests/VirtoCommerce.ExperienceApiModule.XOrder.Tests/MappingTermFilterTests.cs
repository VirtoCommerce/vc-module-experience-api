using System;
using System.Collections.Generic;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.XOrder.Mapping;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.SearchModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests
{
    public class MappingTermFilterTests
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

        public class BaseClass
        {
            public string Prop1 { get; set; }
        }
        public class DerivedClassA : BaseClass
        {
            public string Prop2 { get; set; }
        }
        public class DerivedClassB : BaseClass
        {
            public string Prop3 { get; set; }
        }

        public class ClassAggregate
        {
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
            public string Prop3 { get; set; }
        }

        [Fact]
        public void AAAA()
        {
            // Arrange
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BaseClass, ClassAggregate>().IncludeAllDerived();
                cfg.CreateMap<DerivedClassA, ClassAggregate>().IncludeAllDerived();
                cfg.CreateMap<DerivedClassB, ClassAggregate>().IncludeAllDerived();
            });

            var obj = new DerivedClassB
            {
                Prop1 = "aa",
                Prop3 = "333"            
            };

            

            var mapper = new AutoMapper.Mapper(config);
           // var result = mapper.Map<ClassAggregate>(obj, opts => opts.CreateMissingTypeMaps = true);


        }

    }
}
