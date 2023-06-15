using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Moq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Authorization;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests.Authorization
{
    public class OrderAuthorizationTests
    {
        private readonly Mock<IMemberService> _memberServiceMock;

        public OrderAuthorizationTests()
        {
            _memberServiceMock = new Mock<IMemberService>();

            _memberServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string memberId, string responseGroup, string memberType) => new Contact() { Organizations = new List<string>() { "organization1", "organization2" } });
        }

        [Fact]
        public async Task CanAccessOrderAuthorizationHandler_OrderBelongUser_ShouldSucceed()
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };
            var userId = "userId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", userId) }));
            //var mockService =

            var resource = new CustomerOrder { CustomerId = "userId" };

            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanAccessOrderAuthorizationHandler(_memberServiceMock.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public async Task CanAccessOrderAuthorizationHandler_OrderBelongAnotherUser_ShouldFail()
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };
            var userId = "userId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", userId) }));

            var resource = new CustomerOrder { CustomerId = "AnotherUserId" };

            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanAccessOrderAuthorizationHandler(_memberServiceMock.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasFailed.Should().BeTrue();
        }


        public static IEnumerable<object[]> GetSearchOrderQuery()
        {
            yield return new object[] { new SearchCustomerOrderQuery { CustomerId = "userId" } };
            yield return new object[] { new SearchCustomerOrderQuery { CustomerId = null } };
        }

        [Theory]
        [MemberData(nameof(GetSearchOrderQuery))]
        public async Task CanAccessOrderAuthorizationHandler_SearchOrdersBelongToUser_ShouldSucceed(SearchCustomerOrderQuery query)
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };
            var userId = "userId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", userId) }));

            var context = new AuthorizationHandlerContext(requirements, user, query);
            var subject = new CanAccessOrderAuthorizationHandler(_memberServiceMock.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue();
            query.CustomerId.Should().Be("userId");
        }

        [Theory]
        [MemberData(nameof(GetSearchOrderQuery))]
        public async Task CanAccessOrderAuthorizationHandler_SearchOrdersWithoutAuth_ShouldFail(SearchCustomerOrderQuery query)
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };

            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var context = new AuthorizationHandlerContext(requirements, user, query);
            var subject = new CanAccessOrderAuthorizationHandler(_memberServiceMock.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasFailed.Should().BeTrue();
        }


        public static IEnumerable<object[]> GetSearchPaymentsQuery()
        {
            yield return new object[] { new SearchPaymentsQuery { CustomerId = "userId" } };
            yield return new object[] { new SearchPaymentsQuery { CustomerId = null } };
        }

        [Theory]
        [MemberData(nameof(GetSearchPaymentsQuery))]
        public async Task CanAccessOrderAuthorizationHandler_SearchPaymentsBelongToUser_ShouldSucceed(SearchPaymentsQuery query)
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };
            var userId = "userId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", userId) }));

            var context = new AuthorizationHandlerContext(requirements, user, query);
            var subject = new CanAccessOrderAuthorizationHandler(_memberServiceMock.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue();
            query.CustomerId.Should().Be("userId");
        }

        [Theory]
        [MemberData(nameof(GetSearchPaymentsQuery))]
        public async Task CanAccessOrderAuthorizationHandler_SearchPaymentsWithoutAuth_ShouldFail(SearchPaymentsQuery query)
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };

            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var context = new AuthorizationHandlerContext(requirements, user, query);
            var subject = new CanAccessOrderAuthorizationHandler(_memberServiceMock.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasFailed.Should().BeTrue();
        }

        public static readonly IList<object[]> SearchOrganizationOrderQueryTestData = new List<object[]>
        {
            new object[] { new SearchOrganizationOrderQuery { OrganizationId = "organization1" }, true },
            new object[] { new SearchOrganizationOrderQuery { OrganizationId = "organization3" }, false }
        };

        [Theory]
        [MemberData(nameof(SearchOrganizationOrderQueryTestData))]
        public async Task CanAccessOrderAuthorizationHandler_SearchOrganizationOrderQuery(SearchOrganizationOrderQuery query, bool successed)
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };

            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var context = new AuthorizationHandlerContext(requirements, user, query);
            var subject = new CanAccessOrderAuthorizationHandler(_memberServiceMock.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            Assert.Equal(successed, context.HasSucceeded);
        }
    }
}
