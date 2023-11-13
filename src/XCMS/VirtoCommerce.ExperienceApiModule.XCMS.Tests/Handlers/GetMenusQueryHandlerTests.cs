using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XCMS.Queries;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Tests.Handlers
{
    public class GetMenusQueryHandlerTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public async Task GetMenusQueryHandlerTest(GetMenusQuery request, List<string> expectedMenuNames)
        {
            // Arrange
            var menus = new List<MenuLinkList>
            {
                new MenuLinkList
                {
                    Name = "MainMenu",
                    Language = "en-US",
                },
                new MenuLinkList
                {
                    Name = "SubMenu",
                    Language = "ru-RU",
                }
            };

            var menuServiceMock = new Mock<IMenuService>();
            menuServiceMock
                .Setup(x => x.GetListsByStoreIdAsync(It.Is<string>(x => x == "Store")))
                .ReturnsAsync(menus);

            var handler = new GetMenusQueryHandler(menuServiceMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Menus.Select(x => x.Name).Should().BeEquivalentTo(expectedMenuNames);
        }

        public static IEnumerable<object[]> Data()
        {
            yield return new object[]
            {
                // request
                new GetMenusQuery { StoreId = "NonExistant", },
                // expected
                new List<string>()
            };
            yield return new object[]
            {
                // request
                new GetMenusQuery { StoreId = "Store", },
                // expected
                new List<string>() { "MainMenu", "SubMenu" }
            };
            yield return new object[]
            {
                // request
                new GetMenusQuery { StoreId = "Store", CultureName = "ru-RU" },
                // expected
                new List<string>() { "SubMenu" }
            };
            yield return new object[]
            {
                // request
                new GetMenusQuery { StoreId = "Store", Keyword = "Sub" },
                // expected
                new List<string>() { "SubMenu" }
            };
        }
    }
}
