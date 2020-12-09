using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Authorization;
using VirtoCommerce.Platform.Core.Security;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Tests.Authorization
{
    public class ProfileAuthorizationTests
    {
        private readonly IMemberService _memberService;
        private readonly Func<UserManager<ApplicationUser>> _userManagerFunc;

        private const string UserFromBigOrganizationId = "UserFromBigOrganizationId";
        private const string UserWithoutOrganizationId = "UserWithoutOrganizationId";
        private const string BigOrganizationId = "BigOrganizationId";

        public ProfileAuthorizationTests()
        {
            var memberServiceMock = new Mock<IMemberService>();
            memberServiceMock.Setup(x => x.GetByIdAsync(UserFromBigOrganizationId, null, null)).ReturnsAsync(() => new Contact() {Id = UserFromBigOrganizationId, Organizations = new List<string>() {BigOrganizationId}});
            memberServiceMock.Setup(x => x.GetByIdAsync(UserWithoutOrganizationId, null, null)).ReturnsAsync(() => new Contact() {Id = UserWithoutOrganizationId, Organizations = new List<string>()});
            _memberService = memberServiceMock.Object;

            var userManagerFactoryMock = new Mock<Func<UserManager<ApplicationUser>>>();
            _userManagerFunc = userManagerFactoryMock.Object;
        }

        [Fact]
        public async Task CanEditOrganizationAuthorizationHandler_UserGetOwnOrganization_ShouldSucceed()
        {
            //Arrange    
            var requirements = new[] {new CanEditOrganizationAuthorizationRequirement()};
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim("name", UserFromBigOrganizationId)}));
            var resource = new OrganizationAggregate(new Organization {Id = BigOrganizationId});

            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanEditOrganizationAuthorizationHandler(_memberService, _userManagerFunc);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue();
        }


        [Fact]
        public async Task CanEditOrganizationAuthorizationHandler_UserGetSomebodyElseOrganization_ShouldFail()
        {
            //Arrange    
            var requirements = new[] { new CanEditOrganizationAuthorizationRequirement() };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", UserWithoutOrganizationId) }));
            var resource = new OrganizationAggregate(new Organization { Id = BigOrganizationId });

            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanEditOrganizationAuthorizationHandler(_memberService, _userManagerFunc);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasFailed.Should().BeTrue();
        }

        [Fact]
        public async Task CanEditOrganizationAuthorizationHandler_UserGetItself_ShouldSucceed()
        {
            //Arrange    
            var requirements = new[] { new CanEditOrganizationAuthorizationRequirement() };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", UserFromBigOrganizationId) }));
            var resource = new ApplicationUser() { Id = UserFromBigOrganizationId };

            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanEditOrganizationAuthorizationHandler(_memberService, _userManagerFunc);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue();
        }


        [Fact]
        public async Task CanEditOrganizationAuthorizationHandler_UserGetAnotherUser_ShouldFail()
        {
            //Arrange    
            var requirements = new[] { new CanEditOrganizationAuthorizationRequirement() };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", UserWithoutOrganizationId) }));
            var resource = new ApplicationUser() { Id = UserFromBigOrganizationId };

            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanEditOrganizationAuthorizationHandler(_memberService, _userManagerFunc);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasFailed.Should().BeTrue();
        }
    }
}
