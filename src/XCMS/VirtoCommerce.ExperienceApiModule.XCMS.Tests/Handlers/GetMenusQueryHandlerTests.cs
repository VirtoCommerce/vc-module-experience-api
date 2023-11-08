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
                new()
                {
                    Name = "MainMenu",
                    Language = "en-US",
                },
                new()
                {
                    Name = "SubMenu",
                    Language = "ru-RU",
                },
            };
            var menusSearchResult = new MenuLinkListSearchResult
            {
                Results = menus,
            };

            var menuServiceMock = new Mock<IMenuLinkListSearchService>();
            menuServiceMock
                .Setup(x => x.SearchAsync(It.Is<MenuLinkListSearchCriteria>(c => c.Skip == 0 && c.StoreId == "Store"), It.IsAny<bool>()))
                .ReturnsAsync(menusSearchResult);
            menuServiceMock
                .Setup(x => x.SearchAsync(It.Is<MenuLinkListSearchCriteria>(c => c.Skip > 0 || c.StoreId != "Store"), It.IsAny<bool>()))
                .ReturnsAsync(new MenuLinkListSearchResult());

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
