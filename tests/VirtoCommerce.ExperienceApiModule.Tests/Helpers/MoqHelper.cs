using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.ExperienceApiModule.Tests.Helpers
{
    public class MoqHelper
    {
        protected readonly Fixture _fixture = new Fixture();

        protected const string CURRENCY_CODE = "USD";
        protected const string CULTURE_NAME = "en-US";

        public MoqHelper()
        {
            _fixture.Register(() => new Language(CULTURE_NAME));
            _fixture.Register(() => new Currency(_fixture.Create<Language>(), CURRENCY_CODE));
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
    }
}
