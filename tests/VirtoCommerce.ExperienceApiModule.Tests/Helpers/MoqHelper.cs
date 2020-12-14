using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using GraphQL.DataLoader;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Tests.Helpers
{
    public class MoqHelper
    {
        protected readonly Fixture _fixture = new Fixture();

        protected const string CURRENCY_CODE = "USD";
        protected const string CULTURE_NAME = "en-US";
        protected const string DEFAULT_STORE_ID = "default";

        protected readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        protected readonly Mock<IDataLoaderContextAccessor> _dataLoaderContextAccessorMock = new Mock<IDataLoaderContextAccessor>();

        public MoqHelper()
        {
            _fixture.Register(() => new Language(CULTURE_NAME));
            _fixture.Register(() => new Currency(_fixture.Create<Language>(), CURRENCY_CODE));
            _fixture.Register(() => new SearchRequest
            {
                Filter = new AndFilter()
                {
                    ChildFilters = new List<IFilter>(),
                },
                SearchFields = new List<string> { "__content" },
                Sorting = new List<SortingField> { new SortingField("score",true) },
                Skip = 0,
                Take = 20,
                Aggregations = new List<AggregationRequest>(),
                IncludeFields = new List<string>(),
            });
        }

        protected Discount GetDiscount() => _fixture.Create<Discount>();

        protected Currency GetCurrency() => _fixture.Create<Currency>();

        protected Money GetMoney(decimal? amount = null) => new Money(amount ?? _fixture.Create<decimal>(), GetCurrency());

        public static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store ??= new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;
            options
                .Setup(o => o.Value)
                .Returns(idOptions);

            var userValidators = new List<IUserValidator<TUser>>();
            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>
            {
                new PasswordValidator<TUser>()
            };

            var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            validator
                .Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success))
                .Verifiable();

            return userManager;
        }

        internal IFilter GetFilterByName(string filterName)
        {
            var rangeFilter = _fixture.Create<RangeFilter>();
            var geoDistanceFilter = _fixture.Create<GeoDistanceFilter>();
            var idsFilter = _fixture.Create<IdsFilter>();
            var wildCardTermFilter = _fixture.Create<WildCardTermFilter>();

            return filterName switch
            {
                "rangeFilter" => rangeFilter,
                "geoDistanceFilter" => geoDistanceFilter,
                "idsFilter" => idsFilter,
                "wildCardTermFilter" => wildCardTermFilter,
                _ => throw new NotImplementedException("Add more cases")
            };
        }

        internal void AssertFiltersContainFilterByTypeName(IList<IFilter> filters, string filterName)
        {
            switch (filterName)
            {
                case "rangeFilter":
                    filters.Should().ContainItemsAssignableTo<RangeFilter>();
                    break;

                case "geoDistanceFilter":
                    filters.Should().ContainItemsAssignableTo<GeoDistanceFilter>();
                    break;

                case "idsFilter":
                    filters.Should().ContainItemsAssignableTo<IdsFilter>();
                    break;

                case "wildCardTermFilter":
                    filters.Should().ContainItemsAssignableTo<WildCardTermFilter>();
                    break;

                case "termFilter":
                    filters.Should().ContainItemsAssignableTo<TermFilter>();
                    break;

                default:
                    true.Should().BeFalse("Unknown filterName");
                    break;
            }
        }
    }
}
