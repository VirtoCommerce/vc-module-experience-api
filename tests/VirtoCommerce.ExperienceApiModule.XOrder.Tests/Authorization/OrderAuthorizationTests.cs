using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.XOrder.Authorization;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests.Authorization
{
    public class OrderAuthorizationTests
    {
        [Fact]
        public async Task CanAccessOrderAuthorizationHandler_OrderBelongUser_ShouldSucceed()
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };
            var userId = "userId";
            var user = new ClaimsPrincipal( new ClaimsIdentity( new[] { new Claim("name", userId) }));

            var resource = new CustomerOrder { CustomerId = "userId" };

            var context = new AuthorizationHandlerContext(requirements, user, resource);
            var subject = new CanAccessOrderAuthorizationHandler();

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
            var subject = new CanAccessOrderAuthorizationHandler();

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasFailed.Should().BeTrue();
        }


        public static IEnumerable<object[]> GetSearchOrderQuery()
        {
            yield return new object[] { new SearchOrderQuery {CustomerId = "userId"} };
            yield return new object[] { new SearchOrderQuery { CustomerId = null } };
        }

        [Theory]
        [MemberData(nameof(GetSearchOrderQuery))]
        public async Task CanAccessOrderAuthorizationHandler_SearchOrdersBelongToUser_ShouldSucceed(SearchOrderQuery query)
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };
            var userId = "userId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("name", userId) }));

            var context = new AuthorizationHandlerContext(requirements, user, query);
            var subject = new CanAccessOrderAuthorizationHandler();

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetSearchOrderQuery))]
        public async Task CanAccessOrderAuthorizationHandler_SearchOrdersWithoutAuth_ShouldFail(SearchOrderQuery query)
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };
            
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var context = new AuthorizationHandlerContext(requirements, user, query);
            var subject = new CanAccessOrderAuthorizationHandler();

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
            var subject = new CanAccessOrderAuthorizationHandler();

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetSearchPaymentsQuery))]
        public async Task CanAccessOrderAuthorizationHandler_SearchPaymentsWithoutAuth_ShouldFail(SearchPaymentsQuery query)
        {
            //Arrange    
            var requirements = new[] { new CanAccessOrderAuthorizationRequirement() };

            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var context = new AuthorizationHandlerContext(requirements, user, query);
            var subject = new CanAccessOrderAuthorizationHandler();

            //Act
            await subject.HandleAsync(context);

            //Assert
            context.HasFailed.Should().BeTrue();
        }
    }
}
