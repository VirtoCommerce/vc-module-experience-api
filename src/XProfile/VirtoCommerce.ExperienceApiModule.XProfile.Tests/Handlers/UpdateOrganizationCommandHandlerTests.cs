using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Moq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Tests.Helpers;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Tests.Handlers
{
    public class UpdateOrganizationCommandHandlerTests : MoqHelper
    {
        [Fact]
        public async Task Handle_RequestWithDynamicProperties_UpdateDynamicPropertyCalled()
        {
            // Arragne
            var aggregateRepositoryMock = new Mock<IOrganizationAggregateRepository>();
            var dynamicPropertyUpdaterServiceMock = new Mock<IDynamicPropertyUpdaterService>();
            var mapperMock = new Mock<IMapper>();

            var organization = _fixture.Create<Organization>();
            var organizatnoAggregate = new OrganizationAggregate { Member = organization };

            aggregateRepositoryMock
                .Setup(x => x.GetMemberAggregateRootByIdAsync<OrganizationAggregate>(It.IsAny<string>()))
                .ReturnsAsync(organizatnoAggregate);

            var handler = new UpdateOrganizationCommandHandler(
                mapperMock.Object,
                aggregateRepositoryMock.Object,
                dynamicPropertyUpdaterServiceMock.Object);

            var command = _fixture.Create<UpdateOrganizationCommand>();

            // Act
            var aggregate = await handler.Handle(command, CancellationToken.None);

            // Assert
            dynamicPropertyUpdaterServiceMock.Verify(x => x.UpdateDynamicPropertyValues(It.Is<Organization>(x => x == organization),
                It.Is<IList<DynamicPropertyValue>>(x => x == command.DynamicProperties)), Times.Once);
        }
    }
}
